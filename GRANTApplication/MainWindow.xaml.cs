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


namespace GApplication
{
    
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Settings settings;
        StrategyMgr strategyMgr;
        IBrailleDisplayStrategy brailleDisplayStrategy;
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

            if (e.Key == Key.F1)
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
            }

            if (e.Key == Key.F6)
            {
                List<Strategy> posibleFilter = settings.getPossibleFilters();
                String result = "Mögliche Filter: ";
                foreach (Strategy f in posibleFilter)
                {
                    result = result + f.userName + ", ";
                }
                itemNameTextBox.Text = result;
            }
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

                                    brailleDisplayStrategy.generatedBrailleUi(treeGuiOma);
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
                        brailleDisplayStrategy.generatedBrailleUi(treeGuiOma);
                    }
                    else
                    {
                        OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals("braille123_3") || r.FilteredTree.Equals("braille123_3")); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)

                        strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                        ITreeStrategy<OSMElement.OSMElement> relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationships.BrailleTree, strategyMgr.getBrailleTree());
                        strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);
                        strategyMgr.getSpecifiedBrailleDisplay().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
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

            #region Element 1
            OSMElement.OSMElement osm1 = new OSMElement.OSMElement();
            BrailleRepresentation e1 = new BrailleRepresentation();
            e1.screenName = "screen1";
            Content c1 = new Content();
            //c1.text = "Hallo";
            e1.viewName = "v1";
            c1.fromGuiElement ="nameFiltered";
            c1.showScrollbar = true;
            e1.content = c1;
            Position p1 = new Position();
            p1.height = 8;
            p1.width = 23;
            p1.left = 0;
            p1.top = 0;
            e1.position = p1;
            GeneralProperties proper1 = new GeneralProperties();
            proper1.IdGenerated = "braille123_1";
            osm1.brailleRepresentation = e1;
            osm1.properties = proper1;
            ITreeStrategy<OSMElement.OSMElement> top = osmDau.AddChild(osm1);       
            #endregion

            #region Element 2
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "screen1";
            Content c2 = new Content();
            c2.text = "Hallo 2";
            e2.viewName = "v2";
            c2.showScrollbar = true;
            e2.content = c2;
            Position p2 = new Position();
            p2.height = 12;
            p2.width = 29;
            p2.left = 0;
            p2.top = 16;

            Padding padding = new Padding(1, 1, 1, 1);
            p2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            p2.margin = margin;
            Padding boarder = new Padding(1,1,2,1);
            p2.boarder = boarder;
            e2.position = p2;
            GeneralProperties proper2 = new GeneralProperties();
            proper2.IdGenerated = "braille123_2";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            top = top.AddChild(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            e3.screenName = "screen1";
            Content c3 = new Content();
         //   c3.text = "Start Text";
            c3.fromGuiElement = "nameFiltered";
            e3.viewName = "v3";
            e3.content = c3;
            Position p3 = new Position();
            p3.height = 20;
            p3.width = 30;
            p3.left = 0;
            p3.top = 30;
            e3.position = p3;
            GeneralProperties proper3 = new GeneralProperties();
            proper3.IdGenerated = "braille123_3";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            top = top.AddChild(osm3);
            #endregion

            #region Element 4
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            e4.screenName = "screen1";
            Content c4 = new Content();
            c4.matrix = new bool[,] { 
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
            };
            e4.viewName = "v4";
            e4.content = c4;
            Position p4 = new Position();
            p4.height = 10;
            p4.width = 20;
            p4.left = 40;
            p4.top = 50;
            e4.position = p4;
            GeneralProperties proper4 = new GeneralProperties();
            proper4.IdGenerated = "braille123_4";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            top = top.AddChild(osm4);
            #endregion
            return osmDau;
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
