using System;
using System.Windows;
using System.Windows.Input;
using GRANTManager;
using GRANTManager.Interfaces;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Data;
using System.Linq;
using GRANTManager.TreeOperations;
using System.Windows.Data;

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
        TreeOperation treeOperations;
        TreeViewItem root;
        GuiFunctions guiFunctions;
        bool filterWindowOpen = false;
        bool outputDesignerWindowOpen = false;
        GuiFunctions.MyViewModel filteredPropRoot;

        public GUIInspector()
        {
            InitializeComponent();

            settings = new Settings();
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            treeOperations = new TreeOperation(strategyMgr, grantTrees);

            // Setzen des Eventmanager
            List<Strategy> possibleEventManager = settings.getPossibleEventManager();
            strategyMgr.setSpecifiedEventStrategy(possibleEventManager[0].className);
            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(Settings.strategyUserNameToClassName(cUserOperationSystemName));
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();
            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);
            ITreeStrategy<OSMElements.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();
            List<Strategy> possibleFilter = Settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(Settings.strategyUserNameToClassName(cUserFilterName));
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperations);
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperations);
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

            #region setzen der neuen (Juni 2017) Event Interfaces
            strategyMgr.setSpecifiedEventAction(settings.getPossibleEventAction()[0].className);
            strategyMgr.getSpecifiedEventAction().setGrantTrees(grantTrees);
            strategyMgr.getSpecifiedEventAction().setTreeOperation(treeOperations);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager2()[0].className);
            strategyMgr.setSpecifiedEventProcessor(settings.getPossibleEventProcessor()[0].className);
            strategyMgr.getSpecifiedEventProcessor().setGrantTrees(grantTrees);
            strategyMgr.getSpecifiedEventProcessor().setTreeOperations(treeOperations);
            #endregion


            strategyMgr.setSpecifiedExternalScreenreader(settings.getPossibleExternalScreenreaders()[0].className);
            strategyMgr.setSpecifiedBrailleConverter(settings.getPossibleBrailleConverter()[0].className);

            filteredTreeOutput.SelectedItemChanged +=new RoutedPropertyChangedEventHandler<object>(filteredTreeOutput_SelectedItemChanged);
            guiFunctions = new GuiFunctions(strategyMgr, grantTrees, treeOperations);
            root = new TreeViewItem();
            NodeButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            filteredPropRoot = new GuiFunctions.MyViewModel();
        }

        /// <summary>
        /// Displays properties of the marked tree node of the filtered tree in an table.
        /// </summary>
        /// <param name="IdGenerated"></param>
        void updatePropertiesTable(String IdGenerated)
        {
            OSMElements.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(IdGenerated);
            this.filteredPropRoot = new GuiFunctions.MyViewModel(osmElement);

            if (filteredTreeProp.Columns.Count == 0)
            {
                filteredTreeProp.Columns.Clear();
                /*   int columnIndex = 0;
                   foreach (var name in this.filteredPropRoot.ColumnNames)
                   {
                       filteredTreeProp.Columns.Add(
                           new DataGridTextColumn
                           {
                               Header = name,
                               Binding = new Binding(string.Format("Values[{0}]", columnIndex++))
                           });
                   }*/
                filteredTreeProp.Columns.Add(
                     new DataGridTextColumn
                     {
                         Header = "Property",
                         // Binding = new Binding(string.Format("Values", 0)),
                         Binding = new Binding("Values.Name")
                     }
                 );
                filteredTreeProp.Columns.Add(
                    new DataGridTextColumn
                    {
                        Header = "Content",
                        Binding = new Binding("Values.currentValue")
                    }
                );
            }
            filteredTreeProp.DataContext = this.filteredPropRoot;

            System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
            strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
            NodeButton.CommandParameter = IdGenerated;
        }

        void filteredTreeOutput_SelectedItemChanged(object sender,
        RoutedPropertyChangedEventArgs<object> e)
        {
            NodeButton.IsEnabled = true;
            var tree = sender as TreeView;

            // ... Determine typeOfTemplate of SelectedItem.
            if (tree.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                TreeViewItem item = tree.SelectedItem as TreeViewItem;
                if (item.Header is GuiFunctions.MenuItem && ((GuiFunctions.MenuItem)item.Header).IdGenerated != null)
                {
                    updatePropertiesTable(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                }
                else
                {
                    GuiFunctions.clearTable(filteredTreeProp);
                }
            //Methode MenuItem übergeben - tabelle
            }
            else if (tree.SelectedItem is string)
            {
                // ... Handle a string.
                Console.WriteLine("Fehler: ");
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IOperationSystemStrategy operationSystemStrategy = strategyMgr.getSpecifiedOperationSystem();

            ITreeStrategy<OSMElements.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();

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
                        OSMElements.OSMElement osmElement = filterStrategy.getOSMElement(pointX, pointY);
                        System.Drawing.Rectangle rect = operationSystemStrategy.getRect(osmElement);
                        if (osmElement.properties.isOffscreenFiltered == false)
                        {
                            operationSystemStrategy.paintRect(rect);
                        }
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
                        IntPtr points = operationSystemStrategy.getHWNDByCursorPosition();
                        List<Strategy> possibleFilter = Settings.getPossibleFilters();
                        if (strategyMgr.getSpecifiedFilter() == null)
                        {
                            // auslesen aus GUI..... 
                            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
                            strategyMgr.setSpecifiedFilter(Settings.strategyUserNameToClassName(cUserFilterName));
                            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperations);
                        }
                        guiFunctions.deleteGrantTrees();
                        IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                        Object tree = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                        grantTrees.filteredTree = tree;
                        filteredTreeOutput.Items.Clear();
                        root.Items.Clear();
                        root.Header = "Filtered - Tree";
                        guiFunctions.createTreeForOutput(tree, ref root);
                        SaveButton.IsEnabled = true;
                        filteredTreeOutput.Items.Add(root);
                        NodeButton.IsEnabled = false;
                        /* updatePropertiesTable(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties.IdGenerated);
                        ((TreeViewItem)filteredTreeOutput.Items[0]).IsSelected = true;
                        ((TreeViewItem)filteredTreeOutput.Items[0]).IsExpanded = true;*/
                        GuiFunctions.clearTable(filteredTreeProp);
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
            var button = sender as RadioButton;
            // ... Display button content as title.
            String titleName = button.Content.ToString();
            strategyMgr.setSpecifiedFilter(Settings.strategyUserNameToClassName(titleName));
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperations);
        }

        private void Node_Click(object sender, RoutedEventArgs e)
        {
            if(((Button)sender).CommandParameter == null) { return; }
            System.Console.WriteLine(" ID: " + ((Button)sender).CommandParameter.ToString());
            treeOperations.updateNodes.filterSubtreeWithCurrentFilterStrtegy(((Button)sender).CommandParameter.ToString());
            Object tree = grantTrees.filteredTree;
            filteredTreeOutput.Items.Clear();
            root.Items.Clear();
            root.Header = "Filtered - Tree - Updated";
            guiFunctions.createTreeForOutput(tree, ref root);
            filteredTreeOutput.Items.Add(root);
            NodeButton.IsEnabled = false;
            updatePropertiesTable(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties.IdGenerated);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (grantTrees.filteredTree == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = GuiFunctions.cleanInput("filteredTree_" + strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.nameFiltered); // Default file name
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
            String fileName = guiFunctions.openFileDialog(".grant", "GRANT documents (.grant)|*.grant", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if (fileName == null) { System.Windows.Forms.MessageBox.Show("The chosen screen reader doesn't exist!", "GRANT exception"); return; }

            guiFunctions.loadGrantProject(fileName);
            Object tree = grantTrees.filteredTree;
            filteredTreeOutput.Items.Clear();
            root.Items.Clear();
            root.Header = "Filtered - Tree - Updated";
            guiFunctions.createTreeForOutput(tree, ref root); filteredTreeOutput.Items.Add(root);
            NodeButton.IsEnabled = false;
            updatePropertiesTable(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties.IdGenerated);
            SaveButton.IsEnabled = true;

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
        }

        void aboutOutputDesignerWindow_Closed(object sender, EventArgs e)
        {
            outputDesignerWindowOpen = false;
        }

        private void filteredTreeProp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Überflüssig ?!
            Console.WriteLine("HIER!!!!!!xxx: ");
            DataGrid dataGrid = sender as DataGrid;
            DataRowView rowView = dataGrid.SelectedItem as DataRowView;
            if(rowView == null) { return; }
            string myCellValue = rowView.Row[0].ToString();
            Console.WriteLine("AUSGABE: " + myCellValue);
        }

        private void ExternalScreenreader_Click(object sender, RoutedEventArgs e)
        {
            OSMElements.OSMElement osm = strategyMgr.getSpecifiedExternalScreenreader().getScreenreaderContent();
            if (osm != null)
            {
                treeOperations.updateNodes.addNodeExternalScreenreaderInFilteredTree(osm);
                filteredTreeOutput.Items.Clear();
                root.Items.Clear();                
              //  root.Header = "Filtered - Tree";
                guiFunctions.createTreeForOutput(grantTrees.filteredTree, ref root);
                SaveButton.IsEnabled = true;
                filteredTreeOutput.Items.Add(root);
                NodeButton.IsEnabled = false;

                /* updatePropertiesTable(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.IdGenerated);
                 ((TreeViewItem)filteredTreeOutput.Items[0]).IsSelected = true;
                 ((TreeViewItem)filteredTreeOutput.Items[0]).IsExpanded = true;*/
                GuiFunctions.clearTable(filteredTreeProp);
            }
            else { System.Diagnostics.Debug.WriteLine("Can't find content from an external screenreader!"); }
        }
    }
}