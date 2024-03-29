﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GRANTManager;
using GRANTManager.TreeOperations;
using System.Collections.Generic;
using System.Diagnostics;
using OSMElements;

namespace FilteredTreeTest
{
    [TestClass]
    public class UnitTestLoadTree
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        SearchNodes searchNodes;
        TreeOperation treeOperation;
        GuiFunctions guiFuctions;
        private String treePath;
        private String treePathUia2;


        [TestInitialize]
        public void Initialize()
        {
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            Settings settings = new Settings();
            searchNodes = new SearchNodes(strategyMgr, grantTrees, treeOperation);
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
            strategyMgr.setSpecifiedTree(settings.getPossibleTrees()[0].className);
            strategyMgr.setSpecifiedEventStrategy(settings.getPossibleEventManager()[0].className);
            strategyMgr.setSpecifiedFilter(Settings.getPossibleFilters()[0].className);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            List<GRANTManager.Strategy> posibleOS = settings.getPossibleOperationSystems();
            strategyMgr.setSpecifiedOperationSystem(settings.getPossibleOperationSystems()[0].className);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            guiFuctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);
            String projectPath;
            projectPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "SavedTrees");
            treePathUia2 = System.IO.Path.Combine(projectPath, "filteredTree_Rechner.grant");
            treePath = System.IO.Path.Combine(projectPath, "filteredTree_RechnerUIA.grant");
            //
            
        }

        [TestMethod]
        public void TestLoadFilteredTree()
        {
            guiFuctions.loadGrantProject(treePathUia2);
            if (grantTrees.filteredTree == null)
            {
                Assert.Fail("Es ist kein gefilterter Baum vorhanden!");
            }
            if(strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree) != 45)
            {
                Assert.Fail("Der gefilterte Baum hätte 45 Knoten haben müssen -- er hat {0} Knoten!", strategyMgr.getSpecifiedTree().Count(grantTrees.filteredTree));
            }
            String mainFilterstrategy = treeOperation.searchNodes.getMainFilterstrategyOfTree();
            if (!mainFilterstrategy.Equals("StrategyUIA.FilterStrategyUIA, StrategyUIA"))
            {
                Assert.Fail("Der 1. Knoten hätte mit 'StrategyUIA.FilterStrategyUIA' gefiltert werden sollen -- genutzter Filter ist '{0}'", mainFilterstrategy);
            }
            String nodeFS_ED842B72B012E86CE468B73FA1378361 = treeOperation.searchNodes.getFilteredTreeOsmElementById("ED842B72B012E86CE468B73FA1378361").properties.grantFilterStrategy;
            if (!Settings.strategyUserNameToClassName(nodeFS_ED842B72B012E86CE468B73FA1378361).Equals("StrategyUIA2.FilterStrategyUIA2, StrategyUIA2"))
            {
                Assert.Fail("Der Knoten mit der Id 'ED842B72B012E86CE468B73FA1378361' hätte mit 'StrategyUIA2.FilterStrategyUIA2' gefiltert werden sollen -- genutzter Filter ist '{0}'", nodeFS_ED842B72B012E86CE468B73FA1378361);
            }
            guiFuctions.deleteGrantTrees();
        }

        [TestMethod]
        public void TestLoadFilteredTreeAndCompare()
        {
            guiFuctions.loadGrantProject(treePath);
            if (grantTrees.filteredTree == null)
            {
                Assert.Fail("Es ist kein gefilterter Baum vorhanden!");
            }
            Object loadedTree = strategyMgr.getSpecifiedTree().Copy( grantTrees.filteredTree);
            String processName = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.processName;
            String fileName = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.appPath;
            IntPtr appHwnd = strategyMgr.getSpecifiedOperationSystem().getHandleOfApplication(processName); //Die Anwendung sollte schon offen sein, durch das Laden des Baums
            grantTrees.filteredTree =  strategyMgr.getSpecifiedFilter().filtering(appHwnd);
             Debug.WriteLine("\ngrant\n"+strategyMgr.getSpecifiedTree().ToStringRecursive(grantTrees.filteredTree)+"\n\n");
            Debug.WriteLine("\nloaded\n" + strategyMgr.getSpecifiedTree().ToStringRecursive(loadedTree) + "\n\n");
            HelpFunctions hf = new HelpFunctions(strategyMgr, grantTrees);
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.filteredTree))
            {
                List<Object> nodes = searchNodes.getNodeList(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, loadedTree);
                if (nodes.Count != 1) { Assert.Fail("Es wurde nicht die richtige Anzahl an zugehörigen Knoten im geladenen Baum gefunden! Betrachteter Knoten:\n{0}\n\t Anzahl der gefundenen zugehörigen Knoten im geladenen Baum = {1}", node, nodes.Count); }
                bool isEqual = hf.compareToNodes(node, nodes[0]);
                if (!isEqual)
                {
                    Assert.Fail("Der geladene Baum enthält den Knoten folgenden Knoten nicht:\n{0}", strategyMgr.getSpecifiedTree().GetData(node));
                }
                
            }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(loadedTree))
            {
                List<Object> nodes = searchNodes.getNodeList(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, grantTrees.filteredTree);
                if (nodes.Count != 1) { Assert.Fail("Es wurde nicht die richtige Anzahl an zugehörigen Knoten im gefilterten Baum gefunden! Betrachteter Knoten:\n{0}\n\t Anzahl der gefundenen zugehörigen Knoten im gefilterten Baum = {1}", node, nodes.Count); }
                bool isEqual = hf.compareToNodes(node, nodes[0]);
                if (!isEqual)
                {
                    Assert.Fail("Der gefilterte Baum enthält den Knoten folgenden Knoten nicht:\n{0}", strategyMgr.getSpecifiedTree().GetData(node));
                }
            }
        }

    }
}
