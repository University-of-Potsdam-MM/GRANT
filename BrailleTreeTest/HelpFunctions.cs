using System;
using GRANTManager;
using System.Diagnostics;

namespace BrailleTreeTests
{
    // Selbe Klasse wie in dem Package FilteredTreeTest

    internal class HelpFunctions
    {
        StrategyManager strategyMgr; GeneratedGrantTrees grantTrees;
        public HelpFunctions(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        internal void filterApplication(String processName, String applicationPathName)
        {
            IntPtr appHwnd = startApp(processName, applicationPathName);
            if (appHwnd == IntPtr.Zero) { return; }
            Object filteredTree = strategyMgr.getSpecifiedFilter().filtering(appHwnd);
            grantTrees.filteredTree = filteredTree;
        }

        protected IntPtr startApp(String processNameCalc, String applicationPathName)
        {
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(processNameCalc);
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
                    appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(processNameCalc);
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
