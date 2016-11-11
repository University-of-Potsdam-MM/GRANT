using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
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

namespace TemplatesUi
{
    public class GenaralUI : IGenaralUiTemplate
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        public GenaralUI(StrategyManager strategyMgr) { this.strategyMgr = strategyMgr;   }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperation = treeOperation; }
        /// <summary>
        /// Ausgehend vom gefilterten Baum wird für einige UI-Elemente ein Template (<see cref="TemplateUi"/>) angewendet aus dem eine Standard UI erstellt wird.
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zu der XML des Templates für die UI an</param>
        public void generatedUiFromTemplate(String pathToXml) 
        {
            if (grantTrees == null || grantTrees.getFilteredTree() == null) { Debug.WriteLine("kein gefilterter Baum vorhanden!"); return; }
            if (pathToXml.Equals("")) { Debug.WriteLine("Keine Xml angegeben!"); return; }
            if (!File.Exists(@pathToXml)) { Debug.WriteLine("Die XML exisitert nicht"); return; }
            generatedSymbolViewFromTemplate(@pathToXml);
            generatedLayoutView(@pathToXml);
        }

        /// <summary>
        /// Ausgehend vom gefilterten Baum wird für einige Ui-Elemente ein Template für die Layout-Ansicht angewendet.
        /// </summary>
        /// <param name="pathToTemplate"></param>
        public void generatedLayoutView(String pathToTemplate)
        {
            createScreenshotViews(pathToTemplate);
            createUiElementsAllScreens(pathToTemplate, "LayoutView");
        }

        private void createScreenshotViews(string pathToTemplate)
        {
            XElement xmlDoc = XElement.Load(@pathToTemplate);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element("LayoutView").Elements("Screenshot")
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            /*foreach (XElement element in uiElement)
               {
                   Debug.WriteLine(element);
               }*/
            ATemplateUi generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
            // TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(firstElement);
            foreach (XElement element in uiElement)
            {
                TemplateScreenshotObject templateObject = xmlUiScreenshotToTemplateUiScreenshot(element);
                GeneralProperties prop = new GeneralProperties();
                prop.controlTypeFiltered = templateObject.connectedFilteredNodeControltype;
                List<Object> nodes = treeOperation.searchNodes.searchProperties(grantTrees.getFilteredTree(), prop); 
          
                generalUiInstance.createUiScreenshotFromTemplate(templateObject, nodes);
            }
        }

        /// <summary>
        /// Ausgehend vom gefilterten Baum wird für einige Ui-Elemente ein Template für die Symbol-Ansicht angewendet.
        /// </summary>
        /// <param name="pathToTemplate">gibt den Pfad zu der XML des Templates für die UI an</param>
        public void generatedSymbolViewFromTemplate(String pathToTemplate)
        {
            if (grantTrees == null || grantTrees.getFilteredTree() == null) { Debug.WriteLine("kein gefilterter Baum vorhanden!"); return; }
            if (pathToTemplate.Equals("")) { Debug.WriteLine("Keine Xml angegeben!"); return; }
            if (!File.Exists(@pathToTemplate)) { Debug.WriteLine("Die XML exisitert nicht"); return; }
            createUiElementsWitheNoDependencySymbolView(@pathToTemplate);
            //Anmerkung: Anstelle hier über den ganzen Baum zu iterrieren, könnte auchmittels der Nethode ITreeOperations.searchProperties nach den Elementen gesucht werden, die im Template berücksichtigt werden sollen; aber dann müsste jedesmal über den ganzen Baum iteriert werden
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.getFilteredTree()))
            {
                createElementType(node, pathToTemplate);
            }
            createUiElementsAllScreens(pathToTemplate, "SymbolView");
            createUiElementsNavigationbarScreensSymbolView(pathToTemplate);
            treeOperation.updateNodes.updateBrailleGroups();
        }

        /// <summary>
        /// aktualisiert die Navigationsleisten (Anzahl der Tabs) auf allen Screens, die eine Navigationsleiste anzeigen
        /// </summary>
        /// <param name="pathToXml"></param>
        public void updateNavigationbarScreens(string pathToXml)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames();
            /*
             * im Braillebaum suchen wo alles eine Navigationsleiste vorhanden ist --> von 
             * ergänzen
             */
            List<Object> navigationbars = treeOperation.searchNodes.getListOfNavigationbars();
            List<String> screensForShowNavigationbar = new List<string>();
            foreach (ITreeStrategy<OSMElement.OSMElement> nbar in navigationbars)
            {                
                if (strategyMgr.getSpecifiedTree().HasChild(nbar))
                {
                    treeOperation.updateNodes.removeChildNodeInBrailleTree(nbar);
                    treeOperation.updateNodes.removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(nbar));
                }
                screensForShowNavigationbar.Add(strategyMgr.getSpecifiedTree().GetData(nbar).brailleRepresentation.screenName);
            }
            templateObject.Screens = screensForShowNavigationbar;
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            Object tree = grantTrees.getFilteredTree();
            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Fügt eine Navigationsleiste für den angegebenen Screen hinzu
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zum Template der Navigationsleiste an</param>
        /// <param name="subtree">gibt den braille-Teilbaum an, bei welchem eine Navigationsleiste hinzugefügt werden soll</param>
        public void addNavigationbarForScreen(string pathToXml, Object subtree)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames();
            List<String> screenNavi = new List<string>();
            screenNavi.Add(strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.screenName);
            templateObject.Screens = screenNavi;
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            Object tree = grantTrees.getFilteredTree();
            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Ermittelt von einem angegebenen Template das zugehörige Template-Objekt für die Navigationsleiste
        /// </summary>
        /// <param name="pathToXml"></param>
        /// <returns></returns>
        private TemplateUiObject getTemplateUiObjectOfNavigationbarScreen(string pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element("SymbolView").Elements("UiElement")
                where (string)el.Element("IsGroup") != null && (string)el.Element("IsGroup") != ""
                    && (string)el.Attribute("name") == "NavigationBarScreens"
                select el;
            // Debug.WriteLine("uiElement: " + uiElement.Count());
            if (uiElement.Count() == 0) { return new TemplateUiObject(); }

            //TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(uiElement.First()); //Es darf nur ein element bei der Suche herauskommen
            return xmlUiElementToTemplateUiObject(uiElement.First()); //Es darf nur ein element bei der Suche herauskommen
        }

        /// <summary>
        /// Ermittelt die Positionen der einzelnen "Tabs" in der Navigationsleiste
        /// </summary>
        /// <param name="screens">gibt die Liste der Screens an, die in der Navigationsleiste angezeigt werden sollen</param>
        /// <param name="templateObject"></param>
        /// <param name="index">gibt den Index an, ab welchen die (Tab-)Element die Position berechnet werden soll</param>
        /// <returns></returns>
        private List<OSMElement.OSMElement> calculatePositionOfScreenTab(List<String> screens, TemplateUiObject templateObject, int index = 0)
        {
            List<OSMElement.OSMElement> groupElementsStatic = new List<OSMElement.OSMElement>();
            if (screens == null || templateObject.Equals(new TemplateUiObject())) { return null; }
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

        public void createUiElementsNavigationbarScreensSymbolView(string pathToXml)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            //TODO: mit Events verknüpfen
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames();
            ATemplateUi generalUiInstance;
            if (templateObject.Screens == null)
            {
                //wir wollen alle verfügbaren Screens
                templateObject.Screens = screens;
            }
            List<OSMElement.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElement.OSMElement>())) { return; }
            templateObject.groupElementsStatic = groupElementsStatic;
            Object tree = grantTrees.getFilteredTree();

            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElement.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);

            generalUiInstance.createUiElementFromTemplate(tree, templateObject);//
        }

        /// <summary>
        /// Sofern für das angegebene UI-element ein template vorhanden ist wird dieses angewendet
        /// </summary>
        /// <param name="subtree">gibt den Teilbaum mit dem element, auf welches das Template angewendet werden soll an</param>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        private void createElementType(Object subtree, String pathToXml)
        {
            String controlType = strategyMgr.getSpecifiedTree().GetData(subtree).properties.controlTypeFiltered;
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element("SymbolView").Elements("UiElement")
                where (string)el.Attribute("name") == controlType && 
                (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") != "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
          /*   foreach (XElement element in uiElement)
             {
                 Debug.WriteLine(element);
             }*/
           // XElement firstElement = uiElement.First(); //Achtung hier wird erstmal einfach das erste gefundene genommen
            ATemplateUi generalUiInstance;
           // TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(firstElement);
            foreach (XElement element in uiElement)
            {
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(element);

                if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
                {
                    generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
                }
                else
                {
                    generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees, treeOperation);
                }
                generalUiInstance.createUiElementFromTemplate(subtree, xmlUiElementToTemplateUiObject(element));
            }
        }

        /// <summary>
        /// Wandelt ein XElement in ein <c>TemplateUiObject</c> um
        /// </summary>
        /// <param name="xmlElement">gibt ein XElement aus der TemplateUi.xml an</param>
        /// <returns>ein <c>TemplateUiObject</c></returns>
        private TemplateScreenshotObject xmlUiScreenshotToTemplateUiScreenshot(XElement xmlElement)
        {
            TemplateScreenshotObject templetObject = new TemplateScreenshotObject();
            Int32 result;
            result = 0;
            GeneralProperties properties = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();
            properties.controlTypeFiltered = "Screenshot";
           // braille.fromGuiElement = xmlElement.Element("TextFromUIElement").Value;
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

            if (!xmlElement.Element("Screens").IsEmpty)
            {
                XElement screens = xmlElement.Element("Screens");
                templetObject.Screens = new List<string>();
                foreach (XElement s in screens.Elements("Screen"))
                {
                    templetObject.Screens.Add(s.Value);
                }

            }
            if (!xmlElement.Element("ConnectedFilteredNode").IsEmpty)
            {
                templetObject.connectedFilteredNodeControltype = xmlElement.Element("ConnectedFilteredNode").Value;
            }
            Double zoom = 0;
            if (!xmlElement.Element("Zoom").IsEmpty)
            {
                try
                {
                    Debug.WriteLine(xmlElement.Element("Zoom").Value);
                    Double resultZoom = XmlConvert.ToDouble(xmlElement.Element("Zoom").Value);

                    zoom = resultZoom < 0 ? 0 : (resultZoom > 3 ? 3 : resultZoom);
                }
                catch (ArgumentNullException e) { Debug.WriteLine("Exception beim Casten des Zoom-Wertes in {}. Fehlerbeschreibung:\n{1}", this.ToString(), e); }
                catch (FormatException e) { Debug.WriteLine("Exception beim Casten des Zoom-Wertes in {}. Fehlerbeschreibung:\n{1}", this.ToString(), e); }
                catch (OverflowException e) { Debug.WriteLine("Exception beim Casten des Zoom-Wertes in {}. Fehlerbeschreibung:\n{1}", this.ToString(), e); }
            }
            braille.zoom = zoom;
            int contrast = 0;
            if (!xmlElement.Element("Contrast").IsEmpty)
            {
                int resultcontrast;
                if (Int32.TryParse(xmlElement.Element("Contrast").Value, out resultcontrast))
                {
                    contrast = resultcontrast < 0 ? 0 : (resultcontrast > 255 ? 255 : resultcontrast);
                }
            }
            braille.contrast = contrast;
         //   braille.showScrollbar = Convert.ToBoolean(xmlElement.Element("ShowScrollbar").Value);
            templetObject.name = templetObject.Screens != null ? templetObject.Screens[0] : "all"; //TODO: besserer Name
            OSMElement.OSMElement osm = new OSMElement.OSMElement();
            osm.brailleRepresentation = braille;
            osm.properties = properties;
            templetObject.osm = osm;

            return templetObject;
        }

        /// <summary>
        /// Wandelt ein XElement in ein <c>TemplateUiObject</c> um
        /// </summary>
        /// <param name="xmlElement">gibt ein XElement aus der TemplateUi.xml an</param>
        /// <returns>ein <c>TemplateUiObject</c></returns>
        private TemplateUiObject xmlUiElementToTemplateUiObject(XElement xmlElement)
        {
            TemplateUiObject templetObject = new TemplateUiObject();
            Int32 result;
            result = 0;
            GeneralProperties properties = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();
            templetObject.allElementsOfType = xmlElement.Element("AllElementsOfType") == null ? false : Boolean.Parse( xmlElement.Element("AllElementsOfType").Value);
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
                String orientation = xmlElement.Element("IsGroup").Element("Orientation").Value;
                group.orienataion = orientation.Equals(OSMElement.UiElements.Orientation.Vertical.ToString()) ? OSMElement.UiElements.Orientation.Vertical :
                    orientation.Equals(OSMElement.UiElements.Orientation.Top.ToString()) ? OSMElement.UiElements.Orientation.Top :
                    orientation.Equals(OSMElement.UiElements.Orientation.Right.ToString()) ? OSMElement.UiElements.Orientation.Right :
                    orientation.Equals(OSMElement.UiElements.Orientation.Left.ToString()) ? OSMElement.UiElements.Orientation.Left :
                    orientation.Equals(OSMElement.UiElements.Orientation.Horizontal.ToString()) ? OSMElement.UiElements.Orientation.Horizontal : OSMElement.UiElements.Orientation.Bottom;
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
        /// Erstellt Ui-elemente die keine Verbindung zum gefilterten Baum haben für die Symbol-Ansicht
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        private void createUiElementsWitheNoDependencySymbolView(String pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element("SymbolView").Elements("UiElement")
                where (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") == "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            foreach (XElement element in uiElement)
            {
                //Debug.WriteLine(element);
                ATemplateUi generalUiInstance;
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(element);
                if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
                {
                    generalUiInstance = new  TemplateNode(strategyMgr, grantTrees, treeOperation);
                }
                else
                {
                    generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees, treeOperation);
                }

                Object tree = strategyMgr.getSpecifiedTree().NewTree();
                generalUiInstance.createUiElementFromTemplate(tree, templateObject);
            }
        }

        /// <summary>
        /// Erstellt die Ui-Elemente, welche auf jedem Screen vorhanden sein sollen
        /// </summary>
        /// <param name="pathToXml">gibt den Pfad zu dem zu nutzenden Template an</param>
        /// <param name="nameOfView">gibt den Namen der Ansicht an (momentan verfügbar. "SymbolView" und "LayoutView"</param>
        public void createUiElementsAllScreens(String pathToXml, String nameOfView)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element(nameOfView).Elements("UiElement")
                where (string)el.Element("Screens") != null && (string)el.Element("Screens") == "" && (string)el.Attribute("name") != "NavigationBarScreens"
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            List<String> screenList = treeOperation.searchNodes.getPosibleScreenNames();
            foreach (XElement e in uiElement)
            {
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(e);
                Object tree = strategyMgr.getSpecifiedTree().NewTree(); // <-- ist nur der Fall, wenn es keinen Zusammenhang zum Baum gibt
                if (templateObject.osm.brailleRepresentation.fromGuiElement!= null && !templateObject.osm.brailleRepresentation.fromGuiElement.Equals(""))
                {
                    // elementE im gefilterten Baum suchen
                    GeneralProperties properties = new GeneralProperties();
                    properties.controlTypeFiltered = e.Attribute("name").Value;
                    List<Object> treefilteredElements = treeOperation.searchNodes.searchProperties(grantTrees.getFilteredTree(), properties, OperatorEnum.and);
                    foreach (Object t in treefilteredElements)
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
        private void iterateScreensForTemplate(List<String> screenList, ref Object tree, TemplateUiObject templateObject)
        {
            if (screenList == null) { return; }
            ATemplateUi generalUiInstance;
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
            {
                generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
            }
            else
            {
               // generalUiInstance = new TemplateSubtree(strategyMgr, grantTrees);
                generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees, treeOperation);
            }
           // ATemplateUi generalUiInstance = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees);
            foreach (String screen in screenList)
            {
                templateObject.Screens = new List<string>();
                templateObject.Screens.Add(screen);

                generalUiInstance.createUiElementFromTemplate(tree, templateObject);
            }
        }


        public void createUiElementFromTemplate(Object filteredSubtree, GRANTManager.Interfaces.TemplateUiObject templateObject, String brailleNodeId = null)
        {
            Type typeOfTemplate = Type.GetType(templateObject.osm.brailleRepresentation.templateFullName + ", " + templateObject.osm.brailleRepresentation.templateNamspace);
            if (typeOfTemplate != null)
            {
                ATemplateUi template = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees, treeOperation);
                template.createUiElementFromTemplate(filteredSubtree, templateObject, brailleNodeId);
            }
        }
    }

}
