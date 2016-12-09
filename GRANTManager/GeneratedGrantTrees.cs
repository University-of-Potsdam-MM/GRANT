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
    /// Diese Klasse enthält die erstellten Bäume und deren Beziehungen
    /// </summary>
    public class GeneratedGrantTrees
    {
        private Object filteredTree; // enthält den gefilterten Baum
        private Object brailleTree; // enthält die Baumdarstellung der UI auf der stiftplatte
        /// <summary>
        /// gibt die Beziehung zwischen <code>filteredTree</code> und <code>brailleTree</code> anhand der generierten Id an
        /// </summary>
        private List<OsmConnector<String, String>> osmRelationship = new List<OsmConnector<string, string>>();

        /// <summary>
        /// Enthält die Standardzugehörigkeiten der Renderer zu den ControllTypes
        /// </summary>
        public List<RendererUiElementConnector> rendererUiElementConnection { get; set; }



        /// <summary>
        /// Gibt die Filterstrategien an;
        /// dabei wird diese nur für den Root-Knoten und für alle Knoten bei dennen die Strategy von Standardfilter abweicht angegeben
        /// </summary>
        private List<FilterstrategyOfNode<String, String, String>> filterstrategiesOfNodes = new List<FilterstrategyOfNode<string, string, string>>();

        private TextviewObject textviewobject;
        public TextviewObject TextviewObject { get { if (textviewobject == null) {loadTemplateAllElementsTextview(); }   return textviewobject;  } set { textviewobject = value; } }

        /// <summary>
        /// Setzt den aktuell gefilterten Baum
        /// </summary>
        /// <param name="parentNode">gibt den gefilterten Baum an</param>
        public void setFilteredTree(Object tree)
        {
            filteredTree = tree;
        }

        /// <summary>
        /// Gibt den gefilterten Baum zurück
        /// </summary>
        /// <returns>Ger gefilterte Baum</returns>
        public Object getFilteredTree()
        {
            return filteredTree;
        }

        /// <summary>
        /// Setzt die aktuelle Barille-UI-Darstellung.
        /// </summary>
        /// <param name="parentNode"></param>
        public void setBrailleTree(Object tree)
        {
            brailleTree = tree;
        }

        /// <summary>
        /// Gibt die aktuelle Braille-UI-Darstellung zurück
        /// </summary>
        /// <returns>Braille-UI-Darstellung</returns>
        public Object getBrailleTree()
        {
            return brailleTree;
        }

        /// <summary>
        /// Gibt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value> an
        /// </summary>
        /// <returns>Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value></returns>
        public List<OsmConnector<String, String>> getOsmRelationship()
        {
            return osmRelationship;
        }

        /// <summary>
        /// Setzt die Beziehungen zwischen <value>brailleTree</value> und <value>filteredTree</value>
        /// </summary>
        /// <param name="relationship"></param>
        public void setOsmRelationship(List<OsmConnector<String, String>> relationship)
        {
            osmRelationship = relationship;
        }

        /// <summary>
        /// Gibt die FilterStrategien für die Knoten zurück;
        /// dabei wird diese nur für den Root-Knoten und für alle Knoten bei dennen die Strategy von Standardfilter abweicht angegeben
        /// </summary>
        /// <returns>Liste der Filterstrategien</returns>
        public List<FilterstrategyOfNode<String, String, String>> getFilterstrategiesOfNodes()
        {
            return filterstrategiesOfNodes;
        }

        /// <summary>
        /// Setzt die Filterstrategien für die Knoten
        /// </summary>
        /// <param name="filterstrategiesOfNodes">eine Liste von Filterstrategien</param>
        public void setFilterstrategiesOfNodes(List<FilterstrategyOfNode<String, String, String>> filterstrategiesOfNodes)
        {
            this.filterstrategiesOfNodes = filterstrategiesOfNodes;
        }

        #region auslesen aus XML
        /// <summary>
        /// Liest aus einer XML-Datei informationen zum darstellen der Textview sowie mögliche Abkürzungen aus
        /// </summary>
        /// <param name="path">gibt den Path zur XML-Datei an</param>
        private void loadTemplateAllElementsTextview(String path = null)
        {
            if (path == null)
            {
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TemplateAllElementsTextview.xml");
            }
            if (!File.Exists(path)) { Debug.WriteLine("Die XML exisitert nicht"); return; }
            XElement xmlDoc = XElement.Load(@path);
            //TODO: hier gegen XSD validieren
            IEnumerable<XElement> uiElementOrders = xmlDoc.Elements("Orders").Elements("Element");

            if (uiElementOrders == null || !uiElementOrders.Any()) { return; }
            TextviewObject tvo = new TextviewObject();
            tvo.textviewElements = new List<TextviewElement>();
            foreach (XElement xmlElement in uiElementOrders)
            {
                TextviewElement tve = new TextviewElement();
                tve.order = Int32.Parse(xmlElement.Element("Order").Value);
                tve.property = xmlElement.Element("Property").Value;
                tve.minWidth = Int32.Parse(xmlElement.Element("MinWidth").Value);
                tvo.textviewElements.Add(tve);
            }
            tvo.viewCategory = xmlDoc.Element("ViewCategory").Value;
            tvo.screenName = xmlDoc.Element("Screenname").Value;
            IEnumerable<XElement> uiElementAcronyms = xmlDoc.Elements("Acronyms").Elements("Acronym");

            if (!(uiElementAcronyms == null || !uiElementAcronyms.Any()))
            {
                tvo.acronymsOfPropertyContent = new List<AcronymsOfPropertyContent>();
                foreach (XElement xmlElement in uiElementAcronyms)
                {
                    Debug.WriteLine(xmlElement);
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
