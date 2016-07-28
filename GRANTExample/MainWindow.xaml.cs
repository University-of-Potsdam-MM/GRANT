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


using System.Drawing;
using GRANTApplication;
using OSMElement;
using GRANTManager;
using GRANTManager.Interfaces;

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
            strategyMgr.setSpecifiedDisplayStrategy(settings.getPosibleDisplayStrategies()[2].className);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);

            exampleTree = new ExampleTree(strategyMgr, grantTree);
            exampleInspectGui = new InspectGui(strategyMgr);
            exampleBrailleDis = new ExampleBrailleDis(strategyMgr, grantTree);
            exampleDisplay = new ExampleDisplayStrategy(strategyMgr);
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
            if (e.Key == Key.A)
            {
                NodeBox.Text = exampleDisplay.deviceInfo();
            }
            if (e.Key == Key.B)
            {
                NodeBox.Text = exampleDisplay.allDevices();
            }
            if (e.Key == Key.C)
            {
                exampleDisplay.setMVBDDevice();
            }
            if (e.Key == Key.D)
            {
                exampleDisplay.setBrailleIoSimulatorDevice();
            }



        }
    }
}
