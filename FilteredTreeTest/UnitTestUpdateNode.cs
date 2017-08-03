using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Linq;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestUpdateNode
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String applicationName = "calc";
        private String applicationPathName = @"C:\Windows\system32\calc.exe";
        private String idTextNodeCalc = "F6BC5E5ADD3B17478743923733E4BC8C";
        private String treePathUia2;

        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();

            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
            String projectPath;
            projectPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SavedTrees");
            treePathUia2 = System.IO.Path.Combine(projectPath, "filteredTree_Rechner.grant");
        }


        [TestMethod]
        public void ChangeFilterForNodeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            List<Strategy> filterStrategys = Settings.getPossibleFilters();
            Type currentFilter = strategyMgr.getSpecifiedFilter().GetType();
            int indexNewFilterStrategy = 0;
            foreach (Strategy strategy in filterStrategys)
            {
                if (!strategy.className.Contains(currentFilter.FullName) && !strategy.className.ToLower().Contains("java")) //kann raus wenn Java-Filter vorhanden ist
                {
                    break;
                }
                indexNewFilterStrategy++;
            }
            strategyMgr.setSpecifiedFilter(filterStrategys[indexNewFilterStrategy].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (textNode.properties.grantFilterStrategy == null || !filterStrategys[indexNewFilterStrategy].userName.Equals(textNode.properties.grantFilterStrategy)) { Assert.Fail("Die Filterstrategie wurde für den Knoten (richtig) nicht geändert!\nBtrachtet wurde der Knoten  {0}\nDer filter hätte '{1}' sein sollen", textNode, filterStrategys[indexNewFilterStrategy].userName); }
        }

        /// <summary>
        /// Ändert die Anzeige bei Calc und ließt den Teilbaum neu ein
        /// </summary>
        [TestMethod]
        public void FilterSubtreeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);

            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);                
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            //Send Key -> Inhalt im Textfeld soll sich ändern
            System.Windows.Forms.SendKeys.SendWait("{DEL}");
            System.Windows.Forms.SendKeys.SendWait("24");
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (!textNode.properties.nameFiltered.Equals("24")) { Assert.Fail("Der Knoten wurde nicht richtig geändert oder geupdatet!\nBetrachteter Knoten:\n{0}", textNode); }
        }

        /// <summary>
        /// Ändert die Anzeige bei Calc und prüft, ob die Änderung ausgelesen wird
        /// </summary>
        [TestMethod]
        public void UpdateNodeTest()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);

            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);

            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(applicationName);
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            //Send Key -> Inhalt im Textfeld soll sich ändern
            System.Windows.Forms.SendKeys.SendWait("{DEL}");
            System.Windows.Forms.SendKeys.SendWait("42");
            UpdateNodes up = new UpdateNodes(strategyMgr, grantTrees, treeOperation);
            up.updateNodeOfFilteredTree(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (!textNode.properties.nameFiltered.Equals("42")) { Assert.Fail("Der Knoten wurde nicht richtig geändert oder geupdatet!\nBetrachteter Knoten:\n{0}", textNode); }
        }

        [TestMethod]
        public void UpdateNodeTest_grantFilterstrategyChildren()
        {
            String idPaneNode = "417F2ACC323396E993B4DC2AD2515D5E";
            guiFuctions.loadGrantProject(treePathUia2);
            OSMElement.OSMElement paneNodeOld = treeOperation.searchNodes.getFilteredTreeOsmElementById(idPaneNode).DeepCopy();

            UpdateNodes up = new UpdateNodes(strategyMgr, grantTrees, treeOperation);
            up.updateNodeOfFilteredTree(idPaneNode);
            OSMElement.OSMElement paneNodeNew = treeOperation.searchNodes.getFilteredTreeOsmElementById(idPaneNode);
            Assert.AreEqual(paneNodeOld.properties.grantFilterStrategy, paneNodeNew.properties.grantFilterStrategy);
            //Assert.AreEqual(paneNodeOld.properties.grantFilterStrategiesChildren, paneNodeNew.properties.grantFilterStrategiesChildren);
            if (!paneNodeOld.properties.grantFilterStrategiesChildren.All(p => paneNodeNew.properties.grantFilterStrategiesChildren.Contains(p))) // check whether 'osmParent.properties.grantFilterStrategiesChildren' is a subset of filterStrategiesChildren
            {
                Assert.Fail("The update of the 'grantFilterStrategiesChildren' wasn't correct!");
            }


        }
    }
}
