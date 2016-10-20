using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows.Media;
using System.Data;
using System.Windows.Data;
using System.Globalization;
using System.Collections;
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
        GuiFunctions.MenuItem filteredRoot;
        GuiFunctions.MenuItem brailleRoot;
        GuiFunctions guiFunctions;
        int var2;
        String matrix;

        [System.ComponentModel.BrowsableAttribute(false)]
        public DataGridCell CurrentCell { get; set; }

        public OutputDesigner()
        {
            InitializeComponent();
            settings = new Settings();
            strategyMgr = new StrategyManager();
            grantTrees = new GeneratedGrantTrees();
            treeOperations = new TreeOperation(strategyMgr, grantTrees);
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
            //tvMain.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tvMain_SelectedItemChanged);
            // listBox1.SelectedItem += new RoutedPropertyChangedEventHandler<object>(listBox1_SelectionChanged);

            //tvOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(trees_SelectedItemChanged);
            tvOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tvOutput_SelectedItemChanged);
            
            //brailleOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(trees_SelectedItemChanged);
            brailleOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(brailleOutput_SelectedItemChanged);



            guiFunctions = new GuiFunctions(strategyMgr, grantTrees, treeOperations);

            filteredRoot = new GuiFunctions.MenuItem();
            brailleRoot = new GuiFunctions.MenuItem();

            //NodeButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            LoadTemplate.IsEnabled = false;
            //dataGrid3.Visibility = false;



        }
        public class CellContent
        {

            public String cellinput
            {
                get;
                set;
            }
        }

        /*    private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (((ListBox)sender).SelectedItem != null)
                {
                    String element = (sender as ListBox).SelectedItem.ToString();

                    // tb.Text = "   You selected " + lbi.Content.ToString() + ".";

                    bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(element);


                    // createBrailleDisplayMatrix((guiElementRep.Length / guiElementRep.GetLength(0)), guiElementRep.GetLength(0), dataGrid3, guiElementRep);
                    //dataGrid3.
                    System.Console.WriteLine("WIDTH MAt:" + (guiElementRep.Length / guiElementRep.GetLength(0)).ToString());
                    System.Console.WriteLine("HEIGHT MAt:" + guiElementRep.GetLength(0).ToString());
                    DataTable dataTable4 = new DataTable();

                    for (int a = 0; a < (guiElementRep.Length / guiElementRep.GetLength(0)); a++)//breite 
                    {
                        //System.Console.WriteLine(a.ToString());
                        System.Console.WriteLine("a:" + a.ToString());
                        // DataGridRow row = (DataGridRow)dataGrid3.ItemContainerGenerator
                        //                         .ContainerFromIndex(i);
                        //  DataGridRow row = (DataGridRow)dataGrid3.ItemContainerGenerator
                        //  .ContainerFromIndex(i);
                        // row.Background = Brushes.Red;
                        //DataRow dr = (DataRow)row.DataContext;
                        DataColumn dCol1 = new DataColumn(a.ToString()); // string FirstC=”column1″
                       dataTable4.Columns.Add(dCol1);


                        for (int i = 0; i < guiElementRep.GetLength(0); i++) //höhe
                        {
                            System.Console.WriteLine("i: " + i.ToString());

                            System.Console.WriteLine(guiElementRep[i, a].ToString());
                           // DataRow row1 = dataTable4.NewRow();

                            //dataTable4.Rows.Add(row1);
                           // dataTable4.Rows[i][a] = guiElementRep[i, a].ToString();

                        //System.Console.WriteLine(i.ToString());
                            //DataGridCell cell = new DataGridCell();
                            // string s = (dataGrid3.Items[i] as DataRowView).Row.ItemArray[a].ToString();
                            // dataGrid3.CurrentCell = new DataGridCellInfo(dataGrid3.Items[i], dataGrid3.Columns[a]);

                        }
                        DataRow row1 = dataTable4.NewRow();

                        dataTable4.Rows.Add(row1);
                        //dataTable4.Rows[i][a] = guiElementRep[i, a].ToString();

                    }
                    dataGrid3.ItemsSource = dataTable4.AsDataView();
                }
            }*/


        //Variante datagrid
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dataGrid3.ItemsSource = null;
            if (((ListBox)sender).SelectedItem != null)
            {
                String element = (sender as ListBox).SelectedItem.ToString();
                bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(element);

                //DataTable dataTable4 = createBrailleDisplay3((guiElementRep.Length / guiElementRep.GetLength(0)), guiElementRep.GetLength(0), dataGrid3);

                DataTable dataTable4 = createBrailleDisplayMatrix((guiElementRep.Length / guiElementRep.GetLength(0)), guiElementRep.GetLength(0), dataGrid3);

                for (int a = 0; a < (guiElementRep.Length / guiElementRep.GetLength(0)); a++)//breite / Column
                {
                    for (int i = 0; i < guiElementRep.GetLength(0); i++) //höhe
                    {
                        dataTable4.Rows[i][a] = "";
                        //dataTable4.Rows[i][a] = guiElementRep[i, a].ToString();
                    }

                }
                dataGrid3.ItemsSource = dataTable4.AsDataView();
                paintMatrix(dataGrid3, guiElementRep);
            }
        }

        public void paintMatrix(DataGrid datagrid, bool[,] guiElementRep)
        {
           for (int h = 0; h < dataGrid3.Items.Count; h++)
           // for (int h = 0; h < (guiElementRep.GetLength(0)); h++) //höhe
            {
               DataGridRow row = (DataGridRow)dataGrid3.ItemContainerGenerator.ContainerFromIndex(h);
               if (row == null)
               {
                  dataGrid3.UpdateLayout();
                  dataGrid3.ScrollIntoView(dataGrid3.Items[h]);
                  row = (DataGridRow)dataGrid3.ItemContainerGenerator.ContainerFromIndex(h);
               }
               for (int w = 0; w < dataGrid3.Columns.Count; w++)
                {
                    if (dataGrid3.Columns[w].GetCellContent(row) == null)
                    {
                        dataGrid3.UpdateLayout();
                    }
                    DataGridCell firstColumnInFirstRow = dataGrid3.Columns[w].GetCellContent(row).Parent as DataGridCell;
                    if (guiElementRep[h, w].ToString() == "True") { 
                        firstColumnInFirstRow.Background = Brushes.Black;
                    }
                    else {
                        firstColumnInFirstRow.Background = Brushes.White;
                    }
                }
               
            }
        }


        //wenn unten gemalt wird diese methode raus
        private void createBrailleDisplay(int dWidth, int dHeight, DataGrid dataGrid)
        {
            //var dataGrid = sender as DataGrid;
            System.Console.WriteLine(" DWIDTH: " + dWidth);
            System.Console.WriteLine(" DHEIGHT: " + dHeight);
            // Add 10 Rows

            DataTable dataTable = new DataTable();
            for (int i = 0; i < dWidth; i++)
            {
                DataColumn dCol1 = new DataColumn(i.ToString()); // string FirstC=”column1″
                dataTable.Columns.Add(dCol1);
            }

            //dataGrid2.ItemsSource = dataTable.DefaultView;
            for (int i = 0; i < dHeight; i++)
            {
                DataRow row1 = dataTable.NewRow();
                dataTable.Rows.Add(row1);
                System.Console.WriteLine(" ROW: " + row1);
            }

            //dataGrid.ItemsSource = dataTable.DefaultView;
            dataGrid.ItemsSource = dataTable.AsDataView();
        }

        private DataTable createBrailleDisplayMatrix(int dWidth, int dHeight, DataGrid dataGrid)
        {
            DataTable dataTable = new DataTable();
            for (int i = 0; i < dWidth; i++){
                DataColumn dCol1 = new DataColumn(i.ToString()); // string FirstC=”column1″
                dataTable.Columns.Add(dCol1);
            }
            for (int i = 0; i < dHeight; i++){
                DataRow row1 = dataTable.NewRow();
                dataTable.Rows.Add(row1);
            }
            dataGrid.ItemsSource = dataTable.AsDataView();
            return dataTable;
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (grantTrees.getFilteredTree() == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredProject_" + strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Child( grantTrees.getFilteredTree())).properties.nameFiltered; // Default file name
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
                // guiFunctions.loadFilteredTree(dlg.FileName);
                guiFunctions.loadGrantProject(dlg.FileName);


                Object tree = grantTrees.getFilteredTree();

                tvOutput.Items.Clear();
                filteredRoot.Items.Clear();

                //TreeViewItem root = new TreeViewItem();

                filteredRoot.controlTypeFiltered = "Filtered- Updated- Tree";

                //
                guiFunctions.treeIteration(strategyMgr.getSpecifiedTree().Copy(tree), ref filteredRoot); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
                
                SaveButton.IsEnabled = true;
                LoadTemplate.IsEnabled = true;
                tvOutput.Items.Add(filteredRoot);



                int var3 = comboBox2.Items.IndexOf(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().ToString());
                comboBox2.SelectedIndex = var3;
                listGuiElements();
                int dWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
                int dHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
                createBrailleDisplay(dWidth, dHeight, dataGrid2);

            }// Load Project wirft Fehler
        }

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
                Object tree1 = grantTrees.getBrailleTree();

                brailleOutput.Items.Clear();
                brailleRoot.Items.Clear();

                // TreeViewItem root = new TreeViewItem();

                brailleRoot.controlTypeFiltered = "Braille-Tree";

                //
              //  guiFunctions.createTreeForOutput(strategyMgr.getSpecifiedTree().Copy(tree), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
                guiFunctions.createTreeForOutput(tree1, ref brailleRoot); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
               
                SaveButton.IsEnabled = true;
                brailleOutput.Items.Add(brailleRoot);
            }// Load Project wirft Fehler
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {

            if (strategyMgr.getSpecifiedDisplayStrategy() == null)
            {
                strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            }

            // ... A List.

            //strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);

            List<string> Combobox2items = new List<string>();
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
            // ... Get the ComboBox reference.


            var comboBox = sender as ComboBox;


            //strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice()
            int var3 = comboBox.Items.IndexOf(strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().ToString());

            comboBox.SelectedIndex = var3;
            //comboBox.SelectedIndex = 1;
            //Combobox2items.Add("Choose");
            foreach (Device d in devices)
            {
                Combobox2items.Add(d.ToString());

            }

            // ... Assign the ItemsSource to the List.
            comboBox.ItemsSource = Combobox2items;
            //createBrailleDisplay();


        }

        private void listGuiElements()
        {
            List<string> guiElements = strategyMgr.getSpecifiedBrailleDisplay().getUiElementRenderer();
            listBox1.Items.Clear();
            foreach (String s in guiElements)
            {
                listBox1.Items.Add(s);

            }


        }

   /*     private void createBrailleDisplay(int dWidth, int dHeight, DataGrid dataGrid)
        {
            //var dataGrid = sender as DataGrid;
            System.Console.WriteLine(" DWIDTH: " + dWidth);

            System.Console.WriteLine(" DHEIGHT: " + dHeight);
            // Add 10 Rows
            DataTable dataTable = new DataTable();

            // Add 7 Columns
            for (int i = 0; i < dWidth; i++)
            {

                DataColumn dCol1 = new DataColumn(i.ToString()); // string FirstC=”column1″
                dataTable.Columns.Add(dCol1);
                
            }

            //dataGrid2.ItemsSource = dataTable.DefaultView;
            for (int i = 0; i < dHeight; i++)
            {
                DataRow row1 = dataTable.NewRow();
                dataTable.Rows.Add(row1);
                System.Console.WriteLine(" ROW: " + row1);
            }

            //dataGrid.ItemsSource = dataTable.DefaultView;
            
            
            dataGrid.ItemsSource = dataTable.AsDataView();
        }


     */ 



        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            if (value == null) { return; }
            Device d = strategyMgr.getSpecifiedDisplayStrategy().getDeviceByName(value);
            strategyMgr.getSpecifiedDisplayStrategy().setActiveDevice(d);
            // methode aufrufen
            listGuiElements();
            int dWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            int dHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
            //dataGrid2.ItemsSource = null;
           
            createBrailleDisplay(dWidth, dHeight, dataGrid2); // später das zu matrix machen
            // this.Title = "Selected: " + value;

        }


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
            //dataRow12["Content"] = filteredSubtree.properties.grantFilterStrategy == null ? " " : filteredSubtree.properties.grantFilterStrategy.ToString();
            //dataRow12["Content"] =  filteredSubtree.properties.grantFilterStrategyFullName;
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

            String ids = String.Join(" : ", osmElement.properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray());

            dataRow24["Property"] = "runtimeIDFiltered";
            dataRow24["Content"] = ids;
            dataTable.Rows.Add(dataRow24);

            dataRow25["Property"] = "boundingRectangleFiltered";
            dataRow25["Content"] = osmElement.properties.boundingRectangleFiltered == null ? " " : osmElement.properties.boundingRectangleFiltered.ToString();
            dataTable.Rows.Add(dataRow25);
            dataTable.Rows.Add();



            //dataTable.Rows.Add();

            //dataTable.Rows.Add(dataRow);
            dataGrid4.ItemsSource = dataTable.DefaultView;

            System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);


            // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint)
            strategyMgr.getSpecifiedOperationSystem().paintRect(rect);

            // NodeButton.CommandParameter = IdGenerated;

        }

        void tvOutput_SelectedItemChanged(object sender,
      RoutedPropertyChangedEventArgs<object> e)
        {

            var tree = sender as TreeView;
            System.Console.WriteLine(" NAMEtvoutup!!!!!!!!!!!!!!!!!!!: " + tree.Name);
            GuiFunctions.MenuItem item;
            if (tree.SelectedItem is GuiFunctions.MenuItem)
            {
                // ... Handle a TreeViewItem.
                if (tree.Name.Equals(filteredRoot)) { item = filteredRoot; System.Console.WriteLine(" Filt in filtered: "); }
               
                else if (tree.Name.Equals(brailleRoot)) { item = brailleRoot; System.Console.WriteLine(" Filt in braille: "); }
                else { item = tree.SelectedItem as GuiFunctions.MenuItem; }
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
                if (item.IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(item.IdGenerated);
                    System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
                    if (osmElement.properties.isOffscreenFiltered == false) { 
                     strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                    }
                    int var1 = listBox1.Items.IndexOf(item.controlTypeFiltered);
                    if (var1 < 0) { var1 = listBox1.Items.IndexOf("Text"); }
                    listBox1.SelectedIndex = var1;

                    updateFilteredTable(item.IdGenerated);

                    System.Console.WriteLine(" INDEX: " + var1);

                  }

            }


        }

        void updateBrailleTable(String IdGenerated)
        {
            System.Console.WriteLine(" HIEEEEEEEEER in der Methode: ");

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
            dataRow4[1] = osmElement.properties.controlTypeFiltered.ToString();
            dataTable.Rows.Add(dataRow4);
           
            dataRow5[0] = "Name";
            dataRow5[1] = osmElement.brailleRepresentation.viewName.ToString();
            dataTable.Rows.Add(dataRow5);

            dataRow6[0] = "Visibility";
            dataRow6[1] = osmElement.brailleRepresentation.isVisible.ToString(); 
            dataTable.Rows.Add(dataRow6);

    /*        dataRow7[0] = "Matrix";
            dataRow7[1] = osmElement.brailleRepresentation.matrix.ToString();
            dataTable.Rows.Add(dataRow7);

            dataRow8[0] = "FromGuiElement";
            dataRow8[1] = osmElement.brailleRepresentation.fromGuiElement.ToString();
            dataTable.Rows.Add(dataRow8);

            dataRow9[0] = "Contrast";
            dataRow9[1] = osmElement.brailleRepresentation.contrast.ToString();
            dataTable.Rows.Add(dataRow9);

            dataRow10[0] = "Zoom";
            dataRow10[1] = osmElement.brailleRepresentation.zoom.ToString();
            dataTable.Rows.Add(dataRow10);

            dataRow11[0] = "Screen name";
            dataRow11[1] = osmElement.brailleRepresentation.screenName.ToString();
            dataTable.Rows.Add(dataRow11);

            dataRow12[12] = "Show Scrollbar";
            dataRow12[12] = osmElement.brailleRepresentation.showScrollbar.ToString();
            dataTable.Rows.Add(dataRow12);

            dataRow13[13] = "UIElementSpecialContent";
            dataRow13[13] = osmElement.brailleRepresentation.uiElementSpecialContent.ToString();
            dataTable.Rows.Add(dataRow13);

            dataRow14[0] = "Padding";
            dataRow14[1] = osmElement.brailleRepresentation.padding.ToString();
            dataTable.Rows.Add(dataRow14);

            dataRow15[0] = "Margin";
            dataRow15[1] = osmElement.brailleRepresentation.margin.ToString();
            dataTable.Rows.Add(dataRow15);

            dataRow16[0] = "Boarder";
            dataRow16[1] = osmElement.brailleRepresentation.boarder.ToString();
            dataTable.Rows.Add(dataRow16);

            dataRow17[0] = "ZIndex";
            dataRow17[1] = osmElement.brailleRepresentation.zIntex.ToString();
            dataTable.Rows.Add(dataRow17);
            */

            /*   dataRow23["Property"] = "hWndFiltered";
               dataRow23["Content"] = osmElement.properties.hWndFiltered == null ? " " : osmElement.properties.hWndFiltered.ToString();
               dataTable.Rows.Add(dataRow23);

               String ids = String.Join(" : ", osmElement.properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray());

               dataRow24["Property"] = "runtimeIDFiltered";
               dataRow24["Content"] = ids;
               dataTable.Rows.Add(dataRow24);
               */




            //dataTable.Rows.Add();

            //dataTable.Rows.Add(dataRow);
            dataGrid5.ItemsSource = dataTable.DefaultView;

            //System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);


            // this.Paint += new System.Windows.Forms.PaintEventHandler(this.Window_Paint)
            //strategyMgr.getSpecifiedOperationSystem().paintRect(rect);

            // NodeButton.CommandParameter = IdGenerated;

        }

  /*      void trees_SelectedItemChanged(object sender,
      RoutedPropertyChangedEventArgs<object> e)
        {

            var tree = sender as TreeView;
            System.Console.WriteLine(" NAMEtbraille!!!!!!!!!!!!!!!!!!!: " + tree.Name);
            GuiFunctions.MenuItem item; 
            if (tree.SelectedItem is GuiFunctions.MenuItem)
            {
                // ... Handle a TreeViewItem.
                if (tree.Name == "filteredRoot") { item = filteredRoot; }
                else if (tree.Name == "brailleRoot") { item = brailleRoot;}
                else  { item = tree.SelectedItem as GuiFunctions.MenuItem; }
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
                if (item.IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(item.IdGenerated);
                    System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
                    if (osmElement.properties.isOffscreenFiltered == false)
                    {
                        strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                    }
                    int var1 = listBox1.Items.IndexOf(item.controlTypeFiltered);
                    if (var1 < 0) { var1 = listBox1.Items.IndexOf("Text"); }
                    listBox1.SelectedIndex = var1;

                    if (tree.Name == "filteredRoot") { System.Console.WriteLine(" in filtered: "); updateFilteredTable(item.IdGenerated); }
                    if (tree.Name == "brailleRoot") { System.Console.WriteLine(" in braille: ");  updateBrailleTable(item.IdGenerated); }
                    

                    System.Console.WriteLine(" INDEX: " + var1);

                }

            }
        }
        */

        void brailleOutput_SelectedItemChanged(object sender,
  RoutedPropertyChangedEventArgs<object> e)
        {

            var tree = sender as TreeView;
            System.Console.WriteLine(" NAMEtbraille!!!!!!!!!!!!!!!!!!!: " + tree.Name);

            GuiFunctions.MenuItem item;
            if (tree.SelectedItem is GuiFunctions.MenuItem)
            {
                // ... Handle a TreeViewItem.
                if (tree.Name.Equals(filteredRoot)) { item = filteredRoot; System.Console.WriteLine(" Filt in filtered: "); }

                else if (tree.Name.Equals(brailleRoot)) { item = brailleRoot; System.Console.WriteLine(" Filt in braille: "); }
                else { item = tree.SelectedItem as GuiFunctions.MenuItem; }
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
                if (item.IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperations.searchNodes.getFilteredTreeOsmElementById(item.IdGenerated);
                    //System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
                    //if (osmElement.properties.isOffscreenFiltered == false)
                    //{
                    //    strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                    //}
                    int var1 = listBox1.Items.IndexOf(item.controlTypeFiltered);
                    if (var1 < 0) { var1 = listBox1.Items.IndexOf("Text"); }
                    listBox1.SelectedIndex = var1;

                    updateBrailleTable(item.IdGenerated);

                    System.Console.WriteLine(" INDEX: " + var1);

                }

            }
        }

    }
}
//wird im  moment nicht aufgerufen
/*  private void LoadTree_Click(object sender, RoutedEventArgs e)
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


      ITreeStrategy<OSMElement.OSMElement> parentNode = grantTrees.getFilteredTree();

      tvOutput.Items.Clear();
      root.Items.Clear();

      //TreeViewItem root = new TreeViewItem();
      root.controlTypeFiltered = "Filtered- Updated- Tree";

      //
      guiFunctions.treeIteration(parentNode.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
      SaveButton.IsEnabled = true;
      tvOutput.Items.Add(root);
   }
  }*/
/*    private void LoadDevice_Click(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        // dlg.FileName = "filteredTree_"; // Default file name
        dlg.DefaultExt = ".xml"; // Default file extension
        dlg.Filter = "Text documents (.xml)|*.xml"; // Filter files by extension
        dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        // Show open file dialog box
        Nullable<bool> result = dlg.ShowDialog();

        // Process open file dialog box results
        if (result == true)
        {
            guiFunctions.loadFilteredTree(dlg.FileName);
        }

        ITreeStrategy<OSMElement.OSMElement> parentNode = grantTrees.getFilteredTree();

        tvOutput.Items.Clear();
        root.Items.Clear();

        //TreeViewItem root = new TreeViewItem();
        root.controlTypeFiltered = "Filtered- Updated- Tree";

        //
        guiFunctions.treeIteration(parentNode.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
        SaveButton.IsEnabled = true;
        tvOutput.Items.Add(root);


    }*/

/*         dataGrid3.Columns.Add(new DataGridTextColumn());
         ((DataGridTextColumn)dataGrid3.Columns[a]).Binding = new Binding(".");
         DataGridTextColumn col1 = new DataGridTextColumn();
         dataGrid3.Columns.Add(col1);
         col1.Binding = new Binding(".");
      dataGrid3.ItemsSource = guiElementRep[i, a].ToString();

         if (guiElementRep[i, a].ToString() == "True")
         {
        System.Console.WriteLine("HIER!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!:" + i);

        //dataGrid3.CurrentCell = new DataGridCellInfo(dataGrid3.Items[i], dataGrid3.Columns[a]);


        DataGridRow firstRow = dataGrid3.ItemContainerGenerator.ContainerFromItem(dataGrid3.Items[1]) as DataGridRow;
        System.Console.WriteLine("first: " + firstRow.ToString());
        DataGridCell firstColumnInFirstRow = dataGrid3.Columns[1].GetCellContent(firstRow).Parent as DataGridCell;
        //set background
        firstColumnInFirstRow.Background = Brushes.Black;
    }


    //dataGrid3.ItemsSource = guiElementRep[i, a].ToString();

    //System.Windows.Media.Brush Red = null;
    //dataGrid3.Background = Red;


    // dataTable4.Rows.Add(dataRow);

    // string cellValue;
    // cellValue = dataGrid3.GetRowCellValue(2, "ID").ToString();

    //System.Console.WriteLine(i.ToString());
    // DataGridCell cell = new DataGridCell();
    //  string s = (dataGrid3.Items[i] as DataRowView).Row.ItemArray[a].ToString();
  //  dataGrid3.CurrentCell = new DataGridCellInfo(dataGrid3.Items[i], dataGrid3.Columns[a]);
    //System.Console.WriteLine("Current: " + dataGrid3.CurrentCell.ToString());
    //dataGrid3.Style.TargetType.DeclaringType.
    //dataGrid3.CurrentCell.Column.CellStyle =

}


     }
 // dv = dataTable4.DefaultView;


// System.Windows.Forms.DataGridTableStyle style = new System.Windows.Forms.DataGridTableStyle();
//   style.MappingName = "TblName";
//  dataGrid3.D.Add(style);
}
}

//andere verison zusätzliche spalten und zeilen

/*  private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
{
// CellContent cellContent = new CellContent();
if (((ListBox)sender).SelectedItem != null)
{
String element = (sender as ListBox).SelectedItem.ToString();
bool[,] guiElementRep = strategyMgr.getSpecifiedBrailleDisplay().getRendererExampleRepresentation(element);


DataTable dataTable4 = createBrailleDisplay3((guiElementRep.Length / guiElementRep.GetLength(0)), guiElementRep.GetLength(0), dataGrid3);


for (int i = 0; i < guiElementRep.GetLength(0); i++) //höhe
{
  //System.Console.WriteLine(a.ToString());
  System.Console.WriteLine("i:" + i.ToString());
  // DataGridRow row = (DataGridRow)dataGrid3.ItemContainerGenerator
  //                         .ContainerFromIndex(i);
  //  DataGridRow row = (DataGridRow)dataGrid3.ItemContainerGenerator
  //  .ContainerFromIndex(i);
  // row.Background = Brushes.Red;
  //DataRow dr = (DataRow)row.DataContext;

  DataRow dataRow = dataTable4.NewRow();

  for (int a = 0; a < (guiElementRep.Length / guiElementRep.GetLength(0)); a++)//breite / Column
  {

      DataColumn dc = new DataColumn();
      //  dataTable4.Columns.Add(new DataColumn(guiElementRep[i, a].ToString()));
      System.Console.WriteLine("a: " + a.ToString());

      // cellContent.cellinput = guiElementRep[i, a].ToString();
      System.Console.WriteLine(guiElementRep[i, a].ToString());
      // DataRow row1 = dataTable4.NewRow();

      //dataTable4.Rows.Add(row1);

      //leer
      dataTable4.Rows[i][a] = guiElementRep[i, a].ToString();
      // dataTable4.Rows[i][a] = guiElementRep[i, a].ToString();


      //System.Windows.Media.Brush Red = null;
      //dataGrid3.Background = Red;








      //System.Console.WriteLine(i.ToString());
      //DataGridCell cell = new DataGridCell();
      // string s = (dataGrid3.Items[i] as DataRowView).Row.ItemArray[a].ToString();
      // dataGrid3.CurrentCell = new DataGridCellInfo(dataGrid3.Items[i], dataGrid3.Columns[a]);
      //dataGrid3.Style.TargetType.DeclaringType.
      //dataGrid3.CurrentCell.Column.CellStyle =

  }

  dataTable4.Rows.Add(dataRow);
}

// dv = dataTable4.DefaultView;

dataGrid3.ItemsSource = dataTable4.AsDataView();
// System.Windows.Forms.DataGridTableStyle style = new System.Windows.Forms.DataGridTableStyle();
//   style.MappingName = "TblName";
//  dataGrid3.D.Add(style);
}
}*/
