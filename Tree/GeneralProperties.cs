using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Tree
{
    public struct GeneralProperties
    {

       
        //TODO: evtl. Enum für ControlType erstellen
        public String controlsTypeFiltered
        {
            get;
            set;
        }

        //Property: isEnabled
        // Boolean? => true, false, null
        public Boolean? disabedFiltered
        {
            get;
            set;
        }
        //Property: BoundingRectangle
        public Rect boundingRectangleFiltered
        {
            get;
            set;
        }

        //Property: Name
        public String nameFiltered
        {
            get;
            set;
        }

        //Property: AutoamtionId
        public int IdFiltered
        {
            get;
            set;
        }

        public int IdGenerated
        {
            get;
            set;
        }

        public Boolean? checkedFiltered
        {
            get;
            set;
        }

        public String controlsForFiltered
        {
            get;
            set;
        }

        public String describedbyFiltered
        {
            get;
            set;
        }

        
        public String flowtoFiltered
        {
            get;
            set;
        }

        public Boolean? invalidFiltered
        {
            get;
            set;
        }

        public String labelledbyFiltered
        {
            get;
            set;
        }

        public Boolean? liveFiltered
        {
            get;
            set;
        }


        public Boolean? multiselectedtableFiltered
        {
            get;
            set;
        }

        public Boolean? readonlyFiltered
        {
            get;
            set;
        }

        public Boolean? requiredFiltered
        {
            get;
            set;
        }

        public Boolean? secretFiltered
        {
            get;
            set;
        }

        public int valuemaxFiltered
        {
            get;
            set;
        }

        public int valueminFiltered
        {
            get;
            set;
        }

        public String valuenowFiltered
        {
            get;
            set;
        }


       



}
}

