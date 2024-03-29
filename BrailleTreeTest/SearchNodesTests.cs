﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager.TreeOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElements;
using System.Windows;
using System.Diagnostics;
using GRANTManager;

namespace BrailleTreeTests
{
    [TestClass()]
    public class SearchNodesTests
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
            strategyMgr.setSpecifiedEventStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            /* strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
             strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
             strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);*/
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
        /// Erstellt einen Braille-Tree, welcher zum Testen der suchen genutzt werden kann
        /// </summary>
        private void initilaizeBrailleTree2Screens()
        {
            #region 1. node
            OSMElements.OSMElement osm = new OSMElements.OSMElement();

            osm.brailleRepresentation.isVisible = true;
            osm.brailleRepresentation.screenName = "TestScreen";
            osm.brailleRepresentation.viewName = "TestView";
            osm.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm.properties.controlTypeFiltered = "Text";
            osm.properties.valueFiltered = "Test";

            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion

            #region 2. node
            OSMElements.OSMElement osm2 = new OSMElements.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen -2";
            osm2.brailleRepresentation.viewName = "TestView";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
        }



        [TestMethod()]
        public void getPosibleScreenNamesTest()
        {
            initilaizeBrailleTree2Screens();
            List<String> possibleScreens = treeOperation.searchNodes.getPosibleScreenNames();
            Assert.AreEqual(2, possibleScreens.Count, "Der Baum hätte 2 Screens enthalten müssen!");
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void getUsedViewCategoriesTest()
        {
            initilaizeBrailleTree2Screens();
            List<String> usedViewCategories = treeOperation.searchNodes.getUsedTypesOfViews();
            Assert.AreEqual(1, usedViewCategories.Count, "Der Baum hätte 1 Ansicht enthalten müssen!");
            List<String> possibleViewCategories = Settings.getPossibleTypesOfViews();
            Assert.IsTrue(usedViewCategories.Count <= possibleViewCategories.Count, "Es dürfen nicht mehr Ansichten (typeOfView) genutzt werden als in der Config definiert!");
           foreach(String uVC in usedViewCategories)
            {
                Assert.IsTrue(possibleViewCategories.Contains(uVC), "Der Screen Name '" + uVC + " hätte in der Liste der möglichen Screens aus der Config auftauchen müssen!");
            }
            guiFuctions.deleteGrantTrees();
        }



        [TestMethod]
        public void getBrailleNodeAtPointTest()
        {
            initilaizeBrailleTree2Nodes();
            strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
            strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen("TestScreen");
            Object nodeAtPoint = guiFuctions.getBrailleNodeAtPoint(5, 35);
            Assert.AreNotEqual(null, nodeAtPoint, "Es hätte ein Knoten gefunden werden sollen!");
            OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(nodeAtPoint);
            Assert.AreEqual("TestView - 2", data.brailleRepresentation.viewName, "An der Position (5,35) hätte die 'TestView - 2' sein sollen!");
            strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
            guiFuctions.deleteGrantTrees();
        }


        private void initilaizeBrailleTree2Nodes()
        {
            #region 1. node
            OSMElements.OSMElement osm = new OSMElements.OSMElement();

            osm.brailleRepresentation.isVisible = true;
            osm.brailleRepresentation.screenName = "TestScreen";
            osm.brailleRepresentation.viewName = "TestView";
            osm.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm.properties.controlTypeFiltered = "Text";
            osm.properties.valueFiltered = "Test";
            
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            #region 2. node
            OSMElements.OSMElement osm2 = new OSMElements.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView - 2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test 2";

            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
        }

    }
}