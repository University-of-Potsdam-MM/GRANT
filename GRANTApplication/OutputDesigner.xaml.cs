using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GRANTManager;
using GRANTManager.Interfaces;

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
        GuiFunctions.MenuItem root;
        GuiFunctions guiFunctions;
      

        public OutputDesigner()
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

            //tvMain.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(tvMain_SelectedItemChanged);
           

            guiFunctions = new GuiFunctions(strategyMgr, grantTrees);

            root = new GuiFunctions.MenuItem();
            
            //NodeButton.IsEnabled = false;
            SaveButton.IsEnabled = false;

           

        }

   
        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (grantTrees.getFilteredTree() == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredProject_" + grantTrees.getFilteredTree().Child.Data.properties.nameFiltered; // Default file name
            dlg.DefaultExt = ".grant"; // Default file extension
            dlg.Filter = "Text documents (.grant)|*.grant"; // Filter files by extension
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

            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            tvMain.Items.Clear();
            root.Items.Clear();

            //TreeViewItem root = new TreeViewItem();

            root.controlTypeFiltered = "Filtered- Updated- Tree";

            //
            guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
            SaveButton.IsEnabled = true;
            tvMain.Items.Add(root);
        }
        private void LoadTree_Click(object sender, RoutedEventArgs e)
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

            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            tvMain.Items.Clear();
            root.Items.Clear();

            //TreeViewItem root = new TreeViewItem();
            root.controlTypeFiltered = "Filtered- Updated- Tree";

            //
            guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
            SaveButton.IsEnabled = true;
            tvMain.Items.Add(root);
        }

        private void LoadDevice_Click(object sender, RoutedEventArgs e)
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

            ITreeStrategy<OSMElement.OSMElement> tree = grantTrees.getFilteredTree();

            tvMain.Items.Clear();
            root.Items.Clear();

            //TreeViewItem root = new TreeViewItem();
            root.controlTypeFiltered = "Filtered- Updated- Tree";

            //
            guiFunctions.treeIteration(tree.Copy(), ref root); //Achtung wenn keine kopie erstellt wird wird der Baum im StrategyManager auch verändert (nur noch ein Knoten)
            SaveButton.IsEnabled = true;
            tvMain.Items.Add(root);
           
           
        }
        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            
            // ... A List.
            List<string> Combobox2items = new List<string>();
          
            // ... Get the ComboBox reference.
            var comboBox = sender as ComboBox;

            Combobox2items.Add("Item1");
            

            // ... Assign the ItemsSource to the List.
            //comboBox.ItemsSource = Combobox2items;

            // ... Make the first item selected.
    
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
             foreach (Device d in devices)
            {
                Combobox2items.Add(d.ToString());
             
            }
            comboBox.ItemsSource = Combobox2items;
            comboBox.SelectedIndex = 0;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var comboBox = sender as ComboBox;

            // ... Set SelectedItem as Window Title.
            string value = comboBox.SelectedItem as string;
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            this.Title = "Selected: " + value;
        }
    }

 
}
