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

        //todo inti der prismeventaggreagtorclass in grantapplication einbauen
        public IEventAggregator prismEventAggregatorClass;// = new EventAggregator();

        public void eventTest()
        {
            //erhalt des prismaggregator über interface
            prismEventAggregatorClass = strategyMgr.getSpecifiedEventManager().getSpecifiedEventManagerClass();

            //prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent_PRISMHandler_GrantManager>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin
            prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin

            //Console.WriteLine("test winevent verarbeitet in mainwindowxaml_");
            

        }

        public void generateOSMmwxaml(string osm)
        {
            Debug.WriteLine("winevent verarbeitet in mainwindowxaml_" + osm);
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


            //IEvent_PRISMStrategy test = new StrategyEvent_PRISM.Event_PRISM();
            //Type t = test.GetType();


            strategyMgr.setSpecifiedEventManager(possibleEventManager[0].className);

            #region setzen der neuen (Juni 2017) Event Interfaces
            strategyMgr.setSpecifiedEventAction(settings.getPossibleEventAction()[0].className);
            strategyMgr.getSpecifiedEventAction().setGrantTrees(grantTree);
            strategyMgr.setSpecifiedEventManager2(settings.getPossibleEventManager2()[0].className);
            strategyMgr.setSpecifiedEventProcessor(settings.getPossibleEventProcessor()[0].className);
            #endregion


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
                String element = exampleTree.filterNodeOfApplicatione();
                NodeBox.Text = element;
            }
            if (e.Key == Key.F2)
            {
                List<String> result = exampleBrailleDis.getPosibleScreens();
                NodeBox.Text = "Mögliche Screens: \n";
                NodeBox.Text = result != null ? NodeBox.Text + String.Join(", ", result.Select(p => p.ToString()).ToArray()) : "";
            }
            if (e.Key == Key.F3)
            {
                exampleBrailleDis.changeScreen(Screen.Text);
            }
            if(e.Key == Key.F4)
            {
                exampleBrailleDis.update();
            }
            if (e.Key == Key.F5)
            {
                NodeBox.Text = exampleDisplay.allDevices();
            }
            if (e.Key == Key.F6)
            {
                NodeBox.Text = exampleDisplay.deviceInfo();
            }
       }


        private void Button_Click_Speichern(object sender, RoutedEventArgs e)
        {
            if (grantTree.filteredTree == null) { Console.WriteLine("Der Baum muss vor dem Speichern gefiltert werden."); return; }

            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "filteredProject_" + strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( grantTree.filteredTree)).properties.nameFiltered; // Default file name
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
           // dlg.FileName = "filteredProject_" + grantTree.filteredTree.Child.Data.properties.nameFiltered; // Default file name
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

        private void Button_Click_Screen(object sender, RoutedEventArgs e)
        {
            exampleBrailleDis.changeScreen(Screen.Text);
        }

        #region Notification
        private void AddScreenReaderCommand(object sender, RoutedEventArgs e)
        { 
            String fileExtention = ".grant";
            String fileNamePath = guiFuctions.openFileDialog(fileExtention, "GRANT documents (.grant)|*.grant", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            if(fileNamePath == null) { System.Windows.Forms.MessageBox.Show("The chosen screen reader doesn't exist!", "GRANT exception"); return; }
            ScreenReaderFunctions srf = new ScreenReaderFunctions(strategyMgr);
            srf.addScreenReader(@fileNamePath);           
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
            if (e.Source != null && e.Source.GetType().Equals(typeof(MenuItem)))
            {
                if (((MenuItem)e.Source).Header != null && !((MenuItem)e.Source).Header.Equals("Select screen reader"))
                {
                    MenuItem i = ((MenuItem)e.Source);
                    ((MenuItem)e.Source).IsEnabled = false;
                    ScreenReaderFunctions srf = new ScreenReaderFunctions(strategyMgr);
                    uncheckedMenuItem(i);
                    guiFuctions.loadGrantProject(srf.screenreaders[((MenuItem)e.Source).Header as String]);
                    strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                    
                }
            }
        }

        /// <summary>
        /// Unchecked and enabled all siblings
        /// </summary>
        /// <param name="menuItem"><code>System.Windows.Controls.MenuItem</code> which was checked</param>
        private void uncheckedMenuItem(System.Windows.Controls.MenuItem menuItem)
        {
            if (menuItem.Parent != null)
            {
                foreach (System.Windows.Controls.MenuItem siblingMenuitem in ((System.Windows.Controls.MenuItem)menuItem.Parent).Items)
                {
                    if (!siblingMenuitem.Equals(menuItem))
                    {
                        siblingMenuitem.IsChecked = false;
                        siblingMenuitem.IsEnabled = true;
                    }
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
                if (!((MenuItem)sender).Name.Equals("HideWindowNotification"))
                {
                    //HideWindowNotification.IsChecked = true;
                }else
                {
                    //HideWindowMenuItem.IsChecked = true;
                }
            }
            else
            {
                strategyMgr.getSpecifiedOperationSystem().showWindow(hWnd);
                if (!((MenuItem)sender).Name.Equals("HideWindowNotification"))
                {
                    //HideWindowNotification.IsChecked = false;
                }
                else
                {
                    //HideWindowMenuItem.IsChecked = false;
                }
            }
        }
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        #endregion
    }
}
