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
using FilterBase;
using System.Windows.Automation;

namespace FilterApplication
{
    
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            UIA_Filter uia = new UIA_Filter();
            TreeGenUia treeUia = new TreeGenUia();
            // ... Test for F5 key.
            if (e.Key == Key.F5)
            {
                if (uia.deliverCursorPosition())
                {
                    try
                    {
                        itemNameTextBox.Text = " \nMethode mit GetPhysical:  " + uia.cp.X.ToString() + " " + uia.cp.Y.ToString() + " "
                        //+ " \nFROM-POINT: AutomationElement " + uia.deliverAutomationElementFromPoint()
                        + " \nFROM-POINT:  HWND:  " + uia.getHWND()
                        + " \nFROM-HWND: AutomationElement " + UIA_Filter.deliverAutomationElementFromHWND(uia.getHWND())
                        //+ "\n Parent:" + uia.RootElementofApplication(uia.deliverAutomationElementFromPoint())
                        //+ "\n ARENT:" + uia.GetTopLevelWindow(uia.deliverAutomationElementFromPoint())
                        + " \n NAME:" + uia.deliverAutomationElementFromPoint().Current.Name.ToString()
                        + "\n Process-ID: " + uia.getProcessIdFromHwnd(uia.getHWND())
                        + "\n AE-ID: " + UIA_Filter.deliverAutomationElementID(uia.getHWND())
                        + "\n MAIN-HWND: " + uia.getProcessHwndFromHwnd(UIA_Filter.deliverAutomationElementID(uia.getHWND()));


                        treeUia.generateTree(UIA_Filter.deliverAutomationElementFromHWND(uia.getProcessHwndFromHwnd(UIA_Filter.deliverAutomationElementID(uia.getHWND()))));

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ex);
                    }
                }
            }
        }

    }
}
