using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;
using StrategyManager.AbstractClasses;
using StrategyMVBD;
using StrategyDisplayBrailleDis;

namespace GRANTExample
{
    public class ExampleDisplayStrategy
    {
        StrategyMgr strategyMgr;

        public ExampleDisplayStrategy(StrategyMgr strategy)
        {
            strategyMgr = strategy;
        }

        public void deviceInfo()
        {
            
            Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            Console.WriteLine("Device: {0}", activeDevice);
        }
        public void allDevices()
        {
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
            //AbstractDisplayStrategy.getAllPosibleDevices();
            Console.WriteLine("Alle Geräte:");
           foreach (Device d in devices)
           {
               Console.WriteLine("{0}", d.ToString());
           }
        }

    }

}
