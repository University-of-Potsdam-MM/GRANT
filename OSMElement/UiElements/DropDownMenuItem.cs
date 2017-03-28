using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement.UiElements
{

    public struct DropDownMenuItem
    {
        public bool isOpen { get; set; }
        public bool hasChild { get; set; }
        public bool isChild { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
        public bool isVertical { get; set; }

        public override string ToString()
        {
            return String.Format("DropDownMenuItem:  isOpen = {0}, hasChild = {1}, isChild = {2}, hasNext = {3}, hasPrevious = {4}, isVertical = {5}", isOpen, hasChild, isChild, hasNext, hasPrevious, isVertical);
        }
    }
}
