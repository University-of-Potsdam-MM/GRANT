using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.AbstractClasses;

namespace StrategyDisplayBrailleDis
{
    public class DisplayStrategyBrailleDis : AbstractDisplayStrategy
    {
        private StrategyMgr strategyMgr;
        Device activeDevice;

        public DisplayStrategyBrailleDis(StrategyMgr strategyMgr) : base(strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            activeDevice = new Device(120, 60, OrientationEnum.Front, "BrailleDis", this.GetType());
        }

        public override Device getActiveDevice()
        {
            return activeDevice;
        }

      /*  public override bool setActiveDevice(Device device)
        {
            //ändert (TODO: ggf.) im StrategyMgr die gewählte Implementierung
            strategyMgr.
            return true;
        }*/

        public override List<Device> getPosibleDevices()
        {
            //es gibt nur dieses eine
            List<Device> deviceList = new List<Device>();
            deviceList.Add(activeDevice);
            return deviceList;
        }
    }
}
