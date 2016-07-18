using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer.UiElements
{

    public struct DropDownMenu
    {
        public bool isOpen { get; set; }
        public bool hasChild { get; set; }
        public bool isChild { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
        public bool isVertical { get; set; }
    }
}
