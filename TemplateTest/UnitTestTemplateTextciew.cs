using GRANTManager;
using GRANTManager.TreeOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateTest
{
    [TestClass]
    public class UnitTestTemplateTextview
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String pathToTemplate;

        [TestInitialize]
        public void Initialize()
        {
            #region initialisieren
            strategyMgr = new GRANTManager.StrategyManager();
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
            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUi.xml");
            #endregion
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
        public void createTextviewOfSubtreeTest()
        {
            initilaizeFilteredTree();
            TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperation);
            List<Object> nodes = treeOperation.searchNodes.getNodeList("41B73937D557B2AB5DA85001ABF0C423", grantTrees.filteredTree); // "41B73937D557B2AB5DA85001ABF0C423" is the Id of the "TitleBar" in MS Calc
            Assert.AreNotEqual(null, nodes);
            Assert.AreEqual(1, nodes.Count);
            tempTextView.createTextviewOfSubtree(nodes[0], 0);
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(38, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // 6 Views => 3*5 + 3*7 = 15 +21 = 36 ; 36 + 2 (TextView & Screen) = 38 Nodes
        }

        [TestMethod]
        public void create2xTextviewOfSubtreeTest()
        {
            initilaizeFilteredTree();
            TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperation);
            List<Object> nodes = treeOperation.searchNodes.getNodeList("41B73937D557B2AB5DA85001ABF0C423", grantTrees.filteredTree); // "41B73937D557B2AB5DA85001ABF0C423" is the Id of the "TitleBar" in MS Calc
            Assert.AreNotEqual(null, nodes);
            Assert.AreEqual(1, nodes.Count);
            tempTextView.createTextviewOfSubtree(nodes[0], 0);
            object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(38, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // 6 Views => 3*5 + 3*7 = 15 +21 = 36 ; 36 + 2 (TextView & Screen) = 38 Nodes
            tempTextView.createTextviewOfSubtree(nodes[0], 0);

            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail("Both trees should have the same values.");
            }
        }

        [TestMethod]
        public void create2xTextviewTreeAndSubtreeTest()
        {
            initilaizeFilteredTree();
            TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperation);           
            tempTextView.createTextviewOfSubtree(grantTrees.filteredTree, 0);
            object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            List<Object> nodes = treeOperation.searchNodes.getNodeList("41B73937D557B2AB5DA85001ABF0C423", grantTrees.filteredTree); // "41B73937D557B2AB5DA85001ABF0C423" is the Id of the "TitleBar" in MS Calc
            Assert.AreNotEqual(null, nodes);
            Assert.AreEqual(1, nodes.Count);

            tempTextView.createTextviewOfSubtree(nodes[0], 1*5); 
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeCopy), strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree));
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail("Both trees should have the same values.");
            }
        }

        [TestMethod]
        public void create2xTextviewTest()
        {
            initilaizeFilteredTree();
            TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperation);
            tempTextView.createTextviewOfSubtree(grantTrees.filteredTree, 0);
            object treeCopy = grantTrees.brailleTree.DeepCopy();
            Assert.AreNotEqual(null, grantTrees.brailleTree);

            tempTextView.createTextviewOfSubtree(grantTrees.filteredTree, 0);
            Assert.AreEqual(strategyMgr.getSpecifiedTree().Count(treeCopy), strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree));
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail("Both trees should have the same values.");
            }
        }

        [TestMethod]
        public void generatedLayoutViewTest()
        {
            initilaizeFilteredTree();
            Assert.AreEqual(null, grantTrees.brailleTree);
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedLayoutView(pathToTemplate);
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(5, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // 3 Views + 2 (TypeOfView & Screen) = 5 Nodes
        }

        [TestMethod]
        public void generated2xLayoutViewTest()
        {
            initilaizeFilteredTree();
            Assert.AreEqual(null, grantTrees.brailleTree);
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedLayoutView(pathToTemplate);
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(5, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // 3 Views + 2 (TypeOfView & Screen) = 5 Nodes
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedLayoutView(pathToTemplate);
            Assert.AreEqual(5, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); // 3 Views + 2 (TypeOfView & Screen) = 5 Nodes
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail("Both trees should have the same values.");
            }
        }

        [TestMethod]
        public void generatedSymbolViewTest()
        {
            initilaizeFilteredTree();
            Assert.AreEqual(null, grantTrees.brailleTree);
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedSymbolView(pathToTemplate);
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(12, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); //  1 (TypeOfView = SymbolView) + 1 (Screen = b) + 5 Views (TitleBar, Statusbar, Navigationbar_Group, Menubar_Group, Menubar_Group) + 5 (children of the groups) = 12 Nodes
        }

        [TestMethod]
        public void generated2xSymbolViewTest()
        {
            initilaizeFilteredTree();
            Assert.AreEqual(null, grantTrees.brailleTree);
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedSymbolView(pathToTemplate);
            Assert.AreNotEqual(null, grantTrees.brailleTree);
            Assert.AreEqual(12, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); //  1 (TypeOfView = SymbolView) + 1 (Screen = b) + 5 Views (TitleBar, Statusbar, Navigationbar_Group, Menubar_Group, Menubar_Group) + 5 (children of the groups) = 12 Nodes
            Object treeCopy = grantTrees.brailleTree.DeepCopy();
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedSymbolView(pathToTemplate);
            Debug.WriteLine(strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.brailleTree));
            Assert.AreEqual(12, strategyMgr.getSpecifiedTree().Count(grantTrees.brailleTree)); //  1 (TypeOfView = SymbolView) + 1 (Screen = b) + 5 Views (TitleBar, Statusbar, Navigationbar_Group, Menubar_Group, Menubar_Group) + 5 (children of the groups) = 12 Nodes
            if (!strategyMgr.getSpecifiedTree().Equals(grantTrees.brailleTree, treeCopy))
            {
                Assert.Fail("Both trees should have the same values.");
            }
        }
    }
}
