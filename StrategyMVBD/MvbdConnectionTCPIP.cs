using GRANTManager;
using OSMElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StrategyMVBD
{
    /* TODO: Rooting beachten!
     *  
     * 
     */


    /// <summary>
    /// Handles the TCP/IP connection to MVBD (see: http://download.metec-ag.de/MVBD/ )
    /// </summary>
    internal class MvbdConnectionTCPIP : IDisposable
    {
        #region Root Identifier (see: http://download.metec-ag.de/MVBD/TcpRoots/ )
        private const byte IDENTIFIERIDOFGRANT = 3;
        private const byte IDENTIFIERIDOFNVDA = 2;
        private const byte IDENTIFIERIDOFMVBD = 1;
        #endregion

        private IPEndPoint _endPoint;
        internal TcpClient _tcpClient;
        private bool isDisposed = false;
        private StrategyManager strategyMgr;

        private DisplayStrategyMVBD displayStrategyMVBD;
        private ExternalScreenreaderNVDA externalScreenreaderNVDA;



        public MvbdConnectionTCPIP(StrategyManager strategyMgr, Object o)
        {
            this.strategyMgr = strategyMgr;
            if (!isMvbdRunning()) { Debug.WriteLine("MVBD isn't running!"); return; }
            // if (!strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("NVDA")) { Debug.WriteLine("NVDA isn't running!"); return; }
            if (o.GetType().Equals(typeof(DisplayStrategyMVBD)))
            {
                displayStrategyMVBD =(DisplayStrategyMVBD) o;
            }else
            {
                if (o.GetType().Equals(typeof(ExternalScreenreaderNVDA)))
                {
                    externalScreenreaderNVDA = (ExternalScreenreaderNVDA)o;
                }
            }

            _endPoint = new IPEndPoint(IPAddress.Loopback, 2017); //TODO: auslesen
            
            ThreadPool.QueueUserWorkItem(new WaitCallback(Thread_Callback));
            System.Threading.Thread.Sleep(100); // Wait of connection to MVBD
            SendIdentifierOfClient();
        }

        ~MvbdConnectionTCPIP() { Dispose(); }

        internal bool isMvbdRunning()
        {
            return strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("MVBD");
        }

        private void Thread_Callback(object state)
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

                        if (!isDisposed && _tcpClient.Connected == true)
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
                            switch (cmd)
                            {
                                case 1:
                                    // Pin data (for single cells and brailleline-devices)
                                    getPinData(ba);
                                    break;
                                case 20:
                                    // Device Info
                                    setActiveDeviceGrant(ba);
                                    break;
                                case 26:
                                    // possible Devices
                                    if (displayStrategyMVBD != null)
                                    {
                                        displayStrategyMVBD.deviceList = interpretDeviceList(ba);
                                    }
                                    break;
                                case 31:
                                    // GetTcpRoots
                                    getTcpRoots(ba);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine("Fehler bei 'DisplayStrategyMVBD \n Fehler:\n {0}", e); }
            }
        }

        private void getTcpRoots(byte[] ba)
        {
            // Command 31
            //  16 identifiers 0=Unknown, 1=MVBD, 2=NVDA, 3=GRANT, 4=HyperBrailleGeo, 5=Monitor, 6=MATLAB, 7=Presentation, 8=Eprime, 9...15 are for future use or custom use.
            int commandsCount = ba[0];
            int identifiersCount = ba[1];
            // TODO ...
        }

        private void sendGetTcpRoots()
        {
            // Command 31
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return;

            byte[] ba = new byte[3];
            ba[0] = 31;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendIdentifierOfClient");
            Send(ba);
        }

        private void setActiveDeviceGrant(Byte[] bas)
        {
            if(displayStrategyMVBD == null) { return; }
            OrientationEnum orientation = OrientationEnum.Unknown;
            if (Enum.IsDefined(typeof(OrientationEnum), (int)bas[2]))
            {
                orientation = (OrientationEnum)bas[2];
            }
            String deviceName = "";
            int deviceId = (bas[3] << 0 | bas[4] << 8 | bas[5] << 16 | bas[6] << 24);
            if (displayStrategyMVBD.deviceList != null)
            {
                foreach (Device device in displayStrategyMVBD.deviceList)
                {
                    if (device.id == deviceId)
                    {
                        deviceName = device.name;
                    }
                }
            }
            Device d = new Device(bas[1], bas[0], orientation, deviceName, typeof(DisplayStrategyMVBD));
            d.id = deviceId;
            displayStrategyMVBD.activeDevice = d;
        }

        private void getPinData(byte[] ba)
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) { return; }
            //   Debug.WriteLine("Bytes: " + String.Join(" ", ba));
            List<List<int>>  dots = BytesToDots(ba);
            if (strategyMgr.getSpecifiedBrailleConverter() != null && externalScreenreaderNVDA != null)
            {
                externalScreenreaderNVDA.lastContent = strategyMgr.getSpecifiedBrailleConverter().getStringFromDots(dots);
                //   Debug.WriteLine("Braillezeile NVDA:" + ExternalScreenreaderNVDA.lastContent);
            }
        }

        private List<List<int>> BytesToDots(byte[] ba)
        {
            List<List<int>> dots = new List<List<int>>();
            foreach(byte b in ba)
            {
                dots.Add(ByteToDots(b));
            }
            return dots;
        }

        private List<int> ByteToDots(byte ba)
        {
            /*
             *     see: http://download.metec-ag.de/MVBD/#1
             * 
             *      1   o o     8               1   o o  4 
             *      2   o o     16      -->     2   o o  5
             *      4   o o     32      -->     3   o o  6
             *      64  o o     128             7   o o  8
             */
            
            int ba_int = (int)ba;
            List<int> dots = new List<int>();
            if (ba_int >= 128)
            {
                // set pin the bottom left
                dots.Add(8);
                ba_int = ba_int - 128;
            }
            if (ba_int >= 64)
            {
                // set pin the bottom right
                dots.Add(7);
                ba_int = ba_int - 64;
            }
            if(ba_int >= 32)
            {
                dots.Add(6);
                ba_int = ba_int - 32;
            }
            if (ba_int >= 16)
            {
                dots.Add(5);
                ba_int = ba_int - 16;
            }
            if (ba_int >= 8)
            {
                dots.Add(4);
                ba_int = ba_int - 8;
            }
            if (ba_int >= 4)
            {
                dots.Add(3);
                ba_int = ba_int - 4;
            }
            if (ba_int >= 2)
            {
                dots.Add(2);
                ba_int = ba_int - 2;
            }
            if (ba_int >= 1)
            {
                dots.Add(1);
               // ba_int = ba_int - 1;
            }
            dots.Sort();
            return dots;
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

      

        internal void SendIdentifierOfClient()
        {
            // command 29
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) return;

            byte[] ba = new byte[4];
            ba[0] = 29;
            ba[1] = 1;
            ba[2] = 0;
            ba[3] = IDENTIFIERIDOFGRANT;

            Debug.Print("<-- SendIdentifierOfClient");
            Send(ba);
        }

        internal void SendGetDeviceInfo()
        {
            // command 20
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) { return; }

            byte[] ba = new byte[3];
            ba[0] = 20;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendGetDeviceInfo");
            Send(ba);
        }


        internal void SendGetPosibleDevices()
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) {return; }

            byte[] ba = new byte[3];
            ba[0] = 26;
            ba[1] = 0;
            ba[2] = 0;

            Debug.Print("<-- SendGetPosibleDevices");
            Send(ba);
        }


        private List<Device> interpretDeviceList(Byte[] bas)
        {
            if (bas.Length < 4) { Debug.WriteLine("keine Liste vorhanden"); return new List<Device>(); }

            //ID: immer 4 Bytes
            /* Beispiel für Name
             * Bytes: 3 66 68 49
             * 3 => der Name besteht aus 3 Bytes
             * 66 68 49 => ist der Name
            */
            byte[] ba = new byte[bas.Length - 1];
            Buffer.BlockCopy(bas, 1, ba, 0, ba.Length);
            List<Device> deviceList = new List<Device>();
            int index = 0;
            for (int count = bas[0]; count > 0; count--) //TODO: eigentlich count > 0
            {
                Device d = new Device(typeof(DisplayStrategyMVBD));

                d.id = (ba[0 + index] << 0 | ba[1 + index] << 8 | ba[2 + index] << 16 | ba[3 + index] << 24);
                int nameLength = ba[4 + index];
                d.name = Encoding.UTF8.GetString(ba, 4 + 1 + index, nameLength);

                index = index + 4 + 1 + nameLength;
                deviceList.Add(d);
            }
            return deviceList;
        }

        internal void SendSetDevice(int deviceId)
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) { return; }

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

        /// <summary>
        /// Sends data to MVBD
        /// </summary>
        /// <param name="ba"></param>
        private void Send(byte[] ba)
        {
            if ((_tcpClient == null) || (_tcpClient.Connected == false)) { return; }
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

    }
}
