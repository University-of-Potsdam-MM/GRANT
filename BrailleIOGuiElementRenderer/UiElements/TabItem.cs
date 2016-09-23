using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer.UiElements
{
    public struct TabItem
    {
       // public bool isVertical { get; set; }

        /// <summary>
        /// Gibt die Orientation der Tab-Leiste an;
        /// je nach Orientation müssen die Tabs anders geöffnet sein
        /// </summary>
        public Orientation orientation { get; set; }
        /// <summary>
        /// Gibt das Inhaltsobjekt einer Tab-View an
        /// dies könnte z.B. bei einem Editor mit mehreren Tab das eigentliche (offene) Text-Feld sein
        /// </summary>
       // public object content { get; set; } --> TODO?
        //isDisabled --> Element ist aktiv (kann daher nicht mehr aktiviert werden)

        public override string ToString()
        {
            return String.Format("TabItem:  orientation = {0}", orientation);
        }
    }

    public enum Orientation { Left, Top, Bottom, Right};
}
