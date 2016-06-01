using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;

using BrailleIO;
using Gestures.Recognition;
using StrategyManager.Interfaces;
using StrategyManager;
using OSMElement;


namespace StrategyBrailleIO
{
    public class BrailleDisplayStrategyBrailleIO : IBrailleDisplayStrategy
    {
       IBrailleIOShowOffMonitor monitor;
        BrailleIOMediator brailleIOMediator;
        AbstractBrailleIOAdapterBase showOffAdapter;
        //GestureRecognizer showOffGestureRecognizer;
        //AbstractBrailleIOAdapterManagerBase brailleDisAdapter; //TODO       evtl. beides in Listen u.ä.
        //GestureRecognizer brailleDisRecognizer; //TODO

        private StrategyMgr strategyMgr;
        public StrategyMgr getStrategyMgr() { return strategyMgr; }
        public void setStrategyMgr(StrategyMgr manager)
        {
            strategyMgr = manager;
        }

        /// <summary>
        /// Erstellt, sofern noch nicht vorhanden, ein simulator für das Ausgabegerät
        /// </summary>
        public void initializedSimulator()
        {
            if (brailleIOMediator == null )
            {
                brailleIOMediator = BrailleIOMediator.Instance;
                brailleIOMediator.AdapterManager = new ShowOffBrailleIOAdapterManager();
                monitor = ((ShowOffBrailleIOAdapterManager)brailleIOMediator.AdapterManager).Monitor;
                showOffAdapter = brailleIOMediator.AdapterManager.ActiveAdapter as AbstractBrailleIOAdapterBase;
                
            }
        }


        /// <summary>
        /// Erstellt die GUI auf der Stiftplatte
        /// </summary>
        /// <param name="osm">gibt das Baum-Objekt der Oberflaeche an</param>
        public void generatedBrailleUi(ITreeStrategy<OSMElement.OSMElement> osm)
        {
            createViews(osm);
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
               //  Console.WriteLine();
                // der screen existiert schon -> ok
            }
            catch
            {
                brailleIOMediator.AddView(screenName, new BrailleIOScreen());
            }
        }

        /// <summary>
        /// Konvertiert eine Bool-Matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private bool[,] convertBoolmatrix(bool[][] matrix)
        {
            int length = 0;
            if (matrix.Length > 0)
            {
                length = matrix[0].Length;
            }
            bool[,] convertetMatrix = new bool[matrix.Length, length];
            for (int zeile = 0; zeile < matrix.Length; zeile++)
            {
                for (int zelle = 0; zelle < matrix[0].Length; zelle++)
                {
                    convertetMatrix[zeile, zelle] = matrix[zeile][zelle];
                }
            }

            return convertetMatrix;
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
            bmp = ScreenCapture.CaptureWindow(new IntPtr(node.Data.hWndFiltered), h, w, 0, 0, 0, 0);

            return bmp;
        }

        /// <summary>
        /// Geht rekursive durch alle Baumelemente und erstellt die einzelnen Views
        /// </summary>
        /// <param name="osm">gibt das Baum-Objekt der Oberflaeche an</param>
        private void createViews(ITreeStrategy<OSMElement.OSMElement> osm)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;
            while (osm.HasChild && !(osm.Count == 1 && osm.Depth == -1))
            {
                if (osm.HasChild)
                {
                    node1 = osm.Child;
                    if (!node1.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                    {
                        createScreen(node1.Data.brailleRepresentation.screenName);
                        createViews(node1.Data);
                    }
                    createViews(node1);
                }
            }
            if(osm.Count == 1 && osm.Depth == -1){
                if (!osm.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createScreen(osm.Data.brailleRepresentation.screenName);
                    createViews(osm.Data);
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

        /*    foreach (INode<OSMElement.OSMElement> node in ((ITree<OSMElement.OSMElement>)osm).All.Nodes)
            {
                if (!node.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createScreen(node.Data.brailleRepresentation.screenName);
                    createViews(node.Data.brailleRepresentation, treeStrategy);
                }
            }*/
        }

        /// <summary>
        /// Erstellt aus einer <code>BrailleRepresentation</code> die entsprechende View
        /// </summary>
        /// <param name="brailleRepresentation">gibt die Darstellung des GUI-Objektes fuer die Stiftplatte an</param>
        private void createViews(OSMElement.OSMElement osmElement)
        {
            OSMElement.BrailleRepresentation brailleRepresentation = osmElement.brailleRepresentation;
            if (brailleRepresentation.content.text != null && !brailleRepresentation.content.text.Equals("")) //TODO: Views von leeren Textfeldern
            {
                createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, brailleRepresentation.content.text, brailleRepresentation.content.viewName, brailleRepresentation.position);
                return;
            }
            if (brailleRepresentation.content.fromGuiElement != null && !brailleRepresentation.content.fromGuiElement.Equals("") )
            {
              //  osmRelationship.OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.Second.Equals(osmElement.properties.IdGenerated) || r.First.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)

              //  strategyMgr.getSpecifiedFilter().updateNodeOfMirroredTree(osmRelationships.First);   //nur testweise
                osmRelationship.OsmRelationship<String, String> osmRelationship = strategyMgr.getOsmRelationship().Find(r => r.Second.Equals(osmElement.properties.IdGenerated) || r.First.Equals(osmElement.properties.IdGenerated)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)
                if (osmRelationship == null)
                {
                    Console.WriteLine("kein passendes objekt gefunden");
                    return;
                }
                ITreeStrategy<OSMElement.OSMElement> associatedNode = strategyMgr.getSpecifiedTree().getAssociatedNode(osmRelationship.First);
                String text = "";
                if (associatedNode != null)
                {
                    object objectText = OSMElement.Helper.getGeneralPropertieElement(brailleRepresentation.content.fromGuiElement, associatedNode.Data.properties);
                    text = (objectText != null ? objectText.ToString() : "");
                }
                createViewText(brailleIOMediator.GetView(brailleRepresentation.screenName) as BrailleIOScreen, text, brailleRepresentation.content.viewName, brailleRepresentation.position);
                return;
            }
           //TODO
        }



        #region create Views       
        /// <summary>
        /// Erstellt eine View mit einem Text
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="text">gibt den anzuzeigenden Text an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        private void createViewText(BrailleIOScreen screen, String text, String viewName, Position position)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetText(text);
          //  vr.ShowScrollbars = true;
         //   vr.SetBorder(position.boarder);
          //  vr.SetMargin(position.margin);
           // vr.SetPadding(position.padding);

            screen.AddViewRange(viewName, vr);
        }

        /// <summary>
        /// Erstellt eine View mit einer Bool-Matrix
        /// </summary> //TODO: + weitere Eigenschaften
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="matrix">gibt die bool-Matrix an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        private void createViewMatrix(BrailleIOScreen screen, bool[,] matrix, String viewName, Position position)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetMatrix(matrix);
            screen.AddViewRange(viewName, vr);
        }


        /// <summary>
        /// Erstellt eine View mit einem Bild
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="image">gibt das Bild an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="position">gibt die position des Objektest an</param>
        private void createViewImage(BrailleIOScreen screen, System.Drawing.Image image, String viewName, Position position)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(position.left, position.top, position.width, position.height, new bool[0, 0]);
            vr.SetBitmap(image);
            screen.AddViewRange(viewName, vr);
        }
        #endregion

        #region copy of BrailleIOExample

        #endregion

    }
}
