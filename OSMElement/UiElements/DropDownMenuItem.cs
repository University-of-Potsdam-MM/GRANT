using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement.UiElements
{
    /// <summary>
    /// Specifies the properties of a drop down menu.
    /// </summary>
    [Serializable]
    public struct DropDownMenuItem
    {
        /// <summary>
        /// Determines whether the menu ias open.
        /// </summary>
        public bool isOpen { get; set; }
        public bool hasChild { get; set; }
        /// <summary>
        /// Determines whether the item is a child.
        /// </summary>
        public bool isChild { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
        /// <summary>
        /// Determines whether the item will be shown vertical.
        /// </summary>
        public bool isVertical { get; set; }

        public override string ToString()
        {
            return String.Format("DropDownMenuItem:  isOpen = {0}, hasChild = {1}, isChild = {2}, hasNext = {3}, hasPrevious = {4}, isVertical = {5}", isOpen, hasChild, isChild, hasNext, hasPrevious, isVertical);
        }
    }
}
