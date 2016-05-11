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
        Padding padding { get; set; }
        Padding margin { get; set; }
        Padding boarder { get; set; }
        int width;
        int height;

        //nötig?
        int XOffset;
        int Yoffset;
    }
}
