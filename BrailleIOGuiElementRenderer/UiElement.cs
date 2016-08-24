using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    public struct UiElement //ähnlich OsmElement.BrailleRepresentation
    {
        // public Position position { get; set; }

        /// <summary>
        /// Gibt die View, in welcher der Inhalt angezeigt werden soll an.
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Gibt an, ob der Inhalt/die View sichtbar sein soll
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// Gibt eine matrix die Dargestellt werden soll an.
        /// </summary>
        public bool[,] matrix { get; set; }

        public int contrast { get; set; }
        public double zoom { get; set; }

        /// <summary>
        /// Gibt den Namen des Screens an, auf welchem die View angezeigt werden soll
        /// </summary>
        public String screenName { get; set; }

        /// <summary>
        /// Enthält den darzustellenden Text eines UI-Elements
        /// </summary>
        public String text { get; set; }

        /// <summary>
        /// Gibt an, ob das UI-Element deaktiviert ist
        /// </summary>
        public Boolean isDisabled { get; set; }

        /// <summary>
        /// Gibt an, ob Scrollbalken angezeigt werden sollen, sofern der Inhalt in der View nicht ausreichend Platz hat (falls nicht gesetzt, wird von true ausgegangen)
        /// </summary>
        public Boolean showScrollbar { get; set; }

        /// <summary>
        /// Gibt für UI-Elemente weiteren (speziellen) Inhalt an
        /// </summary>
        public object uiElementSpecialContent { get; set; }

        public override string ToString()
        {   //hier sind nicht alle Eigenschaften Berücksichtigt
            return String.Format("screenName = {0}, viewName = {1}, text = {2}, uiElementSpecialContent = {3}", screenName, viewName, text, uiElementSpecialContent.ToString());
        }

    }
}
