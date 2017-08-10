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
    public class UniquePropertiesTests
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
            strategyMgr.setSpecifiedEventManagerStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
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

            List<String> viewCategories = Settings.getPossibleTypesOfViews();
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
            String processName = "calc";
            String applicationPathName = @"C:\Windows\system32\calc.exe";
            /* IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(moduleName);
             grantTrees.setFilteredTree(strategyMgr.getSpecifiedFilter().filtering(appHwnd));*/
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(processName, applicationPathName);
        }


        [TestMethod]
        public void generatedIdsOfBrailleTreeUniqueTest()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            String nodeId;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.brailleTree))
            {
                nodeId = strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated;
                Assert.AreNotEqual(null, nodeId, "Id is missing! Node:\n" + node);
                foreach (Object nodeCopy in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.brailleTree))
                {
                    if(!strategyMgr.getSpecifiedTree().GetData(node).Equals(strategyMgr.getSpecifiedTree().GetData(nodeCopy)) && nodeId.Equals(strategyMgr.getSpecifiedTree().GetData(nodeCopy).properties.IdGenerated))
                    {
                        Assert.Fail("same ID :(\n node1 = " + node + "\nnode2 = " + nodeCopy);
                    }
                }
            }
        }

        [TestMethod]
        public void uniqueScreenNames()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            HashSet<String> screenNames = new HashSet<string>();
            Debug.WriteLine("\nBraille-Tree:\n"+ strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            foreach(Object typeOfView in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                foreach(Object screen in strategyMgr.getSpecifiedTree().DirectChildrenNodes(typeOfView))
                {
                    bool isUnique = screenNames.Add(strategyMgr.getSpecifiedTree().GetData(screen).brailleRepresentation.screenName);
                    Assert.IsTrue(isUnique, "The screen '" + strategyMgr.getSpecifiedTree().GetData(screen).brailleRepresentation.screenName + "' isn't unique!");
                }
            }            
        }

        [TestMethod]
        public void uniqueScreenNames_AddExistingScreenName()
        {
            initilaizeFilteredTree();
            Assert.IsNull(grantTrees.brailleTree);
            guiFuctions.addFilteredNodeToBrailleTree("screenName_1", "typeOfView_1", "viewName_1");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count( grantTrees.brailleTree)); // 3 => typeOfView-Node + Screen-Node + View-Node
            String idResult = guiFuctions.addFilteredNodeToBrailleTree("screenName_1", "typeOfView_2", "viewName_2"); // Same screenName
            Assert.IsNull(idResult);
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // the node wasn't added
        }
    }
}
