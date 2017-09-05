using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElements;
using System.Windows;
using GRANTManager.TreeOperations;


namespace StrategyJAVA
{
    class FilterStrategyJAVA :IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<OSMElements.OSMElement> specifiedTree;
        private StrategyManager strategyMgr;

        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperation = treeOperation; }
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public StrategyManager getStrategyMgr() { return strategyMgr; }

        public Object filtering(IntPtr hwnd)
        {
            ITreeStrategy<OSMElements.OSMElement> tree = specifiedTree;
            return tree;
        }

        public Object filtering(int pointX, int pointY, TreeScopeEnum treeScope, int depth)
        {
            return specifiedTree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public Object NewNodeTree() 
        {
            ITreeStrategy<OSMElements.OSMElement> tree = specifiedTree;
            return tree;
        }

        public OSMElements.OSMElement getOSMElement(int pointX, int pointY) {
            return new OSMElements.OSMElement();
        }

        public GeneralProperties updateNodeContent(OSMElements.OSMElement osmElementFilteredNode) { return new GeneralProperties(); }


        public Object filtering(IntPtr hwnd, TreeScopeEnum treeScope, int depth)
        {
            throw new NotImplementedException();
        }


        public Object filtering(OSMElements.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScopeEnum)
        {
            throw new NotImplementedException();
        }

        public object filtering(string generatedNodeId, TreeScopeEnum treeScope)
        {
            throw new NotImplementedException();
        }
    }
    
}
