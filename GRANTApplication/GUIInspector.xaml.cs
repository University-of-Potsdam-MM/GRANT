﻿using System;
using System.Windows;
using System.Windows.Input;
using GRANTManager;
using GRANTManager.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Data;
using System.Linq;

namespace GRANTApplication
{
    /// <summary>
    /// Interaktionslogik für GUIInspector.xaml
    /// </summary>
    public partial class GUIInspector : Window
    {
        Settings settings;
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        GuiFunctions.MenuItem root;
        GuiFunctions guiFunctions;
        bool filterWindowOpen = false;
        bool outputDesignerWindowOpen = false;

        public GUIInspector()
        {
            InitializeComponent();

            settings = new Settings();
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
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
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

            strategyMgr.setSpecifiedTreeOperations(settings.getPossibleTreeOperations()[0].className);
            strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedTreeOperations().setGeneratedGrantTrees(grantTrees);

            tvMain.SelectedItemChanged +=new RoutedPropertyChangedEventHandler<object>(tvMain_SelectedItemChanged);
           
            guiFunctions = new GuiFunctions(strategyMgr, grantTrees);

            root = new GuiFunctions.MenuItem();
            NodeButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            SaveStartButton.IsEnabled = false;
        }

        void updatePropertiesTable(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = strategyMgr.getSpecifiedTreeOperations().getFilteredTreeOsmElementById(IdGenerated);
            
            
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

            DataRow dataRow13 = dataTable.NewRow();
            DataRow dataRow14 = dataTable.NewRow();
            DataRow dataRow15 = dataTable.NewRow();
            DataRow dataRow16 = dataTable.NewRow();
            DataRow dataRow17 = dataTable.NewRow();
            DataRow dataRow18 = dataTable.NewRow();
            DataRow dataRow19 = dataTable.NewRow();
            DataRow dataRow20 = dataTable.NewRow();
            DataRow dataRow21 = dataTable.NewRow();
            DataRow dataRow22 = dataTable.NewRow();
            DataRow dataRow23 = dataTable.NewRow();
            DataRow dataRow24 = dataTable.NewRow();
            DataRow dataRow25 = dataTable.NewRow();

            dataRow[0] = "IdGenerated";
            if (osmElement.properties.IdGenerated == null) { return; }
            dataRow[1] = osmElement.properties.IdGenerated.ToString();
            dataTable.Rows.Add(dataRow);


            dataRow1[0] = "ControlType";
            dataRow1[1] = osmElement.properties.controlTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow1);

            dataRow2[0] = "Name";
            dataRow2[1] = osmElement.properties.nameFiltered.ToString();
            dataTable.Rows.Add(dataRow2);

            dataRow3["Property"] = "AcceleratorKey";
            dataRow3["Content"] = osmElement.properties.acceleratorKeyFiltered.ToString();
            dataTable.Rows.Add(dataRow3);

            dataRow4["Property"] = "AccessKey";
            dataRow4["Content"] = osmElement.properties.accessKeyFiltered.ToString();
            dataTable.Rows.Add(dataRow4);

            dataRow5["Property"] = "HelpText";
            dataRow5["Content"] = osmElement.properties.helpTextFiltered.ToString();
            dataTable.Rows.Add(dataRow5);

            dataRow6["Property"] = "AutoamtionId";
            dataRow6["Content"] = osmElement.properties.autoamtionIdFiltered.ToString();
            dataTable.Rows.Add(dataRow6);

            dataRow7["Property"] = "ClassName";
            dataRow7["Content"] = osmElement.properties.classNameFiltered.ToString();
            dataTable.Rows.Add(dataRow7);

            dataRow8["Property"] = "Value";
            dataRow8["Content"] = osmElement.properties.valueFiltered;
            dataTable.Rows.Add(dataRow8);

            dataRow9["Property"] = "FrameWorkId";
            dataRow9["Content"] = osmElement.properties.frameWorkIdFiltered.ToString();
            dataTable.Rows.Add(dataRow9);

            dataRow10["Property"] = "ItemType";
            dataRow10["Content"] = osmElement.properties.itemTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow10);

            dataRow11["Property"] = "ItemStatus";
            dataRow11["Content"] = osmElement.properties.itemStatusFiltered.ToString();
            dataTable.Rows.Add(dataRow11);

            dataRow12["Property"] = "Filterstrategy";
            //dataRow12["Content"] = osmElement.properties.grantFilterStrategy == null ? " " : osmElement.properties.grantFilterStrategy.ToString();
            dataRow12["Content"] = osmElement.properties.grantFilterStrategyFullName;
            dataTable.Rows.Add(dataRow12);

            dataRow13["Property"] = "labeledbyFiltered";
            dataRow13["Content"] = osmElement.properties.labeledbyFiltered == null ? " " : osmElement.properties.labeledbyFiltered.ToString();
            dataTable.Rows.Add(dataRow13);

            dataRow14["Property"] = "localizedControlTypeFiltered";
            dataRow14["Content"] = osmElement.properties.localizedControlTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow14);

            dataRow15["Property"] = "processIdFiltered";
            dataRow15["Content"] = osmElement.properties.processIdFiltered.ToString();
            dataTable.Rows.Add(dataRow15);

            dataRow16["Property"] = "isKeyboardFocusableFiltered";
            dataRow16["Content"] = osmElement.properties.isKeyboardFocusableFiltered == null ? " " : osmElement.properties.isKeyboardFocusableFiltered.ToString();
            dataTable.Rows.Add(dataRow16);

            dataRow17["Property"] = "isEnabledFiltered";
            dataRow17["Content"] = osmElement.properties.isEnabledFiltered == null ? " " : osmElement.properties.isEnabledFiltered.ToString();
            dataTable.Rows.Add(dataRow17);

            dataRow18["Property"] = "isOffscreenFiltered";
            dataRow18["Content"] = osmElement.properties.isOffscreenFiltered == null ? " " : osmElement.properties.isOffscreenFiltered.ToString();
            dataTable.Rows.Add(dataRow18);

            dataRow19["Property"] = "isContentElementFiltered";
            dataRow19["Content"] = osmElement.properties.isContentElementFiltered == null ? " " : osmElement.properties.isContentElementFiltered.ToString();
            dataTable.Rows.Add(dataRow19);

            dataRow20["Property"] = "hasKeyboardFocusFiltered";
            dataRow20["Content"] = osmElement.properties.hasKeyboardFocusFiltered == null ? " " : osmElement.properties.hasKeyboardFocusFiltered.ToString();
            dataTable.Rows.Add(dataRow20);

            dataRow21["Property"] = "isPasswordFiltered";
            dataRow21["Content"] = osmElement.properties.isPasswordFiltered == null ? " " : osmElement.properties.isPasswordFiltered.ToString();
            dataTable.Rows.Add(dataRow21);

            dataRow22["Property"] = "isRequiredForFormFiltered";
            dataRow22["Content"] = osmElement.properties.isRequiredForFormFiltered == null ? " " : osmElement.properties.isRequiredForFormFiltered.ToString();
            dataTable.Rows.Add(dataRow22);

            dataRow23["Property"] = "hWndFiltered";
            dataRow23["Content"] = osmElement.properties.hWndFiltered == null ? " " : osmElement.properties.hWndFiltered.ToString();
            dataTable.Rows.Add(dataRow23);

            String ids = String.Join(" : ", osmElement.properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray());

            dataRow24["Property"] = "runtimeIDFiltered";
            dataRow24["Content"] = ids;
            dataTable.Rows.Add(dataRow24);

            dataRow25["Property"] = "boundingRectangleFiltered";
            dataRow25["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
            dataTable.Rows.Add(dataRow25);
            dataTable.Rows.Add();

            

            dataTable.Rows.Add();

            //dataTable.Rows.Add(dataRow);
            dataGrid1.ItemsSource = dataTable.DefaultView;

            System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);

            
            // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint)
            strategyMgr.getSpecifiedOperationSystem().paintRect(rect);

            NodeButton.CommandParameter = IdGenerated;

        }

        void tvMain_SelectedItemChanged(object sender,
        RoutedPropertyChangedEventArgs<object> e)
        {
            NodeButton.IsEnabled = true;
            
            var tree = sender as TreeView;

            // ... Determine type of SelectedItem.
            if (tree.SelectedItem is GuiFunctions.MenuItem)
            {
                // ... Handle a TreeViewItem.
                GuiFunctions.MenuItem item = tree.SelectedItem as GuiFunctions.MenuItem;
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
                if (item.IdGenerated != null)
                {
                    //Console.WriteLine("HIIIIEEEER: " + item.IdGenerated.ToString());

                    //root = root.parentMenuItem == null ? root : root.parentMenuItem;
                    //  Console.WriteLine("HIIIIEEEER: " + item.classNameFiltered.ToString());

                    //updateProperties(item);
                    updatePropertiesTable(item.IdGenerated);
                }
            //Methode MenuItem übergeben - tabelle
            }
            else if (tree.SelectedItem is string)
            {
                // ... Handle a string.
                //this.Name = "Selected: " + tree.SelectedItem.ToString();
                Console.WriteLine("Fehler: ");

            }
            //OSMElement.OSMElement osmElement = strategyMgr.getSpecifiedTreeOperations().getFilteredTreeOsmElementById(IdGenerated);
            // System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);


            //strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
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
                       
                        if (strategyMgr.getSpecifiedFilter() == null)
                        {
                            // auslesen aus GUI..... 
                            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
                            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
                            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);

                        }



                            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                      //  filterStrategy.setStrategyMgr(strategyMgr);
                        ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        // StrategyGenericTree.TreeStrategyGenericTreeMethodes.printTreeElements(tree, -1);
                        //strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                        grantTrees.setFilteredTree(tree);


                        // TreeViewItem treeItem = null;
                        // treeItem = new TreeViewItem();

                       // ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getFilteredTree();

                       
                            
                        tvMain.Items.Clear();
                        
                        root.Items.Clear();

                        //tree.DeepCopy();



                        //TreeViewItem root = new TreeViewItem();
                        //root.controlTypeFiltered = "Filtered-Tree";
                        //ITreeStrategy<OSMElement.OSMElement> copy = tree.Child;
                        //
       

                        guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
                                                              // root.Selected += root_Selected;
                                                              //
                        SaveButton.IsEnabled = true;
                        SaveStartButton.IsEnabled = true;
                        tvMain.Items.Add(root);
                        NodeButton.IsEnabled = false;
                        updatePropertiesTable(tree.Child.Data.properties.IdGenerated);

                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }


        }
   
      
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
           
            //sender.ToString();
            var button = sender as RadioButton;

            // ... Display button content as title.
            String titleName = button.Content.ToString();
            //Console.WriteLine("Filter: " + Title);
            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(titleName));
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            //Console.WriteLine("Strategy: " + strategyMgr.getSpecifiedFilter().ToString()); 
        }

  /*     private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }*/
        
        private void Node_Click(object sender, RoutedEventArgs e)
        {
            System.Console.WriteLine(" ID: " + ((Button)sender).CommandParameter.ToString());
            UpdateNode node = new UpdateNode(strategyMgr, grantTrees);
            node.updateNodeOfFilteredTree(((Button)sender).CommandParameter.ToString());

            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            tvMain.Items.Clear();
            root.Items.Clear();

            //TreeViewItem root = new TreeViewItem();
            root.controlTypeFiltered = "Filtered- Updated- Tree";

            //
            guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)

            tvMain.Items.Add(root);
            NodeButton.IsEnabled = false;
            updatePropertiesTable(tree.Child.Data.properties.IdGenerated);
            //SaveButton.IsEnabled = true;

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (grantTrees.getFilteredTree() == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredTree_" + grantTrees.getFilteredTree().Child.Data.properties.nameFiltered; // Default file name
            dlg.DefaultExt = ".grant"; // Default file extension
            dlg.Filter = "GRANT documents (.grant)|*.grant"; // Filter files by extension
            dlg.OverwritePrompt = true; // Hinweis wird gezeigt, wenn die Datei schon existiert
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                guiFunctions.saveProject(dlg.FileName);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // dlg.FileName = "filteredTree_"; // Default file name
            dlg.DefaultExt = ".grant"; // Default file extension
            dlg.Filter = "GRANT documents (.grant)|*.grant"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                guiFunctions.loadFilteredTree(dlg.FileName);
           

            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            tvMain.Items.Clear();
            root.Items.Clear();

            //TreeViewItem root = new TreeViewItem();
            root.controlTypeFiltered = "Filtered- Updated- Tree";

            //
            guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
            tvMain.Items.Add(root);
            NodeButton.IsEnabled = false;
            updatePropertiesTable(tree.Child.Data.properties.IdGenerated);
            SaveButton.IsEnabled = true;
            SaveStartButton.IsEnabled = true;
            }
        }


        private void SaveStartButton_Click(object sender, RoutedEventArgs e)
    {
        if (outputDesignerWindowOpen == false)
        {
            var aboutOutputDesigner = new OutputDesigner();
            aboutOutputDesigner.Closed += new EventHandler(aboutOutputDesignerWindow_Closed);
            aboutOutputDesigner.Topmost = true;
            aboutOutputDesigner.Show();
            bool TreeLoad = true;
            SaveStartButton.CommandParameter = TreeLoad;
            
            outputDesignerWindowOpen = true;

        }

        //e.Handled = true;
        // this.Close();

    }

    void aboutOutputDesignerWindow_Closed(object sender, EventArgs e)
    {
        outputDesignerWindowOpen = false;
    }
}
}