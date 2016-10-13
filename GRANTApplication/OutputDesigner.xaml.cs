﻿using System;
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
        TreeOperation treeOperation;
        GuiFunctions.MenuItem root;
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
            treeOperation = new TreeOperation(strategyMgr, grantTrees);
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
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
            strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);

            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);
            //tvMain.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tvMain_SelectedItemChanged);
            tvOutput.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tvOutput_SelectedItemChanged);
           // listBox1.SelectedItem += new RoutedPropertyChangedEventHandler<object>(listBox1_SelectionChanged);

            guiFunctions = new GuiFunctions(strategyMgr, grantTrees, treeOperation);

            root = new GuiFunctions.MenuItem();

            //NodeButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
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
                malemal(dataGrid3, guiElementRep);
            }
        }

        public void malemal(DataGrid datagrid, bool[,] guiElementRep)
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
                root.Items.Clear();

                //TreeViewItem root = new TreeViewItem();

                root.controlTypeFiltered = "Filtered- Updated- Tree";

                //
                guiFunctions.treeIteration(strategyMgr.getSpecifiedTree().Copy(tree), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
                
                SaveButton.IsEnabled = true;
                tvOutput.Items.Add(root);



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
                Object tree = grantTrees.getBrailleTree();

                brailleOutput.Items.Clear();
                root.Items.Clear();

               // TreeViewItem root = new TreeViewItem();

               root.controlTypeFiltered = "Braille-Tree";

                //
              //  guiFunctions.createTreeForOutput(strategyMgr.getSpecifiedTree().Copy(tree), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
                guiFunctions.createTreeForOutput(tree, ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
               
                SaveButton.IsEnabled = true;
                brailleOutput.Items.Add(root);
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


        void tvOutput_SelectedItemChanged(object sender,
      RoutedPropertyChangedEventArgs<object> e)
        {

            var tree = sender as TreeView;
            if (tree.SelectedItem is GuiFunctions.MenuItem)
            {
                // ... Handle a TreeViewItem.
                GuiFunctions.MenuItem item = tree.SelectedItem as GuiFunctions.MenuItem;
                //this.Title = "Selected header: " + item.IdGenerated.ToString();
                if (item.IdGenerated != null)
                {
                    OSMElement.OSMElement osmElement = treeOperation.searchNodes.getFilteredTreeOsmElementById(item.IdGenerated);
                    System.Drawing.Rectangle rect = strategyMgr.getSpecifiedOperationSystem().getRect(osmElement);
                    if (osmElement.properties.isOffscreenFiltered == false) { 
                    strategyMgr.getSpecifiedOperationSystem().paintRect(rect);
                    }
                    //System.Drawing.Color MessageColor = System.Drawing.Color.Red;
                    //System.Windows.Media.Brush Red = null;
                    //listBox1.Foreground = Red;
                    //listBox1.M


                    int var1 = listBox1.Items.IndexOf(item.controlTypeFiltered);
                    if (var1 < 0) { var1 = listBox1.Items.IndexOf("Text"); }
                    listBox1.SelectedIndex = var1;



                    System.Console.WriteLine(" INDEX: " + var1);

                    //listBox1.Foreground = System.Windows.Media.Brushes.DarkRed;
                    // int index = listBox1.(item.controlTypeFiltered, -1);
                    //listBox1.SelectedIndex(item.controlTypeFiltered);
                    //ItemColor = c; 
                    //listGuiElements();


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
