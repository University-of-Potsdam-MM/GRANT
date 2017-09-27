using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElements;
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
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        private String VIEWCATEGORY_SYMBOLVIEW;
        private String VIEWCATEGORY_LAYOUTVIEW;
        public GenaralUI(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            List<String> viewCategories = Settings.getPossibleTypesOfViews();
            if(viewCategories == null) { throw new Exception("There isn't a ViewCategory specifies in the Settings!"); }
            VIEWCATEGORY_SYMBOLVIEW = viewCategories[0];
            VIEWCATEGORY_LAYOUTVIEW = viewCategories[1];
        }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperation = treeOperation; }
        /// <summary>
        /// Generates Ui elements depending on a template file
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        public void generatedUiFromTemplate(String pathToXml) 
        {
            if (grantTrees == null || grantTrees.filteredTree == null) { Debug.WriteLine("There isn't a filtered tree!"); return; }
            if (pathToXml.Equals("")) { Debug.WriteLine("No xml file specified!"); return; }
            if (!File.Exists(@pathToXml)) { Debug.WriteLine("The specified XML file dosn't exist!"); return; }
            generatedSymbolView(@pathToXml);
            generatedLayoutView(@pathToXml);
            Rect rect = new Rect(0, 0, 0, 0);
            
         /*  if (grantTrees.rendererUiElementConnection == null)
            {
                TemplateAllElementsSymbol t = new TemplateAllElementsSymbol(strategyMgr, grantTrees);
                t.loadTemplateConnectionsForAllElements();
            }
            visualizedAllElementsAsSymbols(grantTrees.filteredTree, ref rect);*/
         
        }

        /// <summary>
        /// Generates Ui elements for the layout view depending on a template file
        /// </summary>
        /// <param name="pathToTemplate">path of the used template (XML)</param>
        public void generatedLayoutView(String pathToTemplate)
        {
            createScreenshotViews(pathToTemplate);
            createUiElementsAllScreens(pathToTemplate, VIEWCATEGORY_LAYOUTVIEW);
        }

        private void createScreenshotViews(string pathToTemplate)
        {
            XElement xmlDoc = XElement.Load(@pathToTemplate);
            if(xmlDoc.Element(VIEWCATEGORY_LAYOUTVIEW) == null) { return; }
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element(VIEWCATEGORY_LAYOUTVIEW).Elements("Screenshot")
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            ATemplateUi generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
            foreach (XElement element in uiElement)
            {
                TemplateScreenshotObject templateObject = xmlUiScreenshotToTemplateUiScreenshot(element);
                GeneralProperties prop = new GeneralProperties();
                prop.controlTypeFiltered = templateObject.connectedFilteredNodeControltype;
                List<Object> nodes = treeOperation.searchNodes.getNodesByProperties(grantTrees.filteredTree, prop); 
          
                generalUiInstance.createUiScreenshotFromTemplate(templateObject, nodes);
            }
        }

        /// <summary>
        /// Generates Ui elements for the symbol view depending on a template file
        /// </summary>
        /// <param name="pathToTemplate">path of the used template (XML)</param>
        public void generatedSymbolView(String pathToTemplate)
        {
            if (grantTrees == null || grantTrees.filteredTree == null) { Debug.WriteLine("There isn't a filtered tree!"); return; }
            if (pathToTemplate.Equals("")) { Debug.WriteLine("No xml file specified!"); return; }
            if (!File.Exists(@pathToTemplate)) { Debug.WriteLine("The specified XML file dosn't exist!"); return; }
            createUiElementsWitheNoDependencySymbolView(@pathToTemplate);
            //Note: It is also possible to use the function ITreeOperations.searchProperties BUT in this case it would be iterated over the whole tree every time
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                createElementTypeOfSymbolView(node, pathToTemplate);
            }
            createUiElementsAllScreens(pathToTemplate, VIEWCATEGORY_SYMBOLVIEW);
            createNavigationbar(pathToTemplate, VIEWCATEGORY_SYMBOLVIEW);
            treeOperation.updateNodes.updateBrailleGroups();           
        }

        /// <summary>
        /// Updates all navigation bars in this type of view
        /// </summary>
        /// <param name="pathToXml">path of the used template(XML)</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bar should be updated</param>
        public void updateNavigationbarScreens(string pathToXml, String typeOfView)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames(typeOfView);
            // searches for navigation bars in the braille tree and adds new tabs             
            List<Object> navigationbars = treeOperation.searchNodes.getListOfNavigationbars();
            List<String> screensForShowNavigationbar = new List<string>();
            foreach (Object nbar in navigationbars)
            {                
                if (strategyMgr.getSpecifiedTree().HasChild(nbar))
                {
                    treeOperation.updateNodes.removeChildNodeInBrailleTree(nbar);
                    treeOperation.updateNodes.removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(nbar).properties.IdGenerated);
                }
                screensForShowNavigationbar.Add(strategyMgr.getSpecifiedTree().GetData(nbar).brailleRepresentation.screenName);
            }
            templateObject.Screens = screensForShowNavigationbar;
            List<OSMElements.OSMElement> groupElements = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElements == null || groupElements.Equals(new List<OSMElements.OSMElement>())) { return; }
            templateObject.groupElements = groupElements;
            Object tree = grantTrees.filteredTree;
            
         //   OSMElement.OSMElement osm = templateObject.osm;
            templateObject.osm.brailleRepresentation.groupelementsOfSameType = new GroupelementsOfSameType();
          //  templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Adds a navigatiobar for the screen
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <param name="screenName">name of the screen on wich the navigation bar should be added</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bar should be added</param>
        public void addNavigationbarForScreen(string pathToXml, String screenName, String typeOfView)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames(typeOfView);
            List<String> screenNavi = new List<string>();
            //screenNavi.Add(strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.screenName);
            screenNavi.Add(screenName);
            templateObject.Screens = screenNavi;
            List<OSMElements.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElements.OSMElement>())) { return; }
            templateObject.groupElements = groupElementsStatic;
            Object tree = grantTrees.filteredTree;
            BrailleRepresentation br = templateObject.osm.brailleRepresentation;
            br.groupelementsOfSameType = new GroupelementsOfSameType();
            OSMElements.OSMElement osm = templateObject.osm;
            osm.brailleRepresentation = br;
            templateObject.osm = osm;
            ATemplateUi generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);
        }

        /// <summary>
        /// Creates a template object (<see cref="TemplateUiObject"/>) for navigation bars from a template file (XML) 
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <returns>a template object for navigation bars</returns>
        private TemplateUiObject getTemplateUiObjectOfNavigationbarScreen(string pathToXml)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            try
            {
                IEnumerable<XElement> uiElement =
                    from el in xmlDoc.Element(VIEWCATEGORY_SYMBOLVIEW).Elements("UiElement")
                    where (string)el.Element("IsGroup") != null && (string)el.Element("IsGroup") != ""
                        && (string)el.Attribute("name") == Settings.getNavigationbarSubstring()
                    select el;
                if (uiElement.Count() == 0) { return new TemplateUiObject(); }

                return xmlUiElementToTemplateUiObject(uiElement.First(), VIEWCATEGORY_SYMBOLVIEW); //Only one result should be found
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("It is not symole view specified in the template!");
                return new TemplateUiObject();
            }
        }

        /// <summary>
        /// Calculates the positions of each tab in a navigation bar
        /// </summary>
        /// <param name="screens">specifies a list of screens to show this navigation bar</param>
        /// <param name="templateObject">The template object for the navigation bar</param>
        /// <param name="index">the index where the calculation should be start</param>
        /// <returns></returns>
        private List<OSMElements.OSMElement> calculatePositionOfScreenTab(List<String> screens, TemplateUiObject templateObject, int index = 0)
        {
            List<OSMElements.OSMElement> groupElementsStatic = new List<OSMElements.OSMElement>();
            if (screens == null || templateObject.Equals(new TemplateUiObject())) { return null; }
            foreach (String s in screens)
            {
                OSMElements.OSMElement childOsm = new OSMElements.OSMElement();
                Rect rect = templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle;
                
                if (templateObject.orientation.Equals(OSMElements.UiElements.Orientation.Left) || templateObject.orientation.Equals(OSMElements.UiElements.Orientation.Right))
                {
                    rect.Y = (rect.Height + 1) * index + rect.Y + 1;
                }
                if (templateObject.orientation.Equals(OSMElements.UiElements.Orientation.Top) || templateObject.orientation.Equals(OSMElements.UiElements.Orientation.Bottom))
                {
                    rect.X = (rect.Width + 1) * index + rect.X + 1;
                }

                childOsm.properties.boundingRectangleFiltered = rect;
                childOsm.properties.controlTypeFiltered = templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer;
                childOsm.properties.valueFiltered = s;
                childOsm.brailleRepresentation.isGroupChild = true;
                childOsm.brailleRepresentation.isVisible = true;
                childOsm.brailleRepresentation.viewName = "_" + s;//TODO
                childOsm.brailleRepresentation.typeOfView = templateObject.osm.brailleRepresentation.typeOfView;
                OSMElements.UiElements.TabItem tabView = new OSMElements.UiElements.TabItem();
                tabView.orientation = templateObject.orientation;
                childOsm.brailleRepresentation.uiElementSpecialContent = tabView;
                groupElementsStatic.Add(childOsm);
                index++;
            }
            return groupElementsStatic;
        }

        /// <summary>
        /// Creates navigation bars for every screen in this tpye of view
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <param name="typeOfView">name of the type of view in which the navigation bars should be added</param>
        public void createNavigationbar(string pathToXml, String typeOfView)
        {
            TemplateUiObject templateObject = getTemplateUiObjectOfNavigationbarScreen(pathToXml);
            if (templateObject.Equals(new TemplateUiObject())) { return; }
            //TODO: mit Events verknüpfen
            List<String> screens = treeOperation.searchNodes.getPosibleScreenNames(typeOfView);
            ATemplateUi generalUiInstance;
            if (templateObject.Screens == null)
            {
                //we use all available screens of this tpye of view
                templateObject.Screens = screens;
            }
            List<OSMElements.OSMElement> groupElementsStatic = calculatePositionOfScreenTab(screens, templateObject);
            if (groupElementsStatic == null || groupElementsStatic.Equals(new List<OSMElements.OSMElement>())) { return; }
            templateObject.groupElements = groupElementsStatic;            
          //  templateObject.osm = templateObject.osm;
            templateObject.osm.brailleRepresentation.groupelementsOfSameType = new GroupelementsOfSameType();
            generalUiInstance = new TemplateGroupStatic(strategyMgr, grantTrees, treeOperation);
            Object tree = grantTrees.filteredTree;
            generalUiInstance.createUiElementFromTemplate(tree, templateObject);//
        }

        /// <summary>
        /// Converts a filtered (sub-)tree to symbole (for the braille tree) and adds this node
        /// </summary>
        /// <param name="subtree">filtered (sub-)tree to convert as symbols</param>
        /// <param name="pathToXml">path of the used template (XML)</param>
        private void createElementTypeOfSymbolView(Object subtree, String pathToXml)
        {
            String controlType = strategyMgr.getSpecifiedTree().GetData(subtree).properties.controlTypeFiltered;
            XElement xmlDoc = XElement.Load(@pathToXml);
            if (xmlDoc.Element(VIEWCATEGORY_SYMBOLVIEW) == null) { return; }
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element(VIEWCATEGORY_SYMBOLVIEW).Elements("UiElement")
                where (string)el.Attribute("name") == controlType && 
                (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") != "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            ATemplateUi generalUiInstance;
            foreach (XElement element in uiElement)
            {
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(element, VIEWCATEGORY_SYMBOLVIEW);

                if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
                {
                    generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
                }
                else
                {
                    generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees, treeOperation);
                }
                generalUiInstance.createUiElementFromTemplate(subtree, templateObject);
            }
        }

        /// <summary>
        /// Converts a <see cref="XElement"/> of a screenshot to a <see cref="TemplateScreenshotObject"/> 
        /// </summary>
        /// <param name="xmlElement">a <see cref="XElement"/> from the template file (XML)</param>
        /// <returns>the TemplateScreenshotObject</returns>
        private TemplateScreenshotObject xmlUiScreenshotToTemplateUiScreenshot(XElement xmlElement)
        {
            TemplateScreenshotObject templetObject = new TemplateScreenshotObject();
            Int32 result;
            result = 0;
            templetObject.osm = new OSMElements.OSMElement();
            templetObject.osm.properties.controlTypeFiltered = "Screenshot";
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
            templetObject.osm.properties.boundingRectangleFiltered = rect;

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
                    Double resultZoom = XmlConvert.ToDouble(xmlElement.Element("Zoom").Value);
                    zoom = resultZoom < 0 ? 0 : (resultZoom > 3 ? 3 : resultZoom);
                }
                catch (ArgumentNullException e) { Debug.WriteLine("Exception at casting the zoom value: {0}", e); }
                catch (FormatException e) { Debug.WriteLine("Exception at casting the zoom value: {0}", e); }
                catch (OverflowException e) { Debug.WriteLine("Exception at casting the zoom value: {0}}", e); }
            }
            templetObject.osm.brailleRepresentation.zoom = zoom;
            int contrast = 0;
            if (!xmlElement.Element("Contrast").IsEmpty)
            {
                int resultcontrast;
                if (Int32.TryParse(xmlElement.Element("Contrast").Value, out resultcontrast))
                {
                    contrast = resultcontrast < 0 ? 0 : (resultcontrast > 255 ? 255 : resultcontrast);
                }
            }
            templetObject.osm.brailleRepresentation.contrast = contrast;
            templetObject.osm.brailleRepresentation.typeOfView = VIEWCATEGORY_LAYOUTVIEW;
         //   braille.isScrollbarShow = Convert.ToBoolean(xmlElement.Element("ShowScrollbar").Value);
            templetObject.
                viewName = templetObject.Screens != null ? templetObject.Screens[0] : "all"; //TODO: besserer Name

            return templetObject;
        }

        /// <summary>
        /// Converts a <see cref="XElement"/> to a <see cref="TemplateUiObject"/>
        /// </summary>
        /// <param name="xmlElement">a <see cref="XElement"/> from the template file (XML)</param>
        /// <param name="typeOfView">specifies the type of view</param>
        /// <returns>a TemplateUiObject</returns>
        private TemplateUiObject xmlUiElementToTemplateUiObject(XElement xmlElement, String typeOfView)
        {
            TemplateUiObject templetObject = new TemplateUiObject();
            templetObject.osm = new OSMElements.OSMElement();
            Int32 result;
            result = 0;

            templetObject.osm.brailleRepresentation.typeOfView = typeOfView;
            templetObject.allElementsOfType = xmlElement.Element("AllElementsOfType") == null ? false : Boolean.Parse( xmlElement.Element("AllElementsOfType").Value);
            templetObject.osm.properties.controlTypeFiltered = xmlElement.Element("Renderer").Value;
            templetObject.osm.brailleRepresentation.displayedGuiElementType = xmlElement.Element("TextFromUIElement").Value;
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
            templetObject.osm.properties.boundingRectangleFiltered = rect;

            if (!xmlElement.Element("BoxModel").IsEmpty)
            {
                XElement boxModel = xmlElement.Element("BoxModel");
                if (!boxModel.Element("Padding").IsEmpty)
                {
                    XElement padding = boxModel.Element("Padding");
                    templetObject.osm.brailleRepresentation.padding = new Padding(padding.Element("Left") == null ? 0 : Convert.ToInt32(padding.Element("Left").Value), padding.Element("Top") == null ? 0 : Convert.ToInt32(padding.Element("Top").Value), padding.Element("Right") == null ? 0 : Convert.ToInt32(padding.Element("Right").Value), padding.Element("Bottom") == null ? 0 : Convert.ToInt32(padding.Element("Bottom").Value));
                }
                if (!boxModel.Element("Margin").IsEmpty)
                {
                    XElement margin = boxModel.Element("Margin");
                    templetObject.osm.brailleRepresentation.margin = new Padding(margin.Element("Left") == null ? 0 : Convert.ToInt32(margin.Element("Left").Value), margin.Element("Top") == null ? 0 : Convert.ToInt32(margin.Element("Top").Value), margin.Element("Right") == null ? 0 : Convert.ToInt32(margin.Element("Right").Value), margin.Element("Bottom") == null ? 0 : Convert.ToInt32(margin.Element("Bottom").Value));
                }
                if (!boxModel.Element("Boarder").IsEmpty)
                {
                    XElement boarder = boxModel.Element("Boarder");
                    templetObject.osm.brailleRepresentation.boarder = new Padding(boarder.Element("Left") == null ? 0 : Convert.ToInt32(boarder.Element("Left").Value), boarder.Element("Top") == null ? 0 : Convert.ToInt32(boarder.Element("Top").Value), boarder.Element("Right") == null ? 0 : Convert.ToInt32(boarder.Element("Right").Value), boarder.Element("Bottom") == null ? 0 : Convert.ToInt32(boarder.Element("Bottom").Value));
                }
            }

            if (xmlElement.Element("IsGroup").HasElements)
            {
                templetObject.groupImplementedClassTypeFullName = typeof(TemplateSubtree).FullName;
                templetObject.groupImplementedClassTypeDllName = typeof(TemplateSubtree).Module.Assembly.GetName().Name; // == Dll-Name
                GroupelementsOfSameType group = new GroupelementsOfSameType();
                group.isLinebreak = Convert.ToBoolean(xmlElement.Element("IsGroup").Element("Linebreak").Value);
                String orientation = xmlElement.Element("IsGroup").Element("Orientation").Value;
                group.orienataion = orientation.Equals(OSMElements.UiElements.Orientation.Vertical.ToString()) ? OSMElements.UiElements.Orientation.Vertical :
                    orientation.Equals(OSMElements.UiElements.Orientation.Top.ToString()) ? OSMElements.UiElements.Orientation.Top :
                    orientation.Equals(OSMElements.UiElements.Orientation.Right.ToString()) ? OSMElements.UiElements.Orientation.Right :
                    orientation.Equals(OSMElements.UiElements.Orientation.Left.ToString()) ? OSMElements.UiElements.Orientation.Left :
                    orientation.Equals(OSMElements.UiElements.Orientation.Horizontal.ToString()) ? OSMElements.UiElements.Orientation.Horizontal : OSMElements.UiElements.Orientation.Bottom;
                position = xmlElement.Element("IsGroup").Element("childBoundingRectangle");
                Rect childRect = new Rect();
                isConvertHeight = Int32.TryParse(position.Element("Height").Value, out result);
                childRect.Height = isConvertHeight ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
                isConvertWidth = Int32.TryParse(position.Element("Width").Value, out result);
                childRect.Width = isConvertWidth ? result : strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
                childRect.X = rect.X + templetObject.osm.brailleRepresentation.padding.Left + templetObject.osm.brailleRepresentation.margin.Left + templetObject.osm.brailleRepresentation.boarder.Left;
                childRect.Y = rect.Y + templetObject.osm.brailleRepresentation.padding.Top + templetObject.osm.brailleRepresentation.margin.Top + templetObject.osm.brailleRepresentation.boarder.Top;
                if (!isConvertHeight) { childRect.Height -= childRect.Y; }
                if (!isConvertWidth) { childRect.Width -= childRect.X; }
                group.childBoundingRectangle = childRect;
                group.renderer = xmlElement.Element("Renderer").Value;
                templetObject.osm.brailleRepresentation.groupelementsOfSameType = group;
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
                templetObject.orientation = (value.Equals( OSMElements.UiElements.Orientation.Left.ToString()) ? OSMElements.UiElements.Orientation.Left : (value.Equals( OSMElements.UiElements.Orientation.Bottom.ToString()) ? OSMElements.UiElements.Orientation.Bottom : (value.Equals( OSMElements.UiElements.Orientation.Right.ToString()) ? OSMElements.UiElements.Orientation.Right : OSMElements.UiElements.Orientation.Top)));                
            }
            templetObject.osm.brailleRepresentation.isScrollbarShow = Convert.ToBoolean( xmlElement.Element("ShowScrollbar").Value);
            templetObject.viewName= xmlElement.Attribute("name").Value;
            return templetObject;
        }

        /// <summary>
        /// Creates UI elements (braille nodes) for the symbol view which have no connection to the filtered tree
        /// </summary>
        /// <param name="pathToTemplate">path of the used template (XML)</param>
        private void createUiElementsWitheNoDependencySymbolView(String pathToTemplate)
        {
            XElement xmlDoc = XElement.Load(pathToTemplate);
            if (xmlDoc.Element(VIEWCATEGORY_SYMBOLVIEW) == null) { return; }
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element(VIEWCATEGORY_SYMBOLVIEW).Elements("UiElement")
                where (string)el.Element("TextFromUIElement") != null && (string)el.Element("TextFromUIElement") == "" &&
                (string)el.Element("Screens") != null && (string)el.Element("Screens") != ""
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            foreach (XElement element in uiElement)
            {
                ATemplateUi generalUiInstance;
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(element, VIEWCATEGORY_SYMBOLVIEW);
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
        /// Creates all Ui elements to be shown on all screens (in this type of view)
        /// </summary>
        /// <param name="pathToXml">path of the used template (XML)</param>
        /// <param name="typeOfView">name of the type of view in which this elements should be added (current: "SymbolView", "TextView" or "LayoutView")</param>
        private void createUiElementsAllScreens(String pathToXml, String typeOfView)
        {
            XElement xmlDoc = XElement.Load(@pathToXml);
            if (xmlDoc.Element(typeOfView) == null) { return; }
            IEnumerable<XElement> uiElement =
                from el in xmlDoc.Element(typeOfView).Elements("UiElement")
                where (string)el.Element("Screens") != null && (string)el.Element("Screens") == "" && (string)el.Attribute("name") != Settings.getNavigationbarSubstring()
                select el;
            if (uiElement == null || !uiElement.Any()) { return; }
            List<String> screenList = treeOperation.searchNodes.getPosibleScreenNames(typeOfView);
            foreach (XElement e in uiElement)
            {
                TemplateUiObject templateObject = xmlUiElementToTemplateUiObject(e, typeOfView);
                Object tree = strategyMgr.getSpecifiedTree().NewTree(); // this is necessary if the braille node has no connection to a filtered node
                if (templateObject.osm.brailleRepresentation.displayedGuiElementType!= null && !templateObject.osm.brailleRepresentation.displayedGuiElementType.Equals(""))
                {
                    GeneralProperties properties = new GeneralProperties();
                    properties.controlTypeFiltered = e.Attribute("name").Value;
                    List<Object> treefilteredElements = treeOperation.searchNodes.getNodesByProperties(grantTrees.filteredTree, properties, OperatorEnum.and);
                    foreach (Object t in treefilteredElements)
                    {
                        tree = t;
                        createsBrailleNodeForScreenList(screenList, ref tree, templateObject);
                    }
                }
                else
                {
                    createsBrailleNodeForScreenList(screenList, ref tree, templateObject);
                }
            }
        }      

        /// <summary>
        /// Creates for all screens of the list a braille node depending on the template object
        /// </summary>
        /// <param name="screenList">the list of screen names</param>
        /// <param name="tree">a filtered tree object</param>
        /// <param name="templateObject">the template object</param>
        private void createsBrailleNodeForScreenList(List<String> screenList, ref Object tree, TemplateUiObject templateObject)
        {
            if (screenList == null) { return; }
            ATemplateUi generalUiInstance;
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.Equals(new GroupelementsOfSameType()))
            {
                generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
            }
            else
            {
                generalUiInstance = new TemplateGroupAutomatic(strategyMgr, grantTrees, treeOperation);
            }
            
            foreach (String screen in screenList)
            {
                TemplateUiObject copyTemp = templateObject.DeepCopy();
                copyTemp.Screens = new List<string>();
                copyTemp.Screens.Add(screen);
                generalUiInstance.createUiElementFromTemplate(tree, copyTemp);
            }
        }

        /// <summary>
        /// Creates all UI elements for the Braille tree which are specified in the template
        /// </summary>
        /// <param name="filteredSubtree">the filtered (sub-)tree</param>
        /// <param name="templateObject">the template object for the group to created</param>
        /// <param name="brailleNodeId">Id of the parent element of the group</param>
        public void createUiElementFromTemplate(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null)
        {
            Type typeOfTemplate = Type.GetType(templateObject.osm.brailleRepresentation.templateFullName + ", " + templateObject.osm.brailleRepresentation.templateNamspace);
            if (typeOfTemplate != null)
            {
                ATemplateUi template = (ATemplateUi)Activator.CreateInstance(typeOfTemplate, strategyMgr, grantTrees, treeOperation);
                template.createUiElementFromTemplate(filteredSubtree, templateObject, brailleNodeId);
            }
        }

        #region Dislpay all elements as symbols in the braille tree => Currently it isn't used and it isn't readied

        /// <summary>
        /// Adds all elements (of the subtree) as Sysmbols to the braille tree
        /// </summary>
        /// <param name="subtree">subtree to add as Symbols</param>
        /// <param name="lastRect">position of the last UI element which was added</param>
        /// <param name="idToIgnore">a list of all (ids of) elements which should NOT be added as symbol</param>
        public void addsAllElementsAsSymbols(object subtree, ref Rect  lastRect, string[] idToIgnore = null)
        {
            ATemplateUi generalUiInstance = new TemplateNode(strategyMgr, grantTrees, treeOperation);
            RendererUiElementConnector defaultRendererUiConnector = new RendererUiElementConnector("Text", "Text", new RendererUiElementConnector.SizeUiElement(5, 21));
            
            foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtree))
            {
                /* Prüfen, ob der Knoten Ignoriert werden soll
                 *    nicht ignorieren -> Kinder
                 * nur Kinder Zeichnen -> ControllType der Eltern beachten --> evtl. werden mehrere Kinder mit einmal dargestellt
                 */
                OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node);
                if (idToIgnore == null || !idToIgnore.Contains(osmNode.properties.IdGenerated))
                {
                    RendererUiElementConnector connector = rendererUiElementConnectorContainsControltype(osmNode.properties.controlTypeFiltered);
                    if (connector == null && strategyMgr.getSpecifiedTree().HasChild(node))
                    {
                        //consider the children
                        addsAllElementsAsSymbols(node, ref lastRect, idToIgnore);
                    }
                    else
                    {
                        if(connector == null) { connector = defaultRendererUiConnector; }
                        //TODO: use a diffenent template for every element type?
                        generalUiInstance.createUiElementFromTemplate(node, createTemplateObjectFromNode(node, connector, ref lastRect));//new type of view
                    }
                }
            }
        }

        private TemplateUiObject createTemplateObjectFromNode(Object nodeFiltered, RendererUiElementConnector connector, ref Rect rectLast, String screenName = null)
        {//TODO: a lot of assigned of variables are hard coded and only so because of testing
            OSMElements.OSMElement osmNodeFiltered = strategyMgr.getSpecifiedTree().GetData(nodeFiltered);
            TemplateUiObject templObject = new TemplateUiObject();
            List<String> screens = new List<string>();
            screens.Add(screenName == null ? "neuerScreen" : screenName);
            templObject.Screens = screens;
            templObject.viewName = osmNodeFiltered.properties.IdGenerated;
            templObject.osm = new OSMElements.OSMElement();
            //Attention: currently all elements are below the other
            rectLast.X = 0;
            rectLast.Y = rectLast.Height == 0 ? 0 : rectLast.Y + rectLast.Height + 1;
            rectLast.Height = connector.SizeElement.Height;
            rectLast.Width = connector.SizeElement.Width;
            templObject.osm.properties.boundingRectangleFiltered = rectLast;
            templObject.osm.properties.controlTypeFiltered = connector.RendererName;

            templObject.osm.brailleRepresentation.displayedGuiElementType = osmNodeFiltered.properties.nameFiltered != null ? "nameFiltered" : "valueFiltered";
            templObject.osm.brailleRepresentation.typeOfView = "blub";
            return templObject;
        }

        private RendererUiElementConnector rendererUiElementConnectorContainsControltype(String controltype)
        {
            if(grantTrees.rendererUiElementConnection == null) { return null; }
            foreach(RendererUiElementConnector connector in grantTrees.rendererUiElementConnection)
            {
                if (connector.ControlType.Equals(controltype))
                {
                    return connector;
                }
            }
            return null;
        }
        #endregion
    }

}
