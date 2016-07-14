using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    public struct TextBox : IOtherContent
    {
        public bool showScrollbar { get; set; }
        public String text { get; set; }
        public bool isDisabled { get; set; }
        public String screen { get; set; }
    }
}
