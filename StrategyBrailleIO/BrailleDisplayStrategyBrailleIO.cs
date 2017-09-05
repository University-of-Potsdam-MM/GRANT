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
using OSMElements;
using OSMElements.UiElements;
using BrailleIOGuiElementRenderer;
using BrailleIO.Interface;
using GRANTManager.TreeOperations;
using BrailleIO.Renderer;

namespace StrategyBrailleIO
{
    public class BrailleDisplayStrategyBrailleIO : IBrailleDisplayStrategy
    {
        private IBrailleIOShowOffMonitor monitor;
        private BrailleIOMediator brailleIOMediator { get; set; }
        private Boolean initialized = false;
        private List<uiElementsTypeStruct> uiElementList;
        private AbstractBrailleIOAdapterBase showOffAdapter;
        //GestureRecognizer showOffGestureRecognizer;

        //GestureRecognizer brailleDisRecognizer; //TODO


        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public void setTreeOperation(TreeOperation treeOperation) { uiElementList = getUiElements(); this.treeOperation = treeOperation; }
        public BrailleDisplayStrategyBrailleIO() { uiElementList = getUiElements();}

        public Type getActiveAdapter()
        {
            if(brailleIOMediator != null && brailleIOMediator.AdapterManager != null && brailleIOMediator.AdapterManager.ActiveAdapter != null)
            {
                return BrailleIoAdapterClassTodisplayStrategyClass( brailleIOMediator.AdapterManager.ActiveAdapter.GetType());
            }else { return null; }
        }

        /// <summary>
        /// Creates a simulator if none exisis
        /// </summary>
        private void initializedSimulator()
        {
            if (brailleIOMediator == null )
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            if (brailleIOMediator != null)
            {
                // if the current Adapter manager holds a debug adpter, use it
                foreach (IBrailleIOAdapter adapter in brailleIOMediator.AdapterManager.GetAdapters())
                {
                    if (adapter is BrailleIOAdapter_ShowOff)
                    {
                        //TODO: test whether the simulator is still running
                        adapter.Connect();
                        showOffAdapter = adapter as AbstractBrailleIOAdapterBase;
                        break;
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

        /// <summary>
        /// Removes the active adapter
        /// </summary>
        public void removeActiveAdapter()
        {
            removeAllViews();
            if (brailleIOMediator == null || brailleIOMediator.AdapterManager == null) { return; }
            foreach (IBrailleIOAdapter adapter in brailleIOMediator.AdapterManager.GetAdapters())
            {
                /*if (adapter != null)
                {
                    adapter.Disconnect();
                }*/
                if((brailleIOMediator.AdapterManager.ActiveAdapter is BrailleIOAdapter_ShowOff) || (brailleIOMediator.AdapterManager.ActiveAdapter is BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet_MVBD))
                {
                    adapter.Disconnect();
                }
                if (!(brailleIOMediator.AdapterManager.ActiveAdapter is BrailleIOAdapter_ShowOff))
                {
                    brailleIOMediator.AdapterManager.RemoveAdapter(adapter);
                }
            }
        }

        /// <summary>
        /// Removes all views
        /// </summary>
        public void removeAllViews()
        {
            if (brailleIOMediator == null) { return; }
            foreach (AbstractViewBoxModelBase view in brailleIOMediator.GetViews())
            {
                brailleIOMediator.RemoveView(view.Name);
            }
        }

        /// <summary>
        /// Initialized a braille display.
        /// </summary>
        public void setActiveAdapter()
        {
            if (brailleIOMediator == null)
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            initialized = true;
            Type activeDeviceType = Type.GetType(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeFullName + ", " + strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeNamespace);
            // If the simulartor will be used it will initilaized seperatly
            if (activeDeviceType.Equals(typeof(DisplayStrategyBrailleIoSimulator)))
            {
                initializedSimulator();
                return;
            }
            if (brailleIOMediator != null && brailleIOMediator.AdapterManager != null)
            {
                
                AbstractBrailleIOAdapterBase brailleAdapter = displayStrategyClassToBrailleIoAdapterClass(activeDeviceType);
                brailleIOMediator.AdapterManager.ActiveAdapter = brailleAdapter;
            }
            return;
        }

        /// <summary>
        /// Updated the content of a specific element
        /// </summary>
        /// <param name="element">element which content should be updated</param>
        public void updateViewContent(ref OSMElements.OSMElement element)
        {
            IBrailleIOAdapterManager adapter =  brailleIOMediator.AdapterManager;

            BrailleIOScreen screen = brailleIOMediator.GetView(element.brailleRepresentation.screenName) as BrailleIOScreen;
            if (screen == null)
            {
                throw new Exception("This screen dosn't exist!");
            }
            BrailleIOViewRange view = screen.GetViewRange(element.brailleRepresentation.viewName) as BrailleIOViewRange;
            String uiElementType = element.properties.controlTypeFiltered;
            if (view == null)
            {
                createScreen(element.brailleRepresentation.screenName);
                createView(element);
                view = screen.GetViewRange(element.brailleRepresentation.viewName) as BrailleIOViewRange;
            }
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // for all others it should be used "otherContent"

            {
                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);              
                if (renderer == null)
                {
                    Console.WriteLine("It dosn't exist a renderer for this UI element ({0}).", element.properties.controlTypeFiltered);
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
        /// generated the tactile user interface of a braille device
        /// </summary>
        public void generatedBrailleUi()
        {
            if (!initialized) { setActiveAdapter(); }
            if (grantTrees == null || grantTrees.brailleTree == null) { return; }
            Object osm = strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree);
            createViewsFromTree(osm);
            brailleIOMediator.RenderDisplay();
            String activeScreenName = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreen();
            if (activeScreenName != null)
            {
                strategyMgr.getSpecifiedBrailleDisplay().setVisibleScreen(activeScreenName);
            }
        }
        
        /// <summary>
        /// Creates a new screen if it dosn't exist
        /// </summary>
        /// <param name="screenAvtive">name of the screen</param>
        private void createScreen(String screenName)
        {
            try
            {
                 object screen = brailleIOMediator.GetView(screenName);
                 if (screen == null)
                 {
                     brailleIOMediator.AddView(screenName, new BrailleIOScreen(screenName));
                     // the first created screen will be visible all other are invisible
                     if (brailleIOMediator.Count() > 1)
                     {
                         BrailleIOScreen screenNew =  brailleIOMediator.GetView(screenName) as BrailleIOScreen;
                         screenNew.SetVisibility(false);
                     }
                 }
                // the screen exist -> ok
            }
            catch
            {
                throw new Exception("Exception in createScreen(String screenName) in BrailleDisplayStrategyBrailleIO");
            }
        }

        /// <summary>
        /// Seeks to a node the rectangle of the shown image and this image
        /// </summary>
        /// <param param name="idGeneratedFilteredTreeNode">the id of a node in the braille (output) tree</param>
        /// <returns>an image of the UI element</returns>
        private Image captureScreen(String idGeneratedBrailleNode)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(idGeneratedBrailleNode);
           if (connectedIdFilteredTree == null) { return null; }
            OSMElements.OSMElement nodeFilteredTree = treeOperation.searchNodes.getFilteredTreeOsmElementById(connectedIdFilteredTree);
            if (nodeFilteredTree.Equals(new OSMElements.OSMElement())) { return null; }
            Image bmp;
           /* int h = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Height);
            int w = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Width);
            bmp = ScreenCapture.CaptureWindow(nodeFilteredTree.Data.properties.hWndFiltered, h, w, 0, 0, 0, 0);*/
            Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(nodeFilteredTree);
            if (!nodeFilteredTree.properties.hWndFiltered.Equals(IntPtr.Zero))
            {
                bmp = ScreenCapture.CaptureWindowPartAtScreenpos(nodeFilteredTree.properties.hWndFiltered, Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Height), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Width), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.X), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Y));
            }
            else
            {
                if(grantTrees.filteredTree != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) && !strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.hWndFiltered.Equals(IntPtr.Zero))
                {
                    bmp = ScreenCapture.CaptureWindowPartAtScreenpos(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.hWndFiltered, Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Height), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Width), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.X), Convert.ToInt32(nodeFilteredTree.properties.boundingRectangleFiltered.Y));
                }
                else
                {
                    bmp = ScreenCapture.CaptureScreenPos(rect); // If an other application overlap this application, the screenshot shows a part of both applications
                }                
            }
            return bmp;
        }

        /// <summary>
        /// Iterates all the tree elements and creates the views
        /// </summary>
        /// <param name="tree">a braille tree object</param>
        private void createViewsFromTree(Object tree)
        {
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                //top => node with specification of the name of the typeOfView
                if (!strategyMgr.getSpecifiedTree().IsTop(node))
                {
                    //Depth == 1 => node with specification of the screen names
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
        /// Creates an associated view from an OSM element
        /// </summary>
        /// <param name="osmElement">an OSM element</param>
        private void createView(OSMElements.OSMElement osmElement)
        {
            /* depending on the UI element and the renderer used, different properties are importent
             * die Angabe des 'UI-Element'-Typs steht bei den Propertys in controlTypeFiltered
             */
            if (osmElement.brailleRepresentation.viewName == null || osmElement.brailleRepresentation.viewName.Equals("")) { return; }

            OSMElements.BrailleRepresentation brailleRepresentation = osmElement.brailleRepresentation;

            String uiElementType = osmElement.properties.controlTypeFiltered;
            if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                createViewMatrix(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement);
                return;
            }
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) || 
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) || 
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // for all others it should be used "otherContent"
            {
                
                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);
                if (renderer == null)
                {
                    Console.WriteLine("It dosn't exist a renderer for this UI element ({0})", osmElement.properties.controlTypeFiltered);
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

            // By defaul an empty text view will be used
            createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, osmElement);
        }

        #region creates views       
        /// <summary>
        /// Creates a text view
        /// </summary>
        /// <param name="screen">name of the <code>BrailleIOScreen</code> on which the view souhld be shown</param>
        /// <param name="osmElement">OSM element for this view </param>
        private void createViewText(BrailleIOScreen screen, OSMElements.OSMElement osmElement)
        {
            if(screen == null) { return; }
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetText(brailleUiElement.text);
            vr.ShowScrollbars = brailleUiElement.isScrollbarShow;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Creates a view with a Boolean matrix
        /// </summary>
        /// <param name="screen">name of the <code>BrailleIOScreen</code> on which the view souhld be shown</param>
        /// <param name="osmElement">OSM element for this view </param>
        private void createViewMatrix(BrailleIOScreen screen, OSMElements.OSMElement osmElement)
        {
            if (screen == null) { return; }
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetMatrix(brailleUiElement.matrix);
            vr.ShowScrollbars = brailleUiElement.isScrollbarShow;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Creates a view with a image
        /// </summary>
        /// <param name="screen">name of the <code>BrailleIOScreen</code> on which the view souhld be shown </param>
        /// <param name="osmElement">OSM element for this view </param>
        /// <param name="image">the image</param>
        private void createViewImage(BrailleIOScreen screen,  OSMElements.OSMElement osmElement, System.Drawing.Image image)
        {
            if (screen == null) { return; }
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetBitmap(image);
            vr.ShowScrollbars = brailleUiElement.isScrollbarShow;
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
        /// Creates a view which don't use a standard renderer
        /// </summary>
        /// <param name="screen">name of the <code>BrailleIOScreen</code> on which the view souhld be shown</param>
        /// <param name="osmElement">OSM element for this view </param>
        /// <param name="renderer">name of the renderer</param>
        private void createViewOtherContent(BrailleIOScreen screen, OSMElements.OSMElement osmElement, IBrailleIOContentRenderer renderer)
        {
            if (screen == null) { return; }
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetOtherContent(brailleUiElement, renderer);
            vr.ShowScrollbars = brailleUiElement.isScrollbarShow;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            vr.SetZIndex(osmElement.brailleRepresentation.zIntex);
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }
        #endregion

        /// <summary>
        /// Seeks to a point the associated view name.
        /// </summary>
        /// <param name="x">x coordinat of the point on the braille display</param>
        /// <param name="y">y coordinat of the point on the braille display</param>
        /// <param name="offsetX">x offste of the view</param>
        /// <param name="offsetY">y offste of the view</param>
        /// <returns>the name of the view or <code>null</code></returns>
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
        /// Gives the correct renderer for a UI element
        /// </summary>
        /// <param name="guiElementType">name of the UI element type</param>
        /// <returns>name of the renderer or <c>null</c></returns>
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
        /// Enum of all possible UI elements which have a BrailleIO renderer
        /// </summary>
        private enum uiElementeTypesBrailleIoEnum { Matrix, Text, Screenshot, Button, DropDownMenuItem, TextBox, ListItem, GroupElement, TabItem }

        /// <summary>
        /// Returns the default size of the control type
        /// </summary>
        /// <param name="uiElementType">the name of the control type</param>
        /// <param name="heightMin">the minimum height</param>
        /// <param name="widthMin">the minimum width</param>
        public void getSizeOfUiElementType(String uiElementType, out int heightMin, out int widthMin)
        {
            heightMin = 5;
            widthMin = 5;
            uiElementsTypeStruct uiElement =  uiElementList.Find(ue => ue.uiElementType.Equals(uiElementType));
            if(!uiElement.Equals( new uiElementsTypeStruct()))
            {
                heightMin = uiElement.heightMin;
                widthMin = uiElement.widthMin *3;
            }
        }

        private struct uiElementsTypeStruct
        {
            public String uiElementType { get; set; }
            public int heightMin { get; set; }
            public int widthMin { get; set; }
        }

        /// <summary>
        /// Creates a list of all possible renderer for UI elements and their minimal height.
        /// </summary>
        /// <returns></returns>
        private List<uiElementsTypeStruct> getUiElements()
        {
            List<uiElementsTypeStruct> uiElementList = new List<uiElementsTypeStruct>();
            uiElementsTypeStruct uiElement = new uiElementsTypeStruct();

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Button.ToString();
            uiElement.heightMin = 7; // border (2) + height small letter (3) + free line (2)
            uiElement.widthMin = 6; // border (2) + width letter (2) + space (2)
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.DropDownMenuItem.ToString();
            uiElement.heightMin = 9; // border (4) + height small letter (3) + free line (2)
            uiElement.widthMin = 9; // border (2) + width letter (2) + space (2) + 	bullet point (3)
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Matrix.ToString();
            uiElement.heightMin = 1;
            uiElement.widthMin = 1;
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Screenshot.ToString();
            uiElement.heightMin = 1; // What should be used for a screenshot? 
            uiElement.widthMin = 1; // What should be used for a screenshot? 
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.Text.ToString();
            uiElement.heightMin = 3; // height small letter (3) 
            uiElement.widthMin = 2; // width letter (2) 
            uiElementList.Add(uiElement);

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.TextBox.ToString();
            uiElement.heightMin = 7; // border (2) + height small letter (3) + free line (2)
            uiElement.widthMin = 6; // border (2) + width letter (2) + space (2)
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
                throw new Exception("Attention: The number of renderer in the Enum (uiElementeTypesBrailleIoEnum) and in the list of all UI elements (uiElementList) dosn't match!");
            }
            return uiElementList;
        }

        /// <summary>
        /// Gives a list of all possible renderer depending on the choosen device
        ///  Attention: All newly created renderers must be added in the list of renderers ("getUiElements") and the Enum "uiElementsTypesBrailleIoEnum"
        /// </summary>
        /// <returns>list of all possible renderer</returns>
        public List<String> getUiElementRenderer()
        {
            List<String> uiElementRenderer = new List<String>();
            /* foreach(uiElementeTypesBrailleIoEnum uiEnum in Enum.GetValues(typeof(uiElementeTypesBrailleIoEnum)))
             { 
                 uiElementRenderer.Add(uiEnum.ToString());
             }*/
            Device activeDevice = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
            foreach (uiElementsTypeStruct element in uiElementList)
            {
                //Angaben zur min. Größe prüfen
                
                if (activeDevice.height >= element.heightMin && activeDevice.width >= element.widthMin)
                {
                    uiElementRenderer.Add(element.uiElementType);
                }
            }
            return uiElementRenderer;
        }

        /// <summary>
        /// list of ALL possible renderer
        /// Attention: All newly created renderers must be added in the list of renderers ("getUiElements") and the Enum "uiElementsTypesBrailleIoEnum"
        /// </summary>
        /// <returns>list of all possible renderer</returns>
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
        /// Gives a example tactile representation to a given tree object (node)
        /// </summary>
        /// <param name="osmElementFilteredNode">a node</param>
        /// <returns>Boolean matrix where <code>true</code> represents a shown pin</returns>
        public bool[,] getRendererExampleRepresentation(OSMElements.OSMElement osmElementFilteredNode)
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
                // a screenshot musst be created
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
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // for all others it should be used "otherContent"
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
            else { throw new Exception("It can't found a matching renderer!"); }

            // delete all temporary screens, including their views
            brailleIOMediator.RemoveView(osmElementFilteredNode.brailleRepresentation.screenName);
            return matrix;
        }

        /// <summary>
        /// Gives for a renderer a exemplary tactile representation 
        /// A standard size is used
        /// </summary>
        /// <param name="uiElementType">name of the UI controlle type</param>
        /// <returns>Boolean matrix where<code>true</code> represents a shown pin</returns>
        public bool[,] getRendererExampleRepresentation(String uiElementType)
        {
            if (uiElementType.Equals("GroupElement")) { return new bool[1, 1]; }
            return getRendererExampleRepresentation(getExampleOsmElement(uiElementType));
        }

        #region example OSMElements
        private OSMElements.OSMElement getExampleOsmElement(String uiElementType)
        {
            #region common properties
            OSMElements.OSMElement osmElement = new OSMElements.OSMElement();
            osmElement.properties = new GeneralProperties();
            osmElement.brailleRepresentation = new BrailleRepresentation();
            osmElement.properties.controlTypeFiltered = uiElementType;
            osmElement.brailleRepresentation.screenName = "_tmp_screen_name_";
            osmElement.brailleRepresentation.viewName = "_tmp_view_name_";
            osmElement.brailleRepresentation.isVisible = true;
            Rect rect = new Rect();
            osmElement.properties.isEnabledFiltered = true;
            osmElement.properties.IdGenerated = "_tmp_id_";
            osmElement.properties.valueFiltered = "example";
            #endregion
            #region different properties depending on the type --> if a new renderer added, add a example represemtation here
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
                osmElement.brailleRepresentation.uiElementSpecialContent = dropDownMenu;
            }
            if (uiElementeTypesBrailleIoEnum.Matrix.ToString().Equals(uiElementType))
            {
                rect = new Rect(0, 0, 3, 7);
                osmElement.brailleRepresentation.matrix = new bool[,] { 
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
                osmElement.brailleRepresentation.zoom = 1;
                osmElement.brailleRepresentation.contrast = 120;
            }
            if (uiElementeTypesBrailleIoEnum.Text.ToString().Equals(uiElementType))
            {
                rect = new Rect(0, 0, 25, 5);
            }
            if (uiElementeTypesBrailleIoEnum.TextBox.ToString().Equals(uiElementType))
            {
                osmElement.properties.valueFiltered = "example text example text ...";
                rect = new Rect(0, 0, 25, 10);
                osmElement.brailleRepresentation.isScrollbarShow = true;
            }
            if (uiElementeTypesBrailleIoEnum.ListItem.ToString().Equals(uiElementType))
            {
                osmElement.properties.valueFiltered = "Item 1";
                rect = new Rect(0, 0, 30, 7);
                OSMElements.UiElements.ListMenuItem listMenuItem = new ListMenuItem();
                listMenuItem.hasNext = true;
                listMenuItem.isMultipleSelection = true;
                osmElement.properties.isToggleStateOn = false;
                osmElement.brailleRepresentation.uiElementSpecialContent = listMenuItem;
            }
            if (uiElementeTypesBrailleIoEnum.TabItem.ToString().Equals(uiElementType))
            {
                osmElement.properties.valueFiltered = "TabItem";
                rect = new Rect(0, 0, 8, 8);
                OSMElements.UiElements.TabItem tabview = new TabItem();
                tabview.orientation = OSMElements.UiElements.Orientation.Left;
                osmElement.brailleRepresentation.uiElementSpecialContent = tabview;
            }
            /*if (uiElementeTypesBrailleIoEnum.GroupElement.ToString().Equals(uiElementType))
            {
                osmElement.properties.valueFiltered = "Beispiel Beispieltext Beispieltext";//TODO
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
                
                osmElement.brailleRepresentation.groupelementsOfSameType = group;
                
            }*/
            #endregion
            osmElement.properties.boundingRectangleFiltered = rect;
            return osmElement;
        }

        #endregion

        #region converts objects
        /// <summary>
        /// Converts <code>System.Windows.Forms.Padding</code> in <code>BrailleIO.Structs.BoxModel</code>
        /// </summary>
        /// <param name="padding">a <code>Padding</code> object</param>
        /// <returns>a <code>BoxModel</code> object</returns>
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
        /// Converts DropDownMenuItem (from <code>OSMElement.UiElements.DropDownMenuItem</code> to <code>BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem</code>)
        /// </summary>
        /// <param name="osmMenu"><code>OSMElement.UiElements.DropDownMenuItem</code> DropDownMenuItem</param>
        /// <returns>a <code>BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem</code> object</returns>
        private BrailleIOGuiElementRenderer.UiElements.DropDownMenuItem convertDropDownMenu(OSMElements.UiElements.DropDownMenuItem osmMenu)
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

        /// <summary>
        /// Converts an <c>OSMElement.OSMElement</c> to a <c>BrailleIOGuiElementRenderer.UiElements.ListMenuItem</c>
        /// </summary>
        /// <param name="osmElement">node of braille tree</param>
        /// <returns>a <c>BrailleIOGuiElementRenderer.UiElements.ListMenuItem</c> object</returns>
        private BrailleIOGuiElementRenderer.UiElements.ListMenuItem convertToListItem(OSMElements.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElements.ListMenuItem brailleListItem = new BrailleIOGuiElementRenderer.UiElements.ListMenuItem();
            if(osmElement.Equals(new OSMElements.OSMElement())){return  brailleListItem;}
            if (!osmElement.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElements.UiElements.ListMenuItem)))
            {
                return brailleListItem;
            }
            OSMElements.UiElements.ListMenuItem listItem = (OSMElements.UiElements.ListMenuItem)osmElement.brailleRepresentation.uiElementSpecialContent;
            brailleListItem.hasNext = listItem.hasNext;
            brailleListItem.isMultipleSelection = listItem.isMultipleSelection;
            brailleListItem.isSelected = osmElement.properties.isToggleStateOn != null ? (bool)osmElement.properties.isToggleStateOn : false;
            return brailleListItem;
        }

        /// <summary>
        /// Converts an <c>OSMElement.OSMElement</c> to a <c>BrailleIOGuiElementRenderer.UiElements.ListMenuItem</c>
        /// </summary>
        /// <param name="osmElement">node of braille tree</param>
        /// <returns>a <c>BrailleIOGuiElementRenderer.UiElements.ListMenuItem</c> object</returns>
        private BrailleIOGuiElementRenderer.UiElements.TabItem convertToTabView(OSMElements.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElements.TabItem tabViewBraille = new BrailleIOGuiElementRenderer.UiElements.TabItem();
            if (osmElement.Equals(new OSMElements.OSMElement())) { return tabViewBraille; }
            if (!osmElement.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElements.UiElements.TabItem)))
            {
                return tabViewBraille;
            }
            OSMElements.UiElements.TabItem tabOsm = (OSMElements.UiElements.TabItem)osmElement.brailleRepresentation.uiElementSpecialContent;
            tabViewBraille.orientation = tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Bottom.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Bottom :
                 (tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Top.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Top : (tabOsm.orientation.ToString().Equals(BrailleIOGuiElementRenderer.UiElements.Orientation.Right.ToString()) ? BrailleIOGuiElementRenderer.UiElements.Orientation.Right : BrailleIOGuiElementRenderer.UiElements.Orientation.Left));
            return tabViewBraille;
        }

        /// <summary>
        /// Converts an <code>OSMElement</code> (<code>uiElementSpecialContent</code>) to a special object for BrailleIO
        /// </summary>
        /// <param name="osmElement">a node</param>
        /// <returns>object with the converted content</returns>
        private object convertUiElementSpecialContent(OSMElements.OSMElement osmElement)
        {
            object brailleIOElement = new object();
            if (osmElement.Equals(new OSMElements.OSMElement()) || osmElement.brailleRepresentation.uiElementSpecialContent == null) { return brailleIOElement; }
            Type osmElementspecialcontentType = osmElement.brailleRepresentation.uiElementSpecialContent.GetType();
            if (osmElementspecialcontentType.Equals(typeof(OSMElements.UiElements.DropDownMenuItem)))
            {
                brailleIOElement = convertDropDownMenu((OSMElements.UiElements.DropDownMenuItem)osmElement.brailleRepresentation.uiElementSpecialContent);
            }
            if (osmElementspecialcontentType.Equals(typeof(OSMElements.UiElements.ListMenuItem)))
            {
                brailleIOElement = convertToListItem(osmElement);
            }
            if (osmElementspecialcontentType.Equals(typeof(OSMElements.UiElements.TabItem)))
            {                
                brailleIOElement = convertToTabView(osmElement);
            }
            return brailleIOElement;
        }

        /// <summary>
        /// Converts a <code>OSMElement</code> object to a <code>BrailleIOGuiElementRenderer.UiElement</code> for rendering
        /// </summary>
        /// <param name="osmElement">a node</param>
        /// <returns>a <code>BrailleIOGuiElementRenderer.UiElement</code> object</returns>
        private BrailleIOGuiElementRenderer.UiElement convertToBrailleIOUiElement(OSMElements.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleIOElement = new BrailleIOGuiElementRenderer.UiElement();
            brailleIOElement.contrast = osmElement.brailleRepresentation.contrast;
            brailleIOElement.isDisabled = osmElement.properties.isEnabledFiltered.HasValue ? !(Boolean)osmElement.properties.isEnabledFiltered : false;
            
            brailleIOElement.isVisible = osmElement.brailleRepresentation.isVisible;
            brailleIOElement.matrix = osmElement.brailleRepresentation.matrix;
            brailleIOElement.screenName = osmElement.brailleRepresentation.screenName;
            brailleIOElement.isScrollbarShow = osmElement.brailleRepresentation.isScrollbarShow;
            brailleIOElement.text = osmElement.properties.valueFiltered;
            brailleIOElement.uiElementSpecialContent = convertUiElementSpecialContent(osmElement);
            brailleIOElement.viewName = osmElement.brailleRepresentation.viewName;
            brailleIOElement.zoom = osmElement.brailleRepresentation.zoom;
            if (grantTrees.brailleTree != null)
            {
                List<Object> nodeList = treeOperation.searchNodes.getNodeList(osmElement.properties.IdGenerated, grantTrees.brailleTree); //TODO. gleich übergeben?
                if (nodeList != null && nodeList.Count == 1 && strategyMgr.getSpecifiedTree().HasChild(nodeList[0]))
                {
                    List<BrailleIOGuiElementRenderer.Groupelements> childList = new List<BrailleIOGuiElementRenderer.Groupelements>();
                   // brailleIOElement.isScrollbarShow = true;
                    childList = iteratedChildreen(nodeList[0]);
                    brailleIOElement.child = childList;
                }
            }
            return brailleIOElement;
        }

        private BrailleIOGuiElementRenderer.Groupelements osmSubelementToGroupelement(OSMElements.OSMElement brailleTreeNode)
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
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElements.UiElements.DropDownMenuItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertDropDownMenu((OSMElements.UiElements.DropDownMenuItem)brailleTreeNode.brailleRepresentation.uiElementSpecialContent);
            }
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElements.UiElements.ListMenuItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertToListItem(brailleTreeNode);
            }
            if (brailleTreeNode.brailleRepresentation.uiElementSpecialContent != null && typeof(OSMElements.UiElements.TabItem).Equals(brailleTreeNode.brailleRepresentation.uiElementSpecialContent.GetType()))
            {
                childUi.uiElementSpecialContent = convertToTabView(brailleTreeNode);
            }
            groupelement.childUiElement = childUi;
            
            return groupelement;
        }

        /// <summary>
        /// Creats for every child node (which is part of a group e.g. munuBar) a data structure
        /// </summary>
        /// <param name="parentBrailleTreeNode">parent node</param>
        /// <returns>a list with all group elements</returns>
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
        /// Depending on the choosen DisplayStrategy the adapter to be used is returned
        /// </summary>
        /// <param name="displayStrategyType">DisplayStrategy</param>
        /// <returns>adapter for the (braille) output</returns>
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

        private Type BrailleIoAdapterClassTodisplayStrategyClass(Type brailleIOAdapterType)
        {
            Type brailleAdapterType = null;

            XElement xmlDoc = XElement.Load(@"displayStrategyType.xml");
            IEnumerable<XElement> elements = xmlDoc.Elements("Strategy");
            foreach (XElement strategy in elements)
            {
                if (strategy.Element("AdaptClassTypeFullName").Value.Equals(brailleIOAdapterType.FullName))
                {
                    if (brailleIOAdapterType.Assembly.FullName.Contains(strategy.Element("AdapterClassTypeDllName").Value))
                    {
                        return brailleAdapterType = Type.GetType(strategy.Element("DeviceClassTypeFullName").Value + ", " + strategy.Element("DeviceClassTypeDllName").Value);
                    }
                }
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Specifies whether a braille display was initirized
        /// </summary>
        public bool isInitialized()
        {
            return initialized;
        }

        /// <summary>
        /// Sets the given screen on "visible" and all others of "invisible".
        /// </summary>
        /// <param name="screenName">the name of the screen which should be visible</param>
        public void setVisibleScreen(String screenName)
        {
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
        /// Returns the name of the visible screen
        /// only one screen (per time) should by visible
        /// </summary>
        /// <returns>name of the visible screen or <c>null</c></returns>
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
            return null;
        }

        /// <summary>
        /// Returns the Id of the visible Screen
        /// </summary>
        /// <returns>id of the visible screen or <c>null</c></returns>
        public String getVisibleScreenId()
        {
            String screenName = getVisibleScreen();
            if(screenName != null)
            {
                Object visibleScreen = treeOperation.searchNodes.getSubtreeOfScreen(screenName);
                if(visibleScreen != null)
                {
                    return strategyMgr.getSpecifiedTree().GetData(visibleScreen).properties.IdGenerated;
                }
            }
            return null;
        }

        /// <summary>
        /// moves a (group of) view(s) horizontal
        /// </summary>
        /// <param name="viewNode">the name of the view</param>
        /// <param name="steps">steps to move right</param>
        public void moveViewRangHoricontal(Object viewNode, int steps)
        {
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
        /// moves a (group of) view(s) vertical
        /// </summary>
        /// <param name="viewNode">the name of the view</param>
        /// <param name="steps">steps to move left</param>
        public void moveViewRangVertical(Object viewNode, int steps)
        {
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

        /// <summary>
        /// Gives position and offset of a node
        /// </summary>
        /// <param name="brailleNode">node of the braille (output) tree</param>
        /// <returns>position and offset of a node</returns>
        public TactileNodeInfos getTactileNodeInfos(object brailleNode)
        {
            TactileNodeInfos nodeInfos = new TactileNodeInfos();
            OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(brailleNode);
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
