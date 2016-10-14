using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Diagnostics;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestUniqueIds
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
       // SearchNodes searchNodes;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String applicationName = "calc.exe";
        private String applicationPathName = @"C:\Windows\system32\calc.exe";

        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
          //  searchNodes = new SearchNodes(strategyMgr, grantTrees);
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
        }

        [TestMethod]
        public void TestGeneratedIdsOfFilteredTreeUnique()
        {
            filterApplication();
            if (grantTrees.getFilteredTree() == null) { Assert.Fail("Es ist kein gefilterter Baum vorhanden"); return; }
            Object copyedTree = strategyMgr.getSpecifiedTree().Copy(grantTrees.getFilteredTree());
            String nodeId;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getFilteredTree()))
            {
                nodeId = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
                foreach (Object nodeCopy in strategyMgr.getSpecifiedTree().AllNodes(copyedTree))
                {
                    bool result = strategyMgr.getSpecifiedTree().Equals(node, nodeCopy);
                    if (!strategyMgr.getSpecifiedTree().Equals(node, nodeCopy) && nodeId.Equals(strategyMgr.getSpecifiedTree().GetData(nodeCopy).properties.IdGenerated))
                    {
                        Debug.WriteLine("selbe ID :(");
                        Debug.WriteLine("node1 = " + node + "\nnode2 = " + nodeCopy);
                        Assert.Fail();
                    }
                }
            }            
        }

        private IntPtr startApp(String appMainModulNameCalc)
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

        private void filterApplication()
        {
            IntPtr appHwnd = startApp(applicationName);
            if (appHwnd == IntPtr.Zero) { return; }
            Object filteredTree = strategyMgr.getSpecifiedFilter().filtering(appHwnd);
            grantTrees.setFilteredTree(filteredTree);
        }
    }
}
