using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using OSMElements;

namespace StrategyMVBD
{
    public class DisplayStrategyMVBD : AOutputManager //, IDisposable
    {
        private MvbdConnectionTCPIP mvbdTcpIpConnection;

        private Device _activeDevice { get; set; }
        internal List<Device> deviceList { get; set; }
        internal Device activeDevice {
            get { return _activeDevice; }
            set {
                if (!value.Equals(_activeDevice) && !value.Equals(new Device()))
                {
                    _activeDevice = value;
                    if(deviceList != null && deviceList.Exists(p => p.name.Equals(_activeDevice.name) && p.deviceClassTypeFullName.Equals(typeof(DisplayStrategyMVBD).FullName)))
                    {
                        var result = deviceList.Find(p => p.name.Equals(_activeDevice.name) && p.deviceClassTypeFullName.Equals(typeof(DisplayStrategyMVBD).FullName));
                        int index = deviceList.FindIndex(p => p.name.Equals(_activeDevice.name) && p.deviceClassTypeFullName.Equals(typeof(DisplayStrategyMVBD).FullName));
                        // deviceList.re
                        if (!result.Equals(new Device()))
                        {
                            deviceList.RemoveAt(index);
                            deviceList.Insert(index, _activeDevice);
                            //deviceList.Add(_activeDevice);
                            //deviceList = deviceList.Sort()
                            //result = _activeDevice;
                        }
                    }
                }
            }
        }
        

        public DisplayStrategyMVBD(StrategyManager strategyMgr) : base(strategyMgr) 
        {
            mvbdTcpIpConnection = new MvbdConnectionTCPIP(strategyMgr, this);
            if (mvbdTcpIpConnection._tcpClient != null)
            {
                //Deviceliste abrufen
                deviceList = getPosibleDevices();
            }
        }

        ~DisplayStrategyMVBD() { Dispose(); }

        public override Device getActiveDevice()
        {
            //1. Sende Abfrage falls keine Daten vorhanden sind (jede Änderung wird sofort per TCPIP gesendet und muss eigentlich nich neu abgefragt werden)
            if (activeDevice.Equals( new Device()))
            {
                mvbdTcpIpConnection.SendGetDeviceInfo();
                // 2. Empfange Ergebnis ist im Thread (Thread_Callback)
                //warten auf Antwort
                while (activeDevice.Equals(new Device())) { Task.Delay(10).Wait(); }
            }            
            return activeDevice;       
        }

        public override List<Device> getPosibleDevices()
        {
            //warten bis die TCPIP-Verbindung aufgebaut ist --> nicht unendlich lange warten, evtl. wird mvbd nicht verwendet
            /*   for (int i = 0; i < 10 && (_tcpClient == null || !_tcpClient.Connected); i++)
               {
                   Task.Delay(10).Wait(); 
               }*/
            if (!mvbdTcpIpConnection.isMvbdRunning()) { return null; }
            mvbdTcpIpConnection.SendGetPosibleDevices();
            //wait for answer

            while (deviceList == null) { Task.Delay(10).Wait(); }
            // Thread.Sleep(7000);//damit die Verbindung aufgebaut wird, später müsste hier auf die Antwort gewartet werden
          //  deviceList.Sort();
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
            mvbdTcpIpConnection.SendSetDevice(device.id);            
        }

        protected override bool isDisplayStrategyAvailable()
        {
            return mvbdTcpIpConnection.isMvbdRunning();
        }

        public override void Dispose()
        {
            mvbdTcpIpConnection.Dispose();
        }
    }
}
