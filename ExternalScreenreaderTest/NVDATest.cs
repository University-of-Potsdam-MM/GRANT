using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using OSMElements;
using System.Diagnostics;

namespace ExternalScreenreaderTest
{
    [TestClass]
    public class NVDATest
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;

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
            strategyMgr.setSpecifiedEventStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
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
            strategyMgr.setSpecifiedExternalScreenreader(settings.getPossibleExternalScreenreaders()[0].className);
            strategyMgr.setSpecifiedBrailleConverter(settings.getPossibleBrailleConverter()[0].className);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
        }



        [TestMethod]
        public void TestGetPinsAsOSM()
        {
            System.Windows.Forms.SendKeys.SendWait("{ESC}"); // set focus back to Visual Studio
            System.Threading.Thread.Sleep(2000); // Wait of connection to MVBD & first conntent sets
            if (!strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("MVBD"))
            {
                Assert.Fail("MVBD isn't running!");
            }
            if (!strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("NVDA"))
            {
                Assert.Fail("NVDA isn't running!");
            }
            System.Threading.Thread.Sleep(2000);
            OSMElement osm = strategyMgr.getSpecifiedExternalScreenreader().getContentAsOSM();
            Assert.AreNotEqual(null, osm.properties.valueFiltered);
            Debug.WriteLine("Braillezeile NVDA: "+ osm.properties.valueFiltered);
        }
    }
}
