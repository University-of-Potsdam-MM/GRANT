using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GRANTManager;
using GRANTManager.TreeOperations;
using BrailleTreeTests;
using System.Diagnostics;

namespace BrailleTreeTest
{
    [TestClass]
    public class UniqueIdsTests
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String pathToTemplate;

        private String VIEWCATEGORYSYMBOLVIEW;
        private String VIEWCATEGORYLAYOUTVIEW;

        [TestInitialize]
        public void Initialize()
        {
            #region initialisieren
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();
            List<Strategy> str = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
            #endregion

            List<String> viewCategories = Settings.getPossibleViewCategories();
            if (viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];

            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUiGroups.xml");

        }
        /// <summary>
        /// Erstellt einen gefilterten Tree, welcher zum Testen der genutzt werden kann
        /// </summary>
        private void initilaizeFilteredTree()
        {
            String moduleName = "calc.exe";
            String applicationPathName = @"C:\Windows\system32\calc.exe";
            /* IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(moduleName);
             grantTrees.setFilteredTree(strategyMgr.getSpecifiedFilter().filtering(appHwnd));*/
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(moduleName, applicationPathName);
        }


        [TestMethod]
        public void generatedIdsOfBrailleTreeUniqueTest()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            String nodeId;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getBrailleTree()))
            {
                nodeId = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
                Assert.AreNotEqual(null, nodeId, "Es hätte eine Id vorhanden sein müssen. Betrachteter Knoten:\n" + node);
                foreach (Object nodeCopy in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getBrailleTree()))
                {
                    if (!strategyMgr.getSpecifiedTree().Equals(node, nodeCopy) && nodeId.Equals(strategyMgr.getSpecifiedTree().GetData(nodeCopy).properties.IdGenerated))
                    {
                        Assert.Fail("selbe ID :(\n node1 = " + node + "\nnode2 = " + nodeCopy);
                    }
                }
            }
            // guiFuctions.deleteGrantTrees();
        }
    }
}
