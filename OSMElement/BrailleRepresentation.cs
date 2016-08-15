using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSMElement.UiElements;

namespace OSMElement
{ 
    public struct BrailleRepresentation
    {
        /// <summary>
        /// Gibt die View, in welcher der Inhalt angezeigt werden soll an.
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Gibt an, ob der Inhalt/die View sichtbar sein soll
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// Gibt eine Matrix die Dargestellt werden soll an.
        /// </summary>
        public bool[,] matrix { get; set; }

        /// <summary>
        /// Gibt den Bezug zu einem GUI-Element des gefilterten Baums an! Es kann jede der <code>GeneralProperties</code>-Eigenschaften angegeben werden. Der Wert dieser Eigenschaft soll angezeigt werden
        /// </summary>
        public String fromGuiElement { get; set; }

        public int contrast { get; set; }
        public double zoom { get; set; }

        /// <summary>
        /// Gibt den Namen des Screens an, auf welchem die View angezeigt werden soll
        /// </summary>
        public String screenName { get; set; }

        /// <summary>
        /// Enthält den darzustellenden Text eines UI-Elements
        /// </summary>
     //   public String text { get; set; }

        /// <summary>
        /// Gibt an, ob Scrollbalken angezeigt werden sollen, sofern der Inhalt in der View nicht ausreichend Platz hat (falls nicht gesetzt, wird von true ausgegangen)
        /// </summary>
        public bool showScrollbar { get; set; }

        /// <summary>
        /// Gibt für UI-Elemente weiteren (speziellen) Inhalt an
        /// </summary>
        public object uiElementSpecialContent { get; set; }

        public Padding padding { get; set; }
        public Padding margin { get; set; }
        public Padding boarder { get; set; }

        /// <summary>
        /// Gibt den Z-Index an. Ein Element mit einem größeren z-Index liegt weiter oben.
        /// </summary>
        public int zIntex { get; set; }
    }
}
