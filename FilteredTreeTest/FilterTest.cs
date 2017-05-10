using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using System.Collections.Generic;
using GRANTManager.TreeOperations;
using System.Diagnostics;

namespace FilteredTreeTest
{
    [TestClass]
    public class FilterTest
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;


        private String applicationName = "calc";
        private String applicationPathName = @"C:\Windows\system32\calc.exe";

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
           // strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            /* strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
             strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
             strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);*/
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            /*strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className);
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);*/
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            //Anwendung starten 
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.startApp(applicationName, applicationPathName);
        }

        [TestMethod]
        public void filterCalcTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(applicationName);
            Assert.AreNotEqual(IntPtr.Zero, appHwnd, "Es hätte für die anwendung ein HWND ermittelt werden müssen!");
            Object treeHWND = strategyMgr.getSpecifiedFilter().filtering(appHwnd, TreeScopeEnum.Application, -1);
            Assert.AreNotEqual(null, treeHWND, "Es ist kein gefilterter Baum vorhanden");
            #region Punkt in anwendung ermitteln
            Object subtreeHWND = null;
            if (strategyMgr.getSpecifiedTree().HasChild(treeHWND))
            {
                subtreeHWND = strategyMgr.getSpecifiedTree().Child(treeHWND);
                if (strategyMgr.getSpecifiedTree().HasChild(subtreeHWND))
                {
                    subtreeHWND = strategyMgr.getSpecifiedTree().Child(subtreeHWND);
                }
            }
            Assert.AreNotEqual(null, subtreeHWND, "Es hätte ein Teilbung gefunden werden müssen!");
            OSMElement.OSMElement dataSubtreeHWND = strategyMgr.getSpecifiedTree().GetData(subtreeHWND);
            #endregion
            Object treePoint = strategyMgr.getSpecifiedFilter().filtering(Convert.ToInt32( dataSubtreeHWND.properties.boundingRectangleFiltered.X), Convert.ToInt32( dataSubtreeHWND.properties.boundingRectangleFiltered.Y), TreeScopeEnum.Application, -1);
            Debug.WriteLine("treePoint:\n" + strategyMgr.getSpecifiedTree().ToStringRecursive(treePoint));

            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(treeHWND))
            {
                List<Object> nodes = treeOperation.searchNodes.getNodeList(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, treePoint);
                if (nodes.Count != 1) {
                    Assert.Fail("Es wurde nicht die richtige Anzahl an zugehörigen Knoten im geladenen Baum gefunden! Betrachteter Knoten:\n{0}\n\t Anzahl der gefundenen zugehörigen Knoten im geladenen Baum = {1}", node, nodes.Count); }
                bool isEqual = compareToNodes(node, nodes[0]);
                if (!isEqual)
                {
                    Assert.Fail("Der geladene Baum enthält den Knoten folgenden Knoten nicht:\n{0}", strategyMgr.getSpecifiedTree().GetData(node));
                }

            }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(treePoint))
            {
                List<Object> nodes = treeOperation.searchNodes.getNodeList(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, treeHWND);
                if (nodes.Count != 1) {
                    Assert.Fail("Es wurde nicht die richtige Anzahl an zugehörigen Knoten im gefilterten Baum gefunden! Betrachteter Knoten:\n{0}\n\t Anzahl der gefundenen zugehörigen Knoten im gefilterten Baum = {1}", node, nodes.Count); }
                bool isEqual = compareToNodes(node, nodes[0]);
                if (!isEqual)
                {
                    Assert.Fail("Der gefilterte Baum enthält den Knoten folgenden Knoten nicht:\n{0}", strategyMgr.getSpecifiedTree().GetData(node));
                }
            }
        }

        /// <summary>
        /// Prüft, ob zwei Knoten gleich sind, dabei werden die folgenden Eigenschaften ignoriert: runtimeIDFiltered, hWndFiltered, processIdFiltered
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns><c>true</c> falls beide Knoten gleich sind; sonst <c>false</c></returns>
        private bool compareToNodes(object node1, object node2)
        {
            if (strategyMgr.getSpecifiedTree().Depth(node1) != strategyMgr.getSpecifiedTree().Depth(node2)) { Assert.Fail("Die Tiefe der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (strategyMgr.getSpecifiedTree().DirectChildCount(node1) != strategyMgr.getSpecifiedTree().DirectChildCount(node2)) { Assert.Fail("Die Anzahl der direkten Kinder der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            OSMElement.OSMElement osmNode1 = strategyMgr.getSpecifiedTree().GetData(node1);
            OSMElement.OSMElement osmNode2 = strategyMgr.getSpecifiedTree().GetData(node2);
            if (osmNode1.properties.acceleratorKeyFiltered != null && !osmNode1.properties.acceleratorKeyFiltered.Equals(osmNode2.properties.acceleratorKeyFiltered)) { Assert.Fail("Der acceleratorKey der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.accessKeyFiltered != null && !osmNode1.properties.accessKeyFiltered.Equals(osmNode2.properties.accessKeyFiltered)) { Assert.Fail("Der accessKeyFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.autoamtionIdFiltered != null && !osmNode1.properties.autoamtionIdFiltered.Equals(osmNode2.properties.autoamtionIdFiltered)) { Assert.Fail("Der autoamtionIdFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.classNameFiltered != null && !osmNode1.properties.classNameFiltered.Equals(osmNode2.properties.classNameFiltered)) { Assert.Fail("Der classNameFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.controlTypeFiltered != null && !osmNode1.properties.controlTypeFiltered.Equals(osmNode2.properties.controlTypeFiltered)) { Assert.Fail("Der controlTypeFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.appPath != null && !osmNode1.properties.appPath.Equals(osmNode2.properties.appPath)) { Assert.Fail("Der appPath der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.frameWorkIdFiltered != null && !osmNode1.properties.frameWorkIdFiltered.Equals(osmNode2.properties.frameWorkIdFiltered)) { Assert.Fail("Der frameWorkIdFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.grantFilterStrategy != null && !osmNode1.properties.grantFilterStrategy.Equals(osmNode2.properties.grantFilterStrategy)) { Assert.Fail("Der grantFilterStrategy der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.hasKeyboardFocusFiltered != null && !osmNode1.properties.hasKeyboardFocusFiltered.Equals(osmNode2.properties.hasKeyboardFocusFiltered)) { Assert.Fail("Der hasKeyboardFocusFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.helpTextFiltered != null && !osmNode1.properties.helpTextFiltered.Equals(osmNode2.properties.helpTextFiltered)) { Assert.Fail("Der helpTextFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.IdGenerated != null && !osmNode1.properties.IdGenerated.Equals(osmNode2.properties.IdGenerated)) { Assert.Fail("Der IdGenerated der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isContentElementFiltered != null && !osmNode1.properties.isContentElementFiltered.Equals(osmNode2.properties.isContentElementFiltered)) { Assert.Fail("Der isContentElementFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isEnabledFiltered != null && !osmNode1.properties.isEnabledFiltered.Equals(osmNode2.properties.isEnabledFiltered)) { Assert.Fail("Der isEnabledFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isKeyboardFocusableFiltered != null && !osmNode1.properties.isKeyboardFocusableFiltered.Equals(osmNode2.properties.isKeyboardFocusableFiltered)) { Assert.Fail("Der isKeyboardFocusableFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isOffscreenFiltered != null && !osmNode1.properties.isOffscreenFiltered.Equals(osmNode2.properties.isOffscreenFiltered)) { Assert.Fail("Der isOffscreenFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isPasswordFiltered != null && !osmNode1.properties.isPasswordFiltered.Equals(osmNode2.properties.isPasswordFiltered)) { Assert.Fail("Der isPasswordFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isRequiredForFormFiltered != null && !osmNode1.properties.isRequiredForFormFiltered.Equals(osmNode2.properties.isRequiredForFormFiltered)) { Assert.Fail("Der isRequiredForFormFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.isToggleStateOn != null && !osmNode1.properties.isToggleStateOn.Equals(osmNode2.properties.isToggleStateOn)) { Assert.Fail("Der isToggleStateOn der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.itemStatusFiltered != null && !osmNode1.properties.itemStatusFiltered.Equals(osmNode2.properties.itemStatusFiltered)) { Assert.Fail("Der itemStatusFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.itemTypeFiltered != null && !osmNode1.properties.itemTypeFiltered.Equals(osmNode2.properties.itemTypeFiltered)) { Assert.Fail("Der itemTypeFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.labeledByFiltered != null && !osmNode1.properties.labeledByFiltered.Equals(osmNode2.properties.labeledByFiltered)) { Assert.Fail("Der labeledByFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (!osmNode1.properties.localizedControlTypeFiltered.Equals(osmNode2.properties.localizedControlTypeFiltered)) { Assert.Fail("Der localizedControlTypeFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.processName != null && !osmNode1.properties.processName.Equals(osmNode2.properties.processName)) { Assert.Fail("Der moduleName der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (osmNode1.properties.nameFiltered != null && !osmNode1.properties.nameFiltered.Equals(osmNode2.properties.nameFiltered)) { Assert.Fail("Der nameFiltered der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            if (!osmNode1.properties.rangeValue.Equals(osmNode2.properties.rangeValue)) { Assert.Fail("Der rangeValue der beiden Knoten stimmt nicht überein!\n node1 = {0}\n node2 = {1}", strategyMgr.getSpecifiedTree().GetData(node1), strategyMgr.getSpecifiedTree().GetData(node2)); return false; }
            return true;
        }
    }
}
