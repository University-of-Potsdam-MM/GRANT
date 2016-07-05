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

            /*MenuItem root = new MenuItem();
            
            MenuItem childItem1 = new MenuItem() { Title = "Child item #1" };
            childItem1.Items.Add(new MenuItem() { ID = "Child item #1.1" });
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.1" });
            childItem1.Items.Add(new MenuItem() { Title = "Child item #1.2" });
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() { ID = "Child item #2" });
            tvMain.Items.Add(root);
           */
            
        }


        private void treeIteration(ITreeStrategy<OSMElement.OSMElement> tree, ref MenuItem root)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;

            while (tree.HasChild && !(tree.Count == 1 && tree.Depth == -1))
            {
                
                MenuItem child = new MenuItem();
                node1 = tree.Child;
                child.controlTypeFiltered = node1.Data.properties.controlTypeFiltered;
                child.IdGenerated = node1.Data.properties.IdGenerated;
                child.nameFiltered = node1.Data.properties.nameFiltered;
                child.parentMenuItem = root;
                root.Items.Add(child);

                if (node1.HasChild)
                {
                    treeIteration(node1, ref child);
                }
                else
                {
                    if (node1.IsFirst && node1.IsLast)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }
            }
            while (tree.HasNext)
            {

                MenuItem sibling = new MenuItem();
                node1 = tree.Next;
                sibling.controlTypeFiltered = node1.Data.properties.controlTypeFiltered;
                sibling.IdGenerated = node1.Data.properties.IdGenerated;
                sibling.nameFiltered = node1.Data.properties.nameFiltered;
                sibling.parentMenuItem = root;
                root.Items.Add(sibling);

                if (node1.HasChild)
                {
                    treeIteration(node1, ref sibling);
                }
                else
                {
                    if (node1.IsFirst && node1.IsLast)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    if (!node1.HasChild && !node1.HasNext)
                    {
                        root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    }
                    treeIteration(node1, ref root);
                }
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                tvMain.Items.Add(root);
                return;
            }
            if (!tree.HasChild)
            {
                node1 = tree;
                if (tree.HasParent)
                {
                    node1.Remove();
                    return;
                }
            }
            if (tree.IsFirst && tree.IsLast)
            {
                root = root.parentMenuItem == null ? root : root.parentMenuItem;
            }
         /*   if (!tree.HasNext && !tree.HasParent)
            {
                if (tree.HasPrevious)
                {
                    node1 = tree;
                    node1.Remove();
                }
            }*/
        }

        void root_Selected(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            //you can access item properties eg item.Header etc. 
            //your logic here 
            
            Console.WriteLine("Pointx: " + item.IdGenerated);
           

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

                        MenuItem root = new MenuItem();

                        //TreeViewItem root = new TreeViewItem();
                        root.controlTypeFiltered = tree.Data.properties.controlTypeFiltered;
                        root.controlTypeFiltered = tree.Data.properties.IdGenerated;
                        //
                        treeIteration(tree, ref root);
                       // root.Selected += root_Selected;

                        //


                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }


        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }


            public String acceleratorKeyFiltered
            {
                get;
                set;
            }

            public String accessKeyFiltered
            {
                get;
                set;
            }



            public Boolean? isKeyboardFocusableFiltered
            {
                get;
                set;
            }

            public int[] runtimeIDFiltered
            {
                get;
                set;
            }

            // STATE

            // Boolean? => true, false, null
            public Boolean? isEnabledFiltered
            {
                get;
                set;
            }

            public Boolean? hasKeyboardFocusFiltered
            {
                get;
                set;
            }

            // Visibility

            public Rect boundingRectangleFiltered
            {
                get;
                set;
            }

            public Boolean? isOffscreenFiltered
            {
                get;
                set;
            }

            public String helpTextFiltered
            {
                get;
                set;
            }


            //IDENTIFICATION/Elemttype

            //nicht von UIA
            public String IdGenerated
            {
                get;
                set;
            }

            public String autoamtionIdFiltered
            {
                get;
                set;
            }


            public String classNameFiltered
            {
                get;
                set;
            }

            //Anmerkung: ich habe den LocalizedControlType genommen
            public String controlTypeFiltered
            {
                get;
                set;
            }

            public String frameWorkIdFiltered
            {
                get;
                set;
            }

            //typ?
            // Anmerkung: von String zu int geändert
            public IntPtr hWndFiltered
            {
                get;
                set;
            }

            public Boolean? isContentElementFiltered
            {
                get;
                set;
            }
            //typ?
            public String labeledbyFiltered
            {
                get;
                set;
            }

            public Boolean? isControlElementFiltered
            {
                get;
                set;
            }

            public Boolean? isPasswordFiltered
            {
                get;
                set;
            }

            public String localizedControlTypeFiltered
            {
                get;
                set;
            }

            public String nameFiltered
            {
                get;
                set;
            }

            public int processIdFiltered
            {
                get;
                set;
            }

            public String itemTypeFiltered
            {
                get;
                set;
            }
            public String itemStatusFiltered
            {
                get;
                set;
            }
            public Boolean? isRequiredForFormFiltered
            {
                get;
                set;
            }

            public String valueFiltered { get; set; }

            public MenuItem parentMenuItem
            {
                get;
                set;
            }


            /// <summary>
            /// Enthält die unterstützten Pattern
            /// </summary>
            public object[] suportedPatterns { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

     
    }
  
}
