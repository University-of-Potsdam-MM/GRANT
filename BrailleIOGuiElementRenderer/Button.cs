using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleIOGuiElementRenderer
{
    public struct Button : IOtherContent
    {
        public bool isDisabled { get; set; }
        public String text { get; set; }
    }
}
