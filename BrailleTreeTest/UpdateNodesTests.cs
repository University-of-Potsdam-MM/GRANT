using BrailleTreeTests;
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
            treePath = System.IO.Path.Combine(treePath, "calc.grant");
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