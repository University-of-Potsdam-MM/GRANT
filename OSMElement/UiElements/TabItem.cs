using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements.UiElements
{
    /// <summary>
    /// Specifies the properties of a tab item
    /// </summary>
    [Serializable]
    public struct TabItem
    {

        /// <summary>
        /// Orientation of the tab bar;
        /// The tabs should be "opened" in different directions depending on their orientation.
        /// </summary>
        public Orientation orientation { get; set; }

        public override string ToString()
        {
            return String.Format("TabItem: orientation = {0}", orientation.ToString());
        }
    }

    public enum Orientation { Left, Top, Bottom, Right, Vertical, Horizontal };
}
