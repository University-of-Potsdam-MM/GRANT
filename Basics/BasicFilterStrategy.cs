using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree;


namespace Basics
{
    public class BasicFilterStrategy
    {


        private Interfaces.IFilterStrategy specifiedFilter;
        
        public ITree<GeneralProperties> filtering(IntPtr hwnd){
            return specifiedFilter.filtering(hwnd);
        }

        public void setSpecifiedFilter(Interfaces.IFilterStrategy filter)
        {
            specifiedFilter = filter;
        }

        public Interfaces.IFilterStrategy getSpecifiedFilter()
        {
            return specifiedFilter;
        }
            
        

    }
}
