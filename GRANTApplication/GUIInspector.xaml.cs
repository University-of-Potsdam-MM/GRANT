using System;
using System.Windows;
using System.Windows.Input;
using StrategyManager;
using StrategyManager.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace GRANTApplication
{
    /// <summary>
    /// Interaktionslogik für GUIInspector.xaml
    /// </summary>
    public partial class GUIInspector : Window
    {
        Settings settings;
        StrategyMgr strategyMgr;

        public GUIInspector()
        {
            InitializeComponent();

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

            //brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
            //brailleDisplayStrategy.setStrategyMgr(strategyMgr);

            
        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }
        TreeViewItem child = new TreeViewItem();
       

        private void treeIteration(ITreeStrategy<OSMElement.OSMElement> tree, TreeViewItem root)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;


            while (tree.HasChild && !(tree.Count == 1 && tree.Depth == -1))
            {
                TreeViewItem child = new TreeViewItem();
                node1 = tree.Child;
                child.Header = node1.Data.properties.controlTypeFiltered;
                root.Items.Add(child);

                if (node1.HasChild)
                {
                    treeIteration(node1, child);
                }
                else
                {
                    treeIteration(node1, root);
                }


              
            }
            while (tree.HasNext)
            {
                TreeViewItem sibling = new TreeViewItem();
                node1 = tree.Next;
                sibling.Header = node1.Data.properties.controlTypeFiltered;
                root.Items.Add(sibling);

                if (node1.HasChild)
                {
                    treeIteration(node1, sibling);
                }
                else
                {
                    treeIteration(node1, root);
                }

                
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                //baumSchleife(tree);
                tvMain.Items.Add(root);

                return;
            }
            if (!tree.HasChild)
            {
                node1 = tree;
                if (tree.HasParent)
                {
                    node1.Remove();
                }
            }
            if (!tree.HasNext && !tree.HasParent)
            {
                if (tree.HasPrevious)
                {
                    node1 = tree;
                    node1.Remove();
                }
            }
        }


      
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();

        ITreeStrategy<OSMElement.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();

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
                    System.Drawing.Rectangle rect = operationSystemStrategy.getRect(osmElement);

                    // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint);
                    operationSystemStrategy.paintRect(rect);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

            if (e.Key == Key.F5)
            {
                if (operationSystemStrategy.deliverCursorPosition())
                {
                    try
                    {
                        //Filtermethode
                        IntPtr points = operationSystemStrategy.getHWND();
                        List<Strategy> possibleFilter = settings.getPossibleFilters();
                        String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden

                        strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        filterStrategy.setStrategyMgr(strategyMgr);
                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                        //strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                        strategyMgr.setFilteredTree(tree);


                        // TreeViewItem treeItem = null;
                        // treeItem = new TreeViewItem();
                        
                        TreeViewItem root = new TreeViewItem();
                        root.Header = tree.Data.properties.controlTypeFiltered;
                        treeIteration(tree, root);

                       
                        //


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
