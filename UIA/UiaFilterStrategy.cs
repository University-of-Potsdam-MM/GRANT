using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tree;
using System.Windows.Automation;
using System.Diagnostics;
using Basics;
using Basics.Interfaces;

namespace UIA
{
    public class UiaFilterStrategy : IFilterStrategy
    {
        /// <summary>
        /// Erstellt anhand eines AutomationElements den zugehörigen Baum
        /// </summary>
        /// <param name="mainWindowElement">gibt das AutomationElement an</param>
        /// <returns>ein <code>ITree<GeneralProperties></code>-Baum</returns>
        public ITree<GeneralProperties> filtering(IntPtr hwnd)
        {
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            AutomationElement mainWindowElement = deliverAutomationElementFromHWND(hwnd);
            INode<GeneralProperties> top = tree.AddChild(setProperties(mainWindowElement));
            AutomationElementCollection collection = mainWindowElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            findChildrenOfNode(top, collection, -1);
            return tree;
        }


        /// <summary>
        /// Ordnet die Eigenschaften eines AutomationElements dem <typeparamref name="GeneralProperties"/>-Objekt zu
        /// </summary>
        /// <param name="element">gibt das AutomationElement an</param>
        /// <returns>Ein <typeparamref name="GeneralProperties"/>-Objekt mit den Eigenschaften des AutomationElements.</returns>
        private GeneralProperties setProperties(AutomationElement element)
        {
            GeneralProperties elementP = new GeneralProperties();
            elementP.nameFiltered = element.Current.Name;
            elementP.controlTypeFiltered = element.Current.LocalizedControlType;
            elementP.boundingRectangleFiltered = element.Current.BoundingRectangle;
            elementP.disabedFiltered = element.Current.IsEnabled;
            return elementP;
        }


        /// <summary>
        /// Sucht rekusiv alle Kindelemente eines Knotens und ermittelt dessen Eingenschaften
        /// </summary>
        /// <param name="top">gibt den Namen des Kindelementes an</param>
        /// <param name="collection">gibt die AutomationElement-Collection an</param>
        /// <param name="depth">gibt an wie tief der Suche ausgehend vom Root-Element an.</param>
        private void findChildrenOfNode(INode<GeneralProperties> top, AutomationElementCollection collection, int depth)
        {
            foreach (AutomationElement element in collection)
            {
                if (top.Depth < depth || depth == -1)
                {
                    INode<GeneralProperties> node = top.AddChild(setProperties(element));

                    AutomationElementCollection c = element.FindAll(TreeScope.Children, Condition.TrueCondition);
                    findChildrenOfNode(node, c, depth);
                }
            }
        }

        //  AutomationElement vom HWND
        public static AutomationElement deliverAutomationElementFromHWND(IntPtr hwnd)
        {
            AutomationElement element = AutomationElement.FromHandle(hwnd);

            //element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return element;
        }


        // ProzessID vom AutomationElement
        public int deliverElementID(IntPtr hwnd)
        {
            //window = WindowFromPoint(cp);
            AutomationElement element = AutomationElement.FromHandle(hwnd);

            int processIdentifier = (int)element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return processIdentifier;
        }

    }

}
