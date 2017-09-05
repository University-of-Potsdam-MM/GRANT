using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Diagnostics;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestSearchNodes
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
            strategyMgr.setSpecifiedEventStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
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
        public void TestGetIdFilteredNodeByHwnd()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            Assert.IsNotNull(grantTrees.filteredTree);
            
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                OSMElements.OSMElement osmData = strategyMgr.getSpecifiedTree().GetData(node);
                String foundId = treeOperation.searchNodes.getIdFilteredNodeByHwnd(osmData.properties.hWndFiltered);
                if(IntPtr.Zero.Equals(osmData.properties.hWndFiltered))
                {
                    Assert.AreEqual(null, foundId);
                }else
                {
                    Assert.AreEqual(osmData.properties.IdGenerated, foundId);
                }
            }
        }

        [TestMethod]
        public void TestGetNodeByProperties()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            Assert.IsNotNull(grantTrees.filteredTree);

            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                OSMElements.OSMElement osmData = strategyMgr.getSpecifiedTree().GetData(node);
                OSMElements.OSMElement osmDataWithoutId = osmData.DeepCopy();
                osmDataWithoutId.properties.resetIdGenerated = null;
                List<Object> foundObjects = treeOperation.searchNodes.getNodesByProperties(grantTrees.filteredTree, osmDataWithoutId.properties);
                Assert.AreEqual(1, foundObjects.Count);
                Assert.IsTrue(strategyMgr.getSpecifiedTree().Equals(node, foundObjects[0]));
            }
        }
    }
}
