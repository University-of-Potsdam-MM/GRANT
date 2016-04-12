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
            elementP.acceleratorKeyFiltered = element.Current.AcceleratorKey;
            elementP.accessKeyFiltered = element.Current.AccessKey;
            elementP.autoamtionIdFiltered = element.Current.AutomationId;
            elementP.boundingRectangleFiltered = element.Current.BoundingRectangle;
            elementP.classNameFiltered = element.Current.ClassName;
            elementP.controlTypeFiltered = element.Current.ControlType.LocalizedControlType;
            elementP.frameWorkIdFiltered = element.Current.FrameworkId;
            elementP.hasKeyboardFocusFiltered = element.Current.HasKeyboardFocus;
            elementP.helpTextFiltered = element.Current.HelpText;
            elementP.hWndFiltered = element.Current.NativeWindowHandle;
            elementP.isContentElementFiltered = element.Current.IsContentElement;
            elementP.isControlElementFiltered = element.Current.IsControlElement;
            elementP.isEnabledFiltered = element.Current.IsEnabled;
            elementP.isKeyboardFocusableFiltered = element.Current.IsKeyboardFocusable;
            elementP.isOffscreenFiltered = element.Current.IsOffscreen;
            elementP.isPasswordFiltered = element.Current.IsPassword;
            elementP.isRequiredForFormFiltered = element.Current.IsRequiredForForm;
            elementP.itemStatusFiltered = element.Current.ItemStatus;
            elementP.itemTypeFiltered = element.Current.ItemType;
            
            elementP.localizedControlTypeFiltered = element.Current.LocalizedControlType;
            elementP.nameFiltered = element.Current.Name;
            elementP.processIdFiltered = element.Current.ProcessId;
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

        /// <summary>
        /// Ermittelt das zugehörige AutomationElement eines Knotens aus dem gespiegelten Baum
        /// </summary>
        /// <param name="node">gibt den Knoten an, von dem das zugehörige AutomationElement ermittelt werden soll</param>
        /// <param name="rootElement"></param> --- hier erst ermitteln
        /// <returns>das zugehörige AutomationElement des Knotens</returns>
        private AutomationElement getAutomationElementFromMirroredTree(INode<GeneralProperties> node, AutomationElement rootElement)
        {
            Condition condition = setPropertiesCondition(node.Data);
            AutomationElementCollection foundedAutomationElements = rootElement.FindAll(TreeScope.Descendants, condition);
            if (foundedAutomationElements.Count == 0)
            {
                Console.WriteLine("Kein passendes AutomationElement gefunden!");
                return null;
            }
            if (foundedAutomationElements.Count == 1)
            {
                return foundedAutomationElements[0];
            }
            //TODO: prüfen, welches das richtige Element ist
            return foundedAutomationElements[0];
        }

        /// <summary>
        /// Erstellt einen Baum mit allen Vorfahren eines Elementes
        /// </summary>
        /// <param name="node">gibt den Knoten des gespiegelten Baumes an, von dem die Vorfahren gesucht werden sollen</param>
        /// <param name="hwnd">gibt den Handle des AutomationElementes an um anhand dessen das Root-AutomationElement zu bestimmen</param>
        /// <returns>ein <code>ITree</code>-Objekt mit den Vorfahren des Knotens (inkl. des Knotens selbst)</returns>
        public ITree<GeneralProperties> getParentsOfElement(INode<GeneralProperties> node, IntPtr hwnd)
        {
            BasicWindowsOperations basicWindows = new BasicWindowsOperations();
            AutomationElement rootElement = deliverAutomationElementFromHWND(basicWindows.getProcessHwndFromHwnd(deliverElementID(hwnd))); // Ist das hier noch notwendig, oder bekommen wir an der Stelle eigentlich schon den richtigen Handle?
            AutomationElement element = getAutomationElementFromMirroredTree(node, rootElement);
            if (element == null)
            {
                return null;
            }
            ITree<GeneralProperties> tree = NodeTree<GeneralProperties>.NewTree();
            tree.AddChild(setProperties(element));
            findParentOfElement(element, rootElement, tree);
            return tree;
        }

        /// <summary>
        /// Findet rekusiv alle direkten Vorfahren eines AutomationElements
        /// </summary>
        /// <param name="element">gibt das AutomationElement an von dem die Vorfahren gesucht werden sollen</param>
        /// <param name="rootElement">gibt das rootElemet der Suche an</param>
        /// <param name="tree">gibt den aktuel erstellten Baum mit den Vorfahren des Elements an</param>
        private void findParentOfElement(AutomationElement element, AutomationElement rootElement,ITree<GeneralProperties> tree)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent = walker.GetParent(element);
            addParentOfNode(elementParent, tree);
            if (!rootElement.Equals(elementParent))
            {
                findParentOfElement(elementParent, rootElement, tree);
            }
        }
        
        /// <summary>
        /// Fügt ein neues Elternelement einen Baum hinzu.
        /// </summary>
        /// <param name="parentElement">gibt das neue Elternelement an</param>
        /// <param name="tree">gibt den "alten" Baum an</param>
        private void addParentOfNode(AutomationElement parentElement, ITree<GeneralProperties> tree)
        {
            ITree<GeneralProperties> tree2 = NodeTree<GeneralProperties>.NewTree();
            INode<GeneralProperties> node = tree2.AddChild(setProperties(parentElement));
            node.AddChild(tree);
            tree.Clear();
            tree.AddChild(tree2);
        }

        /// <summary>
        /// Erstellt die Condition zum suchen der AutomationElemente anhand der <code>GeneralProperties</code> eines Baum-Knoten
        /// </summary>
        /// <param name="properties">gibt die <code>GeneralProperties</code> eines Knoten an</param>
        /// <returns>Eine Condition</returns>
        private Condition setPropertiesCondition(GeneralProperties properties)
        {
            //TODO: Achtung einige Eigenschaften vonrscheinlich GeneralProperties sollten wahrscheinlich nicht genutzt werden
            Condition resultCondition;
            #region von allen auslesbar
            resultCondition = new PropertyCondition(AutomationElement.NameProperty, properties.nameFiltered);
            // ...
            #endregion
            if (properties.controlTypeFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, properties.controlTypeFiltered), resultCondition);
            }
            //.. 
            return resultCondition;
        }
    }

}
