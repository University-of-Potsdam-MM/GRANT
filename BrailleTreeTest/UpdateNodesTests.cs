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
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            List<String> viewCategories = Settings.getPossibleViewCategories();
            if(viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];

            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUiGroups.xml");
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
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            //Ebenen:  0. Root; 1. SymbolView; 2. Screen; 3. Inhalt
            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber "+strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree())+" Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()), "Der Root-Knoten muss gerde denau ein Kind haben. Er hat " + strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()) + " Kinder!");
            object firstChildOfRoot = strategyMgr.getSpecifiedTree().Child(grantTrees.getBrailleTree());
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(firstChildOfRoot), "Das Kind von Root muss genau ein Kind haben. Es hat aber "+ strategyMgr.getSpecifiedTree().DirectChildCount(firstChildOfRoot) + " Kinder!");
            Assert.AreEqual("SymbolView", strategyMgr.getSpecifiedTree().GetData(firstChildOfRoot).brailleRepresentation.screenCategory, "Das erste Kind von Root sollte eigentlich angeben, dass es eine SymbolView ist!");
            object firstChildOfChildOfRoot = strategyMgr.getSpecifiedTree().Child(firstChildOfRoot);
            Assert.AreEqual("TestScreen", strategyMgr.getSpecifiedTree().GetData(firstChildOfChildOfRoot).brailleRepresentation.screenName, "Das erste Kind von der 'SymbolView' sollte eigentlich angeben, dass es eine 'TestScreen' ist!");
            Assert.AreNotEqual(null, strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(firstChildOfChildOfRoot)).properties.IdGenerated, "Es muss eine Id vorhanden sein!");
        }

        [TestMethod()]
        public void add2NodesOfSameScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test 2";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(4, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 4 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
        }

        [TestMethod()]
        public void addSameNodesOfSameScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentScreenInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion

            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen - 2";
            braille2.viewName = "TestView";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreEqual(5, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 5 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()), "Das Root Element hätte ein Kind haben müssen");
            foreach(object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(strategyMgr.getSpecifiedTree().Child( grantTrees.getBrailleTree())))
            {
                Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(child));
            }
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentScreenAndViewCategoryInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion

            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen - 2";
            braille2.viewName = "TestView";
            braille2.screenCategory = VIEWCATEGORYLAYOUTVIEW;
            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 6 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            Assert.AreEqual(2, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()), "Das Root Element hätte zwei Kinder haben müssen");
            foreach (object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.getBrailleTree()))
            {
                Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(child));
            }
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod()]
        public void add2NodesOfDifferentViewsInBrailleTreeTest()
        {
            Assert.AreEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum sollte noch leer sein!");
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion

            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;
            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion

            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreEqual(4, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 4 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            Assert.AreEqual(1, strategyMgr.getSpecifiedTree().DirectChildCount(grantTrees.getBrailleTree()), "Das Root Element hätte ein Kind haben müssen");
            object symbolViewSubtree = strategyMgr.getSpecifiedTree().Child(grantTrees.getBrailleTree());
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
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test 2";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().addNavigationbarForScreen(pathToTemplate, braille.screenName, braille.screenCategory);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 7 Knoten haben müssen!");
            bool foundNavbar = false;
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.getBrailleTree()))
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
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test 2";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().createUiElementsNavigationbarScreens(pathToTemplate, braille.screenCategory);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 6 Knoten haben müssen!");
            bool foundNavbar = false;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.getBrailleTree()))
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
            #region erster Knoten
            BrailleRepresentation braille = new BrailleRepresentation();
            braille.isVisible = true;
            braille.screenName = "TestScreen";
            braille.viewName = "TestView";
            braille.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm);
            #endregion
            #region 2. Knoten
            BrailleRepresentation braille2 = new BrailleRepresentation();
            braille2.isVisible = true;
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";
            braille2.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test 2";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().createUiElementsNavigationbarScreens(pathToTemplate, braille.screenCategory);
            //Assert.AreEqual(6, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 6 Knoten haben müssen!");
            #region 3. Knoten
            BrailleRepresentation braille3 = new BrailleRepresentation();
            braille3.isVisible = true;
            braille3.screenName = "TestScreen - 3";
            braille3.viewName = "TestView - 3";
            braille3.screenCategory = VIEWCATEGORYSYMBOLVIEW;

            GeneralProperties prop3 = new GeneralProperties();
            prop3.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop3.controlTypeFiltered = "Text";
            prop3.valueFiltered = "Test 3";

            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            osm3.brailleRepresentation = braille3;
            osm3.properties = prop3;
            treeOperation.updateNodes.addNodeInBrailleTree(osm3);
            #endregion
            strategyMgr.getSpecifiedGeneralTemplateUi().updateNavigationbarScreens(pathToTemplate, braille.screenCategory);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.getBrailleTree()));
            Assert.AreEqual(9, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Nach dem hinzufügen der Navigationsleiste hätte der Baum 9 Knoten haben müssen!");


            Object subtreeNavbar = null;
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.getBrailleTree()))
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

    }
}