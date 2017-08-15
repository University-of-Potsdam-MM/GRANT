using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSMElement.UiElements;
using System.Xml.Serialization;
using System.Windows;

namespace OSMElement
{
    //Attention: Every type wich will be used as <c>uiElementSpecialContent</c> must included here to save this project
    [XmlInclude(typeof(DropDownMenuItem))]
    [XmlInclude(typeof(ListMenuItem))]
    [XmlInclude(typeof(TabItem))]
    [Serializable]
    public class BrailleRepresentation
    {
        /* 
         * Relationship between typeOfView, screenName and viewName
         * There are different type of views (<c>typeOfView</c>) e.g. "symbol view", "layout view" and "text view". For each view many different screens (<c>screenName</c>) can exist. Each screen can contains many different views (<c>viewName</c>).
         */

        /// <summary>
        /// Category of View e.g. "symbol view", "layout view" and "text view"
        /// </summary>
        public String typeOfView { get; set; }

        /// <summary>
        /// name of the screen on which the content will be shown
        /// </summary>
        public String screenName { get; set; }

        /// <summary>
        /// name of the view on which the content will be shown
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Determines whether this view is visible.
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// Boolean matrix where <code>true</code> represents a shown pin
        /// </summary>
        [XmlIgnore]
        public bool[,] matrix { get; set; }

        /// <summary>
        /// Converted <c>matrix</c> to a jagged matrix
        /// this is importent to save and load a project
        /// </summary>
        public bool[][] jaggedMatrix {
            get 
            {                
                if (matrix != null)
                {
                    bool[][] resultmatrix = new bool[matrix.GetLength(0)][];
                    int width = matrix.Length / matrix.GetLength(0);
                     for (int i = 0; i < matrix.GetLength(0); i++)
                    {
                        resultmatrix[i] = new bool[width];
                        for (int j = 0; j < width; j++)
                        {
                            resultmatrix[i][j] = matrix[i, j];
                        }
                    }
                    return resultmatrix;
                }else{
                    return new bool[0][];
                }                
            }
            set
            {
                if (value != null && value.Count() > 0)
                {
                    bool[,] resultmatrix = new bool[value.GetLength(0), value[0].Length];
                    for (int i = 0; i < value.GetLength(0); i++)
                    {
                        for (int j = 0; j < value[0].Length ; j++)
                        {
                            resultmatrix[i, j] = value[i][j];
                        }
                    }
                    matrix = resultmatrix;
                }
            } 
        }

        private String _displayedGuiElementType;
        /// <summary>
        /// Name of the GUI element type of the filtered tree whose value should be shown in this view!
        /// Every controltype from <see cref="GeneralProperties"/> can be used.
        /// </summary>
        public String displayedGuiElementType
        {
            get { return _displayedGuiElementType; }
            set { if (value == null || GeneralProperties.getAllTypes().Contains(value)) { _displayedGuiElementType = value; } }
        }

        private int _contrast;
        /// <summary>
        /// Value of contrast for images 
        /// it must be between 0 and 255 
        /// </summary>
        public int contrast {
            get { return _contrast; }
            set { if (value >= 0 && value <= 255) { _contrast = value; } }
        }

        private double _zoom;
        /// <summary>
        /// zoom level for images
        /// </summary>
        public double zoom {
            get { return _zoom; }
            set { if (value <= 5) // '5' is the MAX_ZOOM_LEVEL in BrailleIO.BrailleIOViewRange
                { _zoom = value; } } }

        /// <summary>
        /// Determines whether scrollbar will be shown
        /// scrollbars are only shown if the view is large enough
        /// </summary>
        public bool isScrollbarShow { get; set; }

        /// <summary>
        /// special content for some UI elements
        /// see e.g <c>UiElements.TabItem</c>
        /// </summary>
        public object uiElementSpecialContent { get; set; }

        /// <summary>
        /// the padding
        /// </summary>
        public Padding padding { get; set; }

        /// <summary>
        /// the margin
        /// </summary>
        public Padding margin { get; set; }

        /// <summary>
        /// the boarder
        /// </summary>
        public Padding boarder { get; set; }

        /// <summary>
        /// z-index of the view. A lager z-index overlays a smaller.
        /// </summary>
        public int zIntex { get; set; }

        /// <summary>
        /// fully qualified name of the type for the used template, including its namespace but not its assembly
        /// It will be used to create childreen elements for groups
        /// </summary>
        public String templateFullName { get; set; }

        /// <summary>
        /// Simple name of the assembly. This is usually, but not necessarily,
        /// the file name of the manifest file of the assembly, minus its extension.
        /// It will be used to create childreen elements for groups
        /// </summary>
        public String templateNamspace { get; set; }

        /// <summary>
        /// Acronym for text
        /// </summary>
        public String textAcronym { get; set; }

        public override string ToString()
        {
            return String.Format("screenName = {0}, viewName = {1},  uiElementSpecialContent = {2}, typeOfView = {3}", screenName, viewName, uiElementSpecialContent == null ? "" : uiElementSpecialContent.ToString(), typeOfView);
        }

        /// <summary>
        /// A "description" for groups which are may be change during runtime e.g. a new tab item
        /// </summary>
        public GroupelementsOfSameType groupelementsOfSameType { get; set; }

        /// <summary>
        /// Determines whether the element is a child of a group.
        /// </summary>
        public bool isGroupChild { get; set; }

        /// <summary>
        /// Gives all Types of <see cref="BrailleRepresentation"/>
        /// </summary>
        /// <returns>list of all Types of <see cref="BrailleRepresentation"/></returns>
        public static List<String> getAllTypes()
        {
            List<String> displayedGuiElements = new List<string>();
            var brailleNames = typeof(BrailleRepresentation).GetProperties();
            foreach (var name in brailleNames)
            {
                if (!name.Name.ToString().Equals("jaggedMatrix"))
                {
                    displayedGuiElements.Add(name.Name.ToString());
                }
            }
            return displayedGuiElements;
        }

        public static List<DataTypeOSMElement> getAllTypes_possibleValues()
        {
            List<DataTypeOSMElement> displayedGuiElements = new List<DataTypeOSMElement>();
            var brailleNames = typeof(BrailleRepresentation).GetProperties();
            foreach (var name in brailleNames)
            {
                if (!name.Name.ToString().Equals("jaggedMatrix"))
                {
                    // displayedGuiElements.Add(name.Name.ToString());
                    displayedGuiElements.Add(new DataTypeOSMElement(name.Name.ToString(), name.PropertyType, getToolTipOfProperty(name.Name.ToString()), getPosibleValuesOfProperty(name.Name.ToString())));
                }
            }
            return displayedGuiElements;
        }

        private static string getToolTipOfProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "displayedGuiElementType":
                    return "Defines which property of the 'Filtered Tree' should be shown. To show this property a 'tree connection' must be set.";
                default:
                    return null;
            }
        }

        private static List<String> getPosibleValuesOfProperty(String propertyName)
        {
            switch (propertyName)
            {
                case "displayedGuiElementType":
                    return GeneralProperties.getAllTypes();
                default:
                    return null;
            }
        }

        public override bool Equals(object obj)
        {
            if ((this == null && obj != null) || (this != null && obj == null)) { return false; }
            if (!obj.GetType().Equals(typeof(BrailleRepresentation))) { return false; }
            BrailleRepresentation brailleR = (BrailleRepresentation)obj;
            bool result = true;
            if(this.boarder != null) { result = this.boarder.Equals(brailleR.boarder); }
            result = result && this.contrast.Equals(brailleR.contrast);
            if (this.displayedGuiElementType != null) { result = result && this.displayedGuiElementType.Equals(brailleR.displayedGuiElementType); }
            if (!this.groupelementsOfSameType.Equals(new GroupelementsOfSameType())) { result = result && this.groupelementsOfSameType.Equals(brailleR.groupelementsOfSameType); }
            result = result && this.isGroupChild.Equals(brailleR.isGroupChild);
            result = result && this.isScrollbarShow.Equals(brailleR.isScrollbarShow);
            result = result && this.isVisible.Equals(brailleR.isVisible);
          //  if (this.jaggedMatrix != null) { result = result && this.jaggedMatrix.Equals(brailleR.jaggedMatrix); }
            if (this.margin != null) { result = result && this.margin.Equals(brailleR.margin); }
            if (this.matrix != null) { result = result && this.matrix.Equals(brailleR.matrix); }
            if (this.padding != null) { result = result && this.padding.Equals(brailleR.padding); }
            if (this.screenName != null) { result = result && this.screenName.Equals(brailleR.screenName); }
            if (this.templateFullName != null) { result = result && this.templateFullName.Equals(brailleR.templateFullName); }
            if (this.templateNamspace != null) { result = result && this.templateNamspace.Equals(brailleR.templateNamspace); }
            if (this.textAcronym != null) { result = result && this.textAcronym.Equals(brailleR.textAcronym); }
            if (this.typeOfView != null) { result = result && this.typeOfView.Equals(brailleR.typeOfView); }
            if (this.uiElementSpecialContent != null) { result = result && this.uiElementSpecialContent.Equals(brailleR.uiElementSpecialContent); }
            if (this.viewName != null) { result = result && this.viewName.Equals(brailleR.viewName); }
            result = result && this.zIntex.Equals(brailleR.zIntex); 
            result = result && this.zoom.Equals(brailleR.zoom);
            return result;
        }

        /// <summary>
        /// Gets a specified property
        /// </summary>
        /// <param name="elementName">name of the wanted property</param>
        /// <param name="propertiesBraille">properties of the node</param>
        /// <returns>the wanted property from <para>properties</para> </returns>
        public static object getPropertyElement(String elementName, BrailleRepresentation propertiesBraille)
        {
            Type t;
           return  getPropertyElement(elementName, propertiesBraille, out t);
        }

        /// <summary>
        /// Gets a specified property
        /// </summary>
        /// <param name="elementName">name of the wanted property</param>
        /// <param name="propertiesBraille">properties of the node</param>
        /// <param name="propertyType">the datatype of the property</param>
        /// <returns>the wanted property from <para>properties</para> </returns>
        public static object getPropertyElement(String elementName, BrailleRepresentation propertiesBraille, out Type propertyType)
        {
            try
            { //see http://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection-in-c-sharp#
                propertyType = propertiesBraille.GetType().GetProperty(elementName).PropertyType;
                return propertiesBraille.GetType().GetProperty(elementName).GetValue(propertiesBraille, null);
            }
            catch
            {
                throw new Exception("Exception in OSMElement.BrailleRepresentaion: An attempt was made to query a non-existent property ('" + elementName + "')");
            }
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
                hash = hash * primeNumber[1] + (this.boarder != null ? boarder.GetHashCode() : 0);
                hash = hash * primeNumber[1] + contrast.GetHashCode();
                hash = hash * primeNumber[1] + (this.displayedGuiElementType != null ? displayedGuiElementType.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (!this.groupelementsOfSameType.Equals(new GroupelementsOfSameType()) ? groupelementsOfSameType.GetHashCode() : 0);
                hash = hash * primeNumber[1] + isGroupChild.GetHashCode();
                hash = hash * primeNumber[1] + isScrollbarShow.GetHashCode();
                hash = hash * primeNumber[1] + isVisible.GetHashCode();
                hash = hash * primeNumber[1] + (this.margin != null ? margin.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.padding != null ? padding.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.screenName != null ? screenName.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.templateFullName != null ? templateFullName.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.templateNamspace != null ? templateNamspace.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.textAcronym != null ? textAcronym.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.typeOfView != null ? typeOfView.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.uiElementSpecialContent != null ? uiElementSpecialContent.GetHashCode() : 0);
                hash = hash * primeNumber[1] + (this.viewName != null ? viewName.GetHashCode() : 0);
                hash = hash * primeNumber[1] + zIntex.GetHashCode();
                hash = hash * primeNumber[1] + zoom.GetHashCode();
                return hash;
            }
        }

    }

    [Serializable]
    public struct GroupelementsOfSameType
    {
        public Boolean? isLinebreak { get; set; }

       // public Boolean vertical { get; set; }
        public UiElements.Orientation orienataion { get; set; }
        public Rect childBoundingRectangle { get; set; }
        public String renderer { get; set; }

        public override string ToString()
        {
            return (this.Equals(new GroupelementsOfSameType()) ? "" : (String.Format("childBoundingRectangle: {0}, renderer: {1}", childBoundingRectangle, renderer)));
        }
    }
}
