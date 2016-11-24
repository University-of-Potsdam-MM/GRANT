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
            if(viewCategories == null) { Assert.Fail("Die ViewCategories sind in der Config nicht richtig angegeben!"); }
            VIEWCATEGORYSYMBOLVIEW = viewCategories[0];
            VIEWCATEGORYLAYOUTVIEW = viewCategories[1];
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

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm, VIEWCATEGORYSYMBOLVIEW);
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
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test 2";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2, VIEWCATEGORYSYMBOLVIEW);
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

            GeneralProperties prop = new GeneralProperties();
            prop.boundingRectangleFiltered = new Rect(0, 0, 20, 10);
            prop.controlTypeFiltered = "Text";
            prop.valueFiltered = "Test";

            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = prop;
            treeOperation.updateNodes.addNodeInBrailleTree(osm, VIEWCATEGORYSYMBOLVIEW);
            #endregion
            Assert.AreNotEqual(null, grantTrees.getBrailleTree(), "Der BrailleBaum darf nun nicht mehr leer sein!");
            Assert.AreEqual(3, strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()), "Der BrailleBaum hätte genau 3 Knoten haben sollen. Er hat aber " + strategyMgr.getSpecifiedTree().Count(grantTrees.getBrailleTree()) + " Knoten!");
            treeOperation.updateNodes.addNodeInBrailleTree(osm, VIEWCATEGORYSYMBOLVIEW);
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
            treeOperation.updateNodes.addNodeInBrailleTree(osm2, VIEWCATEGORYLAYOUTVIEW);
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
            braille2.screenName = "TestScreen";
            braille2.viewName = "TestView - 2";

            GeneralProperties prop2 = new GeneralProperties();
            prop2.boundingRectangleFiltered = new Rect(0, 30, 20, 10);
            prop2.controlTypeFiltered = "Text";
            prop2.valueFiltered = "Test";

            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            osm2.brailleRepresentation = braille2;
            osm2.properties = prop2;
            treeOperation.updateNodes.addNodeInBrailleTree(osm2, VIEWCATEGORYSYMBOLVIEW);
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

    }
}