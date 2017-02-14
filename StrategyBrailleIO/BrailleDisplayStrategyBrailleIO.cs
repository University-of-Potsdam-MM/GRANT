using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using BrailleIO;
using BrailleIO.Structs;
using GRANTManager.Interfaces;
using GRANTManager;
using OSMElement;
using OSMElement.UiElements;
using BrailleIOGuiElementRenderer;
using BrailleIO.Interface;
using GRANTManager.TreeOperations;
using BrailleIO.Renderer;

namespace StrategyBrailleIO
{
    public class BrailleDisplayStrategyBrailleIO : IBrailleDisplayStrategy
    {
       IBrailleIOShowOffMonitor monitor;
       BrailleIOMediator brailleIOMediator {get; set;}
        private Boolean initialized = false;
        private List<uiElementsTypeStruct> uiElementList;
        /// <summary>
        /// Ist der Adapter des Simulators
        /// </summary>
        AbstractBrailleIOAdapterBase showOffAdapter;
        //GestureRecognizer showOffGestureRecognizer;

        /// <summary>
        /// Ist der Adapter des BrailleDis
        /// </summary>
        AbstractBrailleIOAdapterBase brailleAdapter;
        //GestureRecognizer brailleDisRecognizer; //TODO


        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public void setTreeOperation(TreeOperation treeOperation) { uiElementList = getUiElements(); this.treeOperation = treeOperation; }
        public BrailleDisplayStrategyBrailleIO() { uiElementList = getUiElements();}

        /// <summary>
        /// Erstellt, sofern noch nicht vorhanden, ein Simulator für das Ausgabegerät
        /// </summary>
        private void initializedSimulator()
        {
            if (brailleIOMediator == null )
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            /// aus BrailleIOExample (getShowOff()) -> erstmal gekürzt
            if (brailleIOMediator != null)
            {
                // if the current Adapter manager holds an debug dapter, use it
                if (brailleIOMediator.AdapterManager is ShowOffBrailleIOAdapterManager)
                {
                    monitor = ((ShowOffBrailleIOAdapterManager)brailleIOMediator.AdapterManager).Monitor;
                    foreach (var adapter in brailleIOMediator.AdapterManager.GetAdapters())
                    {
                        if (adapter is BrailleIOAdapter_ShowOff)
                        {
                            showOffAdapter = adapter as AbstractBrailleIOAdapterBase;
                            break;
                        }
                    }
                }

                // if no debug device currently exists, create a new one
                if (showOffAdapter == null)
                {
                    monitor = new ShowOff();
                    showOffAdapter = monitor.GetAdapter(brailleIOMediator.AdapterManager);
                    if (showOffAdapter != null) brailleIOMediator.AdapterManager.AddAdapter(showOffAdapter);
                }

                // if a debug adapter could been created, register to its events
                if (showOffAdapter != null)
                {
                    showOffAdapter.Synch = true; // activate that this device receives the pin matrix of the active device, too.

                    /*     #region events

                         showOffAdapter.touchValuesChanged += new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(_bda_touchValuesChanged);
                         showOffAdapter.keyStateChanged += new EventHandler<BrailleIO_KeyStateChanged_EventArgs>(_bda_keyStateChanged);

                         #endregion*/
                }

                /*  if (monitor != null)
                  {
                      monitor.Disposed += new EventHandler(monitor_Disposed);
                  }
                  */
            }
        }

        public void removeActiveAdapter()
        {
            if (brailleIOMediator != null)
            {
                List<AbstractViewBoxModelBase> brailleIOScreens = brailleIOMediator.GetViews();
                foreach (var s in brailleIOScreens)
                {
                    brailleIOMediator.RemoveView(s.Name);
                }
                brailleIOMediator = null;
                initialized = false;
            }
        }

        public void setActiveAdapter()
        {
            if (brailleIOMediator == null)
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            initialized = true;
            Type activeDeviceType = Type.GetType(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeFullName + ", " + strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeNamespace);
            //falls der BrailleIO-Simulator genutzt werden soll, wird dieser extra initialisiert
            if (activeDeviceType.Equals(typeof(DisplayStrategyBrailleIoSimulator)))
            {
                initializedSimulator();
                return;
            }
            if (brailleIOMediator != null && brailleIOMediator.AdapterManager != null)
            {
                brailleAdapter = displayStrategyClassToBrailleIoAdapterClass(activeDeviceType);
                brailleIOMediator.AdapterManager.ActiveAdapter = brailleAdapter;
            }
            return;

        }


        /// <summary>
        /// Ändert den Inhalt einer View
        /// </summary>
        /// <param name="element">Gibt das OSM-element an, bei dem eine Änderung erfolgte</param>
        public void updateViewContent(ref OSMElement.OSMElement element)
        {
            IBrailleIOAdapterManager adapter =  brailleIOMediator.AdapterManager;

            BrailleIOScreen screen = brailleIOMediator.GetView(element.brailleRepresentation.screenName) as BrailleIOScreen;
            if (screen == null)
            {
                throw new Exception("Der Screen existiert nicht!");
            }
            BrailleIOViewRange view = screen.GetViewRange(element.brailleRepresentation.viewName) as BrailleIOViewRange;
            String uiElementType = element.properties.controlTypeFiltered;
            if (view == null)
            {
                createScreen(element.brailleRepresentation.screenName);
                createView(element);
                view = screen.GetViewRange(element.brailleRepresentation.viewName) as BrailleIOViewRange;
                //throw new Exception("Der View existiert (in dem Screen) nicht!");
                Console.WriteLine("Die View exisiterte noch nicht; sie wurde gerade erstellt");
            }
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // für alle anderen muss bei BrailleIO "otherContent" zugewiesen werden
            
            {
                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);              
                if (renderer == null)
                {
                    Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", element.properties.controlTypeFiltered);
                    return;
                }
                view.SetOtherContent(convertToBrailleIOUiElement(element), renderer);
                brailleIOMediator.RenderDisplay();
                return;
            }
            if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString()))
            {
                view.SetMatrix(element.brailleRepresentation.matrix);
                brailleIOMediator.RenderDisplay();
                return;
            }
            if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString()))
            {
                Image img = captureScreen(element.properties.IdGenerated);
                if (img == null) { return; }
                view.SetZoom(element.brailleRepresentation.zoom);
                view.SetContrastThreshold(element.brailleRepresentation.contrast);
                view.SetBitmap(img);
                brailleIOMediator.RenderDisplay();
                return;
            }
            if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString()))
            {
                view.SetText(element.properties.valueFiltered);
                brailleIOMediator.RenderDisplay();
                return;
            }
        }

        /// <summary>
        /// Erstellt das UI auf der Stiftplatte
        /// </summary>
        public void generatedBrailleUi()
        {
            if (!initialized) { setActiveAdapter(); }
            if (grantTrees == null || grantTrees.getBrailleTree() == null) { return; }
            Object osm = strategyMgr.getSpecifiedTree().Copy(grantTrees.getBrailleTree());
            createViewsFromTree(osm);
            brailleIOMediator.RenderDisplay();
            String activeScreenName = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreen();
            if (activeScreenName != null)
            {
                strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen(activeScreenName);
            }
        }
        
        /// <summary>
        /// Erstellt einen neuen Screen, falls dieser noch nicht existiert
        /// </summary>
        /// <param name="screenAvtive">gibt den Namen des Screens an</param>
        private void createScreen(String screenName)
        {
            try
            {
                 object screen = brailleIOMediator.GetView(screenName);
                 if (screen == null)
                 {
                     brailleIOMediator.AddView(screenName, new BrailleIOScreen(screenName));
                     //der erste erstellte Screen wird sichtbar gemacht, alle anderen nicht
                     if (brailleIOMediator.Count() > 1)
                     {
                         BrailleIOScreen screenNew =  brailleIOMediator.GetView(screenName) as BrailleIOScreen;
                         screenNew.SetVisibility(false);
                     }
                 }
                // der screen existiert schon -> ok
            }
            catch
            {
                throw new Exception("Fehler in createScreen(String screenName) in BrailleDisplayStrategyBrailleIO");
            }
        }

        /// <summary>
        /// Ermittelt ein Bereich und das zugehörige anzuzeigende Bild
        /// </summary>
        /// <param param name="idGeneratedFilteredTreeNode">gibt die generierte Id des Knotens das Braille-Baum-Elements an</param>
        /// <returns>ein <code>Image</code> des Bildbereiches</returns>
        private Image captureScreen(String idGeneratedBrailleNode)
        {
            OsmConnector<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(idGeneratedBrailleNode) || r.FilteredTree.Equals(idGeneratedBrailleNode));
            if (osmRelationships == null) { return null; }
            OSMElement.OSMElement nodeFilteredTree = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationships.FilteredTree);
            if (nodeFilteredTree.Equals(new OSMElement.OSMElement())) { return null; }
            Image bmp;
           /* int h = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Height);
            int w = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Width);
            bmp = ScreenCapture.CaptureWindow(nodeFilteredTree.Data.properties.hWndFiltered, h, w, 0, 0, 0, 0);*/
            Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(nodeFilteredTree);
          //  Console.WriteLine("Braille -- Rect: x = {0}, y = {1}, höhe = {2}, breite= {3}", rect.X, rect.Y, rect.Height, rect.Width);
            if (!nodeFilteredTree.properties.hWndFiltered.Equals(IntPtr.Zero))
            {
                bmp = ScreenCapture.CaptureWindowPartAtScreenpos(nodeFilteredTree.properties.hWndFiltered, Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Height), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Width), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.X), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Y));
            }
            else
            {
                if(grantTrees.getFilteredTree() != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.getFilteredTree()) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.hWndFiltered.Equals(IntPtr.Zero))
                {
                    bmp = ScreenCapture.CaptureWindowPartAtScreenpos(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.getFilteredTree())).properties.hWndFiltered, Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Height), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Width), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.X), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Y));
                }
                else
                {
                    bmp = ScreenCapture.CaptureScreenPos(rect); // Wenn eine andere Anwendung die gefilterte Anwendung überdeckt, wird der falsche Screenshot erstellt
                }                
            }
            return bmp;
        }

        /// <summary>
        /// Geht durch alle Baumelemente und erstellt die einzelnen Views
        /// </summary>
        /// <param name="tree">gibt das Baum-Objekt der Oberflaeche an</param>
        private void createViewsFromTree(Object tree)
        {
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                //top => der Knoten mit der Angabe der ViewCategory
                if (!strategyMgr.getSpecifiedTree().IsTop(node))
                {
                    //Depth == 1 => Knoten mit den Screens
                    if (strategyMgr.getSpecifiedTree().Depth(node) ==1 && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.Equals(new BrailleRepresentation()) && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName.Equals(""))
                    {
                        createScreen(strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName);
                    }
                    else
                    {
                        if (!strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.Equals(new BrailleRepresentation()) && !strategyMgr.getSpecifiedTree().GetData(node).properties.Equals(new GeneralProperties()) && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Equals("") && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.isGroupChild)
                        {
                            createView(strategyMgr.getSpecifiedTree().GetData(node));
                        }
                    }
                }
            }
          
        }


        /// <summary>
        /// Erstellt aus einer <code>OSMElement.OSMElement</code> die entsprechende View
        /// </summary>
        /// <param name="brailleRepresentation">gibt die Darstellung des GUI-Objektes fuer die Stiftplatte an</param>
        private void createView(OSMElement.OSMElement osmElement)
        {
            /* je nach UI-Element sind für die Views verschiedene Eigenschaften wichtig
             * die Angabe des 'UI-Element'-Typs steht bei den Propertys in controlTypeFiltered
             */
            if (osmElement.properties.isControlElementFiltered == false || osmElement.brailleRepresentation.viewName == null || osmElement.brailleRepresentation.viewName.Equals("")) { return; }

            OSMElement.BrailleRepresentation brailleRepresentation = osmElement.brailleRepresentation;

            String uiElementType = osmElement.properties.controlTypeFiltered;
            if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                createViewMatrix(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement);
                return;
            }
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) || 
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) || 
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // für alle anderen muss bei BrailleIO "otherContent" zugewiesen werden
            {
                
                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);
                if (renderer == null)
                {
                    Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", osmElement.properties.controlTypeFiltered);
                    return;
                }

                createViewOtherContent(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement, renderer);
                return;
            }
            if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Image img = captureScreen(osmElement.properties.IdGenerated);
                if (img == null) { return; }
                createViewImage(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement, img);
                return;
            }
            if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement);
                return;
            }

            //im Zweifelsfall wird immer eine "Text-View" mit einem leeren Text erstellt
            createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement);
        }

        #region create Views       
        /// <summary>
        /// Erstellt eine View mit einem Text
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="filteredSubtree">gibt das zur View zugehörige OSM-Element an</param>
        private void createViewText(BrailleIOScreen screen, OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetText(brailleUiElement.text);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View mit einer Bool-Matrix
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="filteredSubtree">gibt das zur View zugehörige OSM-Element an</param>
        private void createViewMatrix(BrailleIOScreen screen, OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetMatrix(brailleUiElement.matrix);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View mit einem Bild
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="filteredSubtree">gibt das zur View zugehörige OSM-Element an</param>
        /// <param name="image">gibt das Bild an</param>
        private void createViewImage(BrailleIOScreen screen,  OSMElement.OSMElement osmElement, System.Drawing.Image image)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetBitmap(image);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetContrastThreshold(brailleUiElement.contrast);
            vr.SetZoom(brailleUiElement.zoom);
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View die keinen standard-Renderer verwendet
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="filteredSubtree">gibt das zur View zugehörige OSM-Element an</param>
        /// <param name="renderer">gibt den Renderer für diese View an</param>
        private void createViewOtherContent(BrailleIOScreen screen, OSMElement.OSMElement osmElement, IBrailleIOContentRenderer renderer)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetOtherContent(brailleUiElement, renderer);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }
        #endregion

        /// <summary>
        /// Ermittelt zu einem Punkt den zugehörigen View-Name
        /// </summary>
        /// <param name="x">gibt die horizontale Position des Punktes auf der Stifftplatte an</param>
        /// <param name="y">gibt die vertikale Position des Punktes auf der Stifftplatte an</param>
        /// <returns>falls eine passende View gefunden wurde dessen Name, dieser bezieht sich auf den aktuellen Screen; sonst <code>null</code></returns>
        public String getBrailleUiElementViewNameAtPoint(int x, int y, out int offsetX, out int offsetY)
        {
            BrailleIOViewRange viewAtPoint = brailleIOMediator.GetViewAtPosition(x, y);
            if(viewAtPoint == null){
                Console.WriteLine("zu dem Punkt wurde keine passende View gefunden.");
                offsetX = 0;
                offsetY = 0;
                return null;
            }
            offsetX = viewAtPoint.GetXOffset();
            offsetY = viewAtPoint.GetYOffset();
            return viewAtPoint.Name;
        }

        /// <summary>
        /// Ermittelt den zugehörigen Renderer für ein UI-Element
        /// </summary>
        /// <param name="guiElementType">gibt den Namen des UI-Elements an</param>
        /// <returns>der Renderer für das UI-Element oder null</returns>
        private static IBrailleIOContentRenderer getRenderer(String uiElementType)
        {
            switch (uiElementType)
            {
                case "Button":
                    return new BrailleIOButtonToMatrixRenderer();
                case "TextBox":
                    return new BrailleIOTextBoxToMatrixRenderer();
                case "DropDownMenuItem":
                    return new BrailleIODropDownMenuItemToMatrixRenderer();
                case "GroupElement":
                    return new BrailleIOGroupViewRangeToMatrixRenderer();
                case "ListItem":
                    return new BrailleIOListItemToMatrixRenderer();
                case "TabItem":
                    return new BrailleIOTabItemToMatrixRenderer();
            }
            return null;
        }

        /// <summary>
        /// Enum welches die verschiedenen 'ui-Elemente' enthält, für welches es Renderer in BrailleIO gibt
        /// </summary>
        private enum uiElementeTypesBrailleIoEnum { Matrix, Text, Screenshot, Button, DropDownMenuItem, TextBox, ListItem, GroupElement, TabItem }

        private struct uiElementsTypeStruct
        {
            public String uiElementType { get; set; }
            public int heightMin { get; set; }
            public int widthMin { get; set; }
        }

        /// <summary>
        /// Erstellt eine Liste mit möglichen Ui-Elementen (Renderen) und deren minimale Größe
        /// </summary>
        /// <returns></returns>
        private List<uiElementsTypeStruct> getUiElements()
        {
            List<uiElementsTypeStruct> uiElementList = new List<uiElementsTypeStruct>();
            uiElementsTypeStruct uiElement = new uiElementsTypeStruct();

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Button.ToString();
            uiElement.heightMin = 7; // Rahmen (2) + Höhe kleiner Buchstabe (3) + Freizeile (2)
            uiElement.widthMin = 6; // Rahmen (2) + Breite Buchstabe (2) + Freiraum (2)
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.DropDownMenuItem.ToString();
            uiElement.heightMin = 9; // Rahmen (4) + Höhe kleiner Buchstabe (3) + Freizeile (2)
            uiElement.widthMin = 9; // Rahmen (2) + Breite Buchstabe (2) + Freiraum (2) + Bubel (3)
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Matrix.ToString();
            uiElement.heightMin = 1;
            uiElement.widthMin = 1;
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Screenshot.ToString();
            uiElement.heightMin = 1; // macht das Sinn bei einem Screenshot?
            uiElement.widthMin = 1; // macht das Sinn bei einem Screenshot?
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Text.ToString();
            uiElement.heightMin = 3; // Höhe kleiner Buchstabe (3) 
            uiElement.widthMin = 2; // Breite Buchstabe (2) 
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.TextBox.ToString();
            uiElement.heightMin = 7; // Rahmen (2) + Höhe kleiner Buchstabe (3) + Freizeile (2)
            uiElement.widthMin = 6; // Rahmen (2) + Breite Buchstabe (2) + Freiraum (2)
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.GroupElement.ToString();
            uiElement.heightMin = 7; 
            uiElement.widthMin = 7;
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.ListItem.ToString();
            uiElement.heightMin = 6;
            uiElement.widthMin = 15;
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.TabItem.ToString();
            uiElement.heightMin = 20;
            uiElement.widthMin = 6;
            uiElementList.Add(uiElement);

            if (uiElementList.Count != Enum.GetNames(typeof( uiElementeTypesBrailleIoEnum)).Length)
            {
                throw new Exception("Achtung es wurden nicht gleichviele Elemente im Enum wie in der Liste alle Ui-Elemente angegeben!");
            }
            return uiElementList;
        }

        /// <summary>
        /// Gibt eine Liste mit allen für das gewählte Ausgabegerät möglichen Renderen zurück
        /// Achtung: alle neuerstellten Renderer müssen sowohl der Renderer-Liste (getUiElements) als auch dem -Enum (uiElementeTypesBrailleIoEnum) hinzugefügt werden
        /// </summary>
        /// <returns>Liste der BrailleIO-Renderer</returns>
        public List<String> getUiElementRenderer()
        {
            List<String> uiElementRenderer = new List<String>();
           /* foreach(uiElementeTypesBrailleIoEnum uiEnum in Enum.GetValues(typeof(uiElementeTypesBrailleIoEnum)))
            { 
                uiElementRenderer.Add(uiEnum.ToString());
            }*/

            foreach (uiElementsTypeStruct element in uiElementList)
            {
                //Angaben zur min. Größe prüfen
                Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
                if (activeDevice.height >= element.heightMin && activeDevice.width >= element.widthMin)
                {
                    uiElementRenderer.Add(element.uiElementType);
                }
            }
            return uiElementRenderer;
        }

        /// <summary>
        /// Gibt eine Liste allen mit möglichen Renderen zurück
        /// Achtung: alle neuerstellten Renderer müssen sowohl der Renderer-Liste (getUiElements) als auch dem -Enum (uiElementeTypesBrailleIoEnum) hinzugefügt werden
        /// </summary>
        /// <returns>Liste der BrailleIO-Renderer</returns>
        public List<String> getAllUiElementRenderer()
        {
            List<String> uiElementRenderer = new List<String>();
            foreach (uiElementsTypeStruct element in uiElementList)
            {
                uiElementRenderer.Add(element.uiElementType);
            }
            return uiElementRenderer;
        }

        /// <summary>
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element an, welches für die Braille-UI beispielhaft gerendert werden soll</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        public bool[,] getRendererExampleRepresentation(OSMElement.OSMElement osmElementFilteredNode)
        {
            if (brailleIOMediator == null)
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            if (osmElementFilteredNode.brailleRepresentation.viewName == null) { return new bool[0, 0]; }
            
            createScreen(osmElementFilteredNode.brailleRepresentation.screenName);
            createView(osmElementFilteredNode);
            BrailleIOViewRange tmpView = (brailleIOMediator.GetView(osmElementFilteredNode.brailleRepresentation.screenName) as BrailleIOScreen).GetViewRange(osmElementFilteredNode.brailleRepresentation.viewName);
            if (tmpView == null && !osmElementFilteredNode.properties.controlTypeFiltered.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString())) { return new bool[0, 0]; }
            #region screenshot
            if(osmElementFilteredNode.properties.controlTypeFiltered.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString())){
                //Screenshot muss extra erstellt werden
                // Image img = ScreenCapture.CaptureWindow(strategyMgr.getSpecifiedOperationSystem().deliverDesktopHWND(), Convert.ToInt32(osmElementFilteredNode.properties.boundingRectangleFiltered.Height *10), Convert.ToInt32(osmElementFilteredNode.properties.boundingRectangleFiltered.Width *10), 0, 0, 0, 0);
                Image img;
                if (osmElementFilteredNode.properties.IdGenerated == null || osmElementFilteredNode.properties.IdGenerated.Equals("_tmp_id_"))
                {
                    img = ScreenCapture.CaptureWindow(strategyMgr.getSpecifiedOperationSystem().deliverDesktopHWND(), Convert.ToInt32(osmElementFilteredNode.properties.boundingRectangleFiltered.Height * 10), Convert.ToInt32(osmElementFilteredNode.properties.boundingRectangleFiltered.Width * 10), 0, 0, 0, 0);

                }
                else
                {
                    img = captureScreen(osmElementFilteredNode.properties.IdGenerated);
                }
                if (img == null) { return new bool[0, 0]; }
                //UiElement brailleUiElement = convertToBrailleIOUiElement(osmElementFilteredNode);
                createViewImage(brailleIOMediator.GetView(osmElementFilteredNode.brailleRepresentation.screenName) as BrailleIOScreen, osmElementFilteredNode, img);
                tmpView = (brailleIOMediator.GetView(osmElementFilteredNode.brailleRepresentation.screenName) as BrailleIOScreen).GetViewRange(osmElementFilteredNode.brailleRepresentation.viewName);
            }
            #endregion
            if (tmpView.IsText())
            {
                tmpView.SetText(tmpView.GetText() == null ? "" : tmpView.GetText());
            }
            String uiElementType = osmElementFilteredNode.properties.controlTypeFiltered;
            bool[,] matrix;
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // für alle anderen muss bei BrailleIO "otherContent" zugewiesen werden
            {
                if (osmElementFilteredNode.brailleRepresentation.viewName.Contains("NavigationBarScreens_"))
                {
                    setVisibleScreen(osmElementFilteredNode.brailleRepresentation.screenName);
                }
                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetOtherContent());
                matrix = BrailleIOBorderRenderer.renderMatrix(tmpView, matrix);
                //setVisibleScreen(osmElementFilteredNode.brailleRepresentation.screenName);
            }
            else if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                if (tmpView.GetText() != null && tmpView.GetText() != "")
                {
                    matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetText());
                    BrailleIOViewMatixRenderer vmr = new BrailleIO.Renderer.BrailleIOViewMatixRenderer();
                    matrix = vmr.RenderMatrix(tmpView, matrix);
                    matrix = BrailleIOBorderRenderer.renderMatrix(tmpView, matrix);
                }
                else
                {
                    matrix = new bool[0, 0];
                }
            }
            else if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetMatrix());
                matrix = BrailleIOBorderRenderer.renderMatrix(tmpView, matrix);
            }
            else if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetImage());
                matrix = BrailleIOBorderRenderer.renderMatrix(tmpView, matrix);
            }
            else { throw new Exception("Kein passenden Renderer gefunden!"); }
            
            //löschen den temporär erstellten Screens inkl aller temporär erstellten Views
            brailleIOMediator.RemoveView(osmElementFilteredNode.brailleRepresentation.screenName);
            return matrix;
        }

        /// <summary>
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// Es wird eine Standardgröße genutzt
        /// </summary>
        /// <param name="uiElementType">gibt den Namen des Gui-Elements an</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        public bool[,] getRendererExampleRepresentation(String uiElementType)
        {
            if (uiElementType.Equals("GroupElement")) { return new bool[1, 1]; }
            return getRendererExampleRepresentation(getExampleOsmElement(uiElementType));
        }

        #region beispielOsmElemente
        private OSMElement.OSMElement getExampleOsmElement(String uiElementType)
        {
            #region gemeinsame Eigenschaften
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            GeneralProperties properties = new GeneralProperties();
            BrailleRepresentation brailleR = new BrailleRepresentation();
            properties.controlTypeFiltered = uiElementType;
            brailleR.screenName = "_tmp_screen_name_";
            brailleR.viewName = "_tmp_view_name_";
            brailleR.isVisible = true;
            Rect rect = new Rect();
            properties.isEnabledFiltered = true;
            properties.IdGenerated = "_tmp_id_";
            properties.valueFiltered = "Beispiel";
            #endregion
            #region unterschiedliche Eigenschaften

            if (uiElementeTypesBrailleIoEnum.Button.ToString().Equals(uiElementType))
            {
               rect  = new Rect(0, 0, 30, 8);               
            }
            if (uiElementeTypesBrailleIoEnum.DropDownMenuItem.ToString().Equals(uiElementType))
            {
                rect = new Rect(0,0,25,10);
                DropDownMenuItem dropDownMenu = new DropDownMenuItem();
                dropDownMenu.hasChild = true;
                dropDownMenu.hasNext = true;
                dropDownMenu.hasPrevious = false;
                dropDownMenu.isChild = false;
                dropDownMenu.isOpen = true;
                dropDownMenu.isVertical = true;
                brailleR.uiElementSpecialContent = dropDownMenu;
            }
            if (uiElementeTypesBrailleIoEnum.Matrix.ToString().Equals(uiElementType))
            {
                rect = new Rect(0, 0, 3, 7);
                brailleR.matrix = new bool[,] { 
                    {false, false, true},
                    {true, false, false},
                    {false, true, false},
                    {false, false, false},
                    {true, false, false},
                    {false, false, false},
                    {false, true, false},
                };
            }
            if (uiElementeTypesBrailleIoEnum.Screenshot.ToString().Equals(uiElementType))
            {
                rect = new Rect(0, 0, 20, 10);
                brailleR.zoom = 1;
                brailleR.contrast = 120;
            }
            if (uiElementeTypesBrailleIoEnum.Text.ToString().Equals(uiElementType))
            {
                rect = new Rect(0, 0, 25, 5);
            }
            if (uiElementeTypesBrailleIoEnum.TextBox.ToString().Equals(uiElementType))
            {
                properties.valueFiltered = "Beispiel Beispieltext Beispieltext";
                rect = new Rect(0, 0, 25, 10);
                brailleR.showScrollbar = true;
            }
            if (uiElementeTypesBrailleIoEnum.ListItem.ToString().Equals(uiElementType))
            {
                properties.valueFiltered = "Item 1";
                rect = new Rect(0, 0, 30, 7);
                OSMElement.UiElements.ListMenuItem listMenuItem = new ListMenuItem();
                listMenuItem.hasNext = true;
                listMenuItem.isMultipleSelection = true;
                properties.isToggleStateOn = false;
                brailleR.uiElementSpecialContent = listMenuItem;
            }
            if (uiElementeTypesBrailleIoEnum.TabItem.ToString().Equals(uiElementType))
            {
                properties.valueFiltered = "TabItem";
                rect = new Rect(0, 0, 8, 8);
                OSMElement.UiElements.TabItem tabview = new TabItem();
                tabview.orientation = OSMElement.UiElements.Orientation.Left;
                brailleR.uiElementSpecialContent = tabview;
            }
            /*if (uiElementeTypesBrailleIoEnum.GroupElement.ToString().Equals(uiElementType))
            {
                properties.valueFiltered = "Beispiel Beispieltext Beispieltext";//TODO
                rect = new Rect(0, 0, 25, 10);

                OSMElement.OSMElement childOsm = new OSMElement.OSMElement();
                BrailleRepresentation childBraille = new BrailleRepresentation();
                GroupelementsOfSameType group = new GroupelementsOfSameType();
                group.childBoundingRectangle = new Rect(rect.X + 2, rect.Y + 2, rect.Width - 4, rect.Height - 4);
                group.renderer = "Button";
                childBraille.isGroupChild = true; 
                childBraille.groupelementsOfSameType = group;
                GeneralProperties childProp = new GeneralProperties();
                
                childOsm.properties = childProp;
                childOsm.brailleRepresentation = childBraille;
                
                brailleR.groupelementsOfSameType = group;
                
            }*/
            #endregion
            properties.boundingRectangleFiltered = rect;
            osmElement.brailleRepresentation = brailleR;
            osmElement.properties = properties;
            return osmElement;
        }

        #endregion

        #region Konvertieren von Elementen
        /// <summary>
        /// Wandelt <code>System.Windows.Forms.Padding</code> in <code>BrailleIO.Structs.BoxModel</code> um
        /// </summary>
        /// <param name="padding">gibt ein <code>Padding</code>-Objekt an</param>
        /// <returns>das übergebene Objekt als <code>BoxModel</code></returns>
        private BoxModel paddingToBoxModel(Padding padding)
        {
            BoxModel boxModel = new BoxModel();
            boxModel.Bottom = (uint)padding.Bottom;
            boxModel.Left = (uint)padding.Left;
            boxModel.Right = (uint)padding.Right;
            boxModel.Top = (uint)padding.Top;
            return boxModel;
        }

        /// <summary>
        /// konvertiert das DropDownMenuItem aus <code>OSMElement.UiElements.DropDownMenuItem</code> zu <code>BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem</code>
        /// </summary>
        /// <param name="osmMenu"></param>
        /// <returns></returns>
        private BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem convertDropDownMenu(OSMElement.UiElements.DropDownMenuItem osmMenu)
        {
            BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem brailleIOMenu = new BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem();
            brailleIOMenu.hasChild = osmMenu.hasChild;
            brailleIOMenu.hasNext = osmMenu.hasNext;
            brailleIOMenu.hasPrevious = osmMenu.hasPrevious;
            brailleIOMenu.isChild = osmMenu.isChild;
            brailleIOMenu.isOpen = osmMenu.isOpen;
            brailleIOMenu.isVertical = osmMenu.isVertical;
            return brailleIOMenu;
        }

        private BrailleIOGuiElementRenderer.UiElements.ListMenuItem convertToListItem(OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElements.ListMenuItem brailleListItem = new BrailleIOGuiElementRenderer.UiElements.ListMenuItem();
            if(osmElement.Equals(new OSMElement.OSMElement())){return  brailleListItem;}
            if (!osmElement.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                return brailleListItem;
            }
            OSMElement.UiElements.ListMenuItem listItem = (OSMElement.UiElements.ListMenuItem)osmElement.brailleRepresentation.uiElementSpecialContent;
            brailleListItem.hasNext = listItem.hasNext;
            brailleListItem.isMultipleSelection = listItem.isMultipleSelection;
            brailleListItem.isSelected = osmElement.properties.isToggleStateOn != null ? (bool)osmElement.properties.isToggleStateOn : false;
            return brailleListItem;
        }

        private BrailleIOGuiElementRenderer.UiElements.TabItem convertToTabView(OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElements.TabItem tabViewBraille = new BrailleIOGuiElementRenderer.UiElements.TabItem();
            if (osmElement.Equals(new OSMElement.OSMElement())) { return tabViewBraille; }
            if (!osmElement.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.TabItem)))
            {
                return tabViewBraille;
            }
            
            OSMElement.UiElements.TabItem tabOsm = (OSMElement.UiElements.TabItem)osmElement.brailleRepresentation.uiElementSpecialContent;
           // Debug.WriteLine("4 - Ausrichtung = " + tabOsm.orientation + " (" + osmElement.brailleRepresentation.viewName + ": " + osmElement.properties.valueFiltered + ")");
            tabViewBraille.orientation = tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Bottom.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Bottom :
                (tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Top.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Top : (tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Right.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Right : BrailleIOGuiElementRenderer.UiElements.Orientation.Left));
            return tabViewBraille;
        }

        /// <summary>
        /// Konvertiert den <code>uiElementSpecialContent</code> des <code>OSMElement</code>s zu einem entsprechenden Objekt, welches BrailleIO bekannt ist
        /// </summary>
        /// <param name="filteredSubtree">gibt das filteredSubtree an</param>
        /// <returns>ein Objekt mit dem konvertierten Inhalt</returns>
        private object convertUiElementSpecialContent(OSMElement.OSMElement osmElement)
        {
            object brailleIOElement = new object();
            if (osmElement.Equals(new OSMElement.OSMElement()) || osmElement.brailleRepresentation.uiElementSpecialContent == null) { return brailleIOElement; }
            Type osmElementspecialcontentType = osmElement.brailleRepresentation.uiElementSpecialContent.GetType();
            if (osmElementspecialcontentType.Equals(typeof(OSMElement.UiElements.DropDownMenuItem)))
            {
                brailleIOElement = convertDropDownMenu((OSMElement.UiElements.DropDownMenuItem)osmElement.brailleRepresentation.uiElementSpecialContent);
            }
            if (osmElementspecialcontentType.Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                brailleIOElement = convertToListItem(osmElement);
            }
            if (osmElementspecialcontentType.Equals(typeof(OSMElement.UiElements.TabItem)))
            {                
                brailleIOElement = convertToTabView(osmElement);
            }
            return brailleIOElement;
        }

        /// <summary>
        /// Konvertiert Inhalte des <code>OSMElement</code>s zu einer entsprechendend Darstellung vom <code>BrailleIOGuiElementRenderer.UiElement</code> um das Element einem Renderer zu übergeben
        /// </summary>
        /// <param name="filteredSubtree">gibt das <code>OSMElement</code> an</param>
        /// <returns>ein <code>BrailleIOGuiElementRenderer.UiElement</code></returns>
        private BrailleIOGuiElementRenderer.UiElement convertToBrailleIOUiElement(OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleIOElement = new BrailleIOGuiElementRenderer.UiElement();
            brailleIOElement.contrast = osmElement.brailleRepresentation.contrast;
            brailleIOElement.isDisabled = osmElement.properties.isEnabledFiltered.HasValue ? !(Boolean)osmElement.properties.isEnabledFiltered : false;
            
            brailleIOElement.isVisible = osmElement.brailleRepresentation.isVisible;
            brailleIOElement.matrix = osmElement.brailleRepresentation.matrix;
            brailleIOElement.screenName = osmElement.brailleRepresentation.screenName;
            brailleIOElement.showScrollbar = osmElement.brailleRepresentation.showScrollbar;
            brailleIOElement.text = osmElement.properties.valueFiltered;
            brailleIOElement.uiElementSpecialContent = convertUiElementSpecialContent(osmElement);
            brailleIOElement.viewName = osmElement.brailleRepresentation.viewName;
            brailleIOElement.zoom = osmElement.brailleRepresentation.zoom;
            if (grantTrees.getBrailleTree() != null)
            {
                List<Object> nodeList = treeOperation.searchNodes.getAssociatedNodeList(osmElement.properties.IdGenerated, grantTrees.getBrailleTree()); //TODO. gleich übergeben?
                if (nodeList != null && nodeList.Count == 1 && strategyMgr.getSpecifiedTree().HasChild(nodeList[0]))
                //if (osmElement.brailleRepresentation.groupelementsOfDiffrentTypes  != null)
                {
                    List<BrailleIOGuiElementRenderer.Groupelements> childList = new List<BrailleIOGuiElementRenderer.Groupelements>();

                   // brailleIOElement.showScrollbar = true;

                    childList = iteratedChildreen(nodeList[0]);
                    brailleIOElement.child = childList;
                }
            }
            return brailleIOElement;
        }

        private BrailleIOGuiElementRenderer.Groupelements osmSubelementToGroupelement(OSMElement.OSMElement brailleTreeNode)
        {
            BrailleIOGuiElementRenderer.Groupelements groupelement = new BrailleIOGuiElementRenderer.Groupelements();
            groupelement.childBoundingRectangle = brailleTreeNode.properties.boundingRectangleFiltered;
            if (brailleTreeNode.brailleRepresentation.groupelementsOfSameType.renderer != null)
            {
                groupelement.renderer = getRenderer(brailleTreeNode.brailleRepresentation.groupelementsOfSameType.renderer);
            }
            else
            {
                groupelement.renderer = getRenderer(brailleTreeNode.properties.controlTypeFiltered);
            }

            UiElement childUi = new UiElement();
            childUi.text = brailleTreeNode.properties.valueFiltered;
            childUi.viewName = brailleTreeNode.brailleRepresentation.viewName;
            childUi.isVisible = true;
            childUi.screenName = brailleTreeNode.brailleRepresentation.screenName;
            childUi.isDisabled = brailleTreeNode.properties.isEnabledFiltered == null ? false : !((bool)brailleTreeNode.properties.isEnabledFiltered);
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElement.UiElements.DropDownMenuItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertDropDownMenu((OSMElement.UiElements.DropDownMenuItem)brailleTreeNode.brailleRepresentation.uiElementSpecialContent);
            }
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElement.UiElements.ListMenuItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertToListItem(brailleTreeNode);
            }
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElement.UiElements.TabItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertToTabView(brailleTreeNode);
            }
            groupelement.childUiElement = childUi;
            
            return groupelement;
        }

        /// <summary>
        /// Iterriert über die Kinder eines Knotens und erstellt für jedes Kind ein Ui-Element
        /// </summary>
        /// <param name="parentBrailleTreeNode">gibt den Elternknoten an</param>
        /// <param name="templateObject">gibt das zu nutzende Template an</param>
        private List<BrailleIOGuiElementRenderer.Groupelements> iteratedChildreen(Object parentBrailleTreeNode)
        {
            List<BrailleIOGuiElementRenderer.Groupelements> childList = new List<BrailleIOGuiElementRenderer.Groupelements>();
            if (strategyMgr.getSpecifiedTree().HasChild(parentBrailleTreeNode))
            {
                parentBrailleTreeNode = strategyMgr.getSpecifiedTree().Child(parentBrailleTreeNode);
                childList.Add(osmSubelementToGroupelement(strategyMgr.getSpecifiedTree().GetData(parentBrailleTreeNode)));
            }
            else
            {
                return null;
            }
            while (strategyMgr.getSpecifiedTree().HasNext(parentBrailleTreeNode))
            {
                parentBrailleTreeNode = strategyMgr.getSpecifiedTree().Next(parentBrailleTreeNode);
                childList.Add(osmSubelementToGroupelement(strategyMgr.getSpecifiedTree().GetData(parentBrailleTreeNode)));
            }
            return childList;
        }

        /// <summary>
        /// Ermittelt anhand des genutzten Typs der DisplayStrategy welcher Adapter verwendet werden muss
        /// </summary>
        /// <param name="displayStrategyType">gibt die genutzte DisplayStrategy an</param>
        /// <returns>der Adapter für die Ausgabe</returns>
        private AbstractBrailleIOAdapterBase displayStrategyClassToBrailleIoAdapterClass(Type displayStrategyType)
        {
            Type brailleAdapterType = null;

            XElement xmlDoc = XElement.Load(@"displayStrategyType.xml");
            IEnumerable<XElement> elements = xmlDoc.Elements("Strategy");
            foreach (XElement strategy in elements)
            {
                if (strategy.Element("DeviceClassTypeFullName").Value.Equals(displayStrategyType.FullName) && strategy.Element("DeviceClassTypeDllName").Value.Equals(displayStrategyType.Namespace))
                {
                    brailleAdapterType = Type.GetType(strategy.Element("AdaptClassTypeFullName").Value + ", " + strategy.Element("AdapterClassTypeDllName").Value);
                    break;
                }
            }
            if (brailleAdapterType != null) { return (AbstractBrailleIOAdapterBase)Activator.CreateInstance(brailleAdapterType, brailleIOMediator.AdapterManager); }
            return null;
        }
        #endregion


        public bool isInitialized()
        {
            return initialized;
        }

        /// <summary>
        /// Setzt den angegeben Screen auf Visible und den aktuell sichtbaren Screen auf invisible
        /// </summary>
        /// <param name="screenName">gibt den Namen des Screens an, der sichtbar sein soll</param>
        public void setVisibleScreen(String screenName)
        {
           /* List<Object> allScreens = brailleIOMediator.GetViews();
            foreach (BrailleIOScreen screen in allScreens)
            {
                if (screen.Name.Equals(screenName))
                {
                    screen.SetVisibility(true);
                }
                else
                {
                    screen.SetVisibility(false);
                }
            }*/
            object screens = brailleIOMediator.GetActiveViews();
            if (screens == null || !screens.GetType().Equals(typeof(List<AbstractViewBoxModelBase>))) { return; }
            List<AbstractViewBoxModelBase> screenObjectList = (List<AbstractViewBoxModelBase>)screens;
            if (screenObjectList.Count == 1)
            {
                BrailleIOScreen screenAvtive = screenObjectList[0] as BrailleIOScreen;
                if (screenAvtive != null)
                {
                    screenAvtive.SetVisibility(false);
                    
                    BrailleIOScreen screenAvtiveNew = brailleIOMediator.GetView(screenName) as BrailleIOScreen;
                    if (screenAvtiveNew != null)
                    {
                        screenAvtiveNew.SetVisibility(true);
                        treeOperation.updateNodes.setPropertyInNavigationbarForScreen(screenName, true);
                        
                    }
                    else
                    {
                        screenAvtive.SetVisibility(true);
                        if (treeOperation.searchNodes.getPosibleScreenNames().Contains(screenName))
                        {
                            Debug.WriteLine("TODO");
                        }
                    }
                    brailleIOMediator.RenderDisplay();
                }
            }
        }

        /// <summary>
        /// Gibt den Namen des gerade sichtbaren Screens zurück;
        /// es sollte nur immer ein Screen sichtbar sein
        /// </summary>
        /// <returns>den Namen des Sichtbaren Screens oder <c>null</c></returns>
        public String getVisibleScreen()
        {
            object screens = brailleIOMediator.GetActiveViews();
            if (screens == null || !screens.GetType().Equals(typeof(List<AbstractViewBoxModelBase>))) { return null; }
            List<AbstractViewBoxModelBase> screenObjectList = (List<AbstractViewBoxModelBase>)screens;
            if (screenObjectList.Count == 1)
            {
                BrailleIOScreen screenAvtive = screenObjectList[0] as BrailleIOScreen;
                if (screenAvtive != null)
                {
                      return screenAvtive.Name;
                }
            }
           /* List<Object> allScreens = brailleIOMediator.GetViews();
            foreach (BrailleIOScreen screen in allScreens)
            {
                if (screen.IsVisible())
                {
                    return screen.Name;
                }
            }*/
            return null;
        }

        /// <summary>
        /// Verschiebt eine Gruppen von Views horizontal um den angegebenen Werte
        /// </summary>
        /// <param name="viewNode">gibt den Knoten der zu verschiebenen View an</param>
        /// <param name="steps">positiv entspricht Verschiebung nach rechts</param>
        public void moveViewRangHoricontal(Object viewNode, int steps)
        {
            //es muss aufgepasst werden, dass
            BrailleIOScreen screen = brailleIOMediator.GetView(strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.screenName) as BrailleIOScreen;

            BrailleIOViewRange viewRange;
            if (strategyMgr.getSpecifiedTree().GetData(viewNode).properties.controlTypeFiltered.Equals("TextBox"))
            {
                viewRange = screen.GetViewRange("_TextBoxText_" + strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.viewName) as BrailleIOViewRange;
            }
            else
            {
                viewRange = screen.GetViewRange(strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.viewName) as BrailleIOViewRange;
            }
            //groupView.SetXOffset(mx + groupView.GetXOffset()); 
            if (viewRange != null)
            {
                viewRange.MoveHorizontal(steps);
                brailleIOMediator.RenderDisplay();
            }
        }

        /// <summary>
        /// Verschiebt eine Gruppen von Views vertical um den angegebenen Werte
        /// </summary>
        /// <param name="viewNode">gibt den Knoten der zu verschiebenen View an</param>
        /// <param name="steps">positiv entspricht Verschiebung nach unten</param>
        public void moveViewRangVertical(Object viewNode, int steps)
        {
            //es muss aufgepasst werden, dass
            BrailleIOScreen screen = brailleIOMediator.GetView(strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.screenName) as BrailleIOScreen;
            BrailleIOViewRange viewRange;
            if (strategyMgr.getSpecifiedTree().GetData(viewNode).properties.controlTypeFiltered.Equals("TextBox"))
            {
                viewRange = screen.GetViewRange("_TextBoxText_" + strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.viewName) as BrailleIOViewRange;
            }
            else
            {
                viewRange = screen.GetViewRange(strategyMgr.getSpecifiedTree().GetData(viewNode).brailleRepresentation.viewName) as BrailleIOViewRange;
            }
            //groupView.SetXOffset(mx + groupView.GetXOffset()); 
            viewRange.MoveVertical(steps);
            brailleIOMediator.RenderDisplay();
        }

        public TactileNodeInfos getTactileNodeInfos(object brailleNode)
        {
            TactileNodeInfos nodeInfos = new TactileNodeInfos();
            OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(brailleNode);
            object screen = brailleIOMediator.GetView(data.brailleRepresentation.screenName);
            if(screen is BrailleIOScreen)
            {
                object view = (screen as BrailleIOScreen).GetViewRange(data.brailleRepresentation.viewName);
                if (view is BrailleIOViewRange)
                {
                    BrailleIOViewRange viewRange = view as BrailleIOViewRange;
                    nodeInfos.contentHeight = viewRange.ContentHeight;
                    nodeInfos.contentWidth = viewRange.ContentWidth;
                    nodeInfos.offsetX = viewRange.OffsetPosition.X;
                    nodeInfos.offsetY = viewRange.OffsetPosition.Y;
                }
            }

            return nodeInfos;
        }
    }
}
