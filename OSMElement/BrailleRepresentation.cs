using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSMElement.UiElements;
using System.Xml.Serialization;

namespace OSMElement
{
    [XmlInclude(typeof(DropDownMenu))]  //Achtung, jeder Type der bei <c>uiElementSpecialContent</c> verwendet wird muss mittels xmlInclude hinzugefügt werde, damit das Objekt gespeichert werden kann
    public struct BrailleRepresentation
    {
        /// <summary>
        /// Gibt die View, in welcher der Inhalt angezeigt werden soll an.
        /// </summary>
        public String viewName { get; set; }

        /// <summary>
        /// Gibt an, ob der Inhalt/die View sichtbar sein soll
        /// </summary>
        public bool isVisible { get; set; }

        /// <summary>
        /// Gibt eine Matrix die dargestellt werden soll an.
        /// </summary>
        [XmlIgnore]
        public bool[,] matrix { get; set; }

        /// <summary>
        /// Wandelt die mehrdimensionale Matrix in eine Jagged-Matrix um um diese zu speichern bzw. zu laden
        /// </summary>
        public bool[][] jaggedMatrix {
            get 
            {
                
                if (matrix != null)
                {
                    bool[][] resultmatrix = new bool[matrix.GetLength(0)][];
                    int width = matrix.Length / matrix.GetLength(0);
                    System.Diagnostics.Debug.WriteLine("getLengt(0) = " + matrix.GetLength(0));
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

        /// <summary>
        /// Gibt den Bezug zu einem GUI-Element des gefilterten Baums an! Es kann jede der <code>GeneralProperties</code>-Eigenschaften angegeben werden. Der Wert dieser Eigenschaft soll angezeigt werden
        /// </summary>
        public String fromGuiElement { get; set; }

        public int contrast { get; set; }
        public double zoom { get; set; }

        /// <summary>
        /// Gibt den Namen des Screens an, auf welchem die View angezeigt werden soll
        /// </summary>
        public String screenName { get; set; }

        /// <summary>
        /// Enthält den darzustellenden Text eines UI-Elements
        /// </summary>
     //   public String text { get; set; }

        /// <summary>
        /// Gibt an, ob Scrollbalken angezeigt werden sollen, sofern der Inhalt in der View nicht ausreichend Platz hat (falls nicht gesetzt, wird von true ausgegangen)
        /// </summary>
        public bool showScrollbar { get; set; }

        /// <summary>
        /// Gibt für UI-Elemente weiteren (speziellen) Inhalt an
        /// </summary>
        public object uiElementSpecialContent { get; set; }
        public Padding padding { get; set; }
        public Padding margin { get; set; }
        public Padding boarder { get; set; }

        /// <summary>
        /// Gibt den Z-Index an. Ein Element mit einem größeren z-Index liegt weiter oben.
        /// </summary>
        public int zIntex { get; set; }

        /// <summary>
        /// Gibt den FullName des Typs des zu  nutzenden Templates für die Kindelemente an;
        /// wird nur bei Elternelementen von Gruppen benötigt (vgl. isControlElementFiltered)
        /// </summary>
        public String templateFullName { get; set; }

        /// <summary>
        /// Gibt den Namespace des Typs des zu  nutzenden Templates für die Kindelemente an;
        /// wird nur bei Elternelementen von Gruppen benötigt (vgl. isControlElementFiltered)
        /// </summary>
        public String templateNamspace { get; set; }

        public override string ToString()
        {
            return String.Format("screenName = {0}, viewName = {1},  uiElementSpecialContent = {2}", screenName, viewName, uiElementSpecialContent == null ? "" : uiElementSpecialContent.ToString());
        }

        public Groupelements groupelements { get; set; }
    }

    public struct Groupelements
    {
        /// <summary>
        /// Bei Gruppen-elementen gibt der Wert an, ob am Ende des sichtbaren Bereiches ein Zeilenumbruch (<c>true</c>) erfolgen soll; bei allen anderen Elementen ist der Wert <c>null</c>
        /// </summary>
        public Boolean? linebreak { get; set; }

        public Boolean vertical { get; set; }
        public int? max { get; set; }
    }
}
