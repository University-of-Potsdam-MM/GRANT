using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager.AbstractClasses;
using StrategyManager;

namespace StrategyDisplayBrailleIoSimulator
{
    public class DisplayStrategyBrailleIoSimulator : AbstractDisplayStrategy
    {
        private StrategyMgr strategyMgr;
        Device activeDevice;

        public DisplayStrategyBrailleIoSimulator(StrategyMgr strategyMgr) : base(strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            AdapterClass adapterClassBrailleDisSimulator = new AdapterClass("DisplayStrategyBrailleIoSimulator", "StrategyDisplayBrailleIoSimulator", "StrategyDisplayBrailleIoSimulator");
            activeDevice = new Device(120, 60, OrientationEnum.Front, "BrailleDisSimulator", adapterClassBrailleDisSimulator);
        }

        public override Device getActiveDevice()
        {
            return activeDevice;
        }

        public override List<Device> getPosibleDevices()
        {
            //es gibt nur dieses eine
            List<Device> deviceList = new List<Device>();
            deviceList.Add(activeDevice);
            return deviceList;
        }
    }
}
