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
        String text { get; set; } //TODO: Angabe für "dynamischen" Text
        Image image { get; set; } //TODO: Pfad? oder bezug zum GUI-objekt
        bool[,] matrix { get; set; }
        String viewName { get; set; }
    }
}
