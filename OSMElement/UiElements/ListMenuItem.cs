using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement.UiElements
{
    public  struct ListMenuItem
    {
        public bool isMultipleSelection { get; set; }
        //public bool isSelected { get; set; } // ist in GeneralProperties isToggleStateOn
        public bool hasNext { get; set; }
    }
}
