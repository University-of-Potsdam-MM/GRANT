using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSMElement
{
    //Properties
    public struct GeneralProperties
    {

        //General Accessibility / Interaction

        //Anmerkung: laut den Beschreibungen scheint acceleratorKey und accessKey das gleiche zu sein (https://msdn.microsoft.com/en-us/library/ff400332%28VS.95%29.aspx)
        // -> Es werden aber nicht immer beide Werte zugewiesen
        public String acceleratorKeyFiltered
        {
            get;
            set;
        }

        public String accessKeyFiltered
        {
            get;
            set;
        }



        public Boolean? isKeyboardFocusableFiltered
        {
            get;
            set;
        }

        public int[] runtimeIDFiltered
        {
            get;
            set;
        }

        // STATE

        // Boolean? => true, false, null
        public Boolean? isEnabledFiltered
        {
            get;
            set;
        }

        public Boolean? hasKeyboardFocusFiltered
        {
            get;
            set;
        }

        // Visibility

        public Rect boundingRectangleFiltered
        {
            get;
            set;
        }

        public Boolean? isOffscreenFiltered
        {
            get;
            set;
        }

        public String helpTextFiltered
        {
            get;
            set;
        }


        //IDENTIFICATION/Elemttype

        //nicht von UIA
        public String IdGenerated
        {
            get;
            set;
        }

        public String autoamtionIdFiltered
        {
            get;
            set;
        }


        public String classNameFiltered
        {
            get;
            set;
        }

        //Anmerkung: ich habe den LocalizedControlType genommen
        public String controlTypeFiltered
        {
            get;
            set;
        }

        public String frameWorkIdFiltered
        {
            get;
            set;
        }

        //typ?
        // Anmerkung: von String zu int geändert
        public IntPtr hWndFiltered
        {
            get;
            set;
        }

        public Boolean? isContentElementFiltered
        {
            get;
            set;
        }
        //typ?
        public String labeledbyFiltered
        {
            get;
            set;
        }

        public Boolean? isControlElementFiltered
        {
            get;
            set;
        }

        public Boolean? isPasswordFiltered
        {
            get;
            set;
        }

        public String localizedControlTypeFiltered
        {
            get;
            set;
        }

        public String nameFiltered
        {
            get;
            set;
        }

        public int processIdFiltered
        {
            get;
            set;
        }

        public String itemTypeFiltered
        {
            get;
            set;
        }
        public String itemStatusFiltered
        {
            get;
            set;
        }
        public Boolean? isRequiredForFormFiltered
        {
            get;
            set;
        }

        public String valueFiltered { get; set; }

        public RangeValue rangeValue
        {
            get; set; 
        }


        /// <summary>
        /// Enthält die unterstützten Pattern
        /// </summary>
        public object[] suportedPatterns { get; set; }
        
    }
}




