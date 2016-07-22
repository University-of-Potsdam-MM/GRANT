using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager.AbstractClasses;
using StrategyManager;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace StrategyMVBD
{
    public class DisplayStrategyMVBD : AbstractDisplayStrategy, IDisposable
    {
        protected IPEndPoint _endPoint;
        protected TcpClient _tcpClient;
        private StrategyMgr strategyMgr;

        private Device activeDevice { get; set; }

        public DisplayStrategyMVBD(StrategyMgr strategyMgr) : base(strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            _endPoint = new IPEndPoint(IPAddress.Loopback, 2017); //TODO: auslesen
            ThreadPool.QueueUserWorkItem(new WaitCallback(Thread_Callback));
        }

        ~DisplayStrategyMVBD() { Dispose(); }

        #region Implementierung der Abstrakten Klasse
        public override Device getActiveDevice()
        {
            //1. Sende Abfrage falls keine Daten vorhanden sind (jede Änderung wird sofort per TCPIP gesendet und muss eigentlich nich neu abgefragt werden)
            if (activeDevice.Equals( new Device()))
            {
                SendGetDeviceInfo();
                // 2. Empfange Ergebnis ist im Thread (Thread_Callback)
            }
            return activeDevice;            
        }

      /*  public override bool setActiveDevice(Device device)
        {
            //TODO:
            //1. über TCPIP Auswahl senden
            //2. Ergbnis abwarten
            //3. (falls Ergebnis positiv ist)
            activeDevice = device;
            return true;
        }*/
        

        public override List<Device> getPosibleDevices()
        {
            //TODO: hier muss apäter auf die antwort gewartet werden
            //Die Implemetierung stimmt so noch nicht -> ist nur für Testzwecke da
            List<Device> deviceList = new List<Device>();
            deviceList.Add(new Device(64, 30, OrientationEnum.Front, "MVDB_1", this.GetType()));
            deviceList.Add(new Device(60, 60, OrientationEnum.Front, "MVDB_2", this.GetType()));
            return deviceList;

        }
   
        #endregion

        #region Hilfsfunktionen
        /// <summary>Working Thread</summary>
        protected void Thread_Callback(object o)
        {
            while (true)
            {
                try
                {
                    if (_tcpClient == null)
                    {
                        _tcpClient = new TcpClient();
                        _tcpClient.ReceiveTimeout = 500;
                        _tcpClient.SendTimeout = 500;
                    }


                    if (_tcpClient.Connected == false)
                    {
                        TryConnect();

                        if (_tcpClient.Connected == true)
                        {
                            SendGetDeviceInfo();
                        }
                    }
                    if (_tcpClient.Connected == true)
                    {

                        if (_tcpClient.Available > 3)
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
                                activeDevice = new Device(ba[1], ba[0], orientation, ba[3] + " "+ba[4] + " "+ ba[5] + " "+ ba[6], this.GetType());//TODO: name ordentlich vergeben
                                Debug.Print("--> DeviceInfo {0}x{1}", ba[0], ba[1]);
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

        public void Dispose()
        {
            if ((_tcpClient != null) && (_tcpClient.Connected == true))
            {
                _tcpClient.Close();
            }
        }
        #endregion
    }
}
