using System;
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

        public Object filtering(IntPtr hwnd)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
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
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public OSMElement.OSMElement setOSMElement(int pointX, int pointY) {
            return new OSMElement.OSMElement();
        }

        public GeneralProperties updateNodeContent(OSMElement.OSMElement osmElementFilteredNode) { return new GeneralProperties(); }


        public Object filtering(IntPtr hwnd, TreeScopeEnum treeScope, int depth)
        {
            throw new NotImplementedException();
        }


        public Object updateFiltering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScopeEnum)
        {
            throw new NotImplementedException();
        }
    }
    
}
