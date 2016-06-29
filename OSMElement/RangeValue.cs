using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Entspricht dem DatenType System.Windows.Automation.RangeValuePattern; damit dieser aber nicht UIA spezifisch ist, wurde er neu erstellt
    /// </summary>
    public struct RangeValue
    {
        public Boolean isReadOnly { get; set; }
        public Double maximum { get; set; }
        public Double minimum { get; set; }
        public Double currentValue { get; set; }

        //brauchen wir die beiden nachfolgenden Werte? Evtl. dann für die Braille-GUI
        public Double largeChange { get; set; }
        public Double smallChange { get; set; }
    }
}
