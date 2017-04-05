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
        //see properties uia: https://msdn.microsoft.com/en-us/library/windows/desktop/ee684017(v=vs.85).aspx and https://msdn.microsoft.com/en-us/library/ff400332%28VS.95%29.aspx

        /// <summary>
        /// a string containing the accelerator key combinations for the element.
        /// </summary>
        public String acceleratorKeyFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// a string containing the access key character for the element.
        /// </summary>
        public String accessKeyFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// a value that indicates whether the element can accept keyboard focus.
        /// </summary>
        public Boolean? isKeyboardFocusableFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// Retrieves the unique identifier assigned to the user interface (UI) item.
        /// Attention: This value is deprecated after reloading
        /// </summary>
        [XmlIgnore]
        public int[] runtimeIDFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value that indicates whether the element is enabled.
        /// </summary>
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

        /// <summary>
        /// The coordinates of the rectangle that completely encloses the element.
        /// Attention: This value is deprecated after reloading for the filtered tree but it is necessary for the braille tree.
        /// </summary>
        public Rect boundingRectangleFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// A value that indicates whether the element is visible on the screen.
        /// </summary>
        public Boolean? isOffscreenFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// The help text associated with the element.
        /// </summary>
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
        {
            return String.Format("nameFiltered = {0}, valueFiltered = {1}, controlTypeFiltered = {2},  boundingRectangleFiltered = {3}, id = {4} ", nameFiltered, valueFiltered, controlTypeFiltered, boundingRectangleFiltered.ToString(), IdGenerated);
        }

        /// <summary>
        /// Gives all Types of <see cref="GeneralProperties"/>
        /// </summary>
        /// <returns>list of all Types of <see cref="GeneralProperties"/></returns>
        public static List<String> getAllTypes()
        {//displayedGuiElementType
            List<String> displayedGuiElements = new List<string>();
            var propNames = typeof(GeneralProperties).GetProperties();
            foreach (var name in propNames)
            {
                displayedGuiElements.Add(name.Name.ToString());
            }
            return displayedGuiElements;
        }
    }

  
}




