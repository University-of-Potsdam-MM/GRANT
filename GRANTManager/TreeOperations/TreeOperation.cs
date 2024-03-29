﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.TreeOperations
{
    /// <summary>
    /// Kapselt die Klassen der Tree-Methoden
    /// </summary>
    public class TreeOperation
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        public GeneratedIds generatedIds { get; private set; }
        public SearchNodes searchNodes { get; private set; }
        public UpdateNodes updateNodes { get; private set; }
        public OSMNodeEventActionConnector oSMNodeEventActionConnector { get; private set; }
        public OsmTreeConnector osmTreeConnector { get; private set; }

        public TreeOperation(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            generatedIds = new GeneratedIds(strategyMgr);
            searchNodes = new SearchNodes(strategyMgr, grantTrees, this);
            updateNodes = new UpdateNodes(strategyMgr, grantTrees, this);
            oSMNodeEventActionConnector = new OSMNodeEventActionConnector(grantTrees);
            osmTreeConnector = new OsmTreeConnector(grantTrees);
        }
    }
}
