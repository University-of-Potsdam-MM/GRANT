using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSMElement
{
    public struct Position
    {
        public Padding padding { get; set; }
        public Padding margin { get; set; }
        public Padding boarder { get; set; }
        /// <summary>
        /// gibt die Breite der Ansicht in Pixel/Pins an.
        /// </summary>
        public int width;
        /// <summary>
        /// Gibt die Höhe der Ansicht (view) in Pixeln/Pins an.
        /// </summary>
        public int height;
        /// <summary>
        /// Gibt den Abstand in Pixeln/Pins nach links an
        /// </summary>
        public int left;
        /// <summary>
        /// Gibt den Abstand in Pixeln/Pins nach oben an
        /// </summary>
        public int top;

        //nötig?
        public int XOffset;
        public int Yoffset;
    }
}
