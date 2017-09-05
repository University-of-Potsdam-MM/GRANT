using OSMElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GRANTManager
{
    /*Wird momentan nicht genutzt und ist noch nicht vollständig!
     * Es sollen alle im Baum vorkommenden Elemente angezeigt werden. Dabei werden (einige) UI-Elemente in Symbole umgewandelt.
     */
     
         
    public class TemplateAllElementsSymbol
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grandTrees;

        public TemplateAllElementsSymbol(StrategyManager strategyMgr, GeneratedGrantTrees grandTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grandTrees = grandTrees;
        }

        public void loadTemplateConnectionsForAllElements(String path = null)
        {
            if(path == null)
            {
                path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TelmpalteAllUiElementsAsSymbol.xml");
            }
            if (!File.Exists(path)) { Debug.WriteLine("Die XML exisitert nicht"); return; }
            XElement xmlDoc = XElement.Load(@path);
            //if (xmlDoc.Element("TemplateAllUiElements") == null) { return; } //TODO: hier gegen XSD validieren
            IEnumerable<XElement> uiElement = xmlDoc.Elements("UiConnection");

            if (uiElement == null || !uiElement.Any()) { return; }
            foreach(XElement xmlElement in uiElement)
            {
                RendererUiElementConnector renderer = new RendererUiElementConnector();
                renderer.ControlType = xmlElement.Element("ControllTypeFilteredUi").Value;
                renderer.RendererName = xmlElement.Element("RendererBraille").Value;
                renderer.SizeElement = new RendererUiElementConnector.SizeUiElement(Int32.Parse( xmlElement.Element("Size").Element("Height").Value), Int32.Parse( xmlElement.Element("Size").Element("Width").Value));
                addRendererConnectionUnique(renderer);
           }
        }

        /// <summary>
        /// Fügt den Renderer hinzu, falls für den ControllType schon ein Renderer existiert wird dieser gelöscht
        /// </summary>
        /// <param name="renderer"></param>
        public void addRendererConnectionUnique(RendererUiElementConnector renderer)
        {
            if(grandTrees.rendererUiElementConnection != null)
            {
                foreach(RendererUiElementConnector r in grandTrees.rendererUiElementConnection)
                {
                    if (r.ControlType.Equals(renderer.ControlType))
                    {
                        grandTrees.rendererUiElementConnection.Remove(r);
                        break;
                    }
                }
                grandTrees.rendererUiElementConnection.Add(renderer);
            }
            else
            {
                List<RendererUiElementConnector> list = new List<RendererUiElementConnector>();
                list.Add(renderer);
                grandTrees.rendererUiElementConnection = list;
            }
        }
    }


}
