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
        private StrategyManager strategyMgr;
        protected bool isDisposed = false;
        private Device activeDevice { get; set; }
        private List<Device> deviceList;

        public DisplayStrategyMVBD(StrategyManager strategyMgr) : base(strategyMgr) 
        {
            this.strategyMgr = strategyMgr;
            if (!isMvbdRunning()) { Debug.WriteLine("MVBD ist nicht gestartet!"); return; }
            _endPoint = new IPEndPoint(IPAddress.Loopback, 2017); //TODO: auslesen
            ThreadPool.QueueUserWorkItem(new WaitCallback(Thread_Callback));
        }

        ~DisplayStrategyMVBD() { Dispose(); }

        private bool isMvbdRunning()
        {
            return strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("MVBD.exe") == IntPtr.Zero ? false : true ;
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
                                OrientationEnum orientation = OrientationEnum.Unknown;
                                if(Enum.IsDefined(typeof(OrientationEnum), (int) ba[2]))
                                {
                                    orientation = (OrientationEnum)ba[2];
                                }
                                activeDevice = new Device(ba[1], ba[0], orientation, "MVBD_" + ba[4], this.GetType());//TODO: name ordentlich vergeben
                                Debug.Print("--> DeviceInfo {0}x{1}", ba[0], ba[1]);
                            }
                            if (cmd == 26)
                            {
                                //TODO: antwort interpretieren und Liste richtig setzen
                                deviceList = new List<Device>();
                                deviceList.Add(new Device(64, 30, OrientationEnum.Front, "MVDB_1", this.GetType()));
                                deviceList.Add(new Device(60, 60, OrientationEnum.Front, "MVDB_2", this.GetType()));
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine("Fehler bei 'DisplayStrategyMVBD \n Fehler:\n {0}", e); }
            }
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
