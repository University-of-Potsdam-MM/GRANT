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
using System.Windows.Shapes;

namespace GRANTApplication
{
    /// <summary>
    /// Interaktionslogik für OutputDesigner.xaml
    /// </summary>
    public partial class OutputDesigner : Window
    {
        public OutputDesigner()
        {
            InitializeComponent();

            //brailleDisplayStrategy = strategyMgr.getSpecifiedBrailleDisplay();
            //brailleDisplayStrategy.setStrategyMgr(strategyMgr);
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
