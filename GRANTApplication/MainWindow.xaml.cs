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
            OperationSystemStrategy basicWindows = new OperationSystemStrategy();
            
            FilterStrategy filter = new FilterStrategy();
           
            // ... Test for F5 key.
            if (e.Key == Key.F5)
            {
                if (basicWindows.deliverCursorPosition())
                {
                    try
                    {
                        IntPtr points = basicWindows.getHWND();                        
                       Settings settings = new Settings();
                        List<Filter> possibleFilter = settings.getPosibleFilters();
                       String cUserName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
                       

                        filter.setSpecifiedFilter(settings.filterUserNameToClassName(cUserName));
                        IFilterStrategy filterStrategy =filter.getSpecifiedFilter();
                        ITree<GeneralProperties> tree = filterStrategy.filtering(basicWindows.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        StrategyManager.TreeStrategy2.printTreeElements(tree, -1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }
            if (e.Key == Key.F6)
            {
                Settings settings = new Settings();
                List<Filter> posibleFilter = settings.getPosibleFilters();
                String result = "Mögliche Filter: ";
                foreach (Filter f in posibleFilter)
                {
                    result = result + f.userName + ", ";
                }
                itemNameTextBox.Text = result;
            }

            if (e.Key == Key.F7)
            { /* Testaufruf um die Eltern eines Knotens des gespiegelten Baumes über das AutomationElement zu finden
               * Es werden die Eltern des 3. Elementes des Baumes gesucht
               */

                if (basicWindows.deliverCursorPosition())
                {
                    try
                    {
                        #region kopiert von "if (e.Key == Key.F5) ..."
                        IntPtr points = basicWindows.getHWND();

                        Settings settings = new Settings();

                        List<Filter> possibleFilter = settings.getPosibleFilters();
                        String cUserName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden


                        filter.setSpecifiedFilter(settings.filterUserNameToClassName(cUserName));
                        IFilterStrategy filterStrategy = filter.getSpecifiedFilter();
                        ITree<GeneralProperties> tree = filterStrategy.filtering(basicWindows.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        StrategyManager.TreeStrategy2.printTreeElements(tree, -1);
                        Console.WriteLine("\n");
                        #endregion

                        INode<GeneralProperties> node = tree.Nodes.ElementAt(3); //Exemplarisch rausgesuchter Knoten
                        Console.WriteLine("Node - Name: {0}, Tiefe: {1}", node.Data.nameFiltered, node.Depth);

                        ITree<GeneralProperties> tree2 = filterStrategy.getParentsOfElement(node, points); //Eigentlicher Aufruf der Suche
                        if (tree2 != null)
                        {
                            StrategyManager.TreeStrategy2.printTreeElements(tree2, -1);
                        }
                        
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
