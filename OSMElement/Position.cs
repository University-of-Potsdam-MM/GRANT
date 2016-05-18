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
        public int width;
        public int height;
        public int left;
        public int top;

        //nötig?
        public int XOffset;
        public int Yoffset;
    }
}
