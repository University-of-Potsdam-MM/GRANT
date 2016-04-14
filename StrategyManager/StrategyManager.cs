﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyGenericTree;
using StrategyManager.Interfaces;


namespace StrategyManager
{
    public class StrategyMgr
    {

        private IFilterStrategy specifiedFilter;
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<GeneralProperties> specifiedTree;

        public void setSpecifiedFilter(String filterName)
        {
            Type type = Type.GetType(filterName);
            specifiedFilter =  (IFilterStrategy)Activator.CreateInstance(type);
 
        }
        
        public IFilterStrategy getSpecifiedFilter()
        {
            return specifiedFilter;
        }

         
        public void setSpecifiedOperationSystem(String operationSystemName)
        {
            Type type = Type.GetType(operationSystemName);
            specifiedOperationSystem = (IOperationSystemStrategy)Activator.CreateInstance(type);

        }

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }

        

        public void setSpecifiedTree(String treeName)
        {
            //TODO;
        }

        public ITreeStrategy<GeneralProperties> getSpecifiedTree()
        {
            return specifiedTree;
        }

    }

    }

