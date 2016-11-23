using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager.TreeOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;
using System.Windows;

namespace GRANTManager.TreeOperations.Tests
{
    [TestClass()]
    public class SearchNodesTests
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;

        private String VIEWCATEGORYSYMBOLVIEW;
        private String VIEWCATEGORYLAYOUTVIEW;

        [TestInitialize]
        public void Initialize()
        {
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
            /* strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
             strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
             strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);*/
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            List<String> viewCategories = Settings.getPossibleViewCategories();
            if (viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];
            
        }

        /// <summary>
        /// Erstellt einen Braille-Tree, welcher zum Testen der suchen genutzt werden kann
        /// </summary>
        private void initilaizeBrailleTree2Screens()
        {
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm, VIEWCATEGORYSYMBOLVIEW);
            #endregion

            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen - 2";
            braille2.viewName = "TestView";

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2, VIEWCATEGORYSYMBOLVIEW);
            #endregion
        }

        /// <summary>
        /// Erstellt einen gefilterten Tree, welcher zum Testen der suchen genutzt werden kann
        /// </summary>
        private void initilaizefilteredTree()
        {

        }


        [TestMethod()]
        public void getPosibleScreenNamesTest()
        {
            initilaizeBrailleTree2Screens();
            List<String> possibleScreens = treeOperation.searchNodes.getPosibleScreenNames();
            Assert.AreEqual(2, possibleScreens.Count, "Der Baum hätte 2 Screens enthalten müssen!");
        }

        [TestMethod()]
        public void getUsedViewCategoriesTest()
        {
            initilaizeBrailleTree2Screens();
            List<String> usedViewCategories = treeOperation.searchNodes.getUsedViewCategories();
            Assert.AreEqual(1, usedViewCategories.Count, "Der Baum hätte 1 Ansicht enthalten müssen!");
            List<String> possibleViewCategories = Settings.getPossibleViewCategories();
            Assert.IsTrue(usedViewCategories.Count <= possibleViewCategories.Count, "Es dürfen nicht mehr Ansichten (viewCategory) genutzt werden als in der Config definiert!");
           foreach(String uVC in usedViewCategories)
            {
                Assert.IsTrue(possibleViewCategories.Contains(uVC), "Der Screen Name '" + uVC + " hätte in der Liste der möglichen Screens aus der Config auftauchen müssen!");
            }
        }
    }
}