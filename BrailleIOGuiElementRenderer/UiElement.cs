using BrailleIO.Interface;
using BrailleIO.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BrailleIOGuiElementRenderer
{
    public struct UiElement //similar to OsmElement.BrailleRepresentation
    {
        /// <summary>
        /// name of the view on which the content will be shown
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Determines whether this view is visible.
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// Boolean matrix where <code>true</code> represents a shown pin
        /// </summary>
        public bool[,] matrix { get; set; }

        public int contrast { get; set; }
        public double zoom { get; set; }

        /// <summary>
        /// name of the screen on which the content will be shown
        /// </summary>
        public String screenName { get; set; }

        /// <summary>
        /// Enthält den darzustellenden Text eines UI-Elements
        /// </summary>
        public String text { get; set; }


        /// <summary>
        /// Determines whether a view is disable
        /// </summary>
        public Boolean isDisabled { get; set; }

        /// <summary>
        /// Determines whether scrollbar will be shown
        /// scrollbars are only shown if the view is large enough
        /// </summary>
        public Boolean isScrollbarShow { get; set; }

        /// <summary>
        /// special content for some UI elements
        /// see e.g <c>UiElements.TabItem</c>
        /// </summary>
        public object uiElementSpecialContent { get; set; }

        public override string ToString()
        {
            return String.Format("screenName = {0}, viewName = {1}, text = {2}, uiElementSpecialContent = {3}", screenName, viewName, text, uiElementSpecialContent.ToString());
        }
        public List<Groupelements> child { get; set; }
    }

    public struct Groupelements
    {
        public Rect childBoundingRectangle { get; set; }
        public IBrailleIOContentRenderer renderer { get; set; }
        public UiElement childUiElement { get; set; }
    }
}
