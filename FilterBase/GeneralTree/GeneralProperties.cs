using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilterBase
{
    public struct GeneralProperties
    {
        public String Name
        {
            get;
            set;
        }

        //TODO: evtl. Enum für ControlType erstellen
        public String LocalizedControlType
        {
            get;
            set;
        }

        public Rect BoundingRectangle
        {
            get;
            set;
        }


        public Boolean? IsEnabled
        {// Boolean? => true, false, null
            get;
            set;
        }
    }
}
