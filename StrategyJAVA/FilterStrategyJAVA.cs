using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
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

        public ITreeStrategy<GeneralProperties> filtering(IntPtr hwnd)
        {
            ITreeStrategy<GeneralProperties> tree = specifiedTree;
            return tree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITreeStrategy<GeneralProperties> getParentsOfElement(ITreeStrategy<GeneralProperties> node, IntPtr hwnd)
        {
            ITreeStrategy<GeneralProperties> tree = specifiedTree;
            return tree;
        }

        public ITreeStrategy<GeneralProperties> NewNodeTree() 
        {
            ITreeStrategy<GeneralProperties> tree = specifiedTree;
            return tree;
        }
    }
}
