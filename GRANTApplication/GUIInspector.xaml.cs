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

           

            tvMain.SelectedItemChanged +=
           new RoutedPropertyChangedEventHandler<object>(tvMain_SelectedItemChanged);
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

            InitializeComponent();
           


        }

        void updateProperties(MenuItem item)
        {
            TextBlock prop1;
            TextBlock prop2;
            TextBlock prop3;
            TextBlock prop4;
            TextBlock prop5;
            TextBlock prop6;
            TextBlock prop7;
            TextBlock prop8;
            TextBlock prop9;
            TextBlock prop10;
            TextBlock prop11;
            TextBlock prop12;

            prop1 = new TextBlock();
            prop2 = new TextBlock();
            prop3 = new TextBlock();
            prop4 = new TextBlock();
            prop5 = new TextBlock();
            prop6 = new TextBlock();
            prop7 = new TextBlock();
            prop8 = new TextBlock();
            prop9 = new TextBlock();
            prop10 = new TextBlock();
            prop11 = new TextBlock();
            prop12 = new TextBlock();

            prop1.Text = item.controlTypeFiltered;
            Grid.SetRow(prop1, 1);
            Grid.SetColumn(prop1, 1);

            if (item.IdGenerated == null) { return; }
            prop2.Text = item.IdGenerated.ToString();
            Grid.SetRow(prop2, 2);
            Grid.SetColumn(prop2, 1);


            prop3.Text = item.nameFiltered.ToString();
            Grid.SetRow(prop3, 3);
            Grid.SetColumn(prop3, 1);


            prop4.Text = item.acceleratorKeyFiltered.ToString();
            Grid.SetRow(prop4, 4);
            Grid.SetColumn(prop4, 1);


            prop5.Text = item.accessKeyFiltered.ToString();
            Grid.SetRow(prop5, 5);
            Grid.SetColumn(prop5, 1);


            prop6.Text = item.helpTextFiltered.ToString();
            Grid.SetRow(prop6, 6);
            Grid.SetColumn(prop6, 1);


            prop7.Text = item.autoamtionIdFiltered.ToString();
            Grid.SetRow(prop7, 7);
            Grid.SetColumn(prop7, 1);


            prop8.Text = item.classNameFiltered.ToString();
            Grid.SetRow(prop8, 8);
            Grid.SetColumn(prop8, 1);


            prop9.Text = item.frameWorkIdFiltered.ToString();
            Grid.SetRow(prop9, 9);
            Grid.SetColumn(prop9, 1);


            prop10.Text = item.itemTypeFiltered.ToString();
            Grid.SetRow(prop10, 10);
            Grid.SetColumn(prop10, 1);


            prop11.Text = item.valueFiltered.ToString();
            Grid.SetRow(prop11, 11);
            Grid.SetColumn(prop8, 1);


            prop12.Text = item.itemStatusFiltered.ToString();
            Grid.SetRow(prop12, 12);
            Grid.SetColumn(prop12, 1);
           
            dgUsers.Children.Add(prop1);
            dgUsers.Children.Add(prop2);
            dgUsers.Children.Add(prop3);
            dgUsers.Children.Add(prop4);
            dgUsers.Children.Add(prop5);
            dgUsers.Children.Add(prop6);
            dgUsers.Children.Add(prop7);
            dgUsers.Children.Add(prop8);
            dgUsers.Children.Add(prop9);
            dgUsers.Children.Add(prop10);
            dgUsers.Children.Add(prop11);
            dgUsers.Children.Add(prop12);
           

           
        }

        void tvMain_SelectedItemChanged(object sender,
        RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tree.SelectedItem is MenuItem)
            {
                // ... Handle a TreeViewItem.
                MenuItem item = tree.SelectedItem as MenuItem;
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
               // Console.WriteLine("HIIIIEEEER: " + item.IdGenerated.ToString());
                //root = root.parentMenuItem == null ? root : root.parentMenuItem;
                //  Console.WriteLine("HIIIIEEEER: " + item.classNameFiltered.ToString());

                updateProperties(item);
            //Methode MenuItem übergeben - tabelle
            }
            else if (tree.SelectedItem is string)
            {
                // ... Handle a string.
                this.Name = "Selected: " + tree.SelectedItem.ToString();
            }
        }
        private void treeIteration(ITreeStrategy<OSMElement.OSMElement> tree, ref MenuItem root)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;

            while (tree.HasChild && !(tree.Count == 1 && tree.Depth == -1))
            {
                
                MenuItem child = new MenuItem();
                node1 = tree.Child;
                child.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                child.IdGenerated = node1.Data.properties.IdGenerated == null ? " " : node1.Data.properties.IdGenerated;
                child.nameFiltered = node1.Data.properties.nameFiltered == null ? " " : node1.Data.properties.nameFiltered;
                child.acceleratorKeyFiltered = node1.Data.properties.acceleratorKeyFiltered == null ? " " : node1.Data.properties.acceleratorKeyFiltered;
                child.accessKeyFiltered = node1.Data.properties.accessKeyFiltered == null ? " " : node1.Data.properties.accessKeyFiltered;
                child.helpTextFiltered = node1.Data.properties.helpTextFiltered == null ? " " : node1.Data.properties.helpTextFiltered;
                child.autoamtionIdFiltered = node1.Data.properties.autoamtionIdFiltered == null ? " " : node1.Data.properties.autoamtionIdFiltered;
                child.classNameFiltered = node1.Data.properties.classNameFiltered == null ? " " : node1.Data.properties.classNameFiltered;
                child.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                child.frameWorkIdFiltered = node1.Data.properties.frameWorkIdFiltered == null ? " " : node1.Data.properties.frameWorkIdFiltered;
                child.itemTypeFiltered = node1.Data.properties.itemTypeFiltered == null ? " " : node1.Data.properties.itemTypeFiltered;
                child.itemStatusFiltered = node1.Data.properties.itemStatusFiltered == null ? " " : node1.Data.properties.itemStatusFiltered;
                child.valueFiltered = node1.Data.properties.valueFiltered == null ? " " : node1.Data.properties.valueFiltered;






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
                //Pruefung, ob wir es ans richtige Element ranhängen und ggf. korrigieren
                rootMenuItemCheckSibling(ref root, tree);

                sibling.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                sibling.IdGenerated = node1.Data.properties.IdGenerated == null ? " " : node1.Data.properties.IdGenerated;
                sibling.nameFiltered = node1.Data.properties.nameFiltered == null ? " " : node1.Data.properties.nameFiltered;

                sibling.acceleratorKeyFiltered = node1.Data.properties.acceleratorKeyFiltered == null ? " " : node1.Data.properties.acceleratorKeyFiltered;
                sibling.accessKeyFiltered = node1.Data.properties.accessKeyFiltered == null ? " " : node1.Data.properties.accessKeyFiltered;

                sibling.helpTextFiltered = node1.Data.properties.helpTextFiltered == null ? " " : node1.Data.properties.helpTextFiltered;
                sibling.autoamtionIdFiltered = node1.Data.properties.autoamtionIdFiltered == null ? " " : node1.Data.properties.autoamtionIdFiltered;
                sibling.classNameFiltered = node1.Data.properties.classNameFiltered == null ? " " : node1.Data.properties.classNameFiltered;
                sibling.controlTypeFiltered = node1.Data.properties.controlTypeFiltered == null ? " " : node1.Data.properties.controlTypeFiltered;
                sibling.frameWorkIdFiltered = node1.Data.properties.frameWorkIdFiltered == null ? " " : node1.Data.properties.frameWorkIdFiltered;



                sibling.itemTypeFiltered = node1.Data.properties.itemTypeFiltered == null ? " " : node1.Data.properties.itemTypeFiltered;
                sibling.itemStatusFiltered = node1.Data.properties.itemStatusFiltered == null ? " " : node1.Data.properties.itemStatusFiltered;


                sibling.valueFiltered = node1.Data.properties.valueFiltered == null ? " " : node1.Data.properties.valueFiltered;


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
        }

        /// <summary>
        /// prueft, ob der knoten an der richtigen Stelle dargestellt wird und korrigiert es ggf.
        /// </summary>
        /// <param name="root">gibt das (erwartete) Eltern-menuItem-element an</param>
        /// <param name="parentNode">gibt den vorgängert Knoten (linker Geschwisterknoten) an</param>
        private void rootMenuItemCheckSibling(ref MenuItem root, ITreeStrategy<OSMElement.OSMElement> siblingNode)
        {               
            if (siblingNode.HasParent && !siblingNode.Parent.Data.properties.IdGenerated.Equals(root.IdGenerated))
            {
                Console.WriteLine();
                if (root.parentMenuItem.IdGenerated.Equals(siblingNode.Parent.Data.properties.IdGenerated))
                {
                    root = root.parentMenuItem;
                    rootMenuItemCheckSibling(ref root, siblingNode);
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
                      //  filterStrategy.setStrategyMgr(strategyMgr);
                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                        //strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                        strategyMgr.setFilteredTree(tree);


                        // TreeViewItem treeItem = null;
                        // treeItem = new TreeViewItem();

                        MenuItem root = new MenuItem();
                       

                        //TreeViewItem root = new TreeViewItem();
                        root.controlTypeFiltered = "Filtered-Tree";
                        
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
