using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;

using BrailleIO;
using BrailleIO.Structs;
using Gestures.Recognition;
using GRANTManager.Interfaces;
using GRANTManager;
using OSMElement;
using OSMElement.UiElements;
using BrailleIOGuiElementRenderer;
using BrailleIO.Interface;

namespace StrategyBrailleIO
{
    public class BrailleDisplayStrategyBrailleIO : IBrailleDisplayStrategy
    {
       IBrailleIOShowOffMonitor monitor;
       BrailleIOMediator brailleIOMediator {get; set;}
        private Boolean isInitialized = false;
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
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }

        public BrailleDisplayStrategyBrailleIO() { uiElementList = getUiElements(); }

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

        public void initializedBrailleDisplay()
        {
            if (brailleIOMediator == null)
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            isInitialized = true;
            createBrailleDis();

        }

        private void createBrailleDis()
        {
            Type activeDeviceType = Type.GetType(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeFullName + ", " + strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeNamespace);
               
            //String name = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassType.Name;
            //String ns = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().deviceClassTypeNamespace;
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
               //TODO: bei MVBD ggf. das Ausgabegerät einstellen
                
           /*     #region BrailleDis events
                brailleAdapter.touchValuesChanged += new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(_bda_touchValuesChanged);
                brailleAdapter.keyStateChanged += new EventHandler<BrailleIO_KeyStateChanged_EventArgs>(_bda_keyStateChanged);
                #endregion
                */
                //return brailleAdapter;
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
            if (!isInitialized) { initializedBrailleDisplay(); }
            ITreeStrategy<OSMElement.OSMElement> osm = grantTrees.getBrailleTree().Copy();
            createViews(osm);
            brailleIOMediator.RenderDisplay();
        }

        /// <summary>
        /// Erstellt einen neuen Screen, falls dieser noch nicht existiert
        /// </summary>
        /// <param name="screens">gibt den Namen des Screens an</param>
        private void createScreen(String screenName)
        {
            try
            {
                 object screen = brailleIOMediator.GetView(screenName);
                 if (screen == null)
                 {
                     brailleIOMediator.AddView(screenName, new BrailleIOScreen());

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
            OsmRelationship<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.BrailleTree.Equals(idGeneratedBrailleNode) || r.FilteredTree.Equals(idGeneratedBrailleNode));
            if (osmRelationships == null) { return null; }
            OSMElement.OSMElement nodeFilteredTree = strategyMgr.getSpecifiedTreeOperations().getFilteredTreeOsmElementById(osmRelationships.FilteredTree);
            if (nodeFilteredTree.Equals(new OSMElement.OSMElement())) { return null; }
            Image bmp;
           /* int h = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Height);
            int w = Convert.ToInt32(nodeFilteredTree.Data.properties.boundingRectangleFiltered.Width);
            bmp = ScreenCapture.CaptureWindow(nodeFilteredTree.Data.properties.hWndFiltered, h, w, 0, 0, 0, 0);*/
            Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(nodeFilteredTree);
          //  Console.WriteLine("Braille -- Rect: x = {0}, y = {1}, höhe = {2}, breite= {3}", rect.X, rect.Y, rect.Height, rect.Width);
            bmp = ScreenCapture.CaptureScreenPos(rect);
            return bmp;
        }

        /// <summary>
        /// Geht rekursive durch alle Baumelemente und erstellt die einzelnen Views
        /// </summary>
        /// <param name="osm">gibt das Baum-Objekt der Oberflaeche an</param>
        private void createViews(ITreeStrategy<OSMElement.OSMElement> osm)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;
            //Falls die Baumelemente Kinder des jeweiligen Elements sind
            while (osm.HasChild && !(osm.Count == 1 && osm.Depth == -1))
            {
                if (osm.HasChild)
                {
                    node1 = osm.Child;
                    if (!node1.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                    {
                        createScreen(node1.Data.brailleRepresentation.screenName);
                        createView(node1.Data);
                    }
                    createViews(node1);
                }
            }
            //falls die Baumelemente (alle) am obersten Knoten hängen
            while (osm.HasNext)
            {
                node1 = osm.Next;
                if (!node1.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createScreen(node1.Data.brailleRepresentation.screenName);
                    createView(node1.Data);
                }
                createViews(node1);
            }
            if(osm.Count == 1 && osm.Depth == -1){
                if (!osm.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createScreen(osm.Data.brailleRepresentation.screenName);
                    createView(osm.Data);
                }
            }
            if (!osm.HasChild)
            {
                node1 = osm;
                if (osm.HasParent)
                {
                    node1.Remove();
                }
            }
            if (!osm.HasNext && !osm.HasParent)
            {
                if (osm.HasPrevious)
                {
                    node1 = osm;
                    node1.Remove();
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
        /// <param name="osmElement">gibt das zur View zugehörige OSM-Element an</param>
        private void createViewText(BrailleIOScreen screen, OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetText(brailleUiElement.text);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View mit einer Bool-Matrix
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="osmElement">gibt das zur View zugehörige OSM-Element an</param>
        private void createViewMatrix(BrailleIOScreen screen, OSMElement.OSMElement osmElement)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            vr.SetMatrix(brailleUiElement.matrix);
            vr.ShowScrollbars = brailleUiElement.showScrollbar;
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View mit einem Bild
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="osmElement">gibt das zur View zugehörige OSM-Element an</param>
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
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }

        /// <summary>
        /// Erstellt eine View die keinen standard-Renderer verwendet
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="osmElement">gibt das zur View zugehörige OSM-Element an</param>
        /// <param name="renderer">gibt den Renderer für diese View an</param>
        private void createViewOtherContent(BrailleIOScreen screen, OSMElement.OSMElement osmElement, IBrailleIOContentRenderer renderer)
        {
            BrailleIOGuiElementRenderer.UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            BrailleIOViewRange vr = new BrailleIOViewRange((int)osmElement.properties.boundingRectangleFiltered.Left, (int)osmElement.properties.boundingRectangleFiltered.Top, (int)osmElement.properties.boundingRectangleFiltered.Width, (int)osmElement.properties.boundingRectangleFiltered.Height, new bool[0, 0]);
            BrailleIOButtonToMatrixRenderer buttonRenderer = new BrailleIOButtonToMatrixRenderer();
            vr.SetOtherContent(brailleUiElement, renderer);
            vr.SetPadding(paddingToBoxModel(osmElement.brailleRepresentation.padding));
            vr.SetMargin(paddingToBoxModel(osmElement.brailleRepresentation.margin));
            vr.SetBorder(paddingToBoxModel(osmElement.brailleRepresentation.boarder));
            screen.AddViewRange(brailleUiElement.viewName, vr);
            vr.SetVisibility(brailleUiElement.isVisible);
        }
        #endregion

        /// <summary>
        /// Ermittelt zu einem Punkt den zugehörigen View-Name
        /// </summary>
        /// <param name="x">gibt die horizontale Position des Punktes auf der Stifftplatte an</param>
        /// <param name="y">gibt die vertikale Position des Punktes auf der Stifftplatte an</param>
        /// <returns>falls eine passende View gefunden wurde dessen Name; sonst <code>null</code></returns>
        public String getBrailleUiElementViewNameAtPoint(int x, int y)
        {
            BrailleIOViewRange viewAtPoint = brailleIOMediator.GetViewAtPosition(x, y);
            if(viewAtPoint == null){
                Console.WriteLine("zu dem Punkt wurde keine passende View gefunden.");
                return null;
            }
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
                case "DropDownMenu":
                    return new BrailleIODropDownMenuToMatrixRenderer();
            }
            return null;
        }

        /// <summary>
        /// Enum welches die verschiedenen 'ui-Elemente' enthält, für welches es Renderer in BrailleIO gibt
        /// </summary>
        private enum uiElementeTypesBrailleIoEnum { Matrix, Text, Screenshot, Button, DropDownMenu, TextBox }

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

            uiElement.uiElementType = uiElementeTypesBrailleIoEnum.DropDownMenu.ToString();
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

            if (uiElementList.Count != Enum.GetNames(typeof( uiElementeTypesBrailleIoEnum)).Length)
            {
                throw new Exception("Achtung es wurden nicht gleichviele Elemente im Enum wie in der Liste alle Ui-Elemente angegeben!");
            }
            return uiElementList;
        }

        /// <summary>
        /// Gibt eine Liste mit möglichen Renderen für BrailleIO zurück, welche für die Ausgabegerätgröße sinnvoll sind
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
        /// Gibt zu einem Renderer beispielhaft die Darstellung an
        /// </summary>
        /// <param name="osmElement">gibt das OSM-Element an, welches für die Braille-UI beispielhaft gerendert werden soll</param>
        /// <returns>eine Bool-Matrix mit den gesetzten Pins</returns>
        public bool[,] getRendererExampleRepresentation(OSMElement.OSMElement osmElement)
        {
            if (brailleIOMediator == null)
            {
                brailleIOMediator = BrailleIOMediator.Instance;
            }
            UiElement brailleUiElement = convertToBrailleIOUiElement(osmElement);
            createScreen(osmElement.brailleRepresentation.screenName);
            createView(osmElement);
            BrailleIOViewRange tmpView = (brailleIOMediator.GetView(osmElement.brailleRepresentation.screenName) as BrailleIOScreen).GetViewRange(osmElement.brailleRepresentation.viewName);
            if (tmpView == null && !osmElement.properties.controlTypeFiltered.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString())) { return new bool[0, 0]; }
            if(osmElement.properties.controlTypeFiltered.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString())){
                //Screenshot muss extra erstellt werden
                Image img = ScreenCapture.CaptureWindow(strategyMgr.getSpecifiedOperationSystem().deliverDesktopHWND(), Convert.ToInt32(osmElement.properties.boundingRectangleFiltered.Height *10), Convert.ToInt32(osmElement.properties.boundingRectangleFiltered.Width *10), 0, 0, 0, 0);
                if (img == null) { return new bool[0, 0]; }
                createViewImage(brailleIOMediator.GetView(brailleUiElement.screenName) as BrailleIOScreen, osmElement, img);
                tmpView = (brailleIOMediator.GetView(osmElement.brailleRepresentation.screenName) as BrailleIOScreen).GetViewRange(osmElement.brailleRepresentation.viewName);
            }
            if (tmpView.IsText())
            {
                tmpView.SetText(tmpView.GetText() == null ? "" : tmpView.GetText());
            }
            String uiElementType = osmElement.properties.controlTypeFiltered;
            bool[,] matrix;
            if (!(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase) ||
                uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))) // für alle anderen muss bei BrailleIO "otherContent" zugewiesen werden
            {

                IBrailleIOContentRenderer renderer = getRenderer(uiElementType);
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetOtherContent());
            }else if(uiElementType.Equals(uiElementeTypesBrailleIoEnum.Text.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetText());
            }
            else if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Matrix.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetMatrix());
            }
            else if (uiElementType.Equals(uiElementeTypesBrailleIoEnum.Screenshot.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                matrix = tmpView.ContentRender.RenderMatrix(tmpView, tmpView.GetImage());
            }
            else { throw new Exception("Kein passenden Renderer gefunden!"); }
            
            //löschen den temporär erstellten Screens inkl aller temporär erstellten Views
            brailleIOMediator.RemoveView(osmElement.brailleRepresentation.screenName);
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
               rect  = new Rect(0, 0, 50, 30);               
            }
            if (uiElementeTypesBrailleIoEnum.DropDownMenu.ToString().Equals(uiElementType))
            {
                rect = new Rect(0,0,25,10);
                DropDownMenu dropDownMenu = new DropDownMenu();
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
                rect = new Rect(0, 0, 6, 3);
                brailleR.matrix = new bool[,] { 
                    {false, false, true, true, false, false},
                    {false, true, false, false, false, false},
                    {true, false, false, false, false, false}
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
        /// konvertiert das DropDownMenu aus <code>OSMElement.UiElements.DropDownMenu</code> zu <code>BrailleIOGuiElementRenderer.UiElements.DropDownMenu</code>
        /// </summary>
        /// <param name="osmMenu"></param>
        /// <returns></returns>
        private BrailleIOGuiElementRenderer.UiElements.DropDownMenu convertDropDownMenu(OSMElement.UiElements.DropDownMenu osmMenu)
        {
            BrailleIOGuiElementRenderer.UiElements.DropDownMenu brailleIOMenu = new BrailleIOGuiElementRenderer.UiElements.DropDownMenu();
            brailleIOMenu.hasChild = osmMenu.hasChild;
            brailleIOMenu.hasNext = osmMenu.hasNext;
            brailleIOMenu.hasPrevious = osmMenu.hasPrevious;
            brailleIOMenu.isChild = osmMenu.isChild;
            brailleIOMenu.isOpen = osmMenu.isOpen;
            brailleIOMenu.isVertical = osmMenu.isVertical;
            return brailleIOMenu;
        }

        /// <summary>
        /// Konvertiert den <code>uiElementSpecialContent</code> des <code>OSMElement</code>s zu einem entsprechenden Objekt, welches BrailleIO bekannt ist
        /// </summary>
        /// <param name="osmElement">gibt das osmElement an</param>
        /// <returns>ein Objekt mit dem konvertierten Inhalt</returns>
        private object convertUiElementSpecialContent(object osmElement)
        {
            object brailleIOElement = new object();
            if (osmElement == null) { return brailleIOElement; }
            Type osmElementType = osmElement.GetType();
            if (osmElementType.Equals(typeof(OSMElement.UiElements.DropDownMenu)))
            {
                brailleIOElement = convertDropDownMenu((OSMElement.UiElements.DropDownMenu)osmElement);
            }
            return brailleIOElement;
        }

        /// <summary>
        /// Konvertiert Inhalte des <code>OSMElement</code>s zu einer entsprechendend Darstellung vom <code>BrailleIOGuiElementRenderer.UiElement</code> um das Element einem Renderer zu übergeben
        /// </summary>
        /// <param name="osmElement">gibt das <code>OSMElement</code> an</param>
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
            brailleIOElement.uiElementSpecialContent = convertUiElementSpecialContent(osmElement.brailleRepresentation.uiElementSpecialContent);
            brailleIOElement.viewName = osmElement.brailleRepresentation.viewName;
            brailleIOElement.zoom = osmElement.brailleRepresentation.zoom;
            return brailleIOElement;
        }

        /// <summary>
        /// Ermittelt anhand des genutzten Typs der DisplayStrategy welcher Adapter verwendet werden muss
        /// </summary>
        /// <param name="displayStrategyType">gibt die genutzte DisplayStrategy an</param>
        /// <returns>der Adapter für die Ausgabe</returns>
        private AbstractBrailleIOAdapterBase displayStrategyClassToBrailleIoAdapterClass(Type displayStrategyType)
        {
            Type brailleAdapterType = null;
            //if (displayStrategyClass.namespaceString.Equals("StrategyMVBD") && displayStrategyClass.name.Equals("DisplayStrategyMVBD"))
            if(displayStrategyType.Equals(typeof(StrategyMVBD.DisplayStrategyMVBD)))
            {
                brailleAdapterType = typeof(BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet_MVBD);
            }
            if (displayStrategyType.Equals(typeof(DisplayStrategyBrailleDis)))
            {
                brailleAdapterType = typeof(BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet);
            }
            if (displayStrategyType.Equals(typeof(DisplayStrategyBrailleIoSimulator)))
            {
                //hier brauchen wir den Type nicht, da der Simulator anders erstellt wird
            }
            if (brailleAdapterType != null) { return (AbstractBrailleIOAdapterBase)Activator.CreateInstance(brailleAdapterType, brailleIOMediator.AdapterManager); }
            return null;
        }
        #endregion


        bool IBrailleDisplayStrategy.isInitialized()
        {
            return isInitialized;
        }
    }
}
