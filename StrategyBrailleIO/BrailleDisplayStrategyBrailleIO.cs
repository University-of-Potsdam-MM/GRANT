﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

using BrailleIO;
using BrailleIO.Structs;
using Gestures.Recognition;
using StrategyManager.Interfaces;
using StrategyManager;
using OSMElement;
using BrailleIOGuiElementRenderer;
using BrailleIO.Interface;

namespace StrategyBrailleIO
{
    public class BrailleDisplayStrategyBrailleIO : IBrailleDisplayStrategy
    {
       IBrailleIOShowOffMonitor monitor;
        BrailleIOMediator brailleIOMediator {get; set;}

        /// <summary>
        /// Ist der Adapter des Simolator
        /// </summary>
        AbstractBrailleIOAdapterBase showOffAdapter;
        //GestureRecognizer showOffGestureRecognizer;

        /// <summary>
        /// Ist der Adapter des BrailleDis
        /// </summary>
        AbstractBrailleIOAdapterBase brailleDisAdapter;
        //GestureRecognizer brailleDisRecognizer; //TODO


        private StrategyMgr strategyMgr;
        public StrategyMgr getStrategyMgr() { return strategyMgr; }
        public void setStrategyMgr(StrategyMgr manager)
        {
            strategyMgr = manager;
        }

        /// <summary>
        /// Erstellt, sofern noch nicht vorhanden, ein Simulator für das Ausgabegerät
        /// </summary>
        public void initializedSimulator()
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

         //   createBrailleDis();

        }

        private AbstractBrailleIOAdapterBase createBrailleDis()
        {/// aus BrailleIOExample -> erstmal gekürzt
            if (brailleIOMediator != null && brailleIOMediator.AdapterManager != null)
            {

                // brailleDisAdapter = new BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet(brailleIOMediator.AdapterManager);

                brailleDisAdapter = new BrailleIOBraillDisAdapter.BrailleIOAdapter_BrailleDisNet_MVBD(brailleIOMediator.AdapterManager);
                brailleIOMediator.AdapterManager.ActiveAdapter = brailleDisAdapter;

           /*     #region BrailleDis events
                brailleDisAdapter.touchValuesChanged += new EventHandler<BrailleIO_TouchValuesChanged_EventArgs>(_bda_touchValuesChanged);
                brailleDisAdapter.keyStateChanged += new EventHandler<BrailleIO_KeyStateChanged_EventArgs>(_bda_keyStateChanged);
                #endregion
                */
                return brailleDisAdapter;
            }
            return null;
        }

        /// <summary>
        /// Ändert den Inhalt einer View -- Momentan wird nur der Text geändert!
        /// </summary>
        /// <param name="element">Gibt das OSM-element an, bei dem eine Änderung erfolgte</param>
        public void updateViewContent(OSMElement.OSMElement element)
        {
            BrailleIOScreen screen = brailleIOMediator.GetView(element.brailleRepresentation.screenName) as BrailleIOScreen;
            if (screen == null)
            {
                throw new Exception("Der Screen existiert nicht!");
            }
            BrailleIOViewRange view = screen.GetViewRange(element.brailleRepresentation.viewName) as BrailleIOViewRange;
            if (view == null)
            {
                throw new Exception("Der View existiert (in dem Screen) nicht!");
            }
            if (element.brailleRepresentation.content.otherContent != null)
            {
                IBrailleIOContentRenderer renderer;                
                Type typeOtherContent = element.brailleRepresentation.content.otherContent.GetType();
                if (typeOtherContent.Equals(typeof(BrailleIOGuiElementRenderer.UiObjectsEnum)))
                {
                    renderer = getRenderer((UiObjectsEnum)element.brailleRepresentation.content.otherContent);
                    if (renderer == null)
                    {
                        Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", element.brailleRepresentation.content.otherContent);
                        return;
                    }
                    view.SetOtherContent(element.brailleRepresentation.content.text, renderer);
                }
                else
                {// == Object[]
                    renderer = getRenderer((UiObjectsEnum)(element.brailleRepresentation.content.otherContent as object[])[0]);
                    if (renderer == null)
                    {
                        Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", element.brailleRepresentation.content.otherContent);
                        return;
                    }
                    //TODO: wann wird der Text hinzugefügt?
                    view.SetOtherContent(((element.brailleRepresentation.content.otherContent) as object[])[1], renderer);
                }
                return;
            }
            if (element.brailleRepresentation.content.matrix != null)
            {
                view.SetMatrix(element.brailleRepresentation.content.matrix);
                return;
            }
            if (element.brailleRepresentation.content.text != null)
            {
                view.SetText(element.brailleRepresentation.content.text);
                return;
            }
           // ...
        }

        /// <summary>
        /// Erstellt die GUI auf der Stiftplatte
        /// </summary>
        public void generatedBrailleUi()
        {
            ITreeStrategy<OSMElement.OSMElement> osm = strategyMgr.getBrailleTree().Copy();
            //ITreeStrategy<OSMElement.OSMElement> osm = strategyMgr.getBrailleTree();
            createViews(osm);
            Console.WriteLine();
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
        /// <param name="node">gibt den Knoten an, welcher die Definition für den Bildbereich hat</param>
        /// <returns>ein <code>Image</code> des Bildbereiches</returns>
        private Image captureScreen(ITreeStrategy<GeneralProperties> node)
        {
            Image bmp;

            int h = Convert.ToInt32(node.Data.boundingRectangleFiltered.Height);
            int w = Convert.ToInt32(node.Data.boundingRectangleFiltered.Width);
            bmp = ScreenCapture.CaptureWindow(node.Data.hWndFiltered, h, w, 0, 0, 0, 0);

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
            OSMElement.BrailleRepresentation brailleRepresentation = osmElement.brailleRepresentation;

            if (brailleRepresentation.content.matrix != null)
            {
                createViewMatrix(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, brailleRepresentation.content.matrix, brailleRepresentation.viewName, brailleRepresentation.position, brailleRepresentation.content.showScrollbar, brailleRepresentation.isVisible);
                return;
            }
            if (brailleRepresentation.content.otherContent != null)
            {
                Type typeOtherContent = brailleRepresentation.content.otherContent.GetType();
                IBrailleIOContentRenderer renderer;
                if (typeOtherContent.Equals(typeof(BrailleIOGuiElementRenderer.UiObjectsEnum)))
                {
                    renderer = getRenderer((UiObjectsEnum)brailleRepresentation.content.otherContent);
                    if (renderer == null)
                    {
                        Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", brailleRepresentation.content.otherContent);
                        return;
                    }
                    createViewOtherContent(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, brailleRepresentation.content.text, renderer, brailleRepresentation.viewName, brailleRepresentation.position, brailleRepresentation.isVisible);
                }
                else
                {// == Object[]
                    renderer = getRenderer((UiObjectsEnum)(brailleRepresentation.content.otherContent as object[])[0]);
                    if (renderer == null)
                    {
                        Console.WriteLine("Für das UI-Element '{0}' existiert kein Renderer.", brailleRepresentation.content.otherContent);
                        return;
                    }
                    //TODO: wann wird der Text hinzugefügt?
                    createViewOtherContent(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, ((brailleRepresentation.content.otherContent) as object[])[1], renderer, brailleRepresentation.viewName, brailleRepresentation.position, brailleRepresentation.isVisible);
              
                }
                return;
            }
            if (brailleRepresentation.content.text != null)
            {
                createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, brailleRepresentation.content.text, brailleRepresentation.viewName, brailleRepresentation.position, brailleRepresentation.content.showScrollbar, brailleRepresentation.isVisible);
                return;
            }
            //TODO: weitere Möglichkeiten?

            //im Zweifelsfall wird immer eine "Text-View" mit einem leeren Text erstellt
            createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, (brailleRepresentation.content.text != null) ? brailleRepresentation.content.text : "", brailleRepresentation.viewName, brailleRepresentation.position, brailleRepresentation.content.showScrollbar, brailleRepresentation.isVisible);
        }

        #region create Views       
        /// <summary>
        /// Erstellt eine View mit einem Text
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="text">gibt den anzuzeigenden Text an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        /// <param name="showScrollbar">gibt an, ob Scrollbars gezeigt werden sollen (falls der Text zu lang für die View ist)</param>
        /// <param name="isVisible">gibt an, ob die View angezeigt werden soll</param>
        private void createViewText(BrailleIOScreen screen, String text, String viewName, Position position, Boolean showScrollbar, Boolean isVisible)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetText(text);
            vr.ShowScrollbars = showScrollbar;
            vr.SetPadding(paddingToBoxModel(position.padding));
            vr.SetMargin(paddingToBoxModel(position.margin));
            vr.SetBorder(paddingToBoxModel(position.boarder));
            
            screen.AddViewRange(viewName, vr);
            vr.SetVisibility(isVisible);
        }

        /// <summary>
        /// Erstellt eine View mit einer Bool-Matrix
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="matrix">gibt die bool-Matrix an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        /// <param name="showScrollbar">gibt an, ob Scrollbars gezeigt werden sollen (falls die Matrix zu lang für die View ist)</param>
        /// <param name="isVisible">gibt an, ob die View angezeigt werden soll</param>
        private void createViewMatrix(BrailleIOScreen screen, bool[,] matrix, String viewName, Position position, Boolean showScrollbar, Boolean isVisible)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetMatrix(matrix);
            vr.ShowScrollbars = showScrollbar;
            vr.SetPadding(paddingToBoxModel(position.padding));
            vr.SetMargin(paddingToBoxModel(position.margin));
            vr.SetBorder(paddingToBoxModel(position.boarder));
            
            screen.AddViewRange(viewName, vr);
            vr.SetVisibility(isVisible);
        }


        /// <summary>
        /// Erstellt eine View mit einem Bild
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="image">gibt das Bild an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        /// <param name="showScrollbar">gibt an, ob Scrollbars gezeigt werden sollen (falls das Bild  zu groß für die View ist)</param>
        /// <param name="isVisible">gibt an, ob die View angezeigt werden soll</param>
        private void createViewImage(BrailleIOScreen screen, System.Drawing.Image image, String viewName, Position position, Boolean showScrollbar, Boolean isVisible)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetBitmap(image);
            vr.ShowScrollbars = showScrollbar;
            vr.SetPadding(paddingToBoxModel(position.padding));
            vr.SetMargin(paddingToBoxModel(position.margin));
            vr.SetBorder(paddingToBoxModel(position.boarder));
            
            screen.AddViewRange(viewName, vr);
            vr.SetVisibility(isVisible);
        }


        private void createViewOtherContent(BrailleIOScreen screen, object otherContent, IBrailleIOContentRenderer renderer, String viewName, Position position, Boolean isVisible)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            BrailleIOButtonToMatrixRenderer buttonRenderer = new BrailleIOButtonToMatrixRenderer();
            vr.SetOtherContent(otherContent, renderer);
            vr.SetPadding(paddingToBoxModel(position.padding));
            vr.SetMargin(paddingToBoxModel(position.margin));
            vr.SetBorder(paddingToBoxModel(position.boarder));
         //   vr.SetVisibility(isVisible);
            screen.AddViewRange(viewName, vr);
            vr.SetVisibility(isVisible);
        }
        #endregion

        /// <summary>
        /// Wandelt <code>System.Windows.Forms.Padding</code> in <code>BrailleIO.Structs.BoxModel</code> um
        /// </summary>
        /// <param name="padding">gibt ein <code>Padding</code>-Objekt an</param>
        /// <returns>das übergebene Objekt als <code>BoxModel</code></returns>
        private BoxModel paddingToBoxModel(Padding padding)
        {
            BoxModel boxModel = new BoxModel();
            boxModel.Bottom = (uint) padding.Bottom;
            boxModel.Left = (uint)padding.Left;
            boxModel.Right = (uint)padding.Right;
            boxModel.Top = (uint)padding.Top;
            return boxModel;
        }

        /// <summary>
        /// Ermittelt den zugehörigen renderer für ein UI-Element
        /// </summary>
        /// <param name="guiElementType">gibt den Namen des UI-Elements an</param>
        /// <returns>der Renderer für das UI-Element oder null</returns>
        private static IBrailleIOContentRenderer getRenderer(UiObjectsEnum guiElementType)
        {
           switch (guiElementType.ToString())
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


        #region copy of BrailleIOExample

        #endregion


    }
}
