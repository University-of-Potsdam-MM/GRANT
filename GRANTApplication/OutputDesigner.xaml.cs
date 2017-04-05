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
            strategyMgr.setSpecifiedEventManager(possibleEventManager[0].className);
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
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperations);
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperations);
            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperations);
            filteredTreeOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(filteredTreeOutput_SelectedItemChanged);
            brailleTreeOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(brailleTreeOutput_SelectedItemChanged);
            guiFunctions = new GuiFunctions(strategyMgr, grantTrees, treeOperations);
            filteredRoot = new TreeViewItem();
            brailleRoot = new TreeViewItem();
            screenRoot = new GuiFunctions.MenuItem();
            SaveButton.IsEnabled = false;
            LoadTemplate.IsEnabled = false;
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
                System.Console.WriteLine(" ROW: " + row1);
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
                Object tree = grantTrees.filteredTree;
                filteredTreeOutput.Items.Clear();
                filteredRoot.Items.Clear();
                guiFunctions.createTreeForOutput(tree, ref filteredRoot);
                SaveButton.IsEnabled = true;
                LoadTemplate.IsEnabled = true;
                filteredTreeOutput.Items.Add(filteredRoot);
                int var3 = comboBox2.Items.IndexOf(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().ToString());
                comboBox2.SelectedIndex = var3;
                listGuiElements();
                int dWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
                int dHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
                createBrailleDisplay(dWidth, dHeight, brailleDisplaySimul);
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
                    brailleTreeProp.ItemsSource = "";
                    brailleTreeProp.Items.Refresh();
                    #endregion
                }
            }
        }

        /// <summary>
        /// Load existing design template.
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
            if (result == true)
            {
                strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(dlg.FileName);
                Object tree1 = grantTrees.brailleTree;
                #region Marlene testet
                TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTrees, treeOperations);
                tempTextView.createTextviewOfSubtree(grantTrees.filteredTree);
                #endregion 
                brailleTreeOutput.Items.Clear();
                brailleRoot.Items.Clear();
               brailleRoot.Header = "Braille-Tree";
                guiFunctions.createTreeForOutput(tree1, ref brailleRoot);
                SaveButton.IsEnabled = true;
                brailleTreeOutput.Items.Add(brailleRoot);
                brailleDisplaySimul.Items.Refresh();
                brailleTreeProp.ItemsSource = "";
                brailleTreeProp.Items.Refresh();
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
        /// Displays properties of the marked tree node of the filtered tree in an table.
        /// </summary>
        /// <param name="IdGenerated"></param>
        void updateFilteredTable(String IdGenerated)
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
        }

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
        void updateBrailleTable(String IdGenerated)
        {
            OSMElement.OSMElement osmElement = treeOperations.searchNodes.getBrailleTreeOsmElementById(IdGenerated);
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

            dataRow[0] = "IdGenerated";
            if (osmElement.properties.IdGenerated == null) { return; }
            dataRow[1] = osmElement.properties.IdGenerated.ToString();
            dataTable.Rows.Add(dataRow);

            dataRow1["Property"] = "isEnabledFiltered";
            dataRow1["Content"] = osmElement.properties.isEnabledFiltered == null ? " " : osmElement.properties.isEnabledFiltered.ToString();
            dataTable.Rows.Add(dataRow1);

            dataRow2["Property"] = "boundingRectangleFiltered";
            dataRow2["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
            dataTable.Rows.Add(dataRow2);
            dataTable.Rows.Add();

            dataRow3["Property"] = "Value";
            dataRow3["Content"] = osmElement.properties.valueFiltered;
            dataTable.Rows.Add(dataRow3);

            dataRow4[0] = "ControlType";
            dataRow4[1] = osmElement.properties.controlTypeFiltered;
            dataTable.Rows.Add(dataRow4);
           
            dataRow5[0] = "Name";
            dataRow5[1] = osmElement.brailleRepresentation.viewName;
            dataTable.Rows.Add(dataRow5);

            dataRow6[0] = "Visibility";
            dataRow6[1] = osmElement.brailleRepresentation.isVisible.ToString(); 
            dataTable.Rows.Add(dataRow6);

            dataRow7[0] = "displayedGuiElementType";
            dataRow7[1] = osmElement.brailleRepresentation.displayedGuiElementType == null ? " " : osmElement.brailleRepresentation.displayedGuiElementType.ToString();  
            dataTable.Rows.Add(dataRow7);

            dataRow8[0] = "Contrast";
            dataRow8[1] = osmElement.brailleRepresentation.contrast.ToString();
            dataTable.Rows.Add(dataRow8);

            dataRow9[0] = "Zoom";
            dataRow9[1] = osmElement.brailleRepresentation.zoom.ToString();
            dataTable.Rows.Add(dataRow9);

            dataRow10[0] = "Screen name";
            dataRow10[1] = osmElement.brailleRepresentation.screenName == null ? " " : osmElement.brailleRepresentation.screenName.ToString();
            dataTable.Rows.Add(dataRow10);

            dataRow11[0] = "Show Scrollbar";
            dataRow11[1] = osmElement.brailleRepresentation.isScrollbarShow.ToString();
            dataTable.Rows.Add(dataRow11);

            dataRow12[0] = "UIElementSpecialContent";
            dataRow12[1] = osmElement.brailleRepresentation.uiElementSpecialContent == null ? " " : osmElement.brailleRepresentation.uiElementSpecialContent.ToString();
            dataTable.Rows.Add(dataRow12);

            dataRow13[0] = "Margin";
            dataRow13[1] = osmElement.brailleRepresentation.margin == null ? " " : osmElement.brailleRepresentation.margin.ToString();
            dataTable.Rows.Add(dataRow13);

            dataRow14[0] = "Boarder";
            dataRow14[1] = osmElement.brailleRepresentation.boarder == null ? " " : osmElement.brailleRepresentation.boarder.ToString();
            dataTable.Rows.Add(dataRow14);

            dataRow17[0] = "ZIndex";
            dataRow17[1] = osmElement.brailleRepresentation.zIntex.ToString();
            dataTable.Rows.Add(dataRow17);

            dataRow18[0] = "Padding";
            dataRow18[1] = osmElement.brailleRepresentation.padding == null ? " " : osmElement.brailleRepresentation.padding.ToString();
            dataTable.Rows.Add(dataRow18);
            brailleTreeProp.ItemsSource = dataTable.DefaultView;
        }

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
                        List<TreeViewItem> menuItem = getMenuItemsById(filteredTreeOutput, connectedFilteredNode);
                        TreeViewItem tvi = new TreeViewItem();
                        if (menuItem != null && menuItem.Count == 1)
                        {
                            menuItem[0].IsSelected = true;
                            menuItem[0].BringIntoView();
                        }
                    }
                    else
                    {
                        if (((TreeViewItem)filteredTreeOutput.SelectedItem) != null)
                        {
                            ((TreeViewItem)filteredTreeOutput.SelectedItem).IsSelected = false;
                        }
                        #endregion
                        int var1 = listBox_GuiElements.Items.IndexOf(((GuiFunctions.MenuItem)item.Header).controlTypeFiltered);
                        if (var1 < 0) { var1 = listBox_GuiElements.Items.IndexOf("Text"); }
                        listBox_GuiElements.SelectedIndex = var1;
                    }
                }
            }
        }

        /// <summary>
        /// seek all nodes with a special id
        /// </summary>
        /// <param name="treeView"> the treeView in which will be seek</param>
        /// <param name="id">the searched id</param>
        /// <returns>list of all nodes with have the searched <para>id</para> </returns>
        private List<TreeViewItem> getMenuItemsById(TreeView treeView, String id)
        {
            foreach (TreeViewItem tvi in treeView.Items)
            {
                var result = GuiFunctions.Flatten(tvi).Where(t => t.Header != null && ((GuiFunctions.MenuItem)t.Header).IdGenerated != null && ((GuiFunctions.MenuItem)t.Header).IdGenerated.Equals(id));
                if (result != null && result.Count<TreeViewItem>() > 0)
                    return result.ToList<TreeViewItem>();
            }
            return null;
        }
    }
}

