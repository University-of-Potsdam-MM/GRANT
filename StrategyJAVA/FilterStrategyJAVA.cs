﻿using System;
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
        //Dummy
        public ITree<GeneralProperties> filtering(IntPtr hwnd)
        {
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            return tree;
        }

        public int deliverElementID(IntPtr hwnd)
        {            
            return -1;
        }

        public ITree<GeneralProperties> getParentsOfElement(INode<GeneralProperties> node, IntPtr hwnd)
        {
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            return tree;
        }
    }
}
