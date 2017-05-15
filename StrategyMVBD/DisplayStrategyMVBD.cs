using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.AbstractClasses;
using GRANTManager;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using OSMElement;

namespace StrategyMVBD
{
    public class DisplayStrategyMVBD : AOutputManager //, IDisposable
    {
        protected IPEndPoint _endPoint;
        protected TcpClient _tcpClient;
        protected bool isDisposed = false;
        private Device activeDevice { get; set; }
        private List<Device> deviceList;

        public DisplayStrategyMVBD(StrategyManager strategyMgr) : base(strategyMgr) 
        {
            if (!isMvbdRunning()) { Debug.WriteLine("MVBD ist nicht gestartet!"); return; }
            _endPoint = new IPEndPoint(IPAddress.Loopback, 2017); //TODO: auslesen
            ThreadPool.QueueUserWorkItem(new WaitCallback(Thread_Callback));
            //Deviceliste abrufen
            deviceList = getPosibleDevices();
        }

        ~DisplayStrategyMVBD() { Dispose(); }

        private bool isMvbdRunning()
        {
            return base.strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("MVBD") ;
        }

        #region Implementierung der Abstrakten Klasse
        public override Device getActiveDevice()
        {
            //1. Sende Abfrage falls keine Daten vorhanden sind (jede Änderung wird sofort per TCPIP gesendet und muss eigentlich nich neu abgefragt werden)
            if (activeDevice.Equals( new Device()))
            {
                SendGetDeviceInfo();
                // 2. Empfange Ergebnis ist im Thread (Thread_Callback)
                //warten auf Antwort
                while (activeDevice.Equals(new Device())) { Task.Delay(10).Wait(); }
            }            
            return activeDevice;       
        }

        public override List<Device> getPosibleDevices()
        {
            //warten bis die TCPIP-Verbindung aufgebaut ist --> nicht unendlich lange warten, evtl. wird mvbd nicht verwendet
            for (int i = 0; i < 10 && (_tcpClient == null || !_tcpClient.Connected); i++)
            {
                Task.Delay(10).Wait(); 
            }
            if (_tcpClient == null || !_tcpClient.Connected) { return new List<Device>(); }
           // while (_tcpClient == null || !_tcpClient.Connected) { Task.Delay(10).Wait(); }
            SendGetPosibleDevices();
            //warten bis Antwort kommt
            while (deviceList == null) { Task.Delay(10).Wait(); }
           // Thread.Sleep(7000);//damit die Verbindung aufgebaut wird, später müsste hier auf die Antwort gewartet werden
            return deviceList;

        }

        /// <summary>
        /// Sets the choosen device in MVBD
        /// </summary>
        /// <param name="device"></param>
        protected override void setDevice(Device device)
        {
            // Command 27
            if (device.id == 0 || device.id == activeDevice.id) return;
             activeDevice= new Device();
            SendSetDevice(device.id);            
        }

        protected override bool isDisplayStrategyAvailable()
        {
            return isMvbdRunning();
        }
        #endregion

        #region Hilfsfunktionen
        /// <summary>Working Thread</summary>
        protected void Thread_Callback(object o)
        {
            while (!isDisposed)
            {
                try
                {
                    if (_tcpClient == null)
                    {
                        _tcpClient = new TcpClient();
                        _tcpClient.ReceiveTimeout = 500;
                        _tcpClient.SendTimeout = 500;
                    }
                    if (!isDisposed && _tcpClient.Connected == false)
                    {
                        TryConnect();

                        if (!isDisposed &&_tcpClient.Connected == true)
                        {
                            SendGetDeviceInfo();
                        }
                    }
                    if (!isDisposed && _tcpClient.Connected == true)
                    {
                        if (!isDisposed && _tcpClient.Available > 3)
                        {
                            NetworkStream ns = _tcpClient.GetStream();
                            int cmd = ns.ReadByte();
                            int lenL = ns.ReadByte();
                            int lenH = ns.ReadByte();
                            int len = lenH << 8 | lenL;

                            byte[] ba = new byte[len];
                            ns.Read(ba, 0, len);

                            if (cmd == 20) //20 => Device-Info
                            {
                                setActiveDeviceGrant(ba);
                            }
                            if (cmd == 26)
                            {
                                deviceList = interpretDeviceList(ba);
                            }                            
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine("Fehler bei 'DisplayStrategyMVBD \n Fehler:\n {0}", e); }
            }
        }

        private void setActiveDeviceGrant(Byte[] bas)
        {
            OrientationEnum orientation = OrientationEnum.Unknown;
            if (Enum.IsDefined(typeof(OrientationEnum), (int)bas[2]))
            {
                orientation = (OrientationEnum)bas[2];
            }
            String deviceName = "";
            int deviceId = (bas[3] << 0 | bas[4] << 8 | bas[5] << 16 | bas[6] << 24);
            if (deviceList != null)
            {
                foreach (Device device in deviceList)
                {
                    if (device.id == deviceId)
                    {
                        deviceName = device.name;
                    }
                }
            }
            Device d = new Device(bas[1], bas[0], orientation, deviceName, this.GetType());
            d.id = deviceId;
            activeDevice = d;
        }

        private List<Device> interpretDeviceList(Byte[] bas)
        {
            if(bas.Length < 4) {Debug.WriteLine("keine Liste vorhanden"); return new List<Device>(); }

            //ID: immer 4 Bytes
            /* Beispiel für Name
             * Bytes: 3 66 68 49
             * 3 => der Name besteht aus 3 Bytes
             * 66 68 49 => ist der Name
            */
            byte[] ba = new byte[bas.Length -1];
            Buffer.BlockCopy(bas, 1, ba, 0, ba.Length);
            List <Device> deviceList = new List<Device>();
            int index = 0;
            for(int count = bas[0]; count > 0; count--) //TODO: eigentlich count > 0
            {
                Device d = new Device(this.GetType());
                
                d.id = (ba[0 + index] << 0 | ba[1 + index] << 8 | ba[2 + index] << 16 | ba[3 + index] << 24);
                int nameLength = ba[4 + index];
                d.name = Encoding.UTF8.GetString(ba, 4 + 1 + index, nameLength);

                index = index + 4 + 1 + nameLength;
                deviceList.Add(d);
            }
            return deviceList;
        }

        private void SendGetDeviceList()
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return; // -->

            byte[] ba = new byte[3];
            ba[0] = 26;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendGetDeviceList");
            Send(ba);
        }

        private void SendSetDevice(int deviceId)
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return; // -->

            byte[] ba = new byte[7]; 
            ba[0] = 27; //Command

            int lenData = ba.Length - 3;        // Gesamtlänge - 3 Header
            ba[1] = (byte)(lenData >> 0);   // Length low byte   = 16
            ba[2] = (byte)(lenData >> 8);   // Length high Byte  = 3 * 256

            ba[3] = deviceId > 0 ? (byte)(deviceId >> 00) : (byte)0;

            ba[4] = deviceId > Math.Pow(2, 8) ? (byte)(deviceId >> 08) : (byte)0;

            ba[5] = deviceId > Math.Pow(2, 16) ? (byte)(deviceId >> 24) : (byte)0;
            ba[6] = (byte)0;
            Debug.Print("<-- SendSetDevice");
            Send(ba);
        }

        private void SendGetDeviceInfo()
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return; // -->

            byte[] ba = new byte[3];
            ba[0] = 20;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendGetDeviceInfo");
            Send(ba);
        }

        private void SendGetPosibleDevices()
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return; // -->

            byte[] ba = new byte[3]; 
            ba[0] = 26;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendGetPosibleDevices");
            Send(ba);
        }

        private void Send(byte[] ba)
        {
            try
            {
                NetworkStream ns = _tcpClient.GetStream();
                ns.Write(ba, 0, ba.Length);
                ns.Flush();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

        private void TryConnect()
        {
            try
            {
                _tcpClient.Connect(_endPoint);
            }

            catch (SocketException ex)
            {
                Debug.Print(ex.Message);
            }

            catch (Exception ex)
            {
                _tcpClient.Close();                 // altes Socket schließen
                _tcpClient = null;

                _tcpClient = new TcpClient(_endPoint);  // close bringt hier nix
                Debug.Print(ex.Message);
            }
        }

        public override void Dispose()
        {
            if ((_tcpClient != null) && (_tcpClient.Connected == true))
            {
                try
                {
                    NetworkStream ns = _tcpClient.GetStream();
                    if (ns != null)
                    {
                        ns.Flush();
                        isDisposed = true;
                        ns.Close();
                        ns = null;
                    }
                }
                catch (ObjectDisposedException) { }
                _tcpClient.Close();
                _tcpClient = null;
            }
        }
        #endregion
    }
}
