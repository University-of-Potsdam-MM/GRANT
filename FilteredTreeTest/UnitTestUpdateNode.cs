using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestUpdateNode
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String applicationName = "calc.exe";
        private String applicationPathName = @"C:\Windows\system32\calc.exe";
        private String idTextNodeCalc = "F6BC5E5ADD3B17478743923733E4BC8C";

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
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
        }


        [TestMethod]
        public void TestChangeFilterForNode()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);
            
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            Settings settings = new Settings();
            List<Strategy> filterStrategys = settings.getPossibleFilters();
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
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (textNode.properties.grantFilterStrategy == null || !filterStrategys[indexNewFilterStrategy].userName.Equals(textNode.properties.grantFilterStrategy)) { Assert.Fail("Die Filterstrategie wurde für den Knoten (richtig) nicht geändert!\nBtrachtet wurde der Knoten  {0}\nDer filter hätte '{1}' sein sollen", textNode, filterStrategys[indexNewFilterStrategy].userName); }
        }

        /// <summary>
        /// Ändert die Anzeige bei Calc und prüft, ob die Änderung ausgelesen wird
        /// </summary>
        [TestMethod]
        public void TestChangeContentForNode()
        {
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            hf.filterApplication(applicationName, applicationPathName);

            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().isApplicationRunning(applicationName);                
            strategyMgr.getSpecifiedOperationSystem().setForegroundWindow(appHwnd);
            //Send Key -> Inhalt im Textfeld soll sich ändern
            System.Windows.Forms.SendKeys.SendWait("{DEL}");
            System.Windows.Forms.SendKeys.SendWait("42");
            guiFuctions.filterAndAddSubtreeOfApplication(idTextNodeCalc);
            OSMElement.OSMElement textNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(idTextNodeCalc);
            if (!textNode.properties.nameFiltered.Equals("42")) { Assert.Fail("Der Knoten wurde nicht richtig geändert oder geupdatet!\nBetrachteter Knoten:\n{0}", textNode); }
        }
    }
}
