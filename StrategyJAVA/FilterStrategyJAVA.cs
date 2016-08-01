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

        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> filtering(int pointX, int pointY, TreeScopeEnum treeScope, int depth)
        {
            return specifiedTree;
        }

        //public OSMElement.OSMElement filterElement(IntPtr hwnd)
        //{
        //    OSMElement.OSMElement tree = new OSMElement.OSMElement();
        //    return tree;
        //}

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITreeStrategy<OSMElement.OSMElement> NewNodeTree() 
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }
        /*public void getMouseRect(IntPtr hwnd, int pointx, int pointY, out int x, out int y, out int width, out int height)
        {
            x = 0;
            y = 0;
            width = 0;
            height = 0;
        }*/

        public OSMElement.OSMElement setOSMElement(int pointX, int pointY) {
            return new OSMElement.OSMElement();
        }

        public GeneralProperties updateNodeContent(OSMElement.OSMElement osmElement) { return new GeneralProperties(); }


        public OSMElement.OSMElement filteringMainNode(IntPtr hwnd)
        {
            throw new NotImplementedException();
        }
    }
    
}
