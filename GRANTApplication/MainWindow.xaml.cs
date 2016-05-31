﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StrategyManager;
using StrategyManager.Interfaces;
using StrategyGenericTree;

using System.Windows.Automation;
using StrategyBrailleIO;
using OSMElement;


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
            treeStrategy.setStrategyMgr(strategyMgr);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {



            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();
            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className); // muss dynamisch ermittelt werden
                                                                      // strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

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

                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                        treeStrategy.printTreeElements(tree, -1);
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
                        IntPtr points = operationSystemStrategy.getHWND();
                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        Rect mouseRect = filterStrategy.getMouseRect(points);
                        operationSystemStrategy.paintMouseRect(mouseRect);
                        //AutomationElement element = filterStrategy.deliverAutomationElementFromHWND(points);
                        //ITreeStrategy<GeneralProperties> treeStrategy = strategyMgr.getSpecifiedTree();
                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        //treeStrategy.printTreeElements(tree, -1);
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
            if (e.Key == Key.F7)
            { /* Testaufruf um die Eltern eines Knotens des gespiegelten Baumes über das AutomationElement zu finden
               * Es werden die Eltern des 3. Elementes des Baumes gesucht
               * 
               * Hier wird teilweise direkt auf Methoden der Klasse TreeStrategyGenericTree zugegriffen
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

                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion
                        
                        ITreeStrategy<OSMElement.OSMElement> node = (ITreeStrategy<OSMElement.OSMElement>)((ITree<OSMElement.OSMElement>)tree).Nodes.ElementAt(3);  //Exemplarisch rausgesuchter Knoten
                        Console.WriteLine("Gesuchter Knoten:\nNode - Name: {0}, Tiefe: {1}", node.Data.properties.nameFiltered, node.Depth);

                        ITreeStrategy<OSMElement.OSMElement> tree2 = filterStrategy.getParentsOfElement(node, points); //Eigentlicher Aufruf der Suche
                        if (tree2 != null)
                        {
                            treeStrategy.printTreeElements(tree2, -1);
                            Console.WriteLine();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
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

                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion

                        GeneralProperties searchedProperties = new GeneralProperties();
                        searchedProperties.localizedControlTypeFiltered = "Schaltfläche";
                        //  searchedProperties.nameFiltered = "";

                        Console.Write("Gesuchte Eigenschaften ");
                        treeStrategy.searchProperties(tree, searchedProperties, OperatorEnum.or);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }
            if (e.Key == Key.F9)
            {/* Beispiel zum Schreiben in Datei
              * Achtung: Pfad muss für jeden angepasst werden
              * */

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

                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, 3);
                        Console.WriteLine("\n");
                        #endregion
                        System.IO.FileStream fs = System.IO.File.Create("c:\\Users\\mkarlapp\\Desktop\\testGui.xml");
                        tree.XmlSerialize(fs);
                        fs.Close();

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
                    treeStrategy.printTreeElements(tree3, -1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
            if (e.Key == Key.F2)
            {/*Beispiel BrailleDiss 
              * 
              * */
                try
                {

                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                    filterStrategy.setStrategyMgr(strategyMgr);
                    treeStrategy.setStrategyMgr(strategyMgr);
                   if (strategyMgr.getMirroredTree() == null)
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
                                strategyMgr.setMirroredTree(tree);
                                // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                                //  treeStrategy.printTreeElements(tree, -1);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: '{0}'", ex);
                            }
                        }
                       
                        #endregion
                        
                    }
                    else
                    {
                        osmRelationship.OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.Second.Equals("braille123_3") || r.First.Equals("braille123_3")); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)

                        strategyMgr.getSpecifiedFilter().updateNodeOfMirroredTree(osmRelationships.First);
                    }

                    brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
                    brailleDisplayStrategy.initializedSimulator();
                    


                    IBrailleDisplayStrategy display = strategyMgr.getSpecifiedBrailleDisplay();
                    display.setStrategyMgr(strategyMgr);
                    ITreeStrategy<OSMElement.OSMElement> treeGuiOma = getDauGui();
                    List<osmRelationship.OsmRelationship<String, String>> relationship = setOsmRelationship();
                    strategyMgr.setOsmRelationship(relationship);
                    brailleDisplayStrategy.generatedBrailleUi(treeGuiOma);
                    
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
            c1.text = "Hallo";
            c1.viewName = "v1";
            e1.content = c1;
            Position p1 = new Position();
            p1.height = 8;
            p1.width = 20;
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
            c2.viewName = "v2";
            e2.content = c2;
            Position p2 = new Position();
            p2.height = 8;
            p2.width = 20;
            p2.left = 0;
            p2.top = 16;
            e2.position = p2;
            GeneralProperties proper2 = new GeneralProperties();
            proper2.IdGenerated = "braille123_2";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            ITreeStrategy<OSMElement.OSMElement> child = top.AddChild(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            e3.screenName = "screen1";
            Content c3 = new Content();
            c3.fromGuiElement = "valueFiltered";
            c3.viewName = "v3";
            e3.content = c3;
            Position p3 = new Position();
            p3.height = 8;
            p3.width = 20;
            p3.left = 0;
            p3.top = 30;
            e3.position = p3;
            GeneralProperties proper3 = new GeneralProperties();
            proper3.IdGenerated = "braille123_3";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            top = top.AddChild(osm3);
            #endregion

            return osmDau;
        }

        private List<osmRelationship.OsmRelationship<String, String>> setOsmRelationship()
        {
            List<osmRelationship.OsmRelationship<String, String>> relationships = new List<osmRelationship.OsmRelationship<String, String>>();
            osmRelationship.OsmRelationship<String, String> r1 = new osmRelationship.OsmRelationship<String, String>();
            r1.First = "gui123_1";
            r1.Second = "braille123_1";
            osmRelationship.OsmRelationship<String, String> r3 = new osmRelationship.OsmRelationship<String, String>();
            r3.First = "gui123_3";
            r3.Second = "braille123_3";
            osmRelationship.OsmRelationship<String, String> r2 = new osmRelationship.OsmRelationship<String, String>();
            r2.First = "gui123_2";
            r2.Second = "braille123_2";
           // relationships.Add(r1);
           // relationships.Add(r2);
            relationships.Add(r3);
            return relationships;
        }

        #endregion

    }

}

