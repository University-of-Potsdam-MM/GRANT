using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using System.Windows.Automation;
using System.Diagnostics;
using StrategyManager.Interfaces;
using OSMElement;
using System.Windows;

//using Microsoft.Practices.Prism;
//using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Events;
//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md

//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample


namespace StrategyUIA
{
    #region filterStrategyUIAClass
    public class FilterStrategyUIA : IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree;

        public void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem)
        {
             specifiedOperationSystem = operationSystem;
        }

        public IOperationSystemStrategy getSpecifiedOperationSystem()
        {
            return specifiedOperationSystem;
        }

        
        public void setSpecifiedTree(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            specifiedTree = tree;
        }

        public ITreeStrategy<OSMElement.OSMElement> getSpecifiedTree()
        {
            return specifiedTree;
        }


        /// <summary>
        /// Erstellt anhand des Handles einer Anwendung den zugehörigen Baum
        /// Variable mainWindowElement sollte umbenannt werden, da es nicht das mainwindow sein muss, es ist einfach nur ein automationelement, die übergabe des mainwindow erfolgt beim aufruf von filtering
        /// </summary>
        /// <param name="hwnd">den handle der Anwendung an</param>
        /// <returns>ein <code>ITree<GeneralProperties></code>-Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd)
        {

            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree.NewNodeTree();
            AutomationElement mainWindowElement = deliverAutomationElementFromHWND(hwnd);
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(mainWindowElement);
            ITreeStrategy<OSMElement.OSMElement> top = tree.AddChild(osmElement);
            AutomationElementCollection collection = mainWindowElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            findChildrenOfNode(top, collection, -1);

            UIAEventsMonitor uiaEvents = new UIAEventsMonitor();
            uiaEvents.eventsUIA(hwnd);

            return tree;
        }


        /// <summary>
        /// todo
        /// Ordnet die Eigenschaften eines AutomationElements dem <typeparamref name="GeneralProperties"/>-Objekt zu
        /// Hier sollte noch überall ein try/Catch um jede Abfrage der Properties herum, da einge properties von einigen anwendungen bei der abfrage fehler werfen!
        /// Desweiteren stellt sich die frage, ob cached abgefragt wird, oder current, wegen geschwindigekti der abfrage
        /// </summary>
        /// <param name="element">gibt das AutomationElement an</param>
        /// <returns>Ein <typeparamref name="GeneralProperties"/>-Objekt mit den Eigenschaften des AutomationElements.</returns>
        private GeneralProperties setProperties(AutomationElement element)
        {
            GeneralProperties elementP = new GeneralProperties();
            elementP.acceleratorKeyFiltered = element.Current.AcceleratorKey;
            elementP.accessKeyFiltered = element.Current.AccessKey;
            elementP.autoamtionIdFiltered = element.Current.AutomationId;
            if (!element.Current.BoundingRectangle.IsEmpty) //Anmerkung: Wenn BoundingRectangle == Empty, dann gibt es Probleme beim Einlesen einer erstellten XML (XmlDeserialize)
            {
                elementP.boundingRectangleFiltered = element.Current.BoundingRectangle;
            }            
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
        /// todo bzw. nachfrage: geht die methode findall nicht alleine alle elemente der collection durch? wird in der foreach-schleife dadurch nicht doppelt gearbeitet?
        /// Sucht rekusiv alle Kindelemente eines Knotens und ermittelt dessen Eingenschaften
        /// </summary>
        /// <param name="top">gibt den Namen des Kindelementes an</param>
        /// <param name="collection">gibt die AutomationElement-Collection an</param>
        /// <param name="depth">gibt an wie tief der Suche ausgehend vom Root-Element an.</param>
        private void findChildrenOfNode(ITreeStrategy<OSMElement.OSMElement> top, AutomationElementCollection collection, int depth)
        {
            foreach (AutomationElement element in collection)
            {
                if (top.Depth < depth || depth == -1)
                {
                    OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
                    osmElement.properties = setProperties(element);
                    ITreeStrategy<OSMElement.OSMElement> node = top.AddChild(osmElement);

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

        public void getMouseRect(IntPtr hwnd, out int x, out int y, out int width, out int height)
        {
            AutomationElement mouseElement = deliverAutomationElementFromHWND(hwnd);
            //Rect mouseRect = mouseElement.Current.BoundingRectangle;
            x = (int)mouseElement.Current.BoundingRectangle.TopRight.X;
            y = (int)mouseElement.Current.BoundingRectangle.TopRight.Y;
            height = (int)mouseElement.Current.BoundingRectangle.Width;
            width = (int)mouseElement.Current.BoundingRectangle.Height;
            Console.WriteLine("hier x: " + x);
            Console.WriteLine("hier y: " + y);
            Console.WriteLine("hier w: " + width);
            Console.WriteLine("hier h: " + height);
        }

        /// <summary>
        /// Ermittelt das zugehörige AutomationElement eines Knotens aus dem gespiegelten Baum
        /// </summary>
        /// <param name="node">gibt den Knoten an, von dem das zugehörige AutomationElement ermittelt werden soll</param>
        /// <param name="rootElement"></param> --- hier erst ermitteln
        /// <returns>das zugehörige AutomationElement des Knotens</returns>
        private AutomationElement getAutomationElementFromMirroredTree(ITreeStrategy<OSMElement.OSMElement> node, AutomationElement rootElement)
        {
            Condition condition = setPropertiesCondition(node.Data.properties);
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
        public ITreeStrategy<OSMElement.OSMElement> getParentsOfElement(ITreeStrategy<OSMElement.OSMElement> node, IntPtr hwnd)
        {
            AutomationElement rootElement = deliverAutomationElementFromHWND(specifiedOperationSystem.getProcessHwndFromHwnd(deliverElementID(hwnd)));             
            AutomationElement element = getAutomationElementFromMirroredTree(node, rootElement);
            if (element == null)
            {
                return null;
            }
            ITreeStrategy<OSMElement.OSMElement> tree = specifiedTree.NewNodeTree();
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties =  setProperties(element);
            tree.AddChild(osmElement);
            findParentOfElement(element, rootElement, tree);
            return tree;
        }

        /// <summary>
        /// Findet rekusiv alle direkten Vorfahren eines AutomationElements
        /// </summary>
        /// <param name="element">gibt das AutomationElement an von dem die Vorfahren gesucht werden sollen</param>
        /// <param name="rootElement">gibt das rootElemet der Suche an</param>
        /// <param name="tree">gibt den aktuel erstellten Baum mit den Vorfahren des Elements an</param>
        private void findParentOfElement(AutomationElement element, AutomationElement rootElement,ITreeStrategy<OSMElement.OSMElement> tree)
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
        private void addParentOfNode(AutomationElement parentElement, ITreeStrategy<OSMElement.OSMElement> tree)
        {
            ITreeStrategy<OSMElement.OSMElement> tree2 = specifiedTree.NewNodeTree();
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(parentElement);
            ITreeStrategy<OSMElement.OSMElement> node = tree2.AddChild(osmElement);
            node.AddChild(tree);
            tree.Clear();
            tree.AddChild(tree2);
            Console.WriteLine();
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
            if (properties.acceleratorKeyFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AcceleratorKeyProperty, properties.acceleratorKeyFiltered), resultCondition);
            }
            if (properties.accessKeyFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AccessKeyProperty, properties.accessKeyFiltered), resultCondition);
            }

            //TODO: evtl. hier gleich am Anfang prüfen und ggf. (falls vorhanden) nur diese Propertie nehmen?
            if (properties.autoamtionIdFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, properties.autoamtionIdFiltered), resultCondition);
            }
            //.. 
            return resultCondition;
        }


    }
    #endregion
        
}



