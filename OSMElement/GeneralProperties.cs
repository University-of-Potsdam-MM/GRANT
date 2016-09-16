using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OSMElement
{
    //Propertie
    [Serializable]
    public struct GeneralProperties
    {
        //Link properties uia: https://msdn.microsoft.com/en-us/library/windows/desktop/ee684017(v=vs.85).aspx

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

        [XmlIgnore] //ist beim neuladen veraltet
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

        //[XmlIgnore] //ist beim neuladen veraltet im gefilterten Baum, aber wird für den Braille Baum  benötigt
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

        //Anmerkung: ProgrammaticName
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

        [XmlIgnore]
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

        [XmlIgnore]
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

        public Boolean? isToggleStateOn { get; set; }

        /// <summary>
        /// Enthält die unterstützten Pattern
        /// </summary>
        [XmlIgnore]
        public object[] suportedPatterns { get; set; }

        /// <summary>
        /// gibt die Filter-Strategy an, sofern dieses bei einem Knoten abweichend ist 
        /// (wird gesetzt beim Update eines Konotens gesetzt, wenn es nicht der selbe Filter wie für die ganze Anwendung ist)
        /// </summary>
         public String grantFilterStrategy { get; set; }
      //  public String grantFilterStrategyFullName { get; set; }
      //  public String grantFilterStrategyNamespace { get; set; }
        

        /// <summary>
        /// Gibt den Namen der Anwendung an -- wird benötigt um die Anwendung später wiederzufinden (modulName != className bzw. nameFiltered)
        /// (nur für den ersten Knoten wichtig)
        /// </summary>
        public String moduleName { get; set; }
        public String fileName { get; set; }

        public override string ToString()
        {//Achtung nur einige Eigenschaften berücksichtigt
            return String.Format("nameFiltered = {0}, valueFiltered = {1}, controlTypeFiltered = {2},  boundingRectangleFiltered = {3} ", nameFiltered, valueFiltered, controlTypeFiltered, boundingRectangleFiltered.ToString());
        }
    }
}




