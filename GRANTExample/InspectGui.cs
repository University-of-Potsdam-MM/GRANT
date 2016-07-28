using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using System.Drawing;

namespace GRANTExample
{
    public class InspectGui
    {
        StrategyManager strategyMgr;
        public InspectGui(StrategyManager mgr)
        {
            strategyMgr = mgr;
        }

        public void inspect()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    int pointX;
                    int pointY;
                    strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);

                    Console.WriteLine("Pointx: " + pointX);
                    Console.WriteLine("Pointy: " + pointY);

                    OSMElement.OSMElement osmElement = strategyMgr.getSpecifiedFilter().setOSMElement(pointX, pointY);
                    Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);

                    // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint);
                    strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }
    }
}
