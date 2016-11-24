using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using System.Collections.Generic;
using GRANTManager.TreeOperations;
using OSMElement;
using System.Windows;

namespace BrailleTreeTest
{
    [TestClass]
    public class ConnectedNodesTest
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

        [TestMethod]
        public void getBrailleNodeInGroupAtPointTest()
        {
            guiFuctions.deleteGrantTrees();
            initilaizeFilteredTree();


            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(25, 25);
            Assert.AreNotEqual(null, nodeAtPoint, "Es hätte ein Knoten gefunden werden sollen!");
            OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(nodeAtPoint);
            Assert.AreEqual("0", data.properties.valueFiltered, "An der Position (25,25) hätte der Button mit der Ziffer '0' sein sollen. Somit hätte der Value auch '0' sein sollen!");
            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod]
        public void getConnectedNodeToPoint()
        {            
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(25, 25);
            Assert.AreNotEqual(null, nodeAtPoint, "Es hätte ein Knoten gefunden werden sollen!");
            OSMElement.OSMElement dataBraille = strategyMgr.getSpecifiedTree().GetData(nodeAtPoint);
            OsmConnector<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(dataBraille.properties.IdGenerated));
            Assert.AreNotEqual(null, osmRelationships, "Es hätte ein zugehöriger Knoten im gefilterten Baum gefunden werden müssen.");

            OSMElement.OSMElement dataFiltere = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationships.FilteredTree);
            Assert.AreNotEqual(null, dataFiltere, "Es hätte ein Knoten im gefilterten Baum gefunden werden müssen.");
            Assert.AreNotEqual(new OSMElement.OSMElement(), dataFiltere, "Es hätte ein Knoten im gefilterten Baum gefunden werden müssen.");
            Assert.AreEqual("Button", dataFiltere.properties.controlTypeFiltered, "Der zugehörige Knoten hätte den Controlltype 'Button' haben müssen!");
            Assert.AreEqual("0", dataFiltere.properties.nameFiltered, "Der zugehörige Knoten hätte die Beschriftung '0' haben müssen.");
            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
        }

        /// <summary>
        /// Erstellt einen gefilterten Tree, welcher zum Testen der suchen genutzt werden kann
        /// </summary>
        private void initilaizeFilteredTree()
        {
            String moduleName = "calc.exe";
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(moduleName);
            grantTrees.setFilteredTree(strategyMgr.getSpecifiedFilter().filtering(appHwnd));
        }

    }
}
