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
using TemplatesUi;
using GRANTManager.TreeOperations;


using Prism.Events;
using System.Windows.Interop;

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

            eventTest();
            posibleScreenreaders();
            //InitializeStrategyWindows_Windows_EventsMonitor();
        }

        Settings settings;
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;
        TreeOperation treeOperation;
        SearchNodes searchNodes;
        ExampleTree exampleTree;
        InspectGui exampleInspectGui;
        ExampleBrailleDis exampleBrailleDis;
        ExampleDisplayStrategy exampleDisplay;
        GuiFunctions guiFuctions;
        
        ///// <summary>
        ///// Initialisierung der Eventverfolgung des NuGet-Package MousekeyHook
        ///// diese muss in diesem Projekt und in StrategyWindows eingebunden sein
        ///// </summary>
        //private void InitializeStrategyWindows_Windows_EventsMonitor()
        //{
        //    //Benutzung der Methode InitializeWindows_EventsMonitor() aus StrategyWindows über Interface in Grantmanager/Interfaces/IOperationSystemStrategy
        //    strategyMgr.getSpecifiedOperationSystem().InitializeWindows_EventsMonitor();

        //    //oder, da methode und die klasse public ist objekt erstellen und aufrufen
        //    //StrategyWindows.Windows_EventsMonitor me = new StrategyWindows.Windows_EventsMonitor();
        //    //me.Subscribe();
        //}


        public IEventAggregator prismEventAggregatorClass = new EventAggregator();

        public void eventTest()
        {
            prismEventAggregatorClass = strategyMgr.getSpecifiedEventManager().getSpecifiedEventManagerClass();

            prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin
        }

        public void generateOSMmwxaml(string osm)
        {
            Console.WriteLine("winevent verarbeitet in mainwindowxaml_" + osm);
            //osm = "werhers";
        }


        private void InitializeFilterComponent()
        {
            settings = new Settings();
            strategyMgr = new StrategyManager();
            grantTree = new GeneratedGrantTrees();
            searchNodes = new SearchNodes(strategyMgr, grantTree, treeOperation);
            treeOperation = new TreeOperation(strategyMgr, grantTree);

            // Setzen des Eventmanager
            List<Strategy> possibleEventManager = settings.getPossibleEventManager();
            strategyMgr.setSpecifiedEventManager(possibleEventManager[0].className);

            
            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));

            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);
            


             List<Strategy> possibleFilter = settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
          //  strategyMgr.getSpecifiedFilter().setStrategyMgr(strategyMgr);

            strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
            //if (strategyMgr.getSpecifiedBrailleDisplay() == null)
            {
                Settings settings = new Settings();
                strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

                strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
                strategyMgr.getSpecifiedBrailleDisplay().setGeneratedGrantTrees(grantTree);
                strategyMgr.getSpecifiedBrailleDisplay().setTreeOperation(treeOperation);
                // strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
                //strategyMgr.getSpecifiedBrailleDisplay().setActiveAdapter();
                //strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
            }
            // brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
            //  brailleDisplayStrategy.setStrategyMgr(strategyMgr);

            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[0].className);

            strategyMgr.setSpecifiedGeneralTemplateUi(settings.getPossibleUiTemplateStrategies()[0].className);
            strategyMgr.getSpecifiedGeneralTemplateUi().setGeneratedGrantTrees(grantTree);
            strategyMgr.getSpecifiedGeneralTemplateUi().setTreeOperation(treeOperation);

            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            exampleTree = new ExampleTree(strategyMgr, grantTree, treeOperation);
            exampleInspectGui = new InspectGui(strategyMgr);
            exampleBrailleDis = new ExampleBrailleDis(strategyMgr, grantTree, treeOperation);
            exampleDisplay = new ExampleDisplayStrategy(strategyMgr);

            guiFuctions = new GuiFunctions(strategyMgr, grantTree, treeOperation);
            
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
                  tree2.XmlSerialize(fs);
                  strategyMgr.getSpecifiedTree().XmlSerialize(fs);
                  grantTree.getFilteredTree().XmlSerialize(fs);
                  fs.Close();*/
                Debug.WriteLine("F5");
                //Debug.WriteLine("count =" + strategyMgr.getSpecifiedTree().Count(grantTree.getFilteredTree()));
              //  Debug.WriteLine("gefilterter Baum:\n{0}", strategyMgr.getSpecifiedTree().ToStringRecursive(grantTree.getFilteredTree()));
                //Debug.WriteLine(strategyMgr.getSpecifiedEventManager().deliverString().ToString());
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
                NodeBox.Text = result != null ? NodeBox.Text + String.Join(", ", result.Select(p => p.ToString()).ToArray()) : "";
            }
            if (e.Key == Key.Left || e.Key == Key.NumPad4)
            {
              //  List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("A04CA705E7BA6B44BD902C9F997A4327", grantTree.getBrailleTree()); // => Tabs in Notepad
               // List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("976DB97BBB2C77A1D9D0347AD3D07CFC", grantTree.getBrailleTree()); // = TextBox "976DB97BBB2C77A1D9D0347AD3D07CFC"
                List<Object> nodeList = searchNodes.getAssociatedNodeList("85BA1DC86AD196E006BB0978C40BD171", grantTree.getBrailleTree()); // => Liste in eigener Beispielanwendung
               
                if (nodeList != null && nodeList.Count > 0) 
                {
                    strategyMgr.getSpecifiedBrailleDisplay().moveViewRangHoricontal(nodeList[0], 15);
                    Debug.WriteLine("View (" + strategyMgr.getSpecifiedTree().GetData(nodeList[0]).brailleRepresentation.viewName + ") verschoben!");
                }
            }
            if (e.Key == Key.Right || e.Key == Key.NumPad6)
            {
              //  List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("A04CA705E7BA6B44BD902C9F997A4327", grantTree.getBrailleTree()); // => Tabs in Notepad
                //List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("976DB97BBB2C77A1D9D0347AD3D07CFC", grantTree.getBrailleTree()); // = TextBox "976DB97BBB2C77A1D9D0347AD3D07CFC"
                List<Object> nodeList = searchNodes.getAssociatedNodeList("85BA1DC86AD196E006BB0978C40BD171", grantTree.getBrailleTree()); // => Liste in eigener Beispielanwendung
                if (nodeList != null && nodeList.Count > 0)
                {
                    strategyMgr.getSpecifiedBrailleDisplay().moveViewRangHoricontal(nodeList[0], -15);
                    Debug.WriteLine("View (" + strategyMgr.getSpecifiedTree().GetData(nodeList[0]).brailleRepresentation.viewName + ") verschoben!");
                }
            }
            if (e.Key == Key.Up || e.Key == Key.NumPad8)
            {
              //  List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("A04CA705E7BA6B44BD902C9F997A4327", grantTree.getBrailleTree()); // => Tabs in Notepad
               // List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("976DB97BBB2C77A1D9D0347AD3D07CFC", grantTree.getBrailleTree()); // = TextBox "976DB97BBB2C77A1D9D0347AD3D07CFC"
                List<Object> nodeList = searchNodes.getAssociatedNodeList("85BA1DC86AD196E006BB0978C40BD171", grantTree.getBrailleTree()); // => Liste in eigener Beispielanwendung
                if (nodeList != null && nodeList.Count > 0)
                {
                    strategyMgr.getSpecifiedBrailleDisplay().moveViewRangVertical(nodeList[0], 5);
                    Debug.WriteLine("View (" + strategyMgr.getSpecifiedTree().GetData(nodeList[0]).brailleRepresentation.viewName + ") verschoben!");
                }
            }
            if (e.Key == Key.Down || e.Key == Key.NumPad2)
            {
               // List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("A04CA705E7BA6B44BD902C9F997A4327", grantTree.getBrailleTree()); // => Tabs in Notepad
               // List<ITreeStrategy<OSMElement.OSMElement>> nodeList = strategyMgr.getSpecifiedTreeOperations().getAssociatedNodeList("976DB97BBB2C77A1D9D0347AD3D07CFC", grantTree.getBrailleTree()); // = TextBox "976DB97BBB2C77A1D9D0347AD3D07CFC"
                List<Object> nodeList = searchNodes.getAssociatedNodeList("85BA1DC86AD196E006BB0978C40BD171", grantTree.getBrailleTree()); // => Liste in eigener Beispielanwendung
                if (nodeList != null && nodeList.Count > 0)
                {
                    strategyMgr.getSpecifiedBrailleDisplay().moveViewRangVertical(nodeList[0], -5);
                    Debug.WriteLine("View (" + strategyMgr.getSpecifiedTree().GetData(nodeList[0]).brailleRepresentation.viewName + ") verschoben!");
                }
            }
            if (e.Key == Key.NumPad5)
            {
                object node = guiFuctions.getBrailleNodeAtPoint(19, 23);
                Debug.WriteLine("Braille-Node: " + (node == null ? "null" : strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered));
                NodeBox.Text = strategyMgr.getSpecifiedTree().ToString(node);
            }

            if (e.Key == Key.NumPad7)
            {

                List<Object> nodeList = searchNodes.getAssociatedNodeList("5524107E02B9643EDB67F59CB310C26C", grantTree.getFilteredTree()); //schließen-Button Calc
                if(nodeList == null)
                {
                    nodeList = searchNodes.getAssociatedNodeList("E317735D2EF01B0E4EF8C1AD47C8F9B8", grantTree.getFilteredTree()); //Notepad++
                }
                if (nodeList != null && nodeList.Count > 0)
                {
                    // guiFuctions.addFilteredNodeToBrailleTree("Button", nodeList[0], "neu", "tv2", "neu", new Rect(10, 10, 50, 9));
                 //   guiFuctions.addFilteredNodeToBrailleTree("Button", nodeList[0], null, null, null, new Rect(10, 10, 50, 9));
                    guiFuctions.addFilteredNodeToBrailleTree("TextBox");
                    guiFuctions.addFilteredNodeToBrailleTree("Text");
                    guiFuctions.addFilteredNodeToBrailleTree("Button");
                    guiFuctions.addFilteredNodeToBrailleTree("TabItem");
                }
                nodeList = searchNodes.getAssociatedNodeList("29E003E8F0AB31A3745C4B944C0254CF", grantTree.getFilteredTree()); //DropDownMenu - notepad++

                if (nodeList != null && nodeList.Count > 0)
                {
                    guiFuctions.addFilteredNodeToBrailleTree("DropDownMenuItem",nodeList[0]);
                }
                nodeList = searchNodes.getAssociatedNodeList("58AA232A14172FB8BCD78A576BE94D80", grantTree.getFilteredTree()); //Beispielanwendung - checkbox

                if (nodeList != null && nodeList.Count > 0)
                {
                    guiFuctions.addFilteredNodeToBrailleTree("ListItem", nodeList[0]);
                }
                
            }

       }


        private void Button_Click_Speichern(object sender, RoutedEventArgs e)
        {
            if (grantTree.getFilteredTree() == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredProject_" + strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( grantTree.getFilteredTree())).properties.nameFiltered; // Default file name
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


            TemplateTextview.Textview tempTextView = new TemplateTextview.Textview(strategyMgr, grantTree, treeOperation);
            tempTextView.createTextviewOfSubtree(grantTree.getFilteredTree());

            //  String path = @"Templates" + System.IO.Path.DirectorySeparatorChar + "TemplateUi.xml";
            String path = @"C:\Users\mkarlapp\Desktop\TemplateUi.xml";

            if (!GuiFunctions.isTemplateValid(path))
            {
                Debug.WriteLine("Template ist nicht valide!");
                return;
            }
            int minDeviceHeight;
            int minDeviceWidth;
            if (!guiFuctions.isTemplateUsableForDevice(path, out minDeviceHeight, out minDeviceWidth))
            {
                Debug.WriteLine("Das Template ist für eine größere Stifftplatte (min Height = " + minDeviceHeight + " Width = "+minDeviceWidth+") vorgesehen.");
                return;
            }
            strategyMgr.getSpecifiedGeneralTemplateUi().generatedUiFromTemplate(path);

            if (grantTree.getBrailleTree() != null)
            {
                Debug.WriteLineIf(grantTree.getBrailleTree() != null, "Baum-Elemente Anzahl: " + strategyMgr.getSpecifiedTree().Count( grantTree.getBrailleTree()));
               // Console.WriteLine("Baum:\n" + strategyMgr.getSpecifiedTree().ToStringRecursive(grantTree.getBrailleTree()));
            }
        }

        #region Notification
        private void AddScreenReaderCommand(object sender, RoutedEventArgs e)
        { //TODO
        }

        private void posibleScreenreaders()
        {
            SelectScreenReader.Items.Clear();
            ScreenReaderFunctions srf = new ScreenReaderFunctions(strategyMgr);
            foreach (KeyValuePair<string, string> entry in srf.screenreaders)
            {
                MenuItem m = new MenuItem();
                m.Header = entry.Key;
                m.IsCheckable = true;
                SelectScreenReader.Items.Add(m);
            }
            SelectScreenReader.UpdateLayout();
        }

        private void ScreenReaderCommand(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("");
            if (e.Source != null && e.Source.GetType().Equals(typeof(MenuItem)))
            {
                if (((MenuItem)e.Source).Header != null && !((MenuItem)e.Source).Header.Equals("Select screen reader"))
                {
                    MenuItem i = ((MenuItem)e.Source);
                    ((MenuItem)e.Source).IsEnabled = false;
                    strategyMgr.getSpecifiedBrailleDisplay().removeActiveAdapter();
                    ScreenReaderFunctions srf = new ScreenReaderFunctions(strategyMgr);
                    srf.uncheckedMenuItem(i);
                    guiFuctions.loadGrantProject(srf.screenreaders[((MenuItem)e.Source).Header as String]);
                    strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                    
                }
            }
        }

        private void HideWindowCommand(object sender, RoutedEventArgs e)
        {
            var wih = new WindowInteropHelper(Window.GetWindow(this)); // see http://stackoverflow.com/questions/10675305/how-to-get-the-hwnd-of-window-instance
            IntPtr hWnd = wih.Handle;
            if (((MenuItem)sender).IsChecked)
            {
                strategyMgr.getSpecifiedOperationSystem().hideWindow(hWnd);
            }
            else
            {
                strategyMgr.getSpecifiedOperationSystem().showWindow(hWnd);
            }
        }
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        #endregion
    }
}
