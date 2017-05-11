using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.AbstractClasses;
using StrategyMVBD;
using OSMElement;

namespace GRANTExample
{
    public class ExampleDisplayStrategy
    {
        StrategyManager strategyMgr;

        public ExampleDisplayStrategy(StrategyManager strategy)
        {
            strategyMgr = strategy;
        }

        public String deviceInfo()
        {
            
            Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            Console.WriteLine("Device: {0}", activeDevice);
            return "Aktives Gerät: " + activeDevice.ToString();
        }
        public String allDevices()
        {
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
            String allDevices = "Alle Geräte: ";
            Console.WriteLine("Alle Geräte:");
           foreach (Device d in devices)
           {
               allDevices = allDevices + "\n"  + d.ToString();
               Console.WriteLine("{0}", d.ToString());
           }
           return allDevices;
        }

        public void setMVBDDevice()
        {

            Device device = getDeviceByName("MVDB_2");
            if (!device.Equals(new Device()))
            {
                strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(device);
                Console.WriteLine("Ausgabegerät auf {0} geändert.", device.ToString());
            }
            else
            {
                //strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(new Device(64, 30, OrientationEnum.Front, "MVDB_2", this.GetType()));
                Settings s = new Settings();
                strategyMgr.setSpecifiedDisplayStrategy(s.getPosibleDisplayStrategies()[0].className);
                Console.WriteLine("Ausgabegerät auf 'MVBD' geändert.");
            }
        }

        internal void setBrailleDis()
        {
            Device device = getDeviceByName("BrailleDis");
            if (!device.Equals(new Device()))
            {
                strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(device);
                Console.WriteLine("Ausgabegerät auf {0} geändert.", device.ToString());
            }
            else
            {
                // strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(new Device(64, 30, OrientationEnum.Front, "BrailleDisSimulator", this.GetType()));
                Settings s = new Settings();
                strategyMgr.setSpecifiedDisplayStrategy(s.getPosibleDisplayStrategies()[2].className);
                Console.WriteLine("Ausgabegerät auf 'Simulator-BrailleIO' geändert.");
            }
        }

        public void setBrailleIoSimulatorDevice()
        {

            Device device = getDeviceByName("BrailleDisSimulator");
            //Device device = getDeviceByName("BrailleDis");
            if (!device.Equals(new Device()))
            {
                strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(device);
                Console.WriteLine("Ausgabegerät auf {0} geändert.", device.ToString());
            }
            else
            {
                // strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(new Device(64, 30, OrientationEnum.Front, "BrailleDisSimulator", this.GetType()));
                Settings s = new Settings();
                strategyMgr.setSpecifiedDisplayStrategy(s.getPosibleDisplayStrategies()[0].className);
                Console.WriteLine("Ausgabegerät auf 'Simulator-BrailleIO' geändert.");
            }
        }

        private Device getDeviceByName(String deviceName)
        {
              List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();

              foreach (Device d in devices)
              {
                  if (d.name.Equals(deviceName))
                  {
                      return d;
                  }
              }
            return new Device();
        }


    }

}
