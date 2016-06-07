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
        public String text { get; set; } //TODO: Man könnte an der Stelle auch immer den "dynamisch" ausgelesenen Text reinschreiben
        Image image { get; set; } //TODO: Pfad? oder bezug zum GUI-objekt; public
        public bool[,] matrix { get; set; }
        public String viewName { get; set; }
        public String fromGuiElement { get; set; }
        public Boolean showScrollbar { get; set; }
    }
}
