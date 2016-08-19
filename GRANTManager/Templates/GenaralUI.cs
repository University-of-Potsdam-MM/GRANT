using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace GRANTManager.Templates
{
    public class GenaralUI
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;

        public GenaralUI(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) { this.strategyMgr = strategyMgr; this.grantTrees = grantTrees; }

        /// <summary>
        /// Ausgehend vom gefilterten Baum wird für einige UI-Elemente ein Template (<see cref="TemplateUi"/>) angewendet aus dem eine Standard UI erstellt wird.
        /// </summary>
        public void generatedUiFromTemplate() 
        {
            if (grantTrees == null || grantTrees.getFilteredTree() == null) { Debug.WriteLine("kein gefilterter Baum vorhanden!"); return; }
            //Anmerkung: Anstelle hier über den ganzen Baum zu iterrieren, könnte auchmittels der Nethode ITreeOperations.searchProperties nach den Elementen gesucht werden, die im Template berücksichtigt werden sollen; aber dann müsste jedesmal über den ganzen Baum iteriert werden
            ITreeStrategy<OSMElement.OSMElement> filteredTreeCopy = grantTrees.getFilteredTree().Copy();
            iteratedTree(ref filteredTreeCopy);
        }

        /// <summary>
        /// Iterriert über den gefilterten Baum und ruft für jedes UI-Element die Methode <see cref="createElementType"/> auf
        /// </summary>
        /// <param name="tree">gibt den gefilterten Baum an</param>
        private void iteratedTree(ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;
            //Falls die Baumelemente Kinder des jeweiligen Elements sind
            while ((tree.HasChild || tree.HasNext) && !(tree.Count == 1 && tree.Depth == -1))
            {
                if (tree.HasChild)
                {
                    node1 = tree.Child;
                    createElementType(ref node1);
                    iteratedTree(ref node1);
                }
                else
                {
                    node1 = tree.Next;
                    if (tree.HasNext)
                    {
                        //createElementType(ref tree);
                        createElementType(ref node1);
                    }
                    iteratedTree(ref node1);
                }
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                if (!tree.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createElementType(ref tree);
                }
            }
            if (!tree.HasChild)
            {
                
                if (tree.HasParent)
                {
                    node1 = tree;
                    node1.Remove();
                }
            }
            if (!tree.HasNext && !tree.HasParent)
            {
                if (tree.HasPrevious)
                {
                    node1 = tree;
                    node1.Remove();
                }
            }
        }

        private void createElementType(ref ITreeStrategy<OSMElement.OSMElement> subtree)
        {
            //falls zu einem UI-Elemente ein Teilbaum gehört, so wird der Baum entsprechend gekürzt
            String controlType = subtree.Data.properties.controlTypeFiltered;
            XElement xmlDoc = XElement.Load(@"Templates" + Path.DirectorySeparatorChar + "TemplateUi.xml");
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Elements("UiElement")
                where (string)el.Attribute("name") == controlType
                select el;
            /* foreach (XElement x in uiElement)
             {
                 Debug.WriteLine(x);
             }*/
            if (uiElement == null || !uiElement.Any()) { return; }
            XElement firstElement = uiElement.First(); //Achtung hier wird erstmal einfach das erste gefundene genommen
            //Debug.WriteLine("First = " + firstElement);

            Type typeOfTemplate = Type.GetType(firstElement.Element("ImplementedClassTypeFullName").Value + ", " + firstElement.Element("ImplementedClassTypeDllName").Value);
            if (typeOfTemplate == null) { Debug.WriteLine("Es konnte kein Typ ermittelt werden um das Template für ein UI-Element zu nutzen!"); return; }
            ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
            //AGeneralUi template = new TemplateTitleBar(strategyMgr, grantTrees);
            generalUiInstance.createUiElementFromTemplate(ref subtree, xmlUiElementToTemplateUiobject(firstElement));
            //Debug.WriteLine("");
        }

        public struct TempletUiObject
        {
            public String renderer { get; set; }
            public int minDeviceHeight { get; set; }
            public int minDeviceWidth { get; set; }
            public String TextFromUIElement { get; set; }
        }

        private TempletUiObject xmlUiElementToTemplateUiobject(XElement xmlElement)
        {
            TempletUiObject templetObject = new TempletUiObject();
            Int32 result;
            bool isConvert = Int32.TryParse(xmlElement.Element("MinDeviceHeight").Value, out result);
            templetObject.minDeviceHeight = isConvert ? result : 0;
            result = 0;
            isConvert = Int32.TryParse(xmlElement.Element("MinDeviceWidth").Value, out result);
            templetObject.minDeviceWidth = isConvert ? result : 0;
            templetObject.renderer = xmlElement.Element("Renderer").Value;
            templetObject.TextFromUIElement = xmlElement.Element("TextFromUIElement").Value;
            return templetObject;
        }
    }

}
