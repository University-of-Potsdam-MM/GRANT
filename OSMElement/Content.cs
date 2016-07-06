using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OSMElement
{
    public struct Content
    {
        /// <summary>
        /// gibt den anzuzeigenden Text an
        /// </summary>
        public String text { get; set; }
        public Image image { get; set; } //TODO: Pfad? oder bezug zum GUI-objekt; public

        /// <summary>
        /// Gibt eine matrix die Dargestellt werden soll an.
        /// </summary>
        public bool[,] matrix { get; set; }

        /// <summary>
        /// gibt an, dass es sich bei dem GUI-Element nicht um ein Text, Bild oder eine Matrix handelt sondern um otherContent (vgl. BrailleIO) -> ist ein Verweis zum Renderer
        /// </summary>
        public object otherContent { get; set; }


        /// <summary>
        /// Gibt den Bezug zu einem GUI-Element des gefilterten Baums an! Es kann jede der <code>GeneralProperties</code>-Eigenschaften angegeben werden. Der Wert dieser Eigenschaft soll angezeigt werden
        /// </summary>
        public String fromGuiElement { get; set; }

        /// <summary>
        /// Gibt an, ob Scrollbalken angezeigt werden sollen, sofern der Inhalt in der View nicht ausreichend Platz hat (falls nicht gesetzt, wird von true ausgegangen)
        /// </summary>
        public Boolean showScrollbar { get; set; }
    }
}
