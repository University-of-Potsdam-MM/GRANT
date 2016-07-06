using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{ 
    public struct BrailleRepresentation
    {
        public Position position { get; set; }
        public Content content { get; set; }
        /// <summary>
        /// Gibt den Namen des Screens an, auf welchem die View angezeigt werden soll
        /// </summary>
        public String screenName { get; set; }
        /// <summary>
        /// Gibt die View, in welcher der Inhalt angezeigt werden soll an.
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Gibt an, ob der Inhalt sichtbar sein soll
        /// </summary>
        public bool isVisible { get; set; }
    }
}
