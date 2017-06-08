using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace OSMElement
{
    [Serializable]
    public class GeneralProperties
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
        internal String _idGenerated;

        /// <summary>
        /// The generated Id of the element
        /// </summary>
        public String IdGenerated
        {
            get { return _idGenerated; }
            set { _idGenerated = value;  }
          /*  set
            {
                if (_idGenerated == null) { _idGenerated = value; }
                else
                {
                    if (!_idGenerated.Equals(value))
                    {
                        Debug.WriteLine("");
                    }
                }
            }*/
        }

        /// <summary>
        /// The name of the filter strategy. It is only set if another is selected than for the root node.
        /// </summary>
        public String grantFilterStrategy { get; set; }

        /// <summary>
        /// the name of the process.
        /// It will be important to reload a screen reader.
        /// Only set at the first node.
        /// Attention: processName != className AND processName != nameFiltered
        /// </summary>
        public String processName { get; set; }

        /// <summary>
        /// The full path (including the name) to the application.
        /// </summary>
        public String appPath { get; set; }

        public override string ToString()
        {
            return String.Format("nameFiltered = {0}, valueFiltered = {1}, controlTypeFiltered = {2},  boundingRectangleFiltered = {3}, id = {4} ", nameFiltered, valueFiltered, controlTypeFiltered, boundingRectangleFiltered.ToString(), IdGenerated);
        }
        #endregion

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

        /// <summary>
        /// Gets a specified property
        /// </summary>
        /// <param name="elementName">name of the wanted property</param>
        /// <param name="properties">properties of the node</param>
        /// <returns>the wanted property from <para>properties</para> </returns>
        public static object getPropertyElement(String elementName, GeneralProperties properties)
        {
            try
            { //see http://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection-in-c-sharp#
                return properties.GetType().GetProperty(elementName).GetValue(properties, null);
            }
            catch
            {
                throw new Exception("Exception in OSMElement.GeneralProperties: An attempt was made to query a non-existent property ('" + elementName + "')");
            }
        }

        public bool ItemsEqual(object [] objA, object [] objB)
        {
            if ((objA == null && objB != null) || (objA != null && objB == null)) { return false; }
            if (objA == null && objB == null) { return true; }
            if (objA.Length != objB.Length) { return false; }
            if (objA.Length != objB.Length) { return false; }
            int i = 0;
            foreach(Object o in objA)
            {
                if (!o.Equals(objB[i]))
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public bool ItemsEqual(int[] objA, int[] objB)
        {
            if((objA == null && objB != null) || (objA != null && objB == null)) { return false; }
            if(objA == null && objB == null) { return true; }
            if (objA.Length != objB.Length) { return false; }
            int i = 0;
            foreach (Object o in objA)
            {
                if (!o.Equals(objB[i]))
                {
                    return false;
                }
                i++;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if ((this == null && obj != null) || (this != null && obj == null)) { return false; }
            if (!obj.GetType().Equals(typeof(GeneralProperties))) { return false; }
            GeneralProperties prop = (GeneralProperties)obj;
            bool result = true;
            if (this.acceleratorKeyFiltered != null) { result = this.acceleratorKeyFiltered.Equals(prop.acceleratorKeyFiltered); }
            if (this.accessKeyFiltered != null) { result = result && this.accessKeyFiltered.Equals(prop.accessKeyFiltered); }
            if (this.appPath != null) { result = result && this.appPath.Equals(prop.appPath); }
            if (this.autoamtionIdFiltered != null) { result = result && this.autoamtionIdFiltered.Equals(prop.autoamtionIdFiltered); }
            if (this.boundingRectangleFiltered != null) { result = result && this.boundingRectangleFiltered.Equals(prop.boundingRectangleFiltered); }
            if (this.classNameFiltered != null) { result = result && this.classNameFiltered.Equals(prop.classNameFiltered); }
            if (this.controlTypeFiltered != null) { result = result && this.controlTypeFiltered.Equals(prop.controlTypeFiltered); }
            if (this.frameWorkIdFiltered != null) { result = result && this.frameWorkIdFiltered.Equals(prop.frameWorkIdFiltered); }
            if (this.grantFilterStrategy != null) { result = result && this.grantFilterStrategy.Equals(prop.grantFilterStrategy); }
            if (this.hasKeyboardFocusFiltered != null) { result = result && this.hasKeyboardFocusFiltered.Equals(prop.hasKeyboardFocusFiltered); }
            if (this.helpTextFiltered != null) { result = result && this.helpTextFiltered.Equals(prop.helpTextFiltered); }
            if (this.hWndFiltered != null) { result = result && this.hWndFiltered.Equals(prop.hWndFiltered); }
            if (this.IdGenerated != null) { result = result && this.IdGenerated.Equals(prop.IdGenerated); }
            if (this.isContentElementFiltered != null) { result = result && this.isContentElementFiltered.Equals(prop.isContentElementFiltered); }
            if (this.isControlElementFiltered != null) { result = result && this.isControlElementFiltered.Equals(prop.isControlElementFiltered); }
            if (this.isEnabledFiltered != null) { result = result && this.isEnabledFiltered.Equals(prop.isEnabledFiltered); }
            if (this.isKeyboardFocusableFiltered != null) { result = result && this.isKeyboardFocusableFiltered.Equals(prop.isKeyboardFocusableFiltered); }
            if (this.isOffscreenFiltered != null) { result = result && this.isOffscreenFiltered.Equals(prop.isOffscreenFiltered); }
            if (this.isPasswordFiltered != null) { result = result && this.isPasswordFiltered.Equals(prop.isPasswordFiltered); }
            if (this.isRequiredForFormFiltered != null) { result = result && this.isRequiredForFormFiltered.Equals(prop.isRequiredForFormFiltered); }
            if (this.isToggleStateOn != null) { result = result && this.isToggleStateOn.Equals(prop.isToggleStateOn); }
            if (this.itemStatusFiltered != null) { result = result && this.itemStatusFiltered.Equals(prop.itemStatusFiltered); }
            if (this.itemTypeFiltered != null) { result = result && this.itemTypeFiltered.Equals(prop.itemTypeFiltered); }
            if (this.labeledByFiltered != null) { result = result && this.labeledByFiltered.Equals(prop.labeledByFiltered); }
            if (this.localizedControlTypeFiltered != null) { result = result && this.localizedControlTypeFiltered.Equals(prop.localizedControlTypeFiltered); }
            if (this.nameFiltered != null) { result = result && this.nameFiltered.Equals(prop.nameFiltered); }
            result = result && this.processIdFiltered.Equals(prop.processIdFiltered);
            if (this.processName != null) { result = result && this.processName.Equals(prop.processName); }
            if (!this.rangeValue.Equals(new RangeValue())) { result = result && this.rangeValue.Equals(prop.rangeValue); }
            // if (this.runtimeIDFiltered != null) { result = result && ItemsEqual( this.runtimeIDFiltered, prop.runtimeIDFiltered); }
            if (this.suportedPatterns != null) { result = result && ItemsEqual( this.suportedPatterns, prop.suportedPatterns); }
            if (this.valueFiltered != null) { result = result && this.valueFiltered.Equals(prop.valueFiltered); }
            return result;

        }

        /// <summary>
        /// Hash function.
        /// Attention: The object is mutable, the hash code can change! 
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        { //see: https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            unchecked // Overflow is fine, just wrap
            {
                int[] primeNumber = new int[] { 56467, 606241 };
                int hash = primeNumber[0]; // 	prime number 
                // Suitable nullity checks etc, of course :)
                hash = hash * primeNumber[1] + (this.acceleratorKeyFiltered != null ? acceleratorKeyFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.accessKeyFiltered != null ? accessKeyFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.appPath != null ? appPath.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.autoamtionIdFiltered != null ? autoamtionIdFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.boundingRectangleFiltered != null ? boundingRectangleFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.classNameFiltered != null ? classNameFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.controlTypeFiltered != null ? controlTypeFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.frameWorkIdFiltered != null ? frameWorkIdFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.grantFilterStrategy != null ? grantFilterStrategy.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.hasKeyboardFocusFiltered != null ? hasKeyboardFocusFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.helpTextFiltered != null ? helpTextFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.hWndFiltered != null ? hWndFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.IdGenerated != null ? IdGenerated.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isContentElementFiltered != null ? isContentElementFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isControlElementFiltered != null ? isControlElementFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isEnabledFiltered != null ? isEnabledFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isKeyboardFocusableFiltered != null ? isKeyboardFocusableFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isOffscreenFiltered != null ? isOffscreenFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isPasswordFiltered != null ? isPasswordFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isRequiredForFormFiltered != null ? isRequiredForFormFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.isToggleStateOn != null ? isToggleStateOn.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.itemStatusFiltered != null ? itemStatusFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.itemTypeFiltered != null ? itemTypeFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.labeledByFiltered != null ? labeledByFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.localizedControlTypeFiltered != null ? localizedControlTypeFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.nameFiltered != null ? nameFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + processIdFiltered.GetHashCode();
                hash = hash * primeNumber[1] + (this.processName != null ? processName.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (!this.rangeValue.Equals(new RangeValue()) ? rangeValue.GetHashCode() : 0);
              // hash = hash * primeNumber[1] + (this.runtimeIDFiltered != null ? runtimeIDFiltered.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.suportedPatterns != null ? suportedPatterns.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.valueFiltered != null ? valueFiltered.GetHashCode() : 0);
                return hash;
            }
        }
    }
}




