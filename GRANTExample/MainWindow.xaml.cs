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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

using System.Drawing;
using GRANTApplication;
using OSMElement;
using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.Templates;

namespace GRANTExample
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeFilterComponent();
        }

        Settings settings;
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;

        ExampleTree exampleTree;
        InspectGui exampleInspectGui;
        ExampleBrailleDis exampleBrailleDis;
        ExampleDisplayStrategy exampleDisplay;
        GuiFunctions guiFuctions;
        GenaralUI ui;

        private void InitializeFilterComponent()
        {
            settings = new Settings();
            strategyMgr = new StrategyManager();
            grantTree = new GeneratedGrantTrees();
            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));

            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);


             List<Strategy> possibleFilter = settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
          //  strategyMgr.getSpecifiedFilter().setStrategyMgr(strategyMgr);

         //   strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
           // brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
          //  brailleDisplayStrategy.setStrategyMgr(strategyMgr);

            strategyMgr.setSpecifiedTreeOperations(settings.getPossibleTreeOperations()[0].className);
            strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);
            strategyMgr.getSpecifiedTreeOperations().setGeneratedGrantTrees(grantTree);
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);

            exampleTree = new ExampleTree(strategyMgr, grantTree);
            exampleInspectGui = new InspectGui(strategyMgr);
            exampleBrailleDis = new ExampleBrailleDis(strategyMgr, grantTree);
            exampleDisplay = new ExampleDisplayStrategy(strategyMgr);

            guiFuctions = new GuiFunctions(strategyMgr, grantTree);
            ui = new GenaralUI(strategyMgr, grantTree);
        }


        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {


            if (e.Key == Key.F1)
            {
                exampleInspectGui.inspect();
            }
            if (e.Key == Key.F2)
            {
                exampleBrailleDis.UiBrailleDis(AnzeigeEigenschaftBox.Text);
            }
            if (e.Key == Key.F3)
            {
                exampleTree.setOSMRelationshipImg();
            }
            if (e.Key == Key.F4)
            {
                exampleBrailleDis.updateImg();
            }
            if (e.Key == Key.F5)
            {
                exampleTree.filterTreeOfApplication();
              /*  System.IO.FileStream fs = System.IO.File.Create("c:\\Users\\mkarlapp\\Desktop\\test211.xml");
                StrategyGenericTree.ITree<OSMElement.OSMElement> tree2 = (StrategyGenericTree.ITree<OSMElement.OSMElement>)grantTree.getFilteredTree().Copy();
                StrategyGenericTree.NodeTree<OSMElement.OSMElement> tree3 = (StrategyGenericTree.NodeTree<OSMElement.OSMElement>)grantTree.getFilteredTree();
                tree2.XmlSerialize(fs);
                //tree2.XmlSerialize(fs);
                //strategyMgr.getSpecifiedTree().XmlSerialize(fs);
             //   grantTree.getFilteredTree().XmlSerialize(fs);
                fs.Close();*/
                Debug.WriteLine("F5");
            }
            if (e.Key == Key.F6)
            {
                exampleTree.changeFilter();
            }
            if (e.Key == Key.F7)
            {
                String element = exampleTree.filterNodeOfApplicatione();
                NodeBox.Text = element;
            }
            if (e.Key == Key.F8)
            {
                String localizedControlTypeFiltered = itemNameTextBox.Text;
                exampleTree.searchPropertie(localizedControlTypeFiltered);
            }
            if (e.Key == Key.F9)
            {
                exampleTree.setOSMRelationship();
            }
            if (e.Key == Key.D1)
            {
                NodeBox.Text = exampleDisplay.deviceInfo();
            }
            if (e.Key == Key.D2)
            {
                NodeBox.Text = exampleDisplay.allDevices();
            }
            if (e.Key == Key.D3)
            {
                exampleDisplay.setMVBDDevice();
            }
            if (e.Key == Key.D4)
            {
                exampleDisplay.setBrailleIoSimulatorDevice();
            }
            if (e.Key == Key.D5)
            {
                exampleBrailleDis.getRendererExample();
            }
            if (e.Key == Key.D6)
            {
                exampleBrailleDis.getRendererExample(itemNameTextBox.Text);
            }
          /*  if (e.Key == Key.G)
           {
                List<String> result = exampleBrailleDis.getRendererList();
                //String.Join(":", properties.runtimeIDFiltered.Select(p => p.ToString()).ToArray()
                NodeBox.Text = String.Join("; ", result.Select(p => p.ToString()).ToArray());
            }*/
            if (e.Key == Key.F11)
            {
                exampleTree.filterSubtreeOfApplication();
                Debug.WriteLine("F11");
            }
            if (e.Key == Key.D7)
            {
                exampleBrailleDis.changeScreen(Screen.Text);
                Debug.WriteLine("D7");
            }
            if (e.Key == Key.D8)
            {
                List<String> result = exampleBrailleDis.getPosibleScreens();
                NodeBox.Text = "MöglichenScreens: \n";
                NodeBox.Text = NodeBox.Text + String.Join(", " ,result.Select(p => p.ToString()).ToArray());
            }

       }


        private void Button_Click_Speichern(object sender, RoutedEventArgs e)
        {
            if (grantTree.getFilteredTree() == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredProject_" + grantTree.getFilteredTree().Child.Data.properties.nameFiltered; // Default file name
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
                guiFuctions.saveProject(dlg.FileName);
            }
        }

        private void Button_Click_Laden(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
           // dlg.FileName = "filteredProject_" + grantTree.getFilteredTree().Child.Data.properties.nameFiltered; // Default file name
            dlg.DefaultExt = ".grant"; // Default file extension
            dlg.Filter = "GRANT documents (.grant)|*.grant"; // Filter files by extension
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                guiFuctions.loadGrantProject(dlg.FileName);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (grantTree == null || grantTree.getFilteredTree() == null) { return; }
            ui.generatedUiFromTemplate();

            Debug.WriteLine("Baum-Elemente Anzahl: " + grantTree.getBrailleTree().Count); 
            //strategyMgr.getSpecifiedTreeOperations().printTreeElements(grantTree.getBrailleTree(), -1);
        }
    }
}
