﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    /// <summary>
    /// Represents a control that can be set to a value within a range.
    /// Like System.Windows.Automation.RangeValuePattern
    /// </summary>
    [Serializable]
    public struct RangeValue
    {
        public Boolean isReadOnly { get; set; }
        public Double maximum { get; set; }
        public Double minimum { get; set; }
        public Double currentValue { get; set; }

        public Double largeChange { get; set; }
        public Double smallChange { get; set; }

        public override string ToString()
        {
            return (this.Equals(new RangeValue()) ? "" : (String.Format("currentValue: {0}, maximum: {1}, minimum: {2}, largeChange: {3}, smallChange: {4} ", currentValue, maximum, minimum, largeChange, smallChange)));
        }
    }
}
