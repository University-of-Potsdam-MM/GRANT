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
        #region properties similar to UIA
        //see uia: https://msdn.microsoft.com/en-us/library/windows/desktop/ee684017(v=vs.85).aspx and https://msdn.microsoft.com/en-us/library/ff400332%28VS.95%29.aspx

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

        /// <summary>
        /// a string containing the UI Automation identifier (ID) for the element
        /// TODO: shouldn't be a property here?
        /// </summary>
        public String autoamtionIdFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// a string containing the class name of the element as assigned by the control
        /// </summary>
        public String classNameFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// ControlType of the element
        /// </summary>
        public String controlTypeFiltered
        {
            get;
            set;
        }

        /// <summary>
        ///  A localized description of the control type
        /// </summary>
        public String localizedControlTypeFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the underlying UI framework, 
        /// such as "Win32", "WinForm", or "DirectUI". The default value is an empty string.
        /// </summary>
        public String frameWorkIdFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// The handle of the element's window 
        /// if one exists; otherwise 0.
        /// </summary>
        [XmlIgnore]
        public IntPtr hWndFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// a value that specifies whether the element is a content element
        /// </summary>
        public Boolean? isContentElementFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// the element that contains the text label for this element.
        /// </summary>
        public String labeledByFiltered
        {// Currently not used => for example it will be used in MS Paint (slider zoom)
            get;
            set;
        }

        /// <summary>
        /// a value that indicates whether the element is viewed as a control.
        /// </summary>
        public Boolean? isControlElementFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// a value that indicates whether the element contains protected content
        /// </summary>
        public Boolean? isPasswordFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the user interface (UI) element.
        /// </summary>
        public String nameFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the process that hosts the element. The default value for the property is 0.
        /// </summary>
        [XmlIgnore]
        public int processIdFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// A localized string that describes the item.
        /// </summary>
        public String itemTypeFiltered
        {
            get;
            set;
        }

        /// <summary>
        /// A description of the status of an item within an element.
        /// </summary>
        public String itemStatusFiltered
        {
            get;
            set;
        }

        /// <summary>
        ///  A value that indicates whether the UI Automation element is required to be filled out on a form.
        /// </summary>
        public Boolean? isRequiredForFormFiltered
        {
            get;
            set;
        }

        #region properties from pattern
        /// <summary>
        /// Enthält die unterstützten Pattern
        /// </summary>
        [XmlIgnore]
        public object[] suportedPatterns { get; set; }

        /// <summary>
        /// the value of the element
        /// </summary>
        public String valueFiltered { get; set; }

        /// <summary>
        /// The RangeValue of the element
        ///  Conditional Support e.g.: Edit, Progress Bar, Scroll Bar, Slider, Spinner
        /// </summary>
        public RangeValue rangeValue
        {
            get; set; 
        }

        /// <summary>
        /// Determines whether the element is selected, checked, marked or otherwise activated.
        /// </summary>
        public Boolean? isToggleStateOn { get; set; }
        #endregion
        #endregion

        #region other properties
        public String IdGenerated
        {
            get;
            set;
        }

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
        #endregion
    }


}




