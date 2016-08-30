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
using System.Windows;
using System.Windows.Forms;

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
        /// <param name="pathToXml">gibt den Pfad zu der XML des Templates für die UI an</param>
        public void generatedUiFromTemplate(String pathToXml) 
        {
            if (grantTrees == null || grantTrees.getFilteredTree() == null) { Debug.WriteLine("kein gefilterter Baum vorhanden!"); return; }
            if (pathToXml.Equals("")) { Debug.WriteLine("Keine Xml angegeben!"); return; }
            if (!File.Exists(@pathToXml)) { Debug.WriteLine("Die XML exisitert nicht"); return; }
            createUiElementsWitheNoDependency(@pathToXml);
            //Anmerkung: Anstelle hier über den ganzen Baum zu iterrieren, könnte auchmittels der Nethode ITreeOperations.searchProperties nach den Elementen gesucht werden, die im Template berücksichtigt werden sollen; aber dann müsste jedesmal über den ganzen Baum iteriert werden
            ITreeStrategy<OSMElement.OSMElement> filteredTreeCopy = grantTrees.getFilteredTree().Copy();
            iteratedTreeForTemplate(ref filteredTreeCopy, pathToXml);
            createUiElementsAllScreens(pathToXml);
            strategyMgr.getSpecifiedTreeOperations().updateBrailleGroups();
        }

        /// <summary>
        /// Iterriert über den gefilterten Baum und ruft für jedes UI-Element die Methode <see cref="createElementType"/> auf
        /// </summary>
        /// <param name="parentNode">gibt den gefilterten Baum an</param>
        private void iteratedTreeForTemplate(ref ITreeStrategy<OSMElement.OSMElement> tree, String pathToXml)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;
            //Falls die Baumelemente Kinder des jeweiligen Elements sind
            while ((tree.HasChild || tree.HasNext) && !(tree.Count == 1 && tree.Depth == -1))
            {
                if (tree.HasChild)
                {
                    node1 = tree.Child;
                    createElementType(ref node1, pathToXml);
                    iteratedTreeForTemplate(ref node1, pathToXml);
                }
                else
                {
                    node1 = tree.Next;
                    if (tree.HasNext)
                    {
                        //createElementType(ref parentNode);
                        createElementType(ref node1, pathToXml);
                    }
                    iteratedTreeForTemplate(ref node1, pathToXml);
                }
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                if (!tree.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createElementType(ref tree, pathToXml);
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

        /// <summary>
        /// Sofern für das angegebene UI-element ein template vorhanden ist wird dieses angewendet
        /// </summary>
        /// <param name="subtree">gibt den Teilbaum mit dem element, auf welches das Template angewendet werden soll an</param>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        private void createElementType(ref ITreeStrategy<OSMElement.OSMElement> subtree, String pathToXml)
        {
            //falls zu einem UI-Elemente ein Teilbaum gehört, so wird der Baum entsprechend gekürzt => TODO
            String controlType = subtree.Data.properties.controlTypeFiltered;
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Elements("UiElement")
                where (string)el.Attribute("name") == controlType && 
                (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") != "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            /* foreach (XElement element in uiElement)
             {
                 Debug.WriteLine(element);
             }*/            
            XElement firstElement = uiElement.First(); //Achtung hier wird erstmal einfach das erste gefundene genommen
            //Debug.WriteLine("First = " + firstElement);

            Type typeOfTemplate = Type.GetType(firstElement.Element("ImplementedClassTypeFullName").Value + ", " + firstElement.Element("ImplementedClassTypeDllName").Value);
            if (typeOfTemplate == null) { Debug.WriteLine("Es konnte kein Typ ermittelt werden um das Template für ein UI-Element zu nutzen!"); return; }
            ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
            //AGeneralUi template = new TemplateTitleBar(strategyMgr, grantTrees);
            generalUiInstance.createUiElementFromTemplate(ref subtree, xmlUiElementToTemplateUiObject(firstElement));
        }

        public struct TempletUiObject
        {
            public String renderer { get; set; }
            public Rect rect { get; set; }
            public String textFromUIElement { get; set; }
            public String groupImplementedClassTypeFullName { get; set; }
            public String groupImplementedClassTypeDllName { get; set; }
            public List<String> Screens { get; set; }
            public Padding padding { get; set; }
            public Padding margin { get; set; }
            public Padding boarder { get; set; }
            public String name { get; set; }
            public Boolean linebreak { get; set; }
            //TODO. evtl. auch hier BrailleRepresentaion (oder gleich OSMelement) verwenden
        }

        /// <summary>
        /// Wandelt ein XElement in ein <c>TempletUiObject</c> um
        /// </summary>
        /// <param name="xmlElement">gibt ein XElement aus der TemplateUi.xml an</param>
        /// <returns>ein <c>TempletUiObject</c></returns>
        private TempletUiObject xmlUiElementToTemplateUiObject(XElement xmlElement)
        {
            TempletUiObject templetObject = new TempletUiObject();
            Int32 result;
            result = 0;
            templetObject.renderer = xmlElement.Element("Renderer").Value;
            templetObject.textFromUIElement = xmlElement.Element("TextFromUIElement").Value;
            XElement position = xmlElement.Element("Position");
            bool isConvertHeight = Int32.TryParse(position.Element("Height").Value, out result);
            Rect rect = new Rect();
            rect.Height = isConvertHeight ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
            bool isConvertWidth = Int32.TryParse(position.Element("Width").Value, out result);
            rect.Width = isConvertWidth ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            bool isConvert = Int32.TryParse(position.Element("StartX").Value, out result);
            rect.X = isConvert ? result : (strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width - rect.Width);
            isConvert = Int32.TryParse(position.Element("StartY").Value, out result);
            rect.Y = isConvert ? result : (strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height - rect.Height);
            if (!isConvertHeight) { rect.Height -= rect.Y; }
            if (!isConvertWidth) { rect.Width -= rect.X; }
            templetObject.rect = rect;
            if (xmlElement.Element("IsGroup").HasElements)
            {
                templetObject.groupImplementedClassTypeFullName = xmlElement.Element("IsGroup").Element("ImplementedClassTypeFullName").Value;
                templetObject.groupImplementedClassTypeDllName = xmlElement.Element("IsGroup").Element("ImplementedClassTypeDllName").Value;
                templetObject.linebreak = Convert.ToBoolean( xmlElement.Element("IsGroup").Element("Linebreak").Value);
                Debug.WriteLine("");
            }
            if (!xmlElement.Element("Screens").IsEmpty)
            {
                XElement screens = xmlElement.Element("Screens");
                templetObject.Screens = new List<string>();
                foreach (XElement s in screens.Elements("Screen"))
                {
                    templetObject.Screens.Add(s.Value);
                }

            }
            if (!xmlElement.Element("BoxModel").IsEmpty)
            {
                XElement boxModel = xmlElement.Element("BoxModel");
                if (!boxModel.Element("Padding").IsEmpty)
                {
                    XElement padding = boxModel.Element("Padding");
                    templetObject.padding = new Padding(padding.Element("Left") == null ? 0 : Convert.ToInt32(padding.Element("Left").Value), padding.Element("Top") == null ? 0 : Convert.ToInt32(padding.Element("Top").Value), padding.Element("Right") == null ? 0 : Convert.ToInt32(padding.Element("Right").Value), padding.Element("Buttom") == null ? 0 : Convert.ToInt32(padding.Element("Buttom").Value));
                }
                if (!boxModel.Element("Margin").IsEmpty)
                {
                    XElement margin = boxModel.Element("Margin");
                    templetObject.margin = new Padding(margin.Element("Left") == null ? 0 : Convert.ToInt32(margin.Element("Left").Value), margin.Element("Top") == null ? 0 : Convert.ToInt32(margin.Element("Top").Value), margin.Element("Right") == null ? 0 : Convert.ToInt32(margin.Element("Right").Value), margin.Element("Buttom") == null ? 0 : Convert.ToInt32(margin.Element("Buttom").Value));
                }
                if (!boxModel.Element("Boarder").IsEmpty)
                {
                    XElement boarder = boxModel.Element("Boarder");
                    templetObject.boarder = new Padding(boarder.Element("Left") == null ? 0 : Convert.ToInt32(boarder.Element("Left").Value), boarder.Element("Top") == null ? 0 : Convert.ToInt32(boarder.Element("Top").Value), boarder.Element("Right") == null ? 0 : Convert.ToInt32(boarder.Element("Right").Value), boarder.Element("Buttom") == null ? 0 : Convert.ToInt32(boarder.Element("Buttom").Value));
                }
            }
            templetObject.name = xmlElement.Attribute("name").Value;

            return templetObject;
        }

        /// <summary>
        /// Erstellt Ui-elemente die keine Verbindung zum gefilterten Baum haben
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        private void createUiElementsWitheNoDependency(String pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Elements("UiElement")
                where (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") == "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            foreach (XElement element in uiElement)
            {
                //Debug.WriteLine(element);
                Type typeOfTemplate = Type.GetType(element.Element("ImplementedClassTypeFullName").Value + ", " + element.Element("ImplementedClassTypeDllName").Value);
                if (typeOfTemplate == null) { Debug.WriteLine("Es konnte kein Typ ermittelt werden um das Template für ein UI-Element zu nutzen!"); return; }
                ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
                ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getSpecifiedTree().NewNodeTree();
                generalUiInstance.createUiElementFromTemplate(ref tree, xmlUiElementToTemplateUiObject(element));
            }
        }

        /// <summary>
        /// Erstellt die Ui-Elemente, welche auf jedem Screen vorhanden sein sollen
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        public void createUiElementsAllScreens(String pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Elements("UiElement")
                where (string)el.Element("Screens") != null && (string)el.Element("Screens") == ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            List<String> screenList = strategyMgr.getSpecifiedTreeOperations().getPosibleScreenNames();
            foreach (XElement e in uiElement)
            {
                TempletUiObject templateObject = xmlUiElementToTemplateUiObject(e);
                ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getSpecifiedTree().NewNodeTree(); // <-- ist nur der Fall, wenn es keinen Zusammenhang zum Baum gibt
                if (templateObject.textFromUIElement != null && !templateObject.textFromUIElement.Equals(""))
                {
                    // elementE im gefilterten Baum suchen
                    GeneralProperties properties = new GeneralProperties();
                    properties.controlTypeFiltered = e.Attribute("name").Value;
                    List<ITreeStrategy<OSMElement.OSMElement>> treefilteredElements = strategyMgr.getSpecifiedTreeOperations().searchProperties(grantTrees.getFilteredTree(), properties, OperatorEnum.and);
                    foreach (ITreeStrategy<OSMElement.OSMElement> t in treefilteredElements)
                    {
                        tree = t;
                        Type typeOfTemplate = Type.GetType(e.Element("ImplementedClassTypeFullName").Value + ", " + e.Element("ImplementedClassTypeDllName").Value);
                        if (typeOfTemplate == null) { Debug.WriteLine("Es konnte kein Typ ermittelt werden um das Template für ein UI-Element zu nutzen!"); return; }
                        iterateScreensForTemplate(screenList, ref tree, templateObject, typeOfTemplate);
                    }
                }
                else
                {
                    Type typeOfTemplate = Type.GetType(e.Element("ImplementedClassTypeFullName").Value + ", " + e.Element("ImplementedClassTypeDllName").Value);
                    if (typeOfTemplate == null) { Debug.WriteLine("Es konnte kein Typ ermittelt werden um das Template für ein UI-Element zu nutzen!"); return; }
                    iterateScreensForTemplate(screenList, ref tree, templateObject, typeOfTemplate);
                }

            }
        }

        /// <summary>
        /// Iterriert über alle angegebenen Screens und fügt das Ui-element ggf. hinzu
        /// </summary>
        /// <param name="screenList">gibt die Liste der Screens an, auf die das Ui-Objekt soll</param>
        /// <param name="tree">gibt den gefilterten Baum an</param>
        /// <param name="templateObject">gibt das Template-Objekt an</param>
        /// <param name="typeOfTemplate">gibt den Typ des zu nutzenden Templates an</param>
        private void iterateScreensForTemplate(List<String> screenList, ref ITreeStrategy<OSMElement.OSMElement> tree, TempletUiObject templateObject, Type typeOfTemplate)
        {
            ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
            foreach (String screen in screenList)
            {
                templateObject.Screens = new List<string>();
                templateObject.Screens.Add(screen);

                generalUiInstance.createUiElementFromTemplate(ref tree, templateObject);
            }
        }
    }

}
