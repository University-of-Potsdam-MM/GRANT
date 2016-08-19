﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows;


namespace StrategyJAVA
{
    class FilterStrategyJAVA :IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree;
        private StrategyManager strategyMgr;

        private GeneratedGrantTrees grantTrees;
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public StrategyManager getStrategyMgr() { return strategyMgr; }

        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> filtering(int pointX, int pointY, TreeScopeEnum treeScope, int depth)
        {
            return specifiedTree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITreeStrategy<OSMElement.OSMElement> NewNodeTree() 
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public OSMElement.OSMElement setOSMElement(int pointX, int pointY) {
            return new OSMElement.OSMElement();
        }

        public GeneralProperties updateNodeContent(OSMElement.OSMElement osmElement) { return new GeneralProperties(); }


        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd, TreeScopeEnum treeScope, int depth)
        {
            throw new NotImplementedException();
        }


        public ITreeStrategy<OSMElement.OSMElement> updateFiltering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScopeEnum)
        {
            throw new NotImplementedException();
        }
    }
    
}
