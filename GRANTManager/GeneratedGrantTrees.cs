using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using OSMElement;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;

namespace GRANTManager
{
    /// <summary>
    /// Object with the generated trees, there OSM connections and there events;
    /// Übergabe aller OSM-Bäume und -Verbindungen: OSM-Original, OSM-Braille - Verbindung OSMTreeConnection; OSM-Event OSM-Action und Verbindung zwischen diesen und OSM-Original/Braille: OSMNodeEventActionConnector
    /// </summary>
    public class GeneratedGrantTrees
    {
        /// <summary>
        /// filtered tree object
        /// </summary>
        public Object filteredTree { get; set; }

        /// <summary>
        /// braille (output) tree object
        /// </summary>
        public Object brailleTree { get; set; }

        public List<OSMEvent> osmEvents { get; set; }

        public List<OSMAction> osmActions { get; set; }
        
        /// <summary>
        /// Specifies the relationship between "filteredTree" and "brailleTree" based on the generated id
        /// </summary>
        public List<OsmTreeConnectorTuple> osmTreeConnections = new List<OsmTreeConnectorTuple>();

        /// <summary>
        /// Specifies the connection between a Node of a Tree (filtered tree or Braille tree) <--> Event <--> Action
        /// </summary>
        public List<OSMTreeEvenActionConnectorTriple> osmTreeEventActionConnection = new List<OSMTreeEvenActionConnectorTriple>();

        /// <summary>
        /// A list with the default renderer for the controlle types.
        /// </summary>
        public List<RendererUiElementConnector> rendererUiElementConnection { get; set; }

        private TextviewObject textviewobject;
        public TextviewObject TextviewObject { get { if (textviewobject.Equals(new TextviewObject())) {loadTemplateAllElementsTextview(); }   return textviewobject;  } set { textviewobject = value; } }

        #region auslesen aus XML
        /// <summary>
        /// Reads in a XML file informations about the design of the 'Textview' and posible acronyms
        /// </summary>
        /// <param name="path">path to the XML file</param>
        private void loadTemplateAllElementsTextview(String path = null)
        {
            if (path == null)
            {
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TemplateAllElementsTextview.xml");
            }
            if (!File.Exists(path)) { Debug.WriteLine("The XML file dosn't exist!"); return; }
            XElement xmlDoc = XElement.Load(@path);
            //TODO: hier gegen XSD validieren
            TextviewObject tvo = new TextviewObject();
            #region default order
            IEnumerable<XElement> uiElementDefaultOrders = xmlDoc.Elements("Orders").Elements("DefaultOrder").Elements("Element");
            if (uiElementDefaultOrders == null || !uiElementDefaultOrders.Any()) { return; }
            Orders orders = new Orders();
            orders.defaultOrder = new List<TextviewElement>();
            orders.specialOrders = new List<SpecialOrder>();
            tvo.orders = orders;
            foreach (XElement xmlElement in uiElementDefaultOrders)
            {
                TextviewElement tve = new TextviewElement();
                tve.order = Int32.Parse(xmlElement.Element("Order").Value);
                tve.property = xmlElement.Element("Property").Value;
                tve.minWidth = Int32.Parse(xmlElement.Element("MinWidth").Value);
                XElement xElementSeparator = xmlElement.Element("Separator");
                if (xElementSeparator != null)
                {
                    tve.separator = xElementSeparator.Value;
                    if (tve.separator.Equals("")) { tve.separator = " "; }
                }
                tvo.orders.defaultOrder.Add(tve);
            }
            #endregion
            #region special order
            IEnumerable<XElement> uiElementOrders = xmlDoc.Elements("Orders").Elements("SpecialOrders");
            if (uiElementOrders == null || !uiElementOrders.Any()) { return; }            
            foreach (XElement xmlElement in uiElementOrders)
            {
                IEnumerable<XElement> oneSpecialOrder = xmlElement.Elements("SpecialOrder");
                
                foreach (XElement elementOrder in oneSpecialOrder)
                {
                    SpecialOrder so = new SpecialOrder();
                    List<TextviewElement> oneSpecialOrderList = new List<TextviewElement>();
                    so.controltypeName = elementOrder.Attribute("controlType").Value;
                    foreach (XElement element in elementOrder.Elements("Element"))
                    {
                        TextviewElement tve = new TextviewElement();
                        tve.order = Int32.Parse(element.Element("Order").Value);
                        tve.property = element.Element("Property").Value;
                        tve.minWidth = Int32.Parse(element.Element("MinWidth").Value);
                        XElement xElementSeparator = element.Element("Separator");
                        if (xElementSeparator != null)
                        {
                            tve.separator = xElementSeparator.Value;
                            if (tve.separator.Equals("")) { tve.separator = " "; }
                        }
                        oneSpecialOrderList.Add(tve);
                    }
                    so.order = oneSpecialOrderList;
                    tvo.orders.specialOrders.Add(so);
                }
            }
            #endregion
            tvo.typeOfView = xmlDoc.Element("TypeOfView").Value;
            tvo.screenName = xmlDoc.Element("Screenname").Value;
            XElement itemenumarate = xmlDoc.Element("ItemEnumerate");
            if(itemenumarate != null)
            {
                tvo.itemEnumerate = itemenumarate.Value;
                if (tvo.itemEnumerate.Equals("")) { tvo.itemEnumerate = " "; }
            }
            
            IEnumerable<XElement> uiElementAcronyms = xmlDoc.Elements("Acronyms").Elements("Acronym");

            if (!(uiElementAcronyms == null || !uiElementAcronyms.Any()))
            {
                tvo.acronymsOfPropertyContent = new List<AcronymsOfPropertyContent>();
                foreach (XElement xmlElement in uiElementAcronyms)
                {
                    AcronymsOfPropertyContent aopc = new AcronymsOfPropertyContent();
                    aopc.name = xmlElement.Element("Name").Value;
                    aopc.acronym = xmlElement.Element("Short").Value;
                    tvo.acronymsOfPropertyContent.Add(aopc);
                }
            }
            this.TextviewObject = tvo;
        }
        #endregion
    }
}
