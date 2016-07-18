﻿using System;
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
using StrategyManager;
using StrategyManager.Interfaces;

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
        StrategyMgr strategyMgr;
        IBrailleDisplayStrategy brailleDisplayStrategy;

        ExampleTree exampleTree;
        InspectGui exampleInspectGui;
        ExampleBrailleDis exampleBrailleDis;

        private void InitializeFilterComponent()
        {
            settings = new Settings();
            strategyMgr = new StrategyMgr();
            List<Strategy> possibleOperationSystems = settings.getPossibleOperationSystems();
            String cUserOperationSystemName = possibleOperationSystems[0].userName; // muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedOperationSystem(settings.strategyUserNameToClassName(cUserOperationSystemName));
            strategyMgr.getSpecifiedOperationSystem();

            List<Strategy> possibleTrees = settings.getPossibleTrees();
            strategyMgr.setSpecifiedTree(possibleTrees[0].className);


             List<Strategy> possibleFilter = settings.getPossibleFilters();
            String cUserFilterName = possibleFilter[0].userName; // der Filter muss dynamisch ermittelt werden
            strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
            strategyMgr.getSpecifiedFilter().setStrategyMgr(strategyMgr);

         //   strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden
           // brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
          //  brailleDisplayStrategy.setStrategyMgr(strategyMgr);

            strategyMgr.setSpecifiedTreeOperations(settings.getPossibleTreeOperations()[0].className);
            strategyMgr.getSpecifiedTreeOperations().setStrategyMgr(strategyMgr);

            exampleTree = new ExampleTree(strategyMgr);
            exampleInspectGui = new InspectGui(strategyMgr);
            exampleBrailleDis = new ExampleBrailleDis(strategyMgr);
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
                exampleBrailleDis.getRendererExample();
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




        }
    }
}
