﻿using System;
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
    public struct BrailleRepresentation
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

        /// <summary>
        /// Name of the GUI element type of the filtered tree whose value should be shown in this view!
        /// Every controltype from <code>GeneralProperties</code> can be used.
        /// </summary>
        public String displayedGuiElementType { get; set; }

        /// <summary>
        /// Value of contrast for images 
        /// it must be between 0 and 255 
        /// </summary>
        public int contrast { get; set; }

        /// <summary>
        /// zoom level for images
        /// </summary>
        public double zoom { get; set; }

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

        public Padding padding { get; set; }
        public Padding margin { get; set; }
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
            return String.Format("screenName = {0}, viewName = {1},  uiElementSpecialContent = {2}, screenCategory = {3}", screenName, viewName, uiElementSpecialContent == null ? "" : uiElementSpecialContent.ToString(), typeOfView);
        }

        /// <summary>
        /// A "description" for groups which are may be change during runtime e.g. a new tab item
        /// </summary>
        public GroupelementsOfSameType groupelementsOfSameType { get; set; }

        /// <summary>
        /// Determines whether the element is a child of a group.
        /// </summary>
        public bool isGroupChild { get; set; }

    }

    public struct GroupelementsOfSameType
    {
        public Boolean? isLinebreak { get; set; }

       // public Boolean vertical { get; set; }
        public UiElements.Orientation orienataion { get; set; }
        public Rect childBoundingRectangle { get; set; }
        public String renderer { get; set; }
    }
}
