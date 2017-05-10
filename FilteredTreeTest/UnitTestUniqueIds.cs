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
        private String applicationName = "calc";
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
            
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
        }

        [TestMethod]
        public void TestGeneratedIdsOfFilteredTreeUnique()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            if (grantTrees.filteredTree == null) { Assert.Fail("Es ist kein gefilterter Baum vorhanden"); return; }
            Object copyedTree = strategyMgr.getSpecifiedTree().Copy(grantTrees.filteredTree);
            String nodeId;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                nodeId = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
                foreach (Object nodeCopy in strategyMgr.getSpecifiedTree().AllNodes(copyedTree))
                {
                    if (!strategyMgr.getSpecifiedTree().Equals(node, nodeCopy) && nodeId.Equals(strategyMgr.getSpecifiedTree().GetData(nodeCopy).properties.IdGenerated))
                    {
                        Debug.WriteLine("selbe ID :(");
                        Debug.WriteLine("node1 = " + node + "\nnode2 = " + nodeCopy);
                        Assert.Fail();
                    }
                }
            }            
        }




    }
}
