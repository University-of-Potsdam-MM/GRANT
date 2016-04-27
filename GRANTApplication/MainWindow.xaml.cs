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

namespace GApplication
{
    
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Settings settings = new Settings();

            StrategyMgr strategyMgr = new StrategyMgr();

            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();
            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);

            ITreeStrategy<GeneralProperties> treeStrategy = strategyMgr.getSpecifiedTree();
            
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

                        ITreeStrategy<GeneralProperties> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                       // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                        treeStrategy.printTreeElements(tree, -1);
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

                        ITreeStrategy<GeneralProperties> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion
                        ITreeStrategy<GeneralProperties> node = (ITreeStrategy<GeneralProperties>)((ITree<GeneralProperties>)tree).Nodes.ElementAt(3);  //Exemplarisch rausgesuchter Knoten
                        Console.WriteLine("Gesuchter Knoten:\nNode - Name: {0}, Tiefe: {1}", node.Data.nameFiltered, node.Depth);

                        ITreeStrategy<GeneralProperties> tree2 = filterStrategy.getParentsOfElement(node, points); //Eigentlicher Aufruf der Suche
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

                        ITreeStrategy<GeneralProperties> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion

                        GeneralProperties searchedProperties = new GeneralProperties();
                        searchedProperties.localizedControlTypeFiltered = "Bildlaufleiste";
                        searchedProperties.nameFiltered = "";
                        Console.Write("Gesuchte Eigenschaften ");
                        treeStrategy.searchProperties(tree, searchedProperties, OperatorEnum.or);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
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

                        ITreeStrategy<GeneralProperties> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        treeStrategy.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion
                        System.IO.FileStream fs = System.IO.File.Create("c:\\Users\\mkarlapp\\Desktop\\test2.xml");
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
                    System.IO.FileStream fs = System.IO.File.Open("c:\\Users\\mkarlapp\\Desktop\\test2.xml", System.IO.FileMode.Open, System.IO.FileAccess.Read);
                    ITreeStrategy<GeneralProperties> tree3 = treeStrategy.XmlDeserialize(fs);
                    fs.Close();
                    treeStrategy.printTreeElements(tree3, -1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }
    }
}

}

