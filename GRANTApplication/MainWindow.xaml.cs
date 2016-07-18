using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using StrategyManager;
using StrategyManager.Interfaces;
using StrategyGenericTree;
using OSMElement;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using OSMElement.UiElements;

namespace GRANTApplication
{
    
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings;
        StrategyMgr strategyMgr;
        IBrailleDisplayStrategy brailleDisplayStrategy;
        GUIInspector GuiInspector;
        bool wopen = false;
        
        //private PaintEventHandler Paint;


        public MainWindow()
        {

            InitializeComponent();
            InitializeFilterComponent();
        }
        
        private void InitializeFilterComponent()
        {
            settings = new Settings();
            strategyMgr = new StrategyMgr();
            GuiInspector = new GUIInspector();

            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();
            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);
            ITreeStrategy<OSMElement.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();
            List<Strategy> possibleFilter = settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
            brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
            brailleDisplayStrategy.setStrategyMgr(strategyMgr);

            Type to = typeof(TreeOperations<OSMElement.OSMElement>);
            Console.WriteLine("Type: " + to.Assembly.FullName.ToString());
            Console.WriteLine("Type: " + to.AssemblyQualifiedName.ToString());
            strategyMgr.setSpecifiedTreeOperations(settings.getPossibleTreeOperations()[0].className);
            strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);
        }

      
        private void OpenNewWindow(object sender, RoutedEventArgs e)
         {
           GUIInspector window1 = new GUIInspector();
            GuiInspector.ShowInTaskbar = true;
            GuiInspector.Owner = App.Current.MainWindow;
            window1.ShowDialog();
         }
      

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (wopen == false)
            {
                if (!GuiInspector.IsLoaded)
                {
                    //GuiInspector.Topmost = true;
                    //GuiInspector.Show();
                    GuiInspector.ShowInTaskbar = false;
                    GuiInspector.Owner = App.Current.MainWindow;
                    GuiInspector.Show();

                }
                
            }//e.Handled = true;
            //this.Close();

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Title = "Checked";
            this.GuiInspector.Topmost=true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Title = "Unchecked";
        }

        private void CheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            this.Title = "Indeterminate";
        }


        /* private void Window_GotFocus(object sender, RoutedEventArgs e)
         {
             var mainWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;

             mainWindow.Topmost = true;
             this.Topmost = true;

         }*/
        /*private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            GuiInspector.Close();
            //radioButton_check.IsChecked = false;
        }*/
        /*
        private void Window_Paint(object sender, PaintEventArgs e)
        {
            Console.WriteLine("oder HIIIIIER !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: ");
            GetPixel_Example(e);
        }

        private void GetPixel_Example(PaintEventArgs e)
        {
            
            Console.WriteLine("HIIIIIER !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!: ");
           
            //e.Graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Color.Red, 5), 10,10,10,10);
            //e.Graphics.Dispose();
        }
        */

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();
           
           ITreeStrategy<OSMElement.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();


            // ... Test for F5 key.
/*
            if (e.Key == Key.F5)
            {
                if (operationSystemStrategy.deliverCursorPosition())
                {
                    try
                    {
                        IntPtr points = operationSystemStrategy.getHWND();
                        List<Strategy> possibleFilter = settings.getPossibleFilters();
                        String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden

                        strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        filterStrategy.setStrategyMgr(strategyMgr);
                        //ITreeStrategy<OSMElement.OSMElement> tree1 = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                       // strategyMgr.setFilteredTree(tree1);
                         int pointX;
                         int pointY;
                         operationSystemStrategy.getCursorPoint(out pointX, out pointY);
                         ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Application, 0);
                        strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }

            */
           /* if (e.Key == Key.F1)
            {
                if (operationSystemStrategy.deliverCursorPosition())
                {
                    try
                    {

                       IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                        int pointX;
                        int pointY;

                        operationSystemStrategy.getCursorPoint(out pointX, out pointY);
                        
                        Console.WriteLine("Pointx: " + pointX);
                        Console.WriteLine("Pointy: " + pointY);
                       
                        OSMElement.OSMElement osmElement = filterStrategy.setOSMElement(pointX, pointY);
                        Rectangle rect = operationSystemStrategy.getRect(osmElement);

                        // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint);
                        operationSystemStrategy.paintRect(rect);
    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }*/

      
            if (e.Key == Key.F8)
            { /* Testaufruf: suche nach eigenschaften im Baum
               */

                if (operationSystemStrategy.deliverCursorPosition())
                {
                    try
                    {
                        #region kopiert von "if (e.Key == Key.F5) ..."
                        IntPtr points = operationSystemStrategy.getHWND();

                        List<Strategy> possibleFilter = settings.getPossibleFilters();
                        String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden

                        strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        filterStrategy.setStrategyMgr(strategyMgr);
                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion

                        GeneralProperties searchedProperties = new GeneralProperties();
                        searchedProperties.localizedControlTypeFiltered = "Schaltfläche";
                        //  searchedProperties.nameFiltered = "";

                        Console.Write("Gesuchte Eigenschaften ");
                        strategyMgr.getSpecifiedTreeOperations().searchProperties(tree, searchedProperties, OperatorEnum.or);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }
            if (e.Key == Key.F9)
            {/* Beispiel zum Setzen von einer Beziehung von unserer GUI zur BrailleGUI, dazu muss sich die Maus über dem gewünschten GUI-Element befinden --> es wird nameFiltered angezeigt
              * 
              * Nach dem ein Baum gefiltert wurde (F5) kann mittels (F9) die Beziehung zu dem Element gesetzt werden und dann mittels (F2) auf dem BrailleDis angezeigt werden
              * Soll die selbe Anzeige aktualisiert werden, so kann nochmal (F2) gedrückt werden
              * Soll ein anderes Element der selben Anwendung angezeigt werden, so muss erst wieder (F9) und dann (F2) gedrückt werden
              * Um ein Element einer anderen Anwendung anzuzeigen muss erst (F5), dann (F9) und anschließend (F2)  gedrückt werden
              * */

                if (operationSystemStrategy.deliverCursorPosition())
                {
                    try
                    {

                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        filterStrategy.setStrategyMgr(strategyMgr);
                        ITreeStrategy<OSMElement.OSMElement> treeGuiOma = getDauGui();
                        strategyMgr.setBrailleTree(treeGuiOma);
                        
                            #region kopiert von "if (e.Key == Key.F5) ..."
                            if (operationSystemStrategy.deliverCursorPosition())
                            {
                                try
                                {
                                    IntPtr points = operationSystemStrategy.getHWND();
                                    List<Strategy> possibleFilter = settings.getPossibleFilters();
                                    String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden

                                    strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                                    // IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                                    ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                                    strategyMgr.setFilteredTree(tree);
                                    // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                                    //  treeStrategy.printTreeElements(tree, -1);
                                    brailleDisplayStrategy.initializedSimulator();
                                    brailleDisplayStrategy.initializedBrailleDisplay();

                                    brailleDisplayStrategy.generatedBrailleUi();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: '{0}'", ex);
                                }
                            }

                            #endregion

                            int pointX;
                            int pointY;

                        operationSystemStrategy.getCursorPoint(out pointX, out pointY);
                        OSMElement.OSMElement osmElement = filterStrategy.setOSMElement(pointX, pointY);

                        List<OsmRelationship<String, String>> relationship = setOsmRelationship(osmElement.properties.IdGenerated);
                        strategyMgr.setOsmRelationship(relationship);


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }
            if (e.Key == Key.F4)
            {/*Beispiel zum Lesen aus XML
              * Achtung: Pfad muss für jeden angepasst werden und die Datei muss schon existieren
              * */
                try
                {
                    System.IO.FileStream fs = System.IO.File.Open("c:\\Users\\mkarlapp\\Desktop\\testGui.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ITreeStrategy<OSMElement.OSMElement> tree3 = treeStrategy.XmlDeserialize(fs);
                    fs.Close();
                    strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree3, -1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
            if (e.Key == Key.F2)
            {/*Beispiel BrailleDis 
              * 
              * */
                try
                {

                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                    filterStrategy.setStrategyMgr(strategyMgr);
                    //treeStrategy.setStrategyMgr(strategyMgr);
                    ITreeStrategy<OSMElement.OSMElement> treeGuiOma = getDauGui();
                    strategyMgr.setBrailleTree(treeGuiOma);
                   if (strategyMgr.getFilteredTree() == null)
                    {
                        #region kopiert von "if (e.Key == Key.F5) ..."
                        if (operationSystemStrategy.deliverCursorPosition())
                        {
                            try
                            {
                                IntPtr points = operationSystemStrategy.getHWND();
                                List<Strategy> possibleFilter = settings.getPossibleFilters();
                                String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden

                                strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                               // IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                                ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                                strategyMgr.setFilteredTree(tree);
                                // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                                //  treeStrategy.printTreeElements(tree, -1);
                                brailleDisplayStrategy.initializedSimulator();
                                brailleDisplayStrategy.initializedBrailleDisplay();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: '{0}'", ex);
                            }
                        }
                       
                        #endregion
                        if (strategyMgr.getOsmRelationship() == null)
                        {
                            List<OsmRelationship<String, String>> relationship = setOsmRelationship();
                            strategyMgr.setOsmRelationship(relationship);
                        }
                        brailleDisplayStrategy.generatedBrailleUi();
                    }
                    else
                    {
                        OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals("braille123_3") || r.FilteredTree.Equals("braille123_3")); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)

                        strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                        ITreeStrategy<OSMElement.OSMElement> relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationships.BrailleTree, strategyMgr.getBrailleTree());
                        strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);
                        strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
                        brailleDisplayStrategy.updateViewContent(relatedBrailleTreeObject.Data);
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }

        }

      

        #region Beispielobjekte
        private ITreeStrategy<OSMElement.OSMElement> getDauGui()
        {
            ITreeStrategy<OSMElement.OSMElement> osmDau = strategyMgr.getSpecifiedTree().NewNodeTree();

            #region Element 2
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            GeneralProperties proper2 = new GeneralProperties();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "screen1";
            e2.text = "Hallo 1 Hallo 2 Hallo 3 Hallo 4 Hallo 5";
            //  c2.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e2.showScrollbar = true;
            e2.viewName = "v2";
            e2.isVisible = false;
            Rect p2 = new Rect(42, 90, 29, 15);
            /* p2.Height = 15;
             p2.Width = 29;
             p2.Left = 90;
             p2.Top = 42;*/
            proper2.boundingRectangleFiltered = p2;
            Padding padding = new Padding(1, 1, 1, 1);
            e2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            e2.margin = margin;
            Padding boarder = new Padding(1, 1, 2, 1);
            e2.boarder = boarder;

            proper2.IdGenerated = "braille123_2";
            proper2.controlTypeFiltered = "Text";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            GeneralProperties proper3 = new GeneralProperties();
            e3.screenName = "screen1";
            e3.showScrollbar = false;
            //   c3.text = "Start Text";
            //c3.fromGuiElement = "nameFiltered";
            e3.fromGuiElement = "nameFiltered";
            e3.viewName = "v3";
            e3.isVisible = false;
            Rect p3 = new Rect(30, 70, 30, 20);
            /*p3.Height = 20;
            p3.Width = 30;
            p3.Left = 70;
            p3.Top = 30;*/
            proper3.boundingRectangleFiltered = p3;

            proper3.IdGenerated = "braille123_3";
            proper3.controlTypeFiltered = "Text";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm3);
            #endregion

            #region Element 4
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            GeneralProperties proper4 = new GeneralProperties();
            e4.screenName = "screen1";
            e4.matrix = new bool[,] { 
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
            };
            e4.viewName = "v4";
            Rect p4 = new Rect(50, 40, 20, 7);
            /*   p4.Height = 7;
               p4.Width = 20;
               p4.Left = 40;
               p4.Top = 50;*/
            proper4.boundingRectangleFiltered = p4;
            proper4.IdGenerated = "braille123_4";
            proper4.controlTypeFiltered = "Matrix";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm4);
            #endregion

            #region Element 5
            OSMElement.OSMElement osm5 = new OSMElement.OSMElement();
            BrailleRepresentation e5 = new BrailleRepresentation();
            GeneralProperties proper5 = new GeneralProperties();
            e5.screenName = "screen1";
            e5.text = "Button";
            // c5.uiElementContent = "Hallo - Button";
            // c5.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e5.viewName = "v5";
            e5.isVisible = true;
            Rect p5 = new Rect(30, 55, 24, 9);
            /*  p5.Height = 9;
              p5.Width = 24;
              p5.Left = 55;
              p5.Top = 30;*/
            proper5.boundingRectangleFiltered = p5;
            proper5.IdGenerated = "braille123_5";
            proper5.controlTypeFiltered = "Button";
            osm5.brailleRepresentation = e5;
            osm5.properties = proper5;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm5);
            #endregion

            #region Element 6
            OSMElement.OSMElement osm6 = new OSMElement.OSMElement();
            BrailleRepresentation e6 = new BrailleRepresentation();
            GeneralProperties proper6 = new GeneralProperties();
            e6.screenName = "screen1";
            e6.showScrollbar = true;
            // c6.text = "Text 1 Text 2 Text 3 Text 4 Text 5 Text 6";
            //object[] otherContent6 = { UiObjectsEnum.TextBox, textBox6 };
            e6.fromGuiElement = "nameFiltered";
            //c6.showScrollbar = true;

            e6.viewName = "v6";
            e6.isVisible = true;
            Rect p6 = new Rect(29, 0, 50, 28);
            /* p6.Height = 28;
             p6.Width = 50;
             p6.Left = 0;
             p6.Top = 29;*/
            proper6.boundingRectangleFiltered = p6;
            proper6.IdGenerated = "braille123_6";
            proper6.controlTypeFiltered = "TextBox";
            osm6.brailleRepresentation = e6;
            osm6.properties = proper6;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm6);
            #endregion

            return strategyMgr.getBrailleTree();
        }

        private List<OsmRelationship<String, String>> setOsmRelationship(String guiID)
        {
            List<OsmRelationship<String, String>> relationships = new List<OsmRelationship<String, String>>();
            OsmRelationship<String, String> r3 = new OsmRelationship<String, String>();
            r3.FilteredTree = guiID;
            r3.BrailleTree = "braille123_3";

            relationships.Add(r3);
            return relationships;
        }

        private List<OsmRelationship<String, String>> setOsmRelationship()
        {
            List<OsmRelationship<String, String>> relationships = new List<OsmRelationship<String, String>>();
            OsmRelationship<String, String> r1 = new OsmRelationship<String, String>();
            r1.FilteredTree = "461FD37218F2E2BCBE4C5486629A2FC6"; //Notepad;
            r1.BrailleTree = "braille123_1";
            OsmRelationship<String, String> r2 = new OsmRelationship<String, String>();
            r2.FilteredTree = "gui123_2";
            r2.BrailleTree = "braille123_2";
            OsmRelationship<String, String> r3 = new OsmRelationship<String, String>();
            r3.FilteredTree = "6941463181BDAA498DBC02B4164EF1AA";
            r3.BrailleTree = "braille123_3";

           relationships.Add(r1);
           // relationships.Add(r2);
            relationships.Add(r3);
            return relationships;
        }



        #endregion

      
    }
}

/*Form topMostForm = new Form();
                       // Set the size of the form larger than the default size.
                       topMostForm.Size = new System.Drawing.Size(300, 300);
                       // Set the position of the top most form to center of screen.
                       topMostForm.StartPosition = FormStartPosition.CenterScreen;
                       // Display the form as top most form.
                       topMostForm.TopMost = true;
                       topMostForm.Show();
                       */
