using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows.Media;
using System.Data;
using GRANTManager.TreeOperations;
using System.Linq;
using System.Windows.Data;
using System.Diagnostics;

namespace GRANTApplication
{
    /// <summary>
    /// Interaktionslogik für OutputDesigner.xaml
    /// </summary>
    public partial class OutputDesigner : Window
    {
        Settings settings;
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperations;
        TreeViewItem filteredRoot;
        TreeViewItem brailleRoot;
        GuiFunctions.MenuItem screenRoot;
        GuiFunctions guiFunctions;
        GuiFunctions.MyViewModel braillePropRoot;
        GuiFunctions.MyViewModel filteredPropRoot;

        [System.ComponentModel.BrowsableAttribute(false)]
        public DataGridCell CurrentCell { get; set; }

        public OutputDesigner()
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
            ITreeStrategy<OSMElement.OSMElement> treeStrategy = strategyMgr.getSpecifiedTree();
            List<Strategy> possibleFilter = Settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(Settings.strategyUserNameToClassName(cUserFilterName));
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperations);
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperations);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperations);

            #region setzen der neuen (Juni 2017) Event Interfaces
            strategyMgr.setSpecifiedEventAction(settings.getPossibleEventAction()[0].className);
            strategyMgr.getSpecifiedEventAction().setGrantTrees(grantTrees);
            strategyMgr.getSpecifiedEventAction().setTreeOperation(treeOperations);
            strategyMgr.setSpecifiedEventManager(settings.getPossibleEventManager2()[0].className);
            strategyMgr.setSpecifiedEventProcessor(settings.getPossibleEventProcessor()[0].className);
            #endregion

            filteredTreeOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(filteredTreeOutput_SelectedItemChanged);
            brailleTreeOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(brailleTreeOutput_SelectedItemChanged);
     //       brailleTreeProp.CellEditEnding += brailleTreeProp_CellEditEnding;
            guiFunctions = new GuiFunctions(strategyMgr, grantTrees, treeOperations);
            filteredRoot = new TreeViewItem();
            brailleRoot = new TreeViewItem();
            screenRoot = new GuiFunctions.MenuItem();
            SaveButton.IsEnabled = false;
            LoadTemplate.IsEnabled = false;
            AddNodeButton.IsEnabled = false;
            braillePropRoot = new GuiFunctions.MyViewModel();
            filteredPropRoot = new GuiFunctions.MyViewModel();
          


        }

        /// <summary>
        /// 
        /// </summary>
        public class CellContent
        {
            public String cellinput
            {
                get;
                set;
            }
        }
        
        /// <summary>
        /// Mark selected GUI element in Gui lements list and assign choosen element to datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //Variante datagrid
        private void listBox_GuiElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GuiElementsSimul.ItemsSource = null;
            if (((ListBox)sender).SelectedItem != null)
            {
                String element = (sender as ListBox).SelectedItem.ToString();
                bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(element);
                DataTable dataTable4 = createBrailleDisplayMatrix((guiElementRep.Length / guiElementRep.GetLength(0)), guiElementRep.GetLength(0), GuiElementsSimul);
                for (int a = 0; a < (guiElementRep.Length / guiElementRep.GetLength(0)); a++)//breite / Column
                {
                    for (int i = 0; i < guiElementRep.GetLength(0); i++) //höhe
                    {
                        dataTable4.Rows[i][a] = "";
                    }
                }
                GuiElementsSimul.ItemsSource = dataTable4.AsDataView();
                paintMatrix(GuiElementsSimul, guiElementRep);
            }
        }
        
        /// <summary>
        /// Paint datagrid of an assigned and marked Gui element
        /// </summary>
        /// <param name="datagrid"></param>
        /// <param name="guiElementRep"></param>
        public void paintMatrix(DataGrid datagrid, bool[,] guiElementRep)
        {
            for (int h = 0; h < GuiElementsSimul.Items.Count; h++)
            {
                DataGridRow row = (DataGridRow)GuiElementsSimul.ItemContainerGenerator.ContainerFromIndex(h);
                if (row == null)
                {
                    GuiElementsSimul.UpdateLayout();
                    GuiElementsSimul.ScrollIntoView(GuiElementsSimul.Items[h]);
                    row = (DataGridRow)GuiElementsSimul.ItemContainerGenerator.ContainerFromIndex(h);
                }
                for (int w = 0; w < GuiElementsSimul.Columns.Count; w++)
                {
                    if (GuiElementsSimul.Columns[w].GetCellContent(row) == null)
                    {
                        GuiElementsSimul.UpdateLayout();
                    }
                    DataGridCell firstColumnInFirstRow = GuiElementsSimul.Columns[w].GetCellContent(row).Parent as DataGridCell;
                    if (guiElementRep[h, w].ToString() == "True") {
                        firstColumnInFirstRow.Background = Brushes.Black;
                    }
                    else {
                        firstColumnInFirstRow.Background = Brushes.White;
                    }
                }
            }
        }
        
        /// <summary>
        /// Displays chossen output device as datagrid. 
        /// </summary>
        /// <param name="dWidth"></param>
        /// <param name="dHeight"></param>
        /// <param name="dataGrid"></param>
        //wenn unten gemalt wird diese methode raus
        private void createBrailleDisplay(int dWidth, int dHeight, DataGrid dataGrid)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < dWidth; i++)
            {
                DataColumn dCol1 = new DataColumn(i.ToString()); // string FirstC=”column1″
                dataTable.Columns.Add(dCol1);
            }
            for (int i = 0; i < dHeight; i++)
            {
                DataRow row1 = dataTable.NewRow();
                dataTable.Rows.Add(row1);
            }
            dataGrid.ItemsSource = dataTable.AsDataView();
        }

        /// <summary>
        /// Displays chossen output device as datagrid. 
        /// </summary>
        /// <param name="dWidth"></param>
        /// <param name="dHeight"></param>
        /// <param name="dataGrid"></param>
        /// <returns></returns>
        private DataTable createBrailleDisplayMatrix(int dWidth, int dHeight, DataGrid dataGrid)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < dWidth; i++) {
                DataColumn dCol1 = new DataColumn(i.ToString()); // string FirstC=”column1″
                dataTable.Columns.Add(dCol1);
            }
            for (int i = 0; i < dHeight; i++) {
                DataRow row1 = dataTable.NewRow();
                dataTable.Rows.Add(row1);
            }
            dataGrid.ItemsSource = dataTable.AsDataView();
            return dataTable;
        }
        
        /// <summary>
        /// Save Project with filtered tree and braille tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (grantTrees.filteredTree == null) { Console.WriteLine("Before saving tree must be filtered."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = GuiFunctions.cleanInput( "filteredProject_" + strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.nameFiltered); // Default file name
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
                // guiFunctions.saveFilteredTree(dlg.FileName);
                guiFunctions.saveProject(dlg.FileName);
                loadedProjectName.Content = "Loaded project name: " + dlg.SafeFileName.ToString();
                loadedProjectName.ToolTip = dlg.FileName.ToString();
            }
        }

        /// <summary>
        /// Load existing project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadProject_Click(object sender, RoutedEventArgs e)
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
                guiFunctions.loadGrantProject(dlg.FileName);

                loadedProjectName.Content = "Loaded project name: " + dlg.SafeFileName.ToString();
                loadedProjectName.ToolTip = dlg.FileName.ToString();
                Object tree = grantTrees.filteredTree;
                filteredTreeOutput.Items.Clear();
                filteredRoot.Items.Clear();
                guiFunctions.createTreeForOutput(tree, ref filteredRoot);
              //  filteredTreeProp.ItemsSource = "";
                filteredTreeProp.Items.Refresh();
                SaveButton.IsEnabled = true;
                LoadTemplate.IsEnabled = true;
                filteredTreeOutput.Items.Add(filteredRoot);
                int var3 = comboBox2.Items.IndexOf(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().ToString());
                comboBox2.SelectedIndex = var3;
                listGuiElements();
                int dWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
                int dHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
                createBrailleDisplay(dWidth, dHeight, brailleDisplaySimul);
                AddNodeButton.IsEnabled = true;
                if(grantTrees.brailleTree != null)
                {
                    #region Braille-Baum Darstellung  (Kopie von Template laden => Funktion)
                    brailleTreeOutput.Items.Clear();
                    brailleRoot.Items.Clear();
                    brailleRoot.Header = "Braille-Tree";
                    guiFunctions.createTreeForOutput(grantTrees.brailleTree, ref brailleRoot, false); 
                    SaveButton.IsEnabled = true;
                    brailleTreeOutput.Items.Add(brailleRoot);
                    brailleDisplaySimul.Items.Refresh();

                    #endregion
                }else
                {
                    brailleTreeOutput.Items.Clear();
                    brailleRoot.Items.Clear();
                }
                clearTable(brailleTreeProp);
                clearTable(filteredTreeProp);
                // brailleTreeProp.DataContext = data;
                // brailleTreeProp.ItemsSource = "";
                //  brailleTreeProp.Items.Refresh();
                //   brailleTreeProp.DataContext = "";
                //   brailleTreeProp.Items.Refresh();
                /////oder grid

                //     grid.ItemsSource = "";
                //     grid.Items.Refresh();
            }
        }

        /// <summary>
        /// Load existing design template.l#
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTemplate_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // dlg.FileName = "filteredTree_"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML documents (.xml)|*.xml"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == true && GuiFunctions.isTemplateValid(dlg.FileName))
            {
                strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(dlg.FileName);

                #region Marlene testet
                TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperations);
                tempTextView.createTextviewOfSubtree(grantTrees.filteredTree);
                #endregion 
                // brailleTreeProp.ItemsSource = "";
                // brailleTreeProp.Items.Refresh();
                //   grid.ItemsSource = "";
                //    grid.Items.Refresh();

                reloadTrees();
            }
        }

        /// <summary>
        /// List of connected output displays / hardwares and its sizes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (strategyMgr.getSpecifiedDisplayStrategy() == null)
            {
                strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            }
            List<string> Combobox2items = new List<string>();
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
            var comboBox = sender as ComboBox;
            int var3 = comboBox.Items.IndexOf(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().ToString());
            comboBox.SelectedIndex = var3;
            foreach (Device d in devices)
            {
                Combobox2items.Add(d.ToString());
            }
            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = Combobox2items;
            }

        /// <summary>
        /// Liste of existing standardized GUI elements in braille
        /// </summary>
        private void listGuiElements()
        {
            List<string> guiElements = strategyMgr.getSpecifiedBrailleDisplay().getUiElementRenderer();
            listBox_GuiElements.Items.Clear();
            foreach (String s in guiElements)
            {
                listBox_GuiElements.Items.Add(s);
            }
        }

        /// <summary>
        /// Selected output size / braille device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;
            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            if (value == null) { return; }
            Device d = strategyMgr.getSpecifiedDisplayStrategy().getDeviceByName(value);
            strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(d);
            listGuiElements();
            int dWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            int dHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
            createBrailleDisplay(dWidth, dHeight, brailleDisplaySimul); // später das zu matrix machen
        }

        /// <summary>
        /// Updates the combobox with possible devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDropDownOpened_Devices(object sender, EventArgs e)
        {
            if (comboBox2.IsDropDownOpen == true)
            {
                if (strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices().Count != comboBox2.Items.Count)
                {
                    ComboBox_Loaded(sender, e as RoutedEventArgs);
                }
            }
        }

        /// <summary>
        /// Displays properties of the marked tree node of the filtered tree in an table.
        /// </summary>
        /// <param name="IdGenerated"></param>
        void updateFilteredTable(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(IdGenerated);
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
        }

            /// <summary>
            /// Displays properties of the marked tree node of the filtered tree in an table.
            /// </summary>
            /// <param name="IdGenerated"></param>
            void updateFilteredTable_alt(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(IdGenerated);
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
            dataRow12["Content"] = osmElement.properties.grantFilterStrategy;
            dataTable.Rows.Add(dataRow12);

            dataRow13["Property"] = "labeledByFiltered";
            dataRow13["Content"] = osmElement.properties.labeledByFiltered == null ? " " : osmElement.properties.labeledByFiltered.ToString();
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

            String ids = osmElement.properties.runtimeIDFiltered == null ? " " : String.Join(" : ", osmElement.properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray());

            dataRow24["Property"] = "runtimeIDFiltered";
            dataRow24["Content"] = ids;
            dataTable.Rows.Add(dataRow24);

            dataRow25["Property"] = "boundingRectangleFiltered";
            dataRow25["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
            dataTable.Rows.Add(dataRow25);
            dataTable.Rows.Add();

            filteredTreeProp.ItemsSource = dataTable.DefaultView;
            System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
            strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
        }

        /// <summary>
        /// If a node of the filtered tree is selected the properties in the properties tables and the gui element will be updated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void filteredTreeOutput_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            TreeViewItem item;
            if (tree.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                if (tree.Name.Equals(filteredRoot)) { item = filteredRoot; System.Console.WriteLine(" Filt in filtered: "); }
                else if (tree.Name.Equals(brailleRoot)) { item = brailleRoot; System.Console.WriteLine(" Filt in braille: "); }
                else { item = tree.SelectedItem as TreeViewItem; }
                if (item.Header is GuiFunctions.MenuItem && ((GuiFunctions.MenuItem)item.Header).IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                    System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
                    if (osmElement.properties.isOffscreenFiltered == false)
                    {
                        strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                    }
                    int var1 = listBox_GuiElements.Items.IndexOf(((GuiFunctions.MenuItem)item.Header).controlTypeFiltered);
                    if (var1 < 0) { var1 = listBox_GuiElements.Items.IndexOf("Text"); }
                    listBox_GuiElements.SelectedIndex = var1;
                    updateFilteredTable(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                }
            }
        }

        /* not used
        /// <summary>
        /// Display all gui elements of one view on the simulated braille display
        /// </summary>
        /// <param name="IdGenerated"></param>
        void screenViewCurrentIteration(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getBrailleTreeOsmElementById(IdGenerated);
            String screenName = osmElement.brailleRepresentation.screenName == null ? " " : osmElement.brailleRepresentation.screenName;
            Rect rect = osmElement.properties.boundingRectangleFiltered;
            int x = (int) rect.X;
            int y = (int) rect.Y;
            Object screenTree = treeOperations.searchNodes.getSubtreeOfScreen(screenName);
            bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(osmElement);
            if (guiElementRep != new bool[0, 0])
            // datagrid2
            {
                for (int h = 0; h < (guiElementRep.GetLength(0)); h++) //höhe
                {
                    { 
                        DataGridRow row = (DataGridRow)brailleDisplaySimul.ItemContainerGenerator.ContainerFromIndex(h+y);
                        if (row == null)
                        {
                            brailleDisplaySimul.UpdateLayout();
                            brailleDisplaySimul.ScrollIntoView(brailleDisplaySimul.Items[h+y]);
                            row = (DataGridRow)brailleDisplaySimul.ItemContainerGenerator.ContainerFromIndex(h+y);
                        }
                        for (int w = 0; w < (guiElementRep.Length / guiElementRep.GetLength(0)); w++)
                        {
                            if (brailleDisplaySimul.Columns[w+x].GetCellContent(row) == null)
                            {
                                brailleDisplaySimul.UpdateLayout();
                            }
                            DataGridCell firstColumnInFirstRow = brailleDisplaySimul.Columns[w+x].GetCellContent(row).Parent as DataGridCell;
                            if (guiElementRep[h, w].ToString() == "True")
                            {
                                firstColumnInFirstRow.Background = Brushes.Black;
                            }
                            else
                            {
                                firstColumnInFirstRow.Background = Brushes.White;
                            }
                        }
                    }
                }
            }
        } */

        /// <summary>
        /// Display all GUI elements of an braille view on the simulated braille display.
        /// </summary>
        /// <param name="IdGenerated"></param>
        void screenViewIteration(String IdGenerated)
        {
            brailleDisplaySimul.Items.Refresh();
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getBrailleTreeOsmElementById(IdGenerated);
            String screenName = osmElement.brailleRepresentation.screenName == null ? "" : osmElement.brailleRepresentation.screenName;
            if (screenName != null & screenName != "")
            {
                Object screenTree = treeOperations.searchNodes.getSubtreeOfScreen(screenName);
                foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(screenTree))
                {
                    OSMElement.OSMElement osmElement2 = strategyMgr.getSpecifiedTree().GetData(node);
                    bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(osmElement2);
                    Rect rect = osmElement2.properties.boundingRectangleFiltered;
                    int x = (int)rect.X;
                    int y = (int)rect.Y;
                    if (guiElementRep != new bool[0, 0])
                    // datagrid2
                    {
                        Device device = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice();
                        for (int h = 0; h < (guiElementRep.GetLength(0)) && h+y < device.height ; h++) //höhe
                        {
                            {
                                DataGridRow row = (DataGridRow)brailleDisplaySimul.ItemContainerGenerator.ContainerFromIndex(h + y);
                                if (row == null)
                                {
                                    brailleDisplaySimul.UpdateLayout();
                                    brailleDisplaySimul.ScrollIntoView(brailleDisplaySimul.Items[h + y]);
                                    row = (DataGridRow)brailleDisplaySimul.ItemContainerGenerator.ContainerFromIndex(h + y);
                                }
                                for (int w = 0; w < (guiElementRep.Length / guiElementRep.GetLength(0)) && w+x < device.width; w++)
                                {
                                    if (brailleDisplaySimul.Columns[w + x].GetCellContent(row) == null)
                                    {
                                        brailleDisplaySimul.UpdateLayout();
                                    }
                                    DataGridCell firstColumnInFirstRow = brailleDisplaySimul.Columns[w + x].GetCellContent(row).Parent as DataGridCell;
                                    if (osmElement2.properties.IdGenerated.Equals(osmElement.properties.IdGenerated))
                                    {
                                        if (guiElementRep[h, w].ToString() == "True")
                                        {
                                            firstColumnInFirstRow.Background = Brushes.Red;
                                        }
                                        else
                                        {
                                            firstColumnInFirstRow.Background = Brushes.Yellow;
                                        }
                                    }
                                    else
                                    {
                                        if (guiElementRep[h, w].ToString() == "True")
                                        {
                                            firstColumnInFirstRow.Background = Brushes.Black;
                                        }
                                        else
                                        {
                                            firstColumnInFirstRow.Background = Brushes.White;
                                        }

                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Displays properties of the marked tree node of the braille tree in the properties table.
        /// </summary>
        /// <param name="IdGenerated"></param>
        /// 
        /// diese in die collection übernehmen
        void updateBrailleTable(String IdGenerated)
        {
           OSMElement.OSMElement osmElement = treeOperations.searchNodes.getBrailleTreeOsmElementById(IdGenerated);
           this.braillePropRoot = new GuiFunctions.MyViewModel(osmElement);
         /*   
            if (brailleTreeProp.Columns.Count == 0)
            {
                brailleTreeProp.Columns.Clear();
                int columnIndex = 0;
                foreach (var name in this.braillePropRoot.ColumnNames)
                {
                    brailleTreeProp.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = name,
                            Binding = new Binding(string.Format("Values[{0}]", columnIndex++))
                        });
                }
            }*/
           // brailleTreeProp.ItemsSource = data;
            brailleTreeProp.DataContext = this.braillePropRoot;
            
            if (this.brailleTreeProp.Items.Count > 0)
            {
                var dataGridCellInfo = new DataGridCellInfo(this.brailleTreeProp.Items[0], this.brailleTreeProp.Columns[0]);
                DataGridBoundColumn columni = dataGridCellInfo.Column as DataGridBoundColumn;
                columni.IsReadOnly = true;
            }
            
        }

       

        /*  DataTable dataTable = new DataTable();
          DataColumn dc = new DataColumn();
          dataTable.Columns.Add(new DataColumn("Property"));
          dataTable.Columns.Add(new DataColumn("Content"));
          dataTable.Columns.Add(new DataColumn("Unsichtbar"));

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

          dataRow[0] = "IdGenerated";
          if (osmElement.properties.IdGenerated == null) { return; }
          dataRow[1] = osmElement.properties.IdGenerated.ToString();
          dataRow[2] = "IdGenerated";

          dataTable.Rows.Add(dataRow);
          // dataRow.RejectChanges();

          dataRow1["Property"] = "isEnabledFiltered";
          dataRow1["Content"] = osmElement.properties.isEnabledFiltered == null ? " " : osmElement.properties.isEnabledFiltered.ToString();
          dataRow1[2] = "isEnabledFiltered";
          dataTable.Rows.Add(dataRow1);


          dataRow2["Property"] = "boundingRectangleFiltered";
          dataRow2["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
          dataRow2[2] = "isEnabledFiltered";
          dataTable.Rows.Add(dataRow2);

          dataRow3["Property"] = "Value";
          dataRow3["Content"] = osmElement.properties.valueFiltered;
          dataRow3[2] = "valueFiltered";
          dataTable.Rows.Add(dataRow3);

          dataRow4[0] = "ControlType";
          dataRow4[1] = osmElement.properties.controlTypeFiltered;
          dataRow4[2] = "controlTypeFiltered";
          dataTable.Rows.Add(dataRow4);

          dataRow5[0] = "Name";
          dataRow5[1] = osmElement.brailleRepresentation.viewName;
          dataRow5[2] = "viewName";
          dataTable.Rows.Add(dataRow5);

          dataRow6[0] = "Visibility";
          dataRow6[1] = osmElement.brailleRepresentation.isVisible.ToString();
          dataRow6[2] = "isVisible";
          dataTable.Rows.Add(dataRow6);

          dataRow7[0] = "displayedGuiElementType";
          dataRow7[1] = osmElement.brailleRepresentation.displayedGuiElementType == null ? " " : osmElement.brailleRepresentation.displayedGuiElementType.ToString();
          dataRow7[2] = "displayedGuiElementType";
          dataTable.Rows.Add(dataRow7);

          dataRow8[0] = "Contrast";
          dataRow8[1] = osmElement.brailleRepresentation.contrast.ToString();
          dataRow8[2] = "contrast";
          dataTable.Rows.Add(dataRow8);

          dataRow9[0] = "Zoom";
          dataRow9[1] = osmElement.brailleRepresentation.zoom.ToString();
          dataRow9[2] = "zoom";
          dataTable.Rows.Add(dataRow9);

          dataRow10[0] = "Screen name";
          dataRow10[1] = osmElement.brailleRepresentation.screenName == null ? " " : osmElement.brailleRepresentation.screenName.ToString();
          dataRow10[2] = "screenName";
          dataTable.Rows.Add(dataRow10);

          dataRow11[0] = "Show Scrollbar";
          dataRow11[1] = osmElement.brailleRepresentation.isScrollbarShow.ToString();
          dataRow11[2] = "isScrollbarShow";
          dataTable.Rows.Add(dataRow11);


          dataRow12[0] = "UIElementSpecialContent";
          dataRow12[1] = osmElement.brailleRepresentation.uiElementSpecialContent == null ? " " : osmElement.brailleRepresentation.uiElementSpecialContent.ToString();
          dataRow12[2] = "uiElementSpecialContent";
          dataTable.Rows.Add(dataRow12);

          dataRow13[0] = "Margin";
          dataRow13[1] = osmElement.brailleRepresentation.margin == null ? " " : osmElement.brailleRepresentation.margin.ToString();
          dataRow13[2] = "margin";
          dataTable.Rows.Add(dataRow13);

          dataRow14[0] = "Boarder";
          dataRow14[1] = osmElement.brailleRepresentation.boarder == null ? " " : osmElement.brailleRepresentation.boarder.ToString();
          dataRow14[2] = "boarder";
          dataTable.Rows.Add(dataRow14);

          dataRow17[0] = "ZIndex";
          dataRow17[1] = osmElement.brailleRepresentation.zIntex.ToString();
          dataRow17[2] = "zIntex";
          dataTable.Rows.Add(dataRow17);

          dataRow18[0] = "Padding";
          dataRow18[1] = osmElement.brailleRepresentation.padding == null ? " " : osmElement.brailleRepresentation.padding.ToString();
          dataRow18[2] = "padding";
          dataTable.Rows.Add(dataRow18);

          brailleTreeProp.ItemsSource = dataTable.DefaultView;*/



        /*

        /// <summary>
        /// Displays properties of the marked tree node of the braille tree in the properties table.
        /// </summary>
        /// <param name="IdGenerated"></param>
        void updateBrailleTable(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getBrailleTreeOsmElementById(IdGenerated);
            DataTable dataTable = new DataTable();
            DataColumn dc = new DataColumn();
            dataTable.Columns.Add(new DataColumn("Property"));
            dataTable.Columns.Add(new DataColumn("Content"));
            dataTable.Columns.Add(new DataColumn("Unsichtbar"));

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

            dataRow[0] = "IdGenerated";
            if (osmElement.properties.IdGenerated == null) { return; }
            dataRow[1] = osmElement.properties.IdGenerated.ToString();
            dataRow[2] = "IdGenerated";

            dataTable.Rows.Add(dataRow);
           // dataRow.RejectChanges();

            dataRow1["Property"] = "isEnabledFiltered";
            dataRow1["Content"] = osmElement.properties.isEnabledFiltered == null ? " " : osmElement.properties.isEnabledFiltered.ToString();
            dataRow1[2] = "isEnabledFiltered";
            dataTable.Rows.Add(dataRow1);
            
            
            dataRow2["Property"] = "boundingRectangleFiltered";
            dataRow2["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
            dataRow2[2] = "isEnabledFiltered";
            dataTable.Rows.Add(dataRow2);

            dataRow3["Property"] = "Value";
            dataRow3["Content"] = osmElement.properties.valueFiltered;
            dataRow3[2] = "valueFiltered";
            dataTable.Rows.Add(dataRow3);

            dataRow4[0] = "ControlType";
            dataRow4[1] = osmElement.properties.controlTypeFiltered;
            dataRow4[2] = "controlTypeFiltered";
            dataTable.Rows.Add(dataRow4);
           
            dataRow5[0] = "Name";
            dataRow5[1] = osmElement.brailleRepresentation.viewName;
            dataRow5[2] = "viewName";
            dataTable.Rows.Add(dataRow5);

            dataRow6[0] = "Visibility";
            dataRow6[1] = osmElement.brailleRepresentation.isVisible.ToString();
            dataRow6[2] = "isVisible";
            dataTable.Rows.Add(dataRow6);

            dataRow7[0] = "displayedGuiElementType";
            dataRow7[1] = osmElement.brailleRepresentation.displayedGuiElementType == null ? " " : osmElement.brailleRepresentation.displayedGuiElementType.ToString();
            dataRow7[2] = "displayedGuiElementType";
            dataTable.Rows.Add(dataRow7);

            dataRow8[0] = "Contrast";
            dataRow8[1] = osmElement.brailleRepresentation.contrast.ToString();
            dataRow8[2] = "contrast";
            dataTable.Rows.Add(dataRow8);

            dataRow9[0] = "Zoom";
            dataRow9[1] = osmElement.brailleRepresentation.zoom.ToString();
            dataRow9[2] = "zoom";
            dataTable.Rows.Add(dataRow9);

            dataRow10[0] = "Screen name";
            dataRow10[1] = osmElement.brailleRepresentation.screenName == null ? " " : osmElement.brailleRepresentation.screenName.ToString();
            dataRow10[2] = "screenName";
            dataTable.Rows.Add(dataRow10);

            dataRow11[0] = "Show Scrollbar";
            dataRow11[1] = osmElement.brailleRepresentation.isScrollbarShow.ToString();
            dataRow11[2] = "isScrollbarShow";
            dataTable.Rows.Add(dataRow11);
            

            dataRow12[0] = "UIElementSpecialContent";
            dataRow12[1] = osmElement.brailleRepresentation.uiElementSpecialContent == null ? " " : osmElement.brailleRepresentation.uiElementSpecialContent.ToString();
            dataRow12[2] = "uiElementSpecialContent";
            dataTable.Rows.Add(dataRow12);

            dataRow13[0] = "Margin";
            dataRow13[1] = osmElement.brailleRepresentation.margin == null ? " " : osmElement.brailleRepresentation.margin.ToString();
            dataRow13[2] = "margin";
            dataTable.Rows.Add(dataRow13);

            dataRow14[0] = "Boarder";
            dataRow14[1] = osmElement.brailleRepresentation.boarder == null ? " " : osmElement.brailleRepresentation.boarder.ToString();
            dataRow14[2] = "boarder";
            dataTable.Rows.Add(dataRow14);

            dataRow17[0] = "ZIndex";
            dataRow17[1] = osmElement.brailleRepresentation.zIntex.ToString();
            dataRow17[2] = "zIntex";
            dataTable.Rows.Add(dataRow17);

            dataRow18[0] = "Padding";
            dataRow18[1] = osmElement.brailleRepresentation.padding == null ? " " : osmElement.brailleRepresentation.padding.ToString();
            dataRow18[2] = "padding";
            dataTable.Rows.Add(dataRow18);

            brailleTreeProp.ItemsSource = dataTable.DefaultView;
        }*/

        /*  private void brailleTreeProp_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
          {
              if (((brailleTreeProp.Items)e.Row.Item).IsReadOnly)  //IsReadOnly is a property set in the MyCustomObject which is bound to each row
              {
                  e.Cancel = true;
              }
          }*/
        /// <summary>
        /// If a node of the braille tree is selected the braille display will be updated and also the tabple with the gui properties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void brailleTreeOutput_SelectedItemChanged(object sender,RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            TreeViewItem item;
            if (tree.SelectedItem is TreeViewItem)
            {
                // ... Handle a TreeViewItem.
                if (tree.Name.Equals(filteredRoot)) { item = filteredRoot; System.Console.WriteLine(" Filt in filtered: "); }
                else if (tree.Name.Equals(brailleRoot)) { item = brailleRoot; System.Console.WriteLine(" Filt in braille: "); }
                else { item = tree.SelectedItem as TreeViewItem; }
                if (item.Header is GuiFunctions.MenuItem && ((GuiFunctions.MenuItem)item.Header).IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                    updateBrailleTable(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                    screenViewIteration(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                    #region marked element in filtered tree
                    String connectedFilteredNode = treeOperations.searchNodes.getConnectedFilteredTreenodeId(((GuiFunctions.MenuItem)item.Header).IdGenerated);
                    if (connectedFilteredNode != null && connectedFilteredNode != "")
                    {
                        markedElementInTree(filteredTreeOutput, connectedFilteredNode);
                    }
                    else
                    {
                        clearTable(filteredTreeProp);
                        if (((TreeViewItem)filteredTreeOutput.SelectedItem) != null)
                        {
                            ((TreeViewItem)filteredTreeOutput.SelectedItem).IsSelected = false;
                        }
                        #endregion
                        int var1 = listBox_GuiElements.Items.IndexOf(((GuiFunctions.MenuItem)item.Header).controlTypeFiltered);
                        if (var1 < 0) { var1 = listBox_GuiElements.Items.IndexOf("Text"); }
                        listBox_GuiElements.SelectedIndex = var1;
                    }
                }else
                {
                    clearTable(brailleTreeProp);
                    clearTable(filteredTreeProp);
                }
            }
        }

        /// <summary>
        /// Marked an element in a <see cref="TreeView"/>
        /// </summary>
        /// <param name="treeViewElement">the TreeView element e.g. 'brailleTreeOutput' or 'filteredTreeOutput'</param>
        /// <param name="elementId">the id of the element in the TreeView</param>
        private void markedElementInTree(TreeView treeViewElement, String elementId)
        {
            TreeViewItem menuItem = getMenuItemById(treeViewElement, elementId);
            if (menuItem != null)
            {
                menuItem.IsSelected = true;
                menuItem.BringIntoView();
            }
        }

        /// <summary>
        /// seek all nodes with a special id
        /// </summary>
        /// <param name="treeView"> the treeView in which will be seek</param>
        /// <param name="id">the searched id</param>
        /// <returns>list of all nodes which have the searched <para>id</para> </returns>
        private TreeViewItem getMenuItemById(TreeView treeView, String id)
        {
            foreach (TreeViewItem tvi in treeView.Items)
            {
                Type typ = tvi.Header.GetType();
                try
                {
                    TreeViewItem result = GuiFunctions.Flatten(tvi).First(t => t != null && t.Header != null && t.Header.GetType().Equals(typeof(GuiFunctions.MenuItem)) && ((GuiFunctions.MenuItem)t.Header).IdGenerated != null && ((GuiFunctions.MenuItem)t.Header).IdGenerated.Equals(id));
                    if (result != null)
                    {
                        return result;
                    }
                }
                catch (InvalidOperationException) { }
            }
            return null;
        }

        void brailleTreeProp_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
           //   var p = (sender as DataGrid).CurrentItem;
           
            //MessageBox.Show("p:" + p);


              DataGrid grid = (DataGrid)sender;
              if (e.EditAction == DataGridEditAction.Commit)
              {
                var column = e.Column as DataGridComboBoxColumn;// as DataGridBoundColumn;
                Type t = e.Column.GetType();
                  if (column != null)
                  {
                    //var bindingPath = (column.Binding as Binding).Path.Path;
                    int rowIndex = e.Row.GetIndex();
                    int rowIndex1 = e.Row.GetIndex()-1;
                    
                    var el = e.EditingElement as ComboBox;
                    if(el == null || el.Text == null) { return; }
                    if (el.Items.CurrentItem.Equals(el.Text))
                    {
                        Debug.WriteLine("The item wouldn't change!");
                        return;
                    }
                   if(grid.CurrentCell.Column == null) { return; }
                    int columns = grid.CurrentCell.Column.DisplayIndex;
                    int columns1 = columns - 1;




                    //Variante 1
                    //TextBlock x = grid.Columns[0].GetCellContent(grid.Items[rowIndex1]) as TextBlock;
                    //if (x != null)
                    //    MessageBox.Show(x.Text);

                    
                    //var cellInfo = grid.CurrentCell;
                    if(columns1 < 0) { return; }
                    var dataGridCellInfo = new DataGridCellInfo(grid.Items[rowIndex], grid.Columns[columns1]);
                    if (dataGridCellInfo != null)
                    {
                        var columni = dataGridCellInfo.Column as DataGridBoundColumn;
           
                        if (columni != null)
                        {
                            var element = new FrameworkElement() { DataContext = dataGridCellInfo.Item };
                            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, columni.Binding);
                            var cellValue= element.Tag;
                            Console.WriteLine(" celle:" + cellValue);

                            String braillePropId = braillePropRoot.Items.First(p =>   p.Values.Name.Equals("IdGenerated")).Values.currentValue;
                            treeOperations.updateNodes.setBrailleTreeProperty(braillePropId, cellValue.ToString(), el.Text);
                            
                            // updateBrailleTable(globalID);
                            // screenViewIteration(globalID);

                            reloadTrees(null, braillePropId);
                        }
                    }
                  }
                //screenViewIteration
                //Update Table
              //  e.Row.UpdateLayout();
            }

        }

        private void clearTable(DataGrid table)
        {
            table.DataContext = new GuiFunctions.MyViewModel();
            table.Items.Refresh();
        }

        /// <summary>
        /// Reload the TreeView of the filtered tree and the Braille tree
        /// </summary>
        /// <param name="markedFilteredNodeId">id of the node in the filtered tree which should be marked</param>
        /// <param name="markedBrailleNodeId">id of the node in the Braille tree which should be marked</param>
        private void reloadTrees(String markedFilteredNodeId = null, String markedBrailleNodeId = null)
        {
            #region reload filtered tree
            if (grantTrees.filteredTree != null)
            {
                filteredTreeOutput.Items.Clear();
                filteredRoot.Items.Clear();
                guiFunctions.createTreeForOutput(grantTrees.filteredTree, ref filteredRoot, true);
                filteredTreeOutput.Items.Add(filteredRoot);
                clearTable(filteredTreeProp);
            }
            #endregion

            #region reload braille tree
            if (grantTrees.brailleTree != null)
            {
                brailleTreeOutput.Items.Clear();
                brailleRoot.Items.Clear();
                brailleRoot.Header = "Braille-Tree";
                guiFunctions.createTreeForOutput(grantTrees.brailleTree, ref brailleRoot, false);
                brailleTreeOutput.Items.Add(brailleRoot);
                brailleDisplaySimul.Items.Refresh();
                clearTable(brailleTreeProp);
            }
            #endregion
            if (markedFilteredNodeId != null && !markedFilteredNodeId.Equals(""))
            {
                markedElementInTree(filteredTreeOutput, markedFilteredNodeId);
            }
            if (markedBrailleNodeId != null && !markedBrailleNodeId.Equals(""))
            {
                markedElementInTree(brailleTreeOutput, markedBrailleNodeId);
            }
        }

        private void filteredTreeProp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dataGrid = sender as DataGrid;
            DataRowView rowView = dataGrid.SelectedItem as DataRowView;
            string myCellValue = rowView.Row[0].ToString();
            Console.WriteLine("AUSGABE: " + myCellValue);
        }

        private void brailleTreeProp_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            int columns = grid.CurrentCell.Column.DisplayIndex;
            int columns1 = columns - 1;

            FrameworkElement element_2 = grid.Columns[columns].GetCellContent(e.Row);
            var text2 = ((TextBox)element_2).Text;

            FrameworkElement element_3 = grid.Columns[columns1].GetCellContent(e.Row);
            var text3 = ((TextBox)element_3).Text;

            Console.WriteLine(" text3:" + text3);
            Console.WriteLine(" ID:" + braillePropRoot.Items.First(p => p.Values.Name.Equals("IdGenerated")).Values.currentValue);
            Console.WriteLine(" text2:" + text2);


            //treeOperations.updateNodes.setBrailleTreeProperty(globalID, text2, el.Text);
            //updateBrailleTable(globalID);
            //screenViewIteration(globalID);

            e.Row.HeaderTemplate = (DataTemplate)grid.FindResource("IdGenerated");
            e.Row.UpdateLayout();
        }

        private void brailleTreeProp_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;

            int columns = grid.CurrentCell.Column.DisplayIndex;
            int columns1 = columns - 1;

            //FrameworkElement element_2 = grid.Columns[columns].GetCellContent(e.Row);
            //var text2 = ((TextBox)element_2).Text;



            var _emp = e.Row.Item;
            Console.WriteLine(" emp:" + _emp);

            //FrameworkElement element_3 = grid.Columns[columns1].GetCellContent(e.Row);
            //var text3 = ((TextBox)element_3).Text;
            //var text2 = element_3 as TextBox;


            /*      Console.WriteLine(" text3:" + text3);
                  Console.WriteLine(" ID:" + globalID);
                  Console.WriteLine(" text2:" + text2);
                  */
            //treeOperations.updateNodes.setBrailleTreeProperty(globalID, text2, el.Text);
            //updateBrailleTable(globalID);
            //screenViewIteration(globalID);

            e.Row.HeaderTemplate = (DataTemplate)grid.FindResource("IdGenerated");
            e.Row.UpdateLayout();
        }

        private void AddNode_Click(object sender, RoutedEventArgs e)
        {
            if(grantTrees.filteredTree == null) { return; }
            TreeViewItem selectedItem_filteredTree = filteredTreeOutput.SelectedItem as TreeViewItem;
            String selectedFilteredItemId = "";
            String selectedBrailleItemId = "";
            if (selectedItem_filteredTree != null)
            {
                //Add node & connection to filteredTree
                if (selectedItem_filteredTree.Header != null && selectedItem_filteredTree.Header.GetType().Equals(typeof(GuiFunctions.MenuItem)))
                {
                    selectedFilteredItemId = ((GuiFunctions.MenuItem)selectedItem_filteredTree.Header).IdGenerated;
                }

            }
            TreeViewItem selectedItem_brailleTree = brailleTreeOutput.SelectedItem as TreeViewItem;
            if (selectedItem_brailleTree != null)
            {
                if (selectedItem_brailleTree.Header != null && selectedItem_brailleTree.Header.GetType().Equals(typeof(GuiFunctions.MenuItem)))
                {
                    selectedBrailleItemId = ((GuiFunctions.MenuItem)selectedItem_brailleTree.Header).IdGenerated;
                }
            }
            String guiControlTypeSelected = listBox_GuiElements.SelectedItem as String;

            Object filteredNodeObject = selectedFilteredItemId.Equals("") ? null : treeOperations.searchNodes.getNode(selectedFilteredItemId, grantTrees.filteredTree);
            Object brailleNodeObject = selectedBrailleItemId.Equals("") ? null : treeOperations.searchNodes.getNode(selectedBrailleItemId, grantTrees.brailleTree);
            String brailleNodeIdNew;
            if (brailleNodeObject == null)
            {
                brailleNodeIdNew = guiFunctions.addFilteredNodeToBrailleTree(guiControlTypeSelected, filteredNodeObject);
            }
            else
            {
                OSMElement.OSMElement brailleNodeObject_osm = strategyMgr.getSpecifiedTree().GetData(brailleNodeObject);
                brailleNodeIdNew = guiFunctions.addFilteredNodeToBrailleTree(guiControlTypeSelected, filteredNodeObject, brailleNodeObject_osm.brailleRepresentation.screenName, brailleNodeObject_osm.brailleRepresentation.typeOfView);
            }
            //refresh + select new node
            reloadTrees(null, brailleNodeIdNew);
        }

        private void ClearSelection_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem_filteredTree = filteredTreeOutput.SelectedItem as TreeViewItem;
            if (selectedItem_filteredTree != null)
            {
                if (selectedItem_filteredTree.Header != null && selectedItem_filteredTree.Header.GetType().Equals(typeof(GuiFunctions.MenuItem)))
                {
                   TreeViewItem itemFiltered = getMenuItemById(filteredTreeOutput, ((GuiFunctions.MenuItem)selectedItem_filteredTree.Header).IdGenerated);
                    itemFiltered.IsSelected = false;
                }
                clearTable(filteredTreeProp);
            }
            TreeViewItem selectedItem_brailleTree = brailleTreeOutput.SelectedItem as TreeViewItem;
            if (selectedItem_brailleTree != null)
            {
                if (selectedItem_brailleTree.Header != null && selectedItem_brailleTree.Header.GetType().Equals(typeof(GuiFunctions.MenuItem)))
                {
                    TreeViewItem itemBraille = getMenuItemById(brailleTreeOutput, ((GuiFunctions.MenuItem)selectedItem_brailleTree.Header).IdGenerated);
                    itemBraille.IsSelected = false;
                }
                clearTable(brailleTreeProp);
            }
        }


        /*
        void brailleTreeProp_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
                {
                    DataGrid grid = (DataGrid)sender;
                    Console.WriteLine("Stufe 1:");
                    if (e.EditAction == DataGridEditAction.Commit)
                    {
                        Console.WriteLine("Stufe 2:");
                        var column = e.Column as DataGridBoundColumn;
                        if (column != null)
                        {
                            Console.WriteLine("Stufe 3:");
                            var bindingPath = (column.Binding as Binding).Path.Path;
                           // if (bindingPath == "Value_Titel")
                            //{

                                int rowIndex = e.Row.GetIndex();
                            int rowIndex1 = e.Row.GetIndex()-1;
                            var el = e.EditingElement as TextBox;

                            var dataGridCellInfo = new DataGridCellInfo(grid.Items[rowIndex1], grid.Columns[0]);
                            Console.WriteLine(" cellinfo:" + dataGridCellInfo);



                            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(rowIndex1);
                            Console.WriteLine("row" + row);


                            //  var currentRowIndex = grid.Items.IndexOf(grid.CurrentItem);

                            Console.WriteLine(" text:" + el.Text);
                            Console.WriteLine(" ID:" + globalID);

                            int ColumnIndex = e.Column.DisplayIndex;
                               // Double amount = Double.Parse(((TextBox)e.EditingElement).Text);

                                Console.WriteLine("ColumnIndex:" + ColumnIndex);
                                Console.WriteLine("AUSGABE: " + DataGridEditingUnit.Row.ToString());




                            //   this.braillePropRoot.Items.Where

                            //  int colIndex = brailleTreeProp.Columns.IndexOf(brailleTreeProp.Columns[IdGenerated]);
                            //  DataRow dtr = ((System.Data.DataRowView)(DataGrid1.SelectedValue)).Row;

                            //string strEID = _DataGrid.SelectedCells[0].Item.ToString();
                            // this.braillePropRoot.ColumnNames.Select()

                            // rowIndex has the row index
                            // bindingPath has the column's binding
                            // el.Text has the new, user-entered value
                            //}
                        }
                    }
                }

                private void filteredTreeProp_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    DataGrid dataGrid = sender as DataGrid;
                    DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                    string myCellValue = rowView.Row[0].ToString();
                    Console.WriteLine("AUSGABE: " + myCellValue);
                }

                private void brailleTreeProp_SelectionChanged(object sender, SelectionChangedEventArgs e)
                {
                    DataGrid dataGrid = sender as DataGrid;
                    DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                    string myCellValue = rowView.Row[0].ToString();
                    Console.WriteLine("AUSGABE: " + myCellValue);
                }



                private bool isManualEditCommit;
                private void brailleTreeProp_CellEditEnding_1(object sender, DataGridCellEditEndingEventArgs e)
                {
                    if (!isManualEditCommit)
                    {
                        isManualEditCommit = true;
                        DataGrid grid = (DataGrid)sender;
                        grid.CommitEdit(DataGridEditingUnit.Row, true);
                        isManualEditCommit = false;





                        Console.WriteLine("AUSGABE: " + DataGridEditingUnit.Row.ToString());

                    }
                }



                */





        /*
private void brailleTreeProp_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
{
if(e.EditAction == DataGridEditAction.Commit)
{


}
}*/
    }
}

