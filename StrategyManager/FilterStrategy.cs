using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyGenericTree;
using StrategyManager.Interfaces;


namespace StrategyManager
{
    public class FilterStrategy
    {


        private IFilterStrategy specifiedFilter;
        
        public void setSpecifiedFilter(String filterName)
        {
            Type type = Type.GetType(filterName);
            specifiedFilter =  (IFilterStrategy)Activator.CreateInstance(type);
 
        }
        
        public Interfaces.IFilterStrategy getSpecifiedFilter()
        {
            return specifiedFilter;
        }

    }
}
