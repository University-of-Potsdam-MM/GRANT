using System;
using System.Windows;
using System.Windows.Input;
using StrategyManager;
using StrategyManager.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data;

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
            DataTable dataTable = new DataTable();

            DataColumn dc = new DataColumn();
            dataTable.Columns.Add(new DataColumn("Property"));
            dataTable.Columns.Add(new DataColumn("Content"));
           

            DataRow dataRow = dataTable.NewRow();
            DataRow dataRow1 = dataTable.NewRow();
            DataRow dataRow2 = dataTable.NewRow();
            DataRow dataRow3 = dataTable.NewRow();
            DataRow dataRow4 = dataTable.NewRow();
            DataRow dataRow5 = dataTable.NewRow();
            DataRow dataRow6 = dataTable.NewRow();
            DataRow dataRow7 = dataTable.NewRow();
            DataRow dataRow8 = dataTable.NewRow();
            DataRow dataRow9 = dataTable.NewRow();
            DataRow dataRow10 = dataTable.NewRow();
            DataRow dataRow11 = dataTable.NewRow();
            DataRow dataRow12 = dataTable.NewRow();
          
            dataRow[0] = "IdGenerated";
            dataRow[1] = item.IdGenerated.ToString();
            dataTable.Rows.Add(dataRow);

            
            dataRow1[0] = "ControlTypeFiltered";
            dataRow1[1] = item.controlTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow1);

            dataRow2[0] = "nameFiltered";
            dataRow2[1] = item.nameFiltered.ToString();
            dataTable.Rows.Add(dataRow2);

            dataRow3["Property"] = "acceleratorKeyFiltered";
            dataRow3["Content"] = item.acceleratorKeyFiltered.ToString();
            dataTable.Rows.Add(dataRow3);

            dataRow4["Property"] = "accessKeyFiltered";
            dataRow4["Content"] = item.accessKeyFiltered.ToString();
            dataTable.Rows.Add(dataRow4);

            dataRow5["Property"] = "helpTextFiltered";
            dataRow5["Content"] = item.helpTextFiltered.ToString();
            dataTable.Rows.Add(dataRow5);

            dataRow6["Property"] = "autoamtionIdFiltered";
            dataRow6["Content"] = item.autoamtionIdFiltered.ToString();
            dataTable.Rows.Add(dataRow6);

            dataRow7["Property"] = "classNameFiltered";
            dataRow7["Content"] = item.classNameFiltered.ToString();
            dataTable.Rows.Add(dataRow7);

            dataRow8["Property"] = "controlTypeFiltered";
            dataRow8["Content"] = item.controlTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow8);

            dataRow9["Property"] = "frameWorkIdFiltered";
            dataRow9["Content"] = item.frameWorkIdFiltered.ToString();
            dataTable.Rows.Add(dataRow9);

            dataRow10["Property"] = "itemTypeFiltered";
            dataRow10["Content"] = item.itemTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow10);

            dataRow11["Property"] = "itemStatusFiltered";
            dataRow11["Content"] = item.itemStatusFiltered.ToString();
            dataTable.Rows.Add(dataRow11);

            dataRow12["Property"] = "valueFiltered";
            dataRow12["Content"] = item.valueFiltered.ToString();
            dataTable.Rows.Add(dataRow12);

            //dataTable.Rows.Add(dataRow);
            dataGrid1.ItemsSource = dataTable.DefaultView;
         
           

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
                        filterStrategy.setStrategyMgr(strategyMgr);
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
