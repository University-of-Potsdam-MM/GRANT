using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyManager.Interfaces;
using OSMElement;
using System.Windows;


namespace StrategyJAVA
{
    class FilterStrategyJAVA :IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree;


        public void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem)
        {
            specifiedOperationSystem = operationSystem;
        }

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }


        public void setSpecifiedTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            specifiedTree = tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> getSpecifiedTree()
        {
            return specifiedTree;
        }

        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITreeStrategy<OSMElement.OSMElement> getParentsOfElement(ITreeStrategy<OSMElement.OSMElement> node, IntPtr hwnd)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> NewNodeTree() 
        {
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree;
            return tree;
        }

        public Rect getMouseRect(IntPtr hwnd)
        {
            Rect rect = new Rect();
            return rect;
        }
    }
}
