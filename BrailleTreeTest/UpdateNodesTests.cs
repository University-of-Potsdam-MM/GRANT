﻿using BrailleTreeTests;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace GRANTManager.BrailleTreeTests
{
    [TestClass()]
    public class UpdateNodesTests
    {

        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;

        private String VIEWCATEGORYSYMBOLVIEW;
        private String VIEWCATEGORYLAYOUTVIEW;
        private String pathToTemplate;
        private String treePath;
        private String treePath2;

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
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            List<String> viewCategories = Settings.getPossibleTypesOfViews();
            if(viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];

            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUiGroups.xml");

            treePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SavedTrees");
            treePath2 = System.IO.Path.Combine(treePath, "calc2.grant");
            treePath= System.IO.Path.Combine(treePath, "calc.grant");
        }

        [TestMethod()]
        public void addNodeInBrailleTreeTest()
        {
            /*
             * Follgendes sollte der Fall sein.
             *      - der BrailleBaum ist am Anfang leer
             *      - es wird ein Knoten für den Screen hinzugefügt
             *      - es wird der eigentliche Knoten hinzugefügt
             *      - der Braille-Baum hat somit 2 Knoten (Kindbeziehung)
             *      - ID wurde generiert
             */
            Assert.AreEqual(null, grantTrees.brailleTree, "Der BrailleBaum sollte noch leer sein!");
            #region node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

            osm.brailleRepresentation.isVisible = true;
            osm.brailleRepresentation.screenName = "TestScreen";
            osm.brailleRepresentation.viewName = "TestView";
            osm.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm.properties.controlTypeFiltered = "Text";
            osm.properties.valueFiltered = "Test";
            #endregion
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            //Ebenen:  0. Root; 1. SymbolView; 2. Screen; 3. Inhalt
            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber "+strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)+" Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.brailleTree), "Der Root-Knoten muss gerde denau ein Kind haben. Er hat " + strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.brailleTree) + " Kinder!");
            object firstChildOfRoot = strategyMgr.getSpecifiedTree().Child(grantTrees.brailleTree);
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(firstChildOfRoot), "Das Kind von Root muss genau ein Kind haben. Es hat aber "+ strategyMgr.getSpecifiedTree().DirectChildCount(firstChildOfRoot) + " Kinder!");
            Assert.AreEqual(VIEWCATEGORYSYMBOLVIEW, strategyMgr.getSpecifiedTree().GetData(firstChildOfRoot).brailleRepresentation.typeOfView, "Das erste Kind von Root sollte eigentlich angeben, dass es eine SymbolView ist!");
            object firstChildOfChildOfRoot = strategyMgr.getSpecifiedTree().Child(firstChildOfRoot);
            Assert.AreEqual("TestScreen", strategyMgr.getSpecifiedTree().GetData(firstChildOfChildOfRoot).brailleRepresentation.screenName, "Das erste Kind von der 'SymbolView' sollte eigentlich angeben, dass es eine 'TestScreen' ist!");
            Assert.AreNotEqual(null, strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(firstChildOfChildOfRoot)).properties.IdGenerated, "Es muss eine Id vorhanden sein!");
        }
        [TestMethod]
        public void addFilteredNodeInBrailleTree_controlltype()
        {
            guiFuctions.addFilteredNodeToBrailleTree("Button");
            Assert.AreNotEqual(3, grantTrees.brailleTree, "It should be 3 nodes in the braille tree.");
        }

        [TestMethod]
        public void addFilteredNodeInBrailleTree_tree()
        {
            #region filtering tree
            String processName = "calc";
            String applicationPathName = @"C:\Windows\system32\calc.exe";
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(processName, applicationPathName);
            Assert.AreNotEqual(null, grantTrees.filteredTree);
            #endregion
            #region search node
            GeneralProperties searchProp = new GeneralProperties();
            searchProp.controlTypeFiltered = "Button";
            searchProp.nameFiltered = "2";
            List<Object> searchedTreeList = treeOperation.searchNodes.searchNodeByProperties(grantTrees.filteredTree, searchProp);
            Assert.AreNotEqual(0, searchedTreeList);
            Assert.AreEqual(1, searchedTreeList.Count);
            #endregion
            guiFuctions.addFilteredNodeToBrailleTree("Button", searchedTreeList[0]);
            Assert.AreNotEqual(3, grantTrees.brailleTree, "It should be 3 nodes in the braille tree.");
        }

        [TestMethod()]
        public void add2NodesOfSameScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.brailleTree, "Der BrailleBaum sollte noch leer sein!");
            #region 1. node

            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView -2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test 2";

            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(4, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 4 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
        }

        [TestMethod()]
        public void addSameNodesOfSameScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.brailleTree, "Der BrailleBaum sollte noch leer sein!");
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

            osm.brailleRepresentation.isVisible = true;
            osm.brailleRepresentation.screenName = "TestScreen";
            osm.brailleRepresentation.viewName = "TestView";
            osm.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm.properties.controlTypeFiltered = "Text";
            osm.properties.valueFiltered = "Test";

            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.brailleTree, "Der BrailleBaum sollte noch leer sein!");
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen - 2";
            osm2.brailleRepresentation.viewName = "TestView";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(5, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 5 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.brailleTree), "Das Root Element hätte ein Kind haben müssen");
            foreach(object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(strategyMgr.getSpecifiedTree().Child( grantTrees.brailleTree)))
            {
                Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(child));
            }
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentScreenAndViewCategoryInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.brailleTree);
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

            osm.brailleRepresentation.isVisible = true;
            osm.brailleRepresentation.screenName = "TestScreen";
            osm.brailleRepresentation.viewName = "TestView";
            osm.brailleRepresentation.typeOfView = VIEWCATEGORYLAYOUTVIEW;

            osm.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm.properties.controlTypeFiltered = "Text";
            osm.properties.valueFiltered = "Test";
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion

            #region 2. node
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen - 2";
            osm2.brailleRepresentation.viewName = "TestView";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 6 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
            Assert.AreEqual(2, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.brailleTree), "Das Root Element hätte zwei Kinder haben müssen");
            foreach (object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(child));
            }
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentViewsInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.brailleTree, "Der BrailleBaum sollte noch leer sein!");
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView - 2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.brailleTree, "Der BrailleBaum darf nun nicht mehr leer sein!");
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(4, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Der BrailleBaum hätte genau 4 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree) + " Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.brailleTree), "Das Root Element hätte ein Kind haben müssen");
            object symbolViewSubtree = strategyMgr.getSpecifiedTree().Child(grantTrees.brailleTree);
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(symbolViewSubtree), "Der SymbolView-Knoten hätte genau ein Kind haben müssen!");
            object screensubtree = strategyMgr.getSpecifiedTree().Child(symbolViewSubtree);
            Assert.AreEqual(2, strategyMgr.getSpecifiedTree().DirectChildCount(screensubtree), "Der Screen-Knoten hätte genau zwei Kinder haben müssen!");
            foreach (object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(screensubtree))
            {
                Assert.AreEqual(0, strategyMgr.getSpecifiedTree().DirectChildCount(child));
            }
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void addNavigationbarForScreenTest()
        {
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView -2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test 2";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().addNavigationbarForScreen(pathToTemplate, osm.brailleRepresentation.screenName, osm.brailleRepresentation.typeOfView);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 7 Knoten haben müssen!");
            bool foundNavbar = false;
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName!= null && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Equals("_NavigationBarScreens_groupElementsStatic_Count_1"))
                {
                    foundNavbar = true;
                    break;
                }
            }
            Assert.AreEqual(true, foundNavbar, "In dem Braille-Baum hätte die Navigationsleiste gefunden werden müssen!");
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void createNavigationbarForScreenTest()
        {
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView - 2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test 2";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().createNavigationbar(pathToTemplate, osm.brailleRepresentation.typeOfView);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 6 Knoten haben müssen!");
            bool foundNavbar = false;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName != null && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Equals("_NavigationBarScreens_groupElementsStatic_Count_1"))
                {
                    foundNavbar = true;
                    break;
                }
            }
            Assert.AreEqual(true, foundNavbar, "In dem Braille-Baum hätte die Navigationsleiste gefunden werden müssen!");
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void updateNavigationbarForScreenTest()
        {
            #region 1. node
            OSMElement.OSMElement osm = new OSMElement.OSMElement();

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
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();

            osm2.brailleRepresentation.isVisible = true;
            osm2.brailleRepresentation.screenName = "TestScreen";
            osm2.brailleRepresentation.viewName = "TestView - 2";
            osm2.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm2.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm2.properties.controlTypeFiltered = "Text";
            osm2.properties.valueFiltered = "Test - 2";
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().createNavigationbar(pathToTemplate, osm.brailleRepresentation.typeOfView);
            //Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 6 Knoten haben müssen!");
            #region 3. node
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();

            osm3.brailleRepresentation.isVisible = true;
            osm3.brailleRepresentation.screenName = "TestScreen - 3";
            osm3.brailleRepresentation.viewName = "TestView - 3";
            osm3.brailleRepresentation.typeOfView = VIEWCATEGORYSYMBOLVIEW;

            osm3.properties.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            osm3.properties.controlTypeFiltered = "Text";
            osm3.properties.valueFiltered = "Test 3";
            treeOperation.updateNodes.addNodeInBrailleTree(osm3);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().updateNavigationbarScreens(pathToTemplate, osm.brailleRepresentation.typeOfView);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(9, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 9 Knoten haben müssen!");

            Object subtreeNavbar = null;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName != null && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Equals("_NavigationBarScreens_groupElementsStatic_Count_2"))
                {
                    subtreeNavbar = node;
                    break;
                }
            }
            Assert.AreNotEqual(null, subtreeNavbar, "Es hätte ein Teilbaum für die Navigationsleiste gefunden werden müssen!");
            Assert.AreEqual(2, strategyMgr.getSpecifiedTree().DirectChildCount(subtreeNavbar), "Die Navigationsleiste hätte genau zwei Kindeelemente haben müssen!");
            guiFuctions.deleteGrantTrees();
        }

        #region setBrailleTreeProperty

        [TestMethod]
        public void setBrailleTreePropertyIntTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.brailleRepresentation.contrast, 230, "Im gespeicherten Baum hätte der Kontrast-Wert '230' sein sollen!");
            int contrastNew = 200;
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "contrast", contrastNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.contrast, contrastNew, "Der Kontrast-Wert sollte nun '" + contrastNew + "' sein sollen!");
        }

        [TestMethod]
        public void setBrailleTreePropertyStringTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.properties.valueFiltered, "Rechner", "Im gespeicherten Baum hätte der valueFiltered-Wert 'Rechner' sein sollen!");
            String valueFilteredNew = "Calc";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "valueFiltered", valueFilteredNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.properties.valueFiltered, valueFilteredNew, "Der valueFiltered-Wert sollte nun '" + valueFilteredNew + "' sein sollen!");
        }

        [TestMethod]
        public void setBrailleTreePropertydisplayedGuiElementTypeExistTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.brailleRepresentation.displayedGuiElementType, "nameFiltered", "Im gespeicherten Baum hätte der displayedGuiElementType-Wert 'nameFiltered' sein sollen!");
            String displayedGuiElementTypeNew = "helpTextFiltered";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "displayedGuiElementType", displayedGuiElementTypeNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.displayedGuiElementType, displayedGuiElementTypeNew, "Der displayedGuiElementType-Wert sollte nun '" + displayedGuiElementTypeNew + "' sein sollen!");
        }

        [TestMethod]
        public void setBrailleTreePropertydisplayedGuiElementTypeNotExistTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.brailleRepresentation.displayedGuiElementType, "nameFiltered", "Im gespeicherten Baum hätte der displayedGuiElementType-Wert 'nameFiltered' sein sollen!");
            String displayedGuiElementTypeNew = "notExistProperty";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "displayedGuiElementType", displayedGuiElementTypeNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy, node, "Both nodes should have the same values.");
            Assert.AreEqual(node.brailleRepresentation.displayedGuiElementType, "nameFiltered", "Im gespeicherten Baum hätte der displayedGuiElementType-Wert 'nameFiltered' sein sollen!");
        }

        [TestMethod]
        public void setBrailleTreePropertyTest_NotExistingProperty()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.properties.valueFiltered, "Rechner", "Im gespeicherten Baum hätte der valueFiltered-Wert 'Rechner' sein sollen!");
            String propertyNew = "Calc";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "notExistPropertyName", propertyNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual( nodeCopy, node, "Both nodes should have the same values.");
        }

        [TestMethod]
        public void setBrailleTreePropertyIdTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.properties.IdGenerated, nodeId);
            String IdNew = "NewIdValue";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "IdGenerated", IdNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy.properties.IdGenerated, node.properties.IdGenerated, "Both Id's should have the same values.");
            Assert.AreEqual(nodeCopy, node, "Both nodes should have the same values.");  
        }

        [TestMethod]
        public void setBrailleTreePropertyViewNameTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(node.brailleRepresentation.viewName, "TitleBar");
            String viewNameNew = "viewNameNew";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "viewName", viewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.viewName, viewNameNew, "Der viewName-Wert sollte nun '" + viewNameNew + "' sein sollen!");
        }

        [TestMethod]
        public void setBrailleTreePropertyViewNameExistTest()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null, "Das grant-Object ist leer!");
            Assert.AreNotEqual(grantTrees.filteredTree, null, "Das filteredTree-Object ist leer!");
            Assert.AreNotEqual(grantTrees.brailleTree, null, "Das brailleTree-Object ist leer!");
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            Object nodeObject = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            OSMElement.OSMElement nodeCopy = nodeData.DeepCopy();
            Assert.AreNotEqual(nodeData, new OSMElement.OSMElement(), "Es wurde kein Knoten gefunden!");
            Assert.AreEqual(nodeData.brailleRepresentation.viewName, "TitleBar");
            Assert.IsTrue(strategyMgr.getSpecifiedTree().HasNext(nodeObject));
            Object nextNode = strategyMgr.getSpecifiedTree().Next(nodeObject);
            OSMElement.OSMElement nextNodeData = strategyMgr.getSpecifiedTree().GetData(nextNode);
            String viewNameNew = nextNodeData.brailleRepresentation.viewName;
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "viewName", viewNameNew);
            Assert.IsFalse(result);
            nodeData = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy, nodeData, "Both nodes should have the same values.");
            Assert.AreNotEqual(nodeData.brailleRepresentation.viewName, viewNameNew);
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenNameTest()
        {
            /* 
             * Moves a view to a new screen.
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0"; //Node id of the braille node which is connected to the 'titlebar' -> it's a view in the 'layout view'
            Object nodeObjectOldParent_Lv = strategyMgr.getSpecifiedTree().Parent( treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree)).DeepCopy();
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.screenName, "lv");
            String screenNameNew = "ScreenNameNew";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew); // => move the view to a new screen
            Assert.IsTrue(result);
            Object nodeObjectNew = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree).DeepCopy();
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.screenName, screenNameNew, "Der screenName-Wert sollte nun '" + screenNameNew + "' sein sollen!");
            Object nodeObjectNewParent_Lv = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(nodeObjectOldParent_Lv).properties.IdGenerated, grantTrees.brailleTree);
            Object nodeObjectNewParent_ScreenNameNew = strategyMgr.getSpecifiedTree().Parent( treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(nodeObjectNewParent_Lv).properties.IdGenerated, grantTrees.brailleTree));
            Object nodeObjectOldParent_ScreenNameNew = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(nodeObjectNewParent_ScreenNameNew).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(nodeObjectOldParent_Lv));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(nodeObjectOldParent_Lv) > strategyMgr.getSpecifiedTree().Count(nodeObjectNewParent_Lv));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(nodeObjectNewParent_ScreenNameNew) > strategyMgr.getSpecifiedTree().Count(nodeObjectOldParent_ScreenNameNew));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(nodeObjectOldParent_Lv) - strategyMgr.getSpecifiedTree().Count(nodeObjectNewParent_Lv) + strategyMgr.getSpecifiedTree().Count(nodeObjectOldParent_ScreenNameNew) == strategyMgr.getSpecifiedTree().Count(nodeObjectNewParent_ScreenNameNew));
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenName_ScreenTest()
        {
            /*
             * Rename a screen and all children
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "0FCC24CAB3C124C7E6D010E55E91B195";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.screenName, "lv");
            String screenNameNew = "screenNameNew";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.screenName, screenNameNew, "Der screenName-Wert sollte nun '" + screenNameNew + "' sein sollen!");
            Object subscreenNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach(object o in strategyMgr.getSpecifiedTree().AllNodes(subscreenNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(screenNameNew, data.brailleRepresentation.screenName);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenName_ScreenExistTest()
        {
            /*
             * Try to rename a screen (screen-branch) (and all children) --> it dosn't work because it already exists a screen with the same name wich has some views with the same name
             */
            guiFuctions.loadGrantProject(treePath2);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "64258A7F603E99810EC9D9CC836D8087";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.screenName, "b");
            String screenNameNew = "a1";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy, node, "Both nodes should have the same values.");
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy)) 
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenName_ScreenExistTest2()
        {
            /* 
             * Integrate a screen-branch by rename a screen (screen-branch) (and all children) --> the new screen-branch already exists  BUT all all views has different names
             */
            guiFuctions.loadGrantProject(treePath2);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "64258A7F603E99810EC9D9CC836D8087";
            //del node --> this view-names are exist in the new screen-branch
            Boolean result = treeOperation.updateNodes.removeNodeInBrailleTree("5D91B25288C6011F4591D0CB66C5CE78"); //TilteBar
            Assert.IsTrue(result);
            result = treeOperation.updateNodes.removeNodeInBrailleTree("FFEBEB75CD31AF733A6E5F3A2279DC3C"); //NavigationBar
            Assert.IsTrue(result);
            result = treeOperation.updateNodes.removeNodeInBrailleTree("AEB7F45DB342BE948B1257BC247F7C36"); //StatusBar
            Assert.IsTrue(result);
            String oldScreenName = "b";
            Object nodeObjectScreen_old = treeOperation.searchNodes.getSubtreeOfScreen(oldScreenName).DeepCopy();
            String screenNameNew = "a1";
            Object nodeObjectNewScreen_old = treeOperation.searchNodes.getSubtreeOfScreen(screenNameNew).DeepCopy();
            OSMElement.OSMElement nodeData = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeData, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(nodeData.brailleRepresentation.screenName, oldScreenName);
            
            result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew);
            Assert.IsTrue(result);
            Object nodeObjectNewScreen_new = treeOperation.searchNodes.getSubtreeOfScreen(screenNameNew);
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(nodeObjectScreen_old) + strategyMgr.getSpecifiedTree().Count(nodeObjectNewScreen_old) - 1, strategyMgr.getSpecifiedTree().Count(nodeObjectNewScreen_new));
            foreach(String screenName in treeOperation.searchNodes.getPosibleScreenNames(strategyMgr.getSpecifiedTree().GetData(nodeObjectScreen_old).brailleRepresentation.typeOfView))
            {
                Assert.IsFalse(screenName.Equals(nodeObjectScreen_old));
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenName_ViewExistTest()
        {
            /*
             * Try to move a view to an other existing screen --> it dosn't work because in the existing screen a view with the same name already exist
             */
            guiFuctions.loadGrantProject(treePath2);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "5D91B25288C6011F4591D0CB66C5CE78";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.screenName, "b");
            String screenNameNew = "a1";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy, node, "Both nodes should have the same values.");
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyScreenName_ViewNotExistTest()
        {
            /*
             * Move a view to an other screen
             */
            guiFuctions.loadGrantProject(treePath2);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "B3E21CB6354236C12E53DD34DC343A45";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.screenName, "b");
            String screenNameNew = "a1";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", screenNameNew);
            Assert.IsTrue(result);
            Debug.WriteLine("Neuer Baum:\n " + strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes should have the same values.");
            if (strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail();
            }
            Object subtreeBOld = strategyMgr.getSpecifiedTree().Parent( treeOperation.searchNodes.getNode(nodeId, treeCopy));
            Object subtreeBNew = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(subtreeBOld).properties.IdGenerated, grantTrees.brailleTree);

            Object subtreeA1New = strategyMgr.getSpecifiedTree().Parent(treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree));
            Object subtreeA1Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(subtreeA1New).properties.IdGenerated, treeCopy);

            
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(subtreeBOld) > strategyMgr.getSpecifiedTree().Count(subtreeBNew));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(subtreeA1New) > strategyMgr.getSpecifiedTree().Count(subtreeA1Old));
            Assert.AreEqual((strategyMgr.getSpecifiedTree().Count(subtreeBOld) - strategyMgr.getSpecifiedTree().Count(subtreeBNew)) + strategyMgr.getSpecifiedTree().Count(subtreeA1Old), strategyMgr.getSpecifiedTree().Count(subtreeA1New));
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_Rename()
        {
            /*
             * Rename a typeOfView and all children
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "CF317E437395E0F51B06FF208F4A905C";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "typeOfViewNameNew";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, typeOfViewNameNew, "The new value of 'typeOfView be should now '"+typeOfViewNameNew + "'");
            Object subscreenNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subscreenNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_newTypeOfView()
        {
            /*
             * Move the view-node to a new typeOfView
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "typeOfViewNameNew";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Object subTreeTypeOfViewNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subTreeTypeOfViewNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveScreen()
        {
            /*
             * Move a screen-node (and all children) to an other existing typeOfView (the screen-node doesn't exist in this typeOfView)
             */
             guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "0FCC24CAB3C124C7E6D010E55E91B195"; //screen-node
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "SymbolView";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Object subTreeTypeOfScreenNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subTreeTypeOfScreenNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveScreen2()
        {
            /*
             * Move a screen-node (and all children) to an other existing typeOfView (the screen-node exist in this typeOfView BUT all view-names are different)
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "B2013A50995FC1DC806B5BB29DEDC18A"; //screen-node
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            
            //change screenName
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", "lv");
            Assert.IsTrue(result);
            //del node
            result = treeOperation.updateNodes.removeNodeInBrailleTree("4B84E4829D9CEFB70618B14F87589030");
            Assert.IsTrue(result);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            String typeOfViewNameOld = "SymbolView";
            Assert.AreEqual(node.brailleRepresentation.typeOfView, typeOfViewNameOld);
            object nodeObject_old = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree).DeepCopy();
            Object patentObject_old = strategyMgr.getSpecifiedTree().Parent(nodeObject_old);
            String typeOfViewNameNew = "LayoutView";
            result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            Object subTreeTypeOfViewNew_New = treeOperation.searchNodes.getSubtreeOfTypeOfView(typeOfViewNameNew);
            Object subtreeTypeOfViewNew_Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(subTreeTypeOfViewNew_New).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(nodeObject_old));
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(subtreeTypeOfViewNew_Old) + strategyMgr.getSpecifiedTree().Count(nodeObject_old) -1, strategyMgr.getSpecifiedTree().Count(subTreeTypeOfViewNew_New)); // -1 => the screen-node is used only once
            Object subTreeScreenNew = treeOperation.searchNodes.getSubtreeOfScreen("lv");
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subTreeTypeOfViewNew_New))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
            //Each view-node from the old "typeOfView/Screen" must be in the new one
            foreach (Object o in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject_old))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Boolean bt = treeOperation.searchNodes.existViewInScreen(data.brailleRepresentation.screenName, data.brailleRepresentation.viewName, typeOfViewNameNew);
                Assert.IsTrue( treeOperation.searchNodes.existViewInScreen(data.brailleRepresentation.screenName, data.brailleRepresentation.viewName, typeOfViewNameNew));
            }
            
            foreach(String s in treeOperation.searchNodes.getUsedTypesOfViews())
            {
                Assert.AreNotEqual(typeOfViewNameOld, s);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveScreen3()
        {
            /*
             * Try to move a screen-node (and all children) to an other existing typeOfView (the screen-node exist in this typeOfView AND SOME view-names are EQUAL)
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "B2013A50995FC1DC806B5BB29DEDC18A"; //screen-node
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);

            //change screenName
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", "lv");
            Assert.IsTrue(result);
            //del node
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            String typeOfViewNameOld = "SymbolView";
            Assert.AreEqual(node.brailleRepresentation.typeOfView, typeOfViewNameOld);
            object nodeObject_old = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree).DeepCopy();
            Object patentObject_old = strategyMgr.getSpecifiedTree().Parent(nodeObject_old);
            String typeOfViewNameNew = "LayoutView";
            result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsFalse(result);
            Object subTreeTypeOfViewNew_New = treeOperation.searchNodes.getSubtreeOfTypeOfView(typeOfViewNameNew);
            Object subtreeTypeOfViewNew_Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(subTreeTypeOfViewNew_New).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(nodeObject_old));
            if (!strategyMgr.getSpecifiedTree().Equals(subTreeTypeOfViewNew_New, subtreeTypeOfViewNew_Old))
            {
                Assert.Fail();
            }
            Object nodeObject_new = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            if (!strategyMgr.getSpecifiedTree().Equals(nodeObject_new, nodeObject_old))
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveView()
        {
            /*
             * Move a view-node to an other existing typeOfView (the screen-node doesn't exist in this typeOfView)
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            Object nodeObject = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            OSMElement.OSMElement node = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            
            Object parentParentOld_old = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObject)).DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "SymbolView";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Object subTreeTypeOfScreenNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subTreeTypeOfScreenNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
            Object parentParentOld_new = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentOld_old).properties.IdGenerated, grantTrees.brailleTree);
            Object nodeObjectNew = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            Object parentParentNew_New = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObjectNew));
            Object parentParentNew_Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentNew_New).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(parentParentOld_old));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentOld_old) == 1 + strategyMgr.getSpecifiedTree().Count(parentParentOld_new));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentNew_Old) + 2 ==strategyMgr.getSpecifiedTree().Count(parentParentNew_New)); // 2 -> view + screen

        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveView2()
        {
            /*
             * Move a view-node to an other existing typeOfView (the screen-node doesn't exist in this typeOfView), afer remove all but one node of this screen
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            Object nodeObject = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            OSMElement.OSMElement node = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();

            // remove all but one nodes of this screen
            while(strategyMgr.getSpecifiedTree().HasPrevious(nodeObject))
            {
                treeOperation.updateNodes.removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Previous(nodeObject)).properties.IdGenerated);
            }
            while (strategyMgr.getSpecifiedTree().HasNext(nodeObject))
            {
                treeOperation.updateNodes.removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Next(nodeObject)).properties.IdGenerated);
            }
            Object parentParentOld_old = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObject)).DeepCopy();
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "SymbolView";
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            Object subTreeTypeOfScreenNew = treeOperation.searchNodes.getNode(node.properties.IdGenerated, grantTrees.brailleTree);
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(subTreeTypeOfScreenNew))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
            Object parentParentOld_new = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentOld_old).properties.IdGenerated, grantTrees.brailleTree);
            Object nodeObjectNew = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            Object parentParentNew_New = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObjectNew));
            Object parentParentNew_Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentNew_New).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(parentParentOld_old));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentOld_old) == 2 + strategyMgr.getSpecifiedTree().Count(parentParentOld_new)); // 2 --< view + screen
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentNew_Old) + 2 == strategyMgr.getSpecifiedTree().Count(parentParentNew_New));

        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_moveView3()
        {
            /*
             * Move a view-node to an other existing typeOfView (the screen-node exist in this typeOfView, but in this screen-branch doesn't exist this view-node)
             */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "C0CF02BD3B3567C92BA4A62B09209ACF";
            Object nodeObject = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            OSMElement.OSMElement node = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", "sv"); // rename the 'screenName' --> so the screen with this view exist in the typeOfView (SymbolView) --> but not the view
            Assert.IsTrue(result);
            nodeObject = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Object parentParentOld_old = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObject)).DeepCopy(); // => typeOfView-node
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            String typeOfViewNameNew = "SymbolView";
            result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsTrue(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreNotEqual(nodeCopy, node, "Both nodes shouldn't have the same values.");
            
            Object parentParentOld_new = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentOld_old).properties.IdGenerated, grantTrees.brailleTree);
            Object nodeObjectNew = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            Object parentParentNew_New = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObjectNew));
            Object parentParentNew_Old = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentParentNew_New).properties.IdGenerated, strategyMgr.getSpecifiedTree().Root(parentParentOld_old));
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentOld_old) == 1 + strategyMgr.getSpecifiedTree().Count(parentParentOld_new)); // 1 --> view 
            Assert.IsTrue(strategyMgr.getSpecifiedTree().Count(parentParentNew_Old) + 1 == strategyMgr.getSpecifiedTree().Count(parentParentNew_New));
            foreach (object o in strategyMgr.getSpecifiedTree().AllNodes(parentParentNew_New))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                Assert.AreEqual(typeOfViewNameNew, data.brailleRepresentation.typeOfView);
            }
        }

        [TestMethod]
        public void setBrailleTreePropertyTypeOfView_viewExists()
        {
            /*
            * Try to move a view to an other existing typeOfView  --> it doesn't work because in the existing typeOfView exist a screen-view-combination with the same name
            */
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            Assert.AreNotEqual(grantTrees.brailleTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0"; //TODO: (Eltern-)Screen vorher Umbenennen, damit der Fall eintrifft
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            
            Assert.AreNotEqual(node, new OSMElement.OSMElement(), "Cann't find a node!");
            Assert.AreEqual(node.brailleRepresentation.typeOfView, "LayoutView");
            Boolean result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "screenName", "sv"); // rename the 'screenName' --> so the screen with this view exist in the typeOfView (SymbolView)
            Assert.IsTrue(result);
            OSMElement.OSMElement nodeCopy = node.DeepCopy();
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            String typeOfViewNameNew = "SymbolView";
            result = treeOperation.updateNodes.setBrailleTreeProperty(nodeId, "typeOfView", typeOfViewNameNew);
            Assert.IsFalse(result);
            node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.AreEqual(nodeCopy, node, "Both nodes should have the same values.");
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail();
            }
        }
        #endregion

        [TestMethod]
        public void getProperty_Exist_NotNull_Test()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            String propName = "valueFiltered";
            Assert.IsTrue(SearchNodes.existPropertyName(propName));
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            String value = UpdateNodes.getProperty(propName, node);
            Assert.AreEqual(node.properties.valueFiltered, value);
        }

        [TestMethod]
        public void getProperty_Exist_NotNull2_Test()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            String propName = "valueFiltered";
            Assert.IsTrue(SearchNodes.existPropertyName(propName));
            String value = treeOperation.updateNodes.getPropertyofBrailleTree(propName, nodeId);
            Assert.AreEqual(node.properties.valueFiltered, value);
        }

        [TestMethod]
        public void getProperty_Exist_Null_Test()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            Assert.IsNull(node.properties.helpTextFiltered);
            String propName = "helpTextFiltered";
            String value = UpdateNodes.getProperty(propName, node);
            Assert.IsTrue(SearchNodes.existPropertyName(propName));
            Assert.AreEqual("", value);
        }

        [TestMethod]
        public void getProperty_NotExist_Test()
        {
            guiFuctions.loadGrantProject(treePath);
            Assert.AreNotEqual(grantTrees, null);
            Assert.AreNotEqual(grantTrees.filteredTree, null);
            String nodeId = "766D7B8425177D724B967DE5A55198F0";
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(nodeId);
            String propName = "NotExistProperty";
            String value = UpdateNodes.getProperty(propName, node);
            Assert.IsFalse(SearchNodes.existPropertyName(propName));
            Assert.AreEqual("", value);
        }
    }
}