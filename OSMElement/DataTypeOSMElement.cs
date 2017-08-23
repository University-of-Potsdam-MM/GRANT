using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    /// <summary>
    /// Properties of an OSMElement for the "DAu" UI
    /// </summary>
    public class DataTypeOSMElement
    {
        public String OSMName { get; internal set; }
        public Type DataType { get; internal set; }
        public List<String> Values { get; set; }
        public Range Range { get; internal set; }
        public String Tooltip { get; internal set; }

        public DataTypeOSMElement(String oSMName, Type dataType, String tooltip, List<String> values, int minRange, int maxRange)
        {
            OSMName = oSMName;
            DataType = dataType;
            setValues(dataType, ref values);
            Values = values;
            Range = new Range(minRange, maxRange);
            Tooltip = tooltip;
        }

        public DataTypeOSMElement(String oSMName, Type dataType, String tooltip, List<String> values)
        {
            OSMName = oSMName;
            DataType = dataType;
            setValues(dataType, ref values);
            Values = values;
            Tooltip = tooltip;
            
        }

      public DataTypeOSMElement(String oSMName, Type dataType, String tooltip, int minRange, int maxRange)
        {
            OSMName = oSMName;
            DataType = dataType;
            Range = new Range(minRange, maxRange);
            Tooltip = tooltip;
        }
        
        private void setValues(Type dataType, ref List<String> values)
        {
            if (dataType.Equals(typeof(Boolean)) || dataType.Equals(typeof(bool)))
            {
                values = new List<string>(new String[] { "True", "False" });
            }
            else
            {
                if (dataType.Equals(typeof(Boolean?)) || dataType.Equals(typeof(bool?)))
                {
                    values = new List<string>(new String[] { "True", "False", "" });
                }
                else
                {
                    Values = values;
                }
            }
        }
    }

    public class Range
    {
        public int maxRange { get; internal set; }
        public int minRange { get; internal set; }

        public Range(int minRange, int maxRange)
        {
            this.minRange = minRange;
            this.maxRange = maxRange;
        }
        
    }
}
