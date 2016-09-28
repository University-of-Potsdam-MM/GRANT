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
using GRANTManager.Interfaces;

namespace TemplatesUi
{
    public class GenaralUI : IGenaralUiTemplate
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
            createUiElementsNavigationbarScreens(pathToXml);
            strategyMgr.getSpecifiedTreeOperations().updateBrailleGroups();
        }

        /// <summary>
        /// aktualisiert die Navigationsleisten (Anzahl der Tabs) auf allen Screens, die eine Navigationsleiste anzeigen
        /// </summary>
        /// <param name="pathToXml"></param>
        public void updateNavigationbarScreens(string pathToXml)
        {
            TempletUiObject templateObject = getTemplateUiObjectOfnavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TempletUiObject())) { return; }
            List<String> screens = strategyMgr.getSpecifiedTreeOperations().getPosibleScreenNames();
            /*
             * im Braillebaum suchen wo alles eine Navigationsleiste vorhanden ist --> von 
             * ergänzen
             */
            List<ITreeStrategy<OSMElement.OSMElement>> navigationbars = strategyMgr.getSpecifiedTreeOperations().getListOfNavigationbars();
            List<String> screensForShowNavigationbar = new List<string>();
            foreach (ITreeStrategy<OSMElement.OSMElement> nbar in navigationbars)
            {                
                if (nbar.HasChild)
                {
                    strategyMgr.getSpecifiedTreeOperations().removeChildNodeInBrailleTree(nbar);
                    strategyMgr.getSpecifiedTreeOperations().removeNodeInBrailleTree(nbar.Data);
                }
                screensForShowNavigationbar.Add(nbar.Data.brailleRepresentation.screenName);
            }
            templateObject.Screens = screensForShowNavigationbar;
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Fügt eine Navigationsleiste für den angegebenen Screen hinzu
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zum Template der Navigationsleiste an</param>
        /// <param name="subtree">gibt den braille-Teilbaum an, bei welchem eine Navigationsleiste hinzugefügt werden soll</param>
        public void addNavigationbarForScreen(string pathToXml, ITreeStrategy<OSMElement.OSMElement> subtree)
        {
            TempletUiObject templateObject = getTemplateUiObjectOfnavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TempletUiObject())) { return; }
            List<String> screens = strategyMgr.getSpecifiedTreeOperations().getPosibleScreenNames();
            List<String> screenNavi = new List<string>();
            screenNavi.Add(subtree.Data.brailleRepresentation.screenName);
            templateObject.Screens = screenNavi;
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();
            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Ermittelt von einem angegebenen Template das zugehörige Template-Objekt für die Navigationsleiste
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <returns></returns>
        private TempletUiObject getTemplateUiObjectOfnavigationbarScreen(string pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Elements("UiElement")
                where (string)el.Element("IsGroup") != null && (string)el.Element("IsGroup") != ""
                    && (string)el.Attribute("name") == "NavigationBarScreens"
                select el;
            // Debug.WriteLine("uiElement: " + uiElement.Count());
            if (uiElement.Count() == 0) { return new TempletUiObject(); }

            //TempletUiObject templateObject = xmlUiElementToTemplateUiObject(uiElement.First()); //Es darf nur ein element bei der Suche herauskommen
            return xmlUiElementToTemplateUiObject(uiElement.First()); //Es darf nur ein element bei der Suche herauskommen
        }

        /// <summary>
        /// Ermittelt die Positionen der einzelnen "Tabs" in der Navigationsleiste
        /// </summary>
        /// <param name="screens">gibt die Liste der Screens an, die in der Navigationsleiste angezeigt werden sollen</param>
        /// <param name="templateObject"></param>
        /// <param name="index">gibt den Index an, ab welchen die (Tab-)Element die Position berechnet werden soll</param>
        /// <returns></returns>
        private List<OSMElement.OSMElement> calculatePositionOfScreenTab(List<String> screens, TempletUiObject templateObject, int index = 0)
        {
            List<OSMElement.OSMElement> groupElementsStatic = new List<OSMElement.OSMElement>();
            if (screens == null || templateObject.Equals(new TempletUiObject())) { return null; }
            foreach (String s in screens)
            {
                OSMElement.OSMElement childOsm = new OSMElement.OSMElement();
                GeneralProperties childProp = new GeneralProperties();
                Rect rect = templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle;
                //if (templateObject.orientation != null)
                {
                    if (templateObject.orientation.Equals(OSMElement.UiElements.Orientation.Left) || templateObject.orientation.Equals(OSMElement.UiElements.Orientation.Right))
                    {
                        rect.Y = (rect.Height + 1) * index + rect.Y + 1;
                    }
                    if (templateObject.orientation.Equals(OSMElement.UiElements.Orientation.Top) || templateObject.orientation.Equals(OSMElement.UiElements.Orientation.Bottom))
                    {
                        rect.X = (rect.Width + 1) * index + rect.X + 1;
                    }
                }
               // Debug.WriteLine("Rect = " + rect);
                childProp.boundingRectangleFiltered = rect;
                childProp.controlTypeFiltered = templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer;
                childProp.valueFiltered = s;
                childOsm.properties = childProp;
                BrailleRepresentation childBraille = new BrailleRepresentation();
                childBraille.isGroupChild = true;
                childBraille.isVisible = true;
                childBraille.viewName = "_" + s;//TODO
                OSMElement.UiElements.TabItem tabView = new OSMElement.UiElements.TabItem();
                tabView.orientation = templateObject.orientation;
                childBraille.uiElementSpecialContent = tabView;
                childOsm.brailleRepresentation = childBraille;
                groupElementsStatic.Add(childOsm);
                index++;
            }
            return groupElementsStatic;
        }

        public void createUiElementsNavigationbarScreens(string pathToXml)
        {
            TempletUiObject templateObject = getTemplateUiObjectOfnavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TempletUiObject())) { return; }
            //TODO: mit Events verknüpfen
            List<String> screens = strategyMgr.getSpecifiedTreeOperations().getPosibleScreenNames();
            ATemplateUi generalUiInstance;
            if (templateObject.Screens == null)
            {
                //wir wollen alle verfügbaren Screens
                templateObject.Screens = screens;
            }
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees);

            generalUiInstance.createUiElementFromTemplate(tree, templateObject);//
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
            ATemplateUi generalUiInstance;
            TempletUiObject templateObject = xmlUiElementToTemplateUiObject(firstElement);
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
            {
                generalUiInstance = new TemplateNode(strategyMgr, grantTrees);
            }
            else
            {
                generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees);
            }
            generalUiInstance.createUiElementFromTemplate(subtree, xmlUiElementToTemplateUiObject(firstElement));
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
            GeneralProperties properties = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();
            properties.controlTypeFiltered = xmlElement.Element("Renderer").Value;
            braille.fromGuiElement = xmlElement.Element("TextFromUIElement").Value;
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
            properties.boundingRectangleFiltered = rect;

            if (!xmlElement.Element("BoxModel").IsEmpty)
            {
                XElement boxModel = xmlElement.Element("BoxModel");
                if (!boxModel.Element("Padding").IsEmpty)
                {
                    XElement padding = boxModel.Element("Padding");
                    braille.padding = new Padding(padding.Element("Left") == null ? 0 : Convert.ToInt32(padding.Element("Left").Value), padding.Element("Top") == null ? 0 : Convert.ToInt32(padding.Element("Top").Value), padding.Element("Right") == null ? 0 : Convert.ToInt32(padding.Element("Right").Value), padding.Element("Bottom") == null ? 0 : Convert.ToInt32(padding.Element("Bottom").Value));
                }
                if (!boxModel.Element("Margin").IsEmpty)
                {
                    XElement margin = boxModel.Element("Margin");
                    braille.margin = new Padding(margin.Element("Left") == null ? 0 : Convert.ToInt32(margin.Element("Left").Value), margin.Element("Top") == null ? 0 : Convert.ToInt32(margin.Element("Top").Value), margin.Element("Right") == null ? 0 : Convert.ToInt32(margin.Element("Right").Value), margin.Element("Bottom") == null ? 0 : Convert.ToInt32(margin.Element("Bottom").Value));
                }
                if (!boxModel.Element("Boarder").IsEmpty)
                {
                    XElement boarder = boxModel.Element("Boarder");
                    braille.boarder = new Padding(boarder.Element("Left") == null ? 0 : Convert.ToInt32(boarder.Element("Left").Value), boarder.Element("Top") == null ? 0 : Convert.ToInt32(boarder.Element("Top").Value), boarder.Element("Right") == null ? 0 : Convert.ToInt32(boarder.Element("Right").Value), boarder.Element("Bottom") == null ? 0 : Convert.ToInt32(boarder.Element("Bottom").Value));
                }
            }

            if (xmlElement.Element("IsGroup").HasElements)
            {
                templetObject.groupImplementedClassTypeFullName = typeof(TemplateSubtree).FullName;
                templetObject.groupImplementedClassTypeDllName = typeof(TemplateSubtree).Module.Assembly.GetName().Name; // == Dll-Name
                GroupelementsOfSameType group = new GroupelementsOfSameType();
                group.linebreak = Convert.ToBoolean(xmlElement.Element("IsGroup").Element("Linebreak").Value);
                group.vertical = Convert.ToBoolean(xmlElement.Element("IsGroup").Element("Vertical").Value);
                position = xmlElement.Element("IsGroup").Element("childBoundingRectangle");
                Rect childRect = new Rect();
                isConvertHeight = Int32.TryParse(position.Element("Height").Value, out result);
                childRect.Height = isConvertHeight ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
                isConvertWidth = Int32.TryParse(position.Element("Width").Value, out result);
                childRect.Width = isConvertWidth ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
                childRect.X = rect.X + braille.padding.Left + braille.margin.Left + braille.boarder.Left;
                childRect.Y = rect.Y + braille.padding.Top + braille.margin.Top + braille.boarder.Top;
                if (!isConvertHeight) { childRect.Height -= childRect.Y; }
                if (!isConvertWidth) { childRect.Width -= childRect.X; }
                group.childBoundingRectangle = childRect;
                group.renderer = xmlElement.Element("Renderer").Value;
                braille.groupelementsOfSameType = group;
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
            if (xmlElement.Element("Orientation") != null && !xmlElement.Element("Orientation").IsEmpty)
            {
                String value = xmlElement.Element("Orientation").Value;
              //  templetObject.orientation = (value == OSMElement.UiElements.Orientation.Left.ToString() ? OSMElement.UiElements.Orientation.Left : (value == OSMElement.UiElements.Orientation.Bottom.ToString() ? OSMElement.UiElements.Orientation.Bottom : (value == OSMElement.UiElements.Orientation.Right.ToString() ? OSMElement.UiElements.Orientation.Right : OSMElement.UiElements.Orientation.Top)));
                templetObject.orientation = (value.Equals( OSMElement.UiElements.Orientation.Left.ToString()) ? OSMElement.UiElements.Orientation.Left : (value.Equals( OSMElement.UiElements.Orientation.Bottom.ToString()) ? OSMElement.UiElements.Orientation.Bottom : (value.Equals( OSMElement.UiElements.Orientation.Right.ToString()) ? OSMElement.UiElements.Orientation.Right : OSMElement.UiElements.Orientation.Top)));
                
            }
            braille.showScrollbar = Convert.ToBoolean( xmlElement.Element("ShowScrollbar").Value);
            templetObject.name = xmlElement.Attribute("name").Value;
            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = properties;
            templetObject.osm = osm;

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
                ATemplateUi generalUiInstance;
                TempletUiObject templateObject = xmlUiElementToTemplateUiObject(element);
                if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
                {
                    generalUiInstance = new  TemplateNode(strategyMgr, grantTrees);
                }
                else
                {
                    generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees);
                }

                ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getSpecifiedTree().NewNodeTree();
                generalUiInstance.createUiElementFromTemplate(tree, templateObject);
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
                where (string)el.Element("Screens") != null && (string)el.Element("Screens") == "" && (string)el.Attribute("name") != "NavigationBarScreens"
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            List<String> screenList = strategyMgr.getSpecifiedTreeOperations().getPosibleScreenNames();
            foreach (XElement e in uiElement)
            {
                TempletUiObject templateObject = xmlUiElementToTemplateUiObject(e);
                ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getSpecifiedTree().NewNodeTree(); // <-- ist nur der Fall, wenn es keinen Zusammenhang zum Baum gibt
                if (templateObject.osm.brailleRepresentation.fromGuiElement!= null && !templateObject.osm.brailleRepresentation.fromGuiElement.Equals(""))
                {
                    // elementE im gefilterten Baum suchen
                    GeneralProperties properties = new GeneralProperties();
                    properties.controlTypeFiltered = e.Attribute("name").Value;
                    List<ITreeStrategy<OSMElement.OSMElement>> treefilteredElements = strategyMgr.getSpecifiedTreeOperations().searchProperties(grantTrees.getFilteredTree(), properties, OperatorEnum.and);
                    foreach (ITreeStrategy<OSMElement.OSMElement> t in treefilteredElements)
                    {
                        tree = t;
                        iterateScreensForTemplate(screenList, ref tree, templateObject);
                    }
                }
                else
                {
                    iterateScreensForTemplate(screenList, ref tree, templateObject);
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
        private void iterateScreensForTemplate(List<String> screenList, ref ITreeStrategy<OSMElement.OSMElement> tree, TempletUiObject templateObject)
        {
            if (screenList == null) { return; }
            ATemplateUi generalUiInstance;
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
            {
                generalUiInstance = new TemplateNode(strategyMgr, grantTrees);
            }
            else
            {
                generalUiInstance = new TemplateSubtree(strategyMgr, grantTrees);
            }
           // ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
            foreach (String screen in screenList)
            {
                templateObject.Screens = new List<string>();
                templateObject.Screens.Add(screen);

                generalUiInstance.createUiElementFromTemplate(tree, templateObject);
            }
        }


        public void createUiElementFromTemplate(GRANTManager.Interfaces.ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GRANTManager.Interfaces.TempletUiObject templateObject, String brailleNodeId = null)
        {
            Type typeOfTemplate = Type.GetType(templateObject.osm.brailleRepresentation.templateFullName + ", " + templateObject.osm.brailleRepresentation.templateNamspace);
            if (typeOfTemplate != null)
            {
                ATemplateUi template = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
                template.createUiElementFromTemplate(filteredSubtree, templateObject, brailleNodeId);
            }
        }
    }

}
