using System;
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
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
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

                        int x;
                        int y;
                        int width;
                        int height;
                        filterStrategy.getMouseRect(points, out x, out y, out width, out height);
                        System.Windows.Rect mouseRect = new System.Windows.Rect(x, y, width, height);
                        operationSystemStrategy.paintMouseRect(mouseRect);

                        Console.WriteLine("x: " + x);
                        Console.WriteLine("y: " + y);
                        Console.WriteLine("w: " + width);
                        Console.WriteLine("h: " + height);

                        //soperationSystemStrategy.paintMouseRect(mouseRect);
                        
                        
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

                        System.IO.FileStream fs = System.IO.File.Create("c:\\Users\\mkarlapp\\Desktop\\test.xml");
                        tree.XmlSerialize(fs);
                        fs.Close();

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
            {/*Beispiel zum Lesenaus XML
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
              * Beispiel-Datei liegt mit im git
              * */
                try
                {
                    brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
                    brailleDisplayStrategy.initializedSimulator();


                 //   System.IO.FileStream fs = System.IO.File.Open("c:\\Users\\mkarlapp\\Desktop\\testBraille2.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    System.IO.FileStream fs = System.IO.File.Open("..\\..\\..\\testBraille2.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ITreeStrategy<OSMElement.OSMElement> osm = treeStrategy.XmlDeserialize(fs);
                    fs.Close();
                    treeStrategy.printTreeElements(osm, -1);

                    brailleDisplayStrategy.generatedBrailleUi(osm);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }

        }
    }

}

