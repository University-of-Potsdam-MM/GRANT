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
        public String text { get; set; } //TODO: Angabe für "dynamischen" Text
        Image image { get; set; } //TODO: Pfad? oder bezug zum GUI-objekt; public
        bool[,] matrix { get; set; } //TODO: public
        public String viewName { get; set; }
        public String fromGuiElement { get; set; }
    }
}
