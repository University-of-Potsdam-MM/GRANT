using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using System.Diagnostics;

namespace FilteredTreeTest
{
    internal class HelpFunctions
    {
        StrategyManager strategyMgr; GeneratedGrantTrees grantTrees;
        public HelpFunctions(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        internal void filterApplication(String applicationName, String applicationPathName)
        {
            IntPtr appHwnd = startApp(applicationName, applicationPathName);
            if (appHwnd == IntPtr.Zero) { return; }
            Object filteredTree = strategyMgr.getSpecifiedFilter().filtering(appHwnd);
            grantTrees.setFilteredTree(filteredTree);
        }

        protected IntPtr startApp(String appMainModulNameCalc, String applicationPathName)
        {
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(appMainModulNameCalc);
            if (appHwnd.Equals(IntPtr.Zero))
            {
                bool openApp = strategyMgr.getSpecifiedOperationSystem().openApplication(applicationPathName);
                if (!openApp)
                {
                    Debug.WriteLine("Anwendung konnte nicht geöffnet werden! Ggf. Pfad der Anwendung anpassen.");
                    return IntPtr.Zero; ;
                }
                else
                {
                    appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(appMainModulNameCalc);
                }
            }
            else
            {
                strategyMgr.getSpecifiedOperationSystem().showWindow(appHwnd);
            }
            return appHwnd;
        }
    }
}
