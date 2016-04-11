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


        //General Accessibility / Interaction

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
        public String hWndFiltered
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


    }
}




