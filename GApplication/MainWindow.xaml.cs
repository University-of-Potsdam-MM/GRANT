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
using Basics;
using Basics.Interfaces;
using UIA;
using Tree;
using System.Windows.Automation;

namespace GApplication
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
            BasicWindowsOperations basicWindows = new BasicWindowsOperations();
            
            BasicFilterStrategy filter = new BasicFilterStrategy();
            
            
            // ... Test for F5 key.
            if (e.Key == Key.F5)
            {
                if (basicWindows.deliverCursorPosition())
                {
                    try
                    {
                        IntPtr points = basicWindows.getHWND();

                        // entscheiden, welche Bibliothek !!!! User gibt UIA an TODO
                        
                        UiaFilterStrategy f = new UiaFilterStrategy();
                        // dieser Parameter muss dynamisch ermittelt werden
                        filter.setSpecifiedFilter(f);
                        /*int  processIdentifier = UiaFilter.deliverAutomationElementID(points);
                        IntPtr mainPointer = basicWindows.getProcessHwndFromHwnd(processIdentifier);
                        ITree<GeneralProperties> tree = filter.filtering(mainPointer);
                        Basics.BasicFilter.printTreeElements(tree, -1);*/
                        ITree<GeneralProperties> tree = filter.filtering(basicWindows.getProcessHwndFromHwnd(UiaFilterStrategy.deliverAutomationElementID(points)));
                        Basics.BasicTreeOperations.printTreeElements(tree, -1);
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
