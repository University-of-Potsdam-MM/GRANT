using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement.UiElements
{
    /// <summary>
    /// Specifies the properties of a list menu item.
    /// </summary>
    [Serializable]
    public struct ListMenuItem
    {
        /// <summary>
        /// Determines whether multiple selected is allowed.
        /// </summary>
        public bool isMultipleSelection { get; set; }

        //public bool isSelected { get; set; } // see  <see cref="GeneralProperties"/> isToggleStateOn

        /// <summary>
        /// Indicates if a next item follows.
        /// </summary>
        public bool hasNext { get; set; }
        public override string ToString()
        {
            return String.Format("ListMenuItem: isMultipleSelection = {0}, hasNext = {1}", isMultipleSelection, hasNext);
        }
    }
}
