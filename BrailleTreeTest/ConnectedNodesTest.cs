using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using System.Collections.Generic;
using GRANTManager.TreeOperations;
using OSMElement;
using System.Windows;

namespace BrailleTreeTests
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

            List<String> viewCategories = Settings.getPossibleTypesOfViews();
            if (viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];

            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUiGroups.xml");

        }

        [TestMethod]
        public void getBrailleNodeInGroupAtPointTest()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            // auf dem Screen 'a1' befindet sich der gesuchte Knoten
            strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen("a1");
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(25, 25);
            Assert.AreNotEqual(null, nodeAtPoint, "Es hätte ein Knoten gefunden werden sollen!");
            OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(nodeAtPoint);
            Assert.AreEqual("0", data.properties.valueFiltered, "An der Position (25,25) hätte der Button mit der Ziffer '0' sein sollen. Somit hätte der Value auch '0' sein sollen!");
            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod]
        public void getConnectedNodeToPointTest()
        {            
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            // auf dem Screen 'a1' befindet sich der gesuchte Knoten
            strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen("a1");
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(25, 25);
            Assert.AreNotEqual(null, nodeAtPoint, "Es hätte ein Knoten gefunden werden sollen!");
            OSMElement.OSMElement dataBraille = strategyMgr.getSpecifiedTree().GetData(nodeAtPoint);
            OsmTreeConnectorTuple<String, String> osmRelationships = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(dataBraille.properties.IdGenerated));
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
            String processName = "calc";
            String applicationPathName = @"C:\Windows\system32\calc.exe";
            /* IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(moduleName);
             grantTrees.setFilteredTree(strategyMgr.getSpecifiedFilter().filtering(appHwnd));*/
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(processName, applicationPathName);
        }


        [TestMethod]
        public void getScreenshotPointInApplicationTest()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            //in dem genutzten Template ist bei der View 'lv' der Screenshot
            strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen("lv"); 
            int pointX = 6;
            int pointY = 42;
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(pointX, pointY);
            int clickX;
            int clickY;
            guiFuctions.getScreenshotPointInApplication(nodeAtPoint, pointX, pointY, out clickX, out clickY);
            //nun folgt der Abgleich, ob die richtige Position ermittelt wurde => es sollte der Button '7' auf dem Taschenrechner sein
            OSMElement.OSMElement dataOfPoint = strategyMgr.getSpecifiedFilter().getOSMElement(clickX, clickY);
            Assert.AreEqual("Button", dataOfPoint.properties.controlTypeFiltered, "Es hätte der Button sein sollen!");
            Assert.AreEqual("7", dataOfPoint.properties.nameFiltered, "auf dem Button hätte die Zahl '7' stehen müssen!");
            List<Object> searchresult = treeOperation.searchNodes.searchNodeByProperties(grantTrees.filteredTree, dataOfPoint.properties);
            Assert.AreNotEqual(null, searchresult, "Es hätte ein Knoten im gefilterten Baum gefunden werden müssen!");
            Assert.AreNotEqual(new List<Object>(), searchresult, "Es hätte ein Knoten im gefilterten Baum gefunden werden müssen!");
            Assert.AreEqual(1, searchresult.Count, "Es hätte genau ein Knoten gefunden werden müssen!");
            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod]
        public void getScreenshotPointInApplication_FailureTest()
        {
            initilaizeFilteredTree();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(pathToTemplate);
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            //in dem genutzten Template ist bei der View 'a1' kein Screenshot
            strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen("a1");
            int pointX = 6;
            int pointY = 42;
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(pointX, pointY);
            Assert.AreNotEqual(null, nodeAtPoint);
            int clickX;
            int clickY;
            guiFuctions.getScreenshotPointInApplication(nodeAtPoint, pointX, pointY, out clickX, out clickY);
            Assert.AreEqual(-1, clickX, "An der Stelle hätte kein Screenshot sein, dürfen, deshalb hätte auch keine Position (x) ermittelt werden dürfen!");
            Assert.AreEqual(-1, clickY, "An der Stelle hätte kein Screenshot sein, dürfen, deshalb hätte auch keine Position (y) ermittelt werden dürfen!");

            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
            
        }
    }
}
