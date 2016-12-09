using GRANTManager;
using GRANTManager.TreeOperations;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TemplateTextview
{
    public class Textview
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        TextviewObject tvo;
        private int shiftingPerDepth = 2;

        public Textview(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
            #region hier nur Testweise einlesen der XML; sollte global passieren(?)
            tvo = loadTemplateAllElementsTextview();
            #endregion
        }


        public void createTextviewOfSubtree(Object subtree, int startYPosition = 0)
        {
            
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                addElementInBrailleTree(node, ref startYPosition);
            }
        }

        private void addElementInBrailleTree(object filteredNode, ref int startYPosition)
        {
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(filteredNode);
            int depth = strategyMgr.getSpecifiedTree().Depth(filteredNode) * shiftingPerDepth;
            startYPosition += 5;


            int x = depth;
            foreach(TextviewElement tve in tvo.textviewElements)
            {
                OSMElement.OSMElement osmBraille = new OSMElement.OSMElement();
                GeneralProperties propBraille = new GeneralProperties();
                BrailleRepresentation braille = new BrailleRepresentation();

                braille.isVisible = true;
                propBraille.controlTypeFiltered = "Text";
                braille.screenCategory = tvo.viewCategory;
                braille.screenName = tvo.screenName;
                braille.fromGuiElement = tve.property;
                braille.viewName = osmFiltered.properties.IdGenerated + "_" + tve.order;
                propBraille.boundingRectangleFiltered = new Rect(x, startYPosition, tve.minWidth, 5);//TODO: richtig machen
                x += tve.minWidth + 2;

                osmBraille.properties = propBraille;
                osmBraille.brailleRepresentation = braille;

                #region Id und Beziehung hinzufühgen
                String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(osmBraille);
                if (idGenerated == null) { return; }
                propBraille.IdGenerated = idGenerated;
                osmBraille.properties = propBraille;
                List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeConnector.addOsmConnection(osmFiltered.properties.IdGenerated, idGenerated, ref relationship);
                #endregion
                treeOperation.updateNodes.updateNodeOfBrailleUi(ref osmBraille);
            }
        }
        

        //Das Auslesen sollte später woanders passieren
        private TextviewObject loadTemplateAllElementsTextview(String path = null)
        {
            if (path == null)
            {
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TemplateAllElementsTextview.xml");
            }
            if (!File.Exists(path)) { Debug.WriteLine("Die XML exisitert nicht"); return null; }
            XElement xmlDoc = XElement.Load(@path);
            //TODO: hier gegen XSD validieren
            IEnumerable<XElement> uiElementOrders = xmlDoc.Elements("Orders").Elements("Element");

            if (uiElementOrders == null || !uiElementOrders.Any()) { return null; }
            TextviewObject tvo = new TextviewObject();
            tvo.textviewElements = new List<TextviewElement>();
            foreach (XElement xmlElement in uiElementOrders)
            {
                TextviewElement tve = new TextviewElement();
                tve.order = Int32.Parse( xmlElement.Element("Order").Value);
                tve.property = xmlElement.Element("Property").Value;
                tve.minWidth = Int32.Parse( xmlElement.Element("MinWidth").Value);
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

            return tvo;
        }
    }
}
