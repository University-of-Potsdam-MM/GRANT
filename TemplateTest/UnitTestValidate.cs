using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;

namespace TemplateTest
{
    [TestClass]
    public class UnitTestValidate
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        SearchNodes searchNodes;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String pathToTemplate;


        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            searchNodes = new SearchNodes(strategyMgr, grantTrees);
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            pathToTemplate = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Template");
            pathToTemplate = System.IO.Path.Combine(pathToTemplate, "TemplateUi.xml");
            //
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod]
        public void isTemplateValidTest()
        {
            bool isValid = GuiFunctions.isTemplateValid(pathToTemplate);
           Assert.AreEqual(true, isValid, "Das Template ist nicht valide!");
        }

        [TestMethod]
        public void isTemplateUsableForDeviceTest()
        {
            bool isValid = guiFuctions.isTemplateUsableForDevice(pathToTemplate);
            Assert.AreEqual(true, isValid, "Das Template ist für das Ausgabegerät nicht geeignet!");
            //if (!isValid) { Assert.Fail("Das Template ist für das Ausgabegerät nicht geeignet!"); }
            guiFuctions.deleteGrantTrees();
        }
    }
}
