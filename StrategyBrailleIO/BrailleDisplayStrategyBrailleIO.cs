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
        /// Erstellt die iniziale UI auf der Striftplatte
        /// </summary>
        /// <param name="xmlObject">gibt die (ausgelesene) Beschreibung der GUI an</param>
        /// <param name="tree">gibt den zugehörigen Baum an</param>
        /// <param name="treeStrategy">enthält die verwendete Tree-Strategy</param> //TODO. sollte nicht jedesmal übergeben werden
        public void generatedBrailleUi(XMLDevice xmlObject, ITreeStrategy<OSMElement.OSMElement> tree, ITreeStrategy<OSMElement.OSMElement> treeStrategy)
        {
            createScreens(xmlObject.Screens);
        //    createViews(xmlObject.Objects, tree, treeStrategy); TODO
        }

        /// <summary>
        /// Erstellt alle potenziell anzuzeigenden Screens
        /// </summary>
        /// <param name="screens"></param>
        private void createScreens(String[] screens)
        {
            foreach (String s in screens)
            {
                brailleIOMediator.AddView(s, new BrailleIOScreen());
            }
        }

        private bool isFixedTextFirst(XMLDeviceObjectText order)
        {
            //TODO
            return false;
        }

        private String getDynamicTextFromTree(ITreeStrategy<GeneralProperties> tree, String generatedId, ITreeStrategy<GeneralProperties> treeStrategy)
        { //nur Beispielhaft -> so nicht verwenden
            
            GeneralProperties propertie = new GeneralProperties();
            propertie.IdGenerated = generatedId;
            ITreeStrategy<GeneralProperties> node = treeStrategy.searchProperties(tree, propertie, OperatorEnum.and)[0];
            return node.Data.nameFiltered;
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
        /// Erstellt aus einem angegeben (XML-)Objekt die entsprechenden View
        /// </summary>
        /// <param name="xmlViews">Enthält die Beschreibung, wie die Views angeordnet werden sollen</param>
        /// <param name="tree">gibt den zugehörigen Baum der gefilterten Anwendung an</param>
        /// <param name="treeStrategy">enthält die verwendete Tree-Strategy</param> //TODO. sollte nicht jedesmal übergeben werden
        private void createViews(XMLDeviceObject[] xmlViews, ITreeStrategy<GeneralProperties> tree, ITreeStrategy<GeneralProperties> treeStrategy)
        {
            XMLDeviceObjectText textObject = new XMLDeviceObjectText();
            foreach (XMLDeviceObject o in xmlViews)
            {
                if (o.Text != null && !o.Text.Equals(textObject))
                {
                    String text;
                    if (isFixedTextFirst(o.Text))
                    {
                        text = o.Text.fix + getDynamicTextFromTree(tree, o.GeneratedId, treeStrategy);
                    }
                    else
                    {
                        //TODO: auslesen lassen
                        text = getDynamicTextFromTree(tree, o.GeneratedId, treeStrategy) + o.Text.fix;
                    }
                    createViewText(brailleIOMediator.GetView(o.Screen) as BrailleIOScreen, text, o.Position.View, o.Position.ViewRange.Left, o.Position.ViewRange.Top, o.Position.ViewRange.Width, o.Position.ViewRange.Height);
                }
                else
                {
                    bool[][] a = new bool[0][];

                    if (o.Matrix != null && !Enumerable.SequenceEqual(o.Matrix, new bool[0][]))
                    {
                        bool[,] matrix = convertBoolmatrix(o.Matrix);
                        createViewMatrix(brailleIOMediator.GetView(o.Screen) as BrailleIOScreen, matrix, o.Position.View, o.Position.ViewRange.Left, o.Position.ViewRange.Top, o.Position.ViewRange.Width, o.Position.ViewRange.Height);
                    }
                    else
                    {
                        if (o.Bitmap != null)
                        {
                            if (o.Bitmap.guiObject)
                            {
                                GeneralProperties propertie = new GeneralProperties();

                                propertie.IdGenerated = o.GeneratedId;
                                ITreeStrategy<GeneralProperties> tree2 = treeStrategy.searchProperties(tree, propertie, OperatorEnum.and)[0]; //Achtung kann Fehler verursachen
                                createViewImage(brailleIOMediator.GetView(o.Screen) as BrailleIOScreen, captureScreen(tree2), o.Position.View, o.Position.ViewRange.Left, o.Position.ViewRange.Top, o.Position.ViewRange.Width, o.Position.ViewRange.Height);
                            }
                        }
                    }
                }
            }
        }

        #region create Views
        /// <summary>
        /// Erstellt eine Text-View
        /// </summary> //TODO: + weitere Eigenschaften
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="text">gibt den anzuzeigenden Text an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="left">gibt den Versatz nach links an</param>
        /// <param name="top">gibt den Versatz nach oben an</param>
        /// <param name="width">gibt die Breite an</param>
        /// <param name="height">gibt dei Höhe an</param>
        private void createViewText(BrailleIOScreen screen, String text, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetText(text);
            vr.ShowScrollbars = true;
            //vr.SetPadding(2);
            //vr.SetMargin(2);
            //vr.SetBorder(1,0);

            screen.AddViewRange(viewName, vr);
        }

        /// <summary>
        /// Erstellt eine View mit einer Bool-Matrix
        /// </summary> //TODO: + weitere Eigenschaften
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="matrix">gibt die bool-Matrix an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="left">gibt den Versatz nach links an</param>
        /// <param name="top">gibt den Versatz nach oben an</param>
        /// <param name="width">gibt die Breite an</param>
        /// <param name="height">gibt dei Höhe an</param>
        private void createViewMatrix(BrailleIOScreen screen, bool[,] matrix, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetMatrix(matrix);

            screen.AddViewRange(viewName, vr);
        }


        /// <summary>
        /// Erstellt eine View mit einem Bild
        /// </summary>
        /// <param name="screen">gibt den <code>BrailleIOScreen</code> an, auf dem die View angezeigt werden </param>
        /// <param name="image">gibt das Bild an</param>
        /// <param name="viewName">gibt den Namen der view an</param>
        /// <param name="left">gibt den Versatz nach links an</param>
        /// <param name="top">gibt den Versatz nach oben an</param>
        /// <param name="width">gibt die Breite an</param>
        /// <param name="height">gibt dei Höhe an</param>
        private void createViewImage(BrailleIOScreen screen, System.Drawing.Image image, String viewName, int left, int top, int width, int height)
        {
            BrailleIOViewRange vr = new BrailleIOViewRange(left, top, width, height, new bool[0, 0]);
            vr.SetBitmap(image);
            screen.AddViewRange(viewName, vr);
        }
        #endregion

        #region copy of BrailleIOExample

        #endregion

    }
}
