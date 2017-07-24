using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    public class DataTypeOSMElement
    {
        public String OSMName { get; internal set; }
        public Type DataType { get; internal set; }
        public List<String> Values { get; internal set; }
        public Range Range { get; internal set; }

        public DataTypeOSMElement(String oSMName, Type dataType, List<String> values, int minRange, int maxRange)
        {
            OSMName = oSMName;
            DataType = dataType;
            Values = values;
            Range = new Range(minRange, maxRange);
        }

        public DataTypeOSMElement(String oSMName, Type dataType, List<String> values)
        {
            OSMName = oSMName;
            DataType = dataType;
            Values = values;
        }

        public DataTypeOSMElement(String oSMName, Type dataType, int minRange, int maxRange)
        {
            OSMName = oSMName;
            DataType = dataType;
            Range = new Range(minRange, maxRange);
        }

        public bool Remove(String name)
        {
            return false;
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
