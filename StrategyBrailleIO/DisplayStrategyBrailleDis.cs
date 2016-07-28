using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.AbstractClasses;

namespace StrategyBrailleIO
{
    public class DisplayStrategyBrailleDis : AOutputManager
    {
        private StrategyManager strategyMgr;
        Device activeDevice;

        public DisplayStrategyBrailleDis(StrategyManager strategyMgr) : base(strategyMgr)
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
            //ändert (TODO: ggf.) im StrategyManager die gewählte Implementierung
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

        public override void Dispose()
        {
        }
    }
}
