using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using StrategyGenericTree;
using StrategyManager.Interfaces;


namespace StrategyJAVA
{
    class FilterStrategyJAVA : IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<GeneralProperties> specifiedTree;


        public void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem)
        {
            specifiedOperationSystem = operationSystem;
        }

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }


        public void setSpecifiedTree(ITreeStrategy<GeneralProperties> tree)
        {
            specifiedTree = tree;
        }

        public ITreeStrategy<GeneralProperties> getSpecifiedTree()
        {
            return specifiedTree;
        }

        public ITree<GeneralProperties> filtering(IntPtr hwnd)
        {
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            return tree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITree<GeneralProperties> getParentsOfElement(INode<GeneralProperties> node, IntPtr hwnd, IOperationSystemStrategy operationSystemStrategy)
        {
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            return tree;
        }
    }
}
