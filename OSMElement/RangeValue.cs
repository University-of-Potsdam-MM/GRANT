using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Represents a control that can be set to a value within a range.
    /// Like System.Windows.Automation.RangeValuePattern
    /// </summary>
    public struct RangeValue
    {
        public Boolean isReadOnly { get; set; }
        public Double maximum { get; set; }
        public Double minimum { get; set; }
        public Double currentValue { get; set; }

        public Double largeChange { get; set; }
        public Double smallChange { get; set; }
    }
}
