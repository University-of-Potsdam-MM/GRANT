using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.AbstractClasses;
using GRANTManager;

namespace StrategyDisplayBrailleIoSimulator
{
    public class DisplayStrategyBrailleIoSimulator : AbstractDisplayStrategy
    {
        private StrategyMgr strategyMgr;
        Device activeDevice;

        public DisplayStrategyBrailleIoSimulator(StrategyMgr strategyMgr) : base(strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            activeDevice = new Device(120, 60, OrientationEnum.Front, "BrailleDisSimulator", this.GetType());
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

        public override void Dispose()
        {
        }
    }
}
