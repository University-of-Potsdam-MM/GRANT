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

namespace StrategyUIA
{
    public class FilterStrategyUIA : IFilterStrategy
    {
        private IOperationSystemStrategy specifiedOperationSystem;
        private ITreeStrategy<OSMElement.OSMElement> specifiedTree; //ersetzen durch
        private StrategyMgr strategyMgr;

        public void setStrategyMgr(StrategyMgr manager) { strategyMgr = manager; }
        public StrategyMgr getStrategyMgr() { return strategyMgr; }




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

            UIAEventMonitor uiaEvents = new UIAEventMonitor();
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
            try
            {
            elementP.acceleratorKeyFiltered = element.Current.AcceleratorKey;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (Accelerator) '{0}'", a.ToString());
            }
            try
            {
            elementP.accessKeyFiltered = element.Current.AccessKey;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (AccessKey) '{0}'", a.ToString());
            }
            try
            {
            elementP.autoamtionIdFiltered = element.Current.AutomationId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (AutomationId) '{0}'", a.ToString());
            }

            try {
            if (!element.Current.BoundingRectangle.IsEmpty) //Anmerkung: Wenn BoundingRectangle == Empty, dann gibt es Probleme beim Einlesen einer erstellten XML (XmlDeserialize)
            {
                elementP.boundingRectangleFiltered = element.Current.BoundingRectangle;
                }
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (BoundingRectangle) '{0}'", a.ToString());
            }
            try {
            elementP.classNameFiltered = element.Current.ClassName;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ClassName) '{0}'", a.ToString());
            }
            try {
            elementP.controlTypeFiltered = element.Current.ControlType.LocalizedControlType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try {
            elementP.frameWorkIdFiltered = element.Current.FrameworkId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (FrameworkId) '{0}'", a.ToString());
            }
            try {
            elementP.hasKeyboardFocusFiltered = element.Current.HasKeyboardFocus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HasKeyboardFocus) '{0}'", a.ToString());
            }
            try {
            elementP.helpTextFiltered = element.Current.HelpText;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HelpText) '{0}'", a.ToString());
            }
            try {
            elementP.hWndFiltered = element.Current.NativeWindowHandle;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (NativeWindowHandle) '{0}'", a.ToString());
            }
            try {
            elementP.isContentElementFiltered = element.Current.IsContentElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsContentElement) '{0}'", a.ToString());
            }
            try {
            elementP.isControlElementFiltered = element.Current.IsControlElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsControlElement) '{0}'", a.ToString());
            }
            try {
            elementP.isEnabledFiltered = element.Current.IsEnabled;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsEnabled) '{0}'", a.ToString());
            }
            try {
            elementP.isKeyboardFocusableFiltered = element.Current.IsKeyboardFocusable;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsKeyboardFocusable) '{0}'", a.ToString());
            }
            try {
            elementP.isOffscreenFiltered = element.Current.IsOffscreen;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsOffscreen) '{0}'", a.ToString());
            }
            try {
            elementP.isPasswordFiltered = element.Current.IsPassword;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsPassword) '{0}'", a.ToString());
            }
            try {
            elementP.isRequiredForFormFiltered = element.Current.IsRequiredForForm;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsRequiredForForm) '{0}'", a.ToString());
            }
            try {
            elementP.itemStatusFiltered = element.Current.ItemStatus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemStatus) '{0}'", a.ToString());
            }
            try {
            elementP.itemTypeFiltered = element.Current.ItemType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemType) '{0}'", a.ToString());
            }
            try {
            elementP.localizedControlTypeFiltered = element.Current.LocalizedControlType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try {
                elementP.nameFiltered = element.Current.Name;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (Name) '{0}'", a.ToString());
            }
            try
            {
            elementP.processIdFiltered = element.Current.ProcessId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ProcessId) '{0}'", a.ToString());
            }
            setPropertiesOfPattern(ref elementP, element);
            if (elementP.IdGenerated == null)
            {
                elementP.IdGenerated = Helper.generatedId(elementP); //TODO: bessere Stelle für den Aufruf; sollte eigentlich nicht wieder neu berechnet werden
                Console.WriteLine("hash = " + elementP.IdGenerated);
            }

            return elementP;
        }

        /// <summary>
        /// Sofern vorhanden, wird der Text aus Eingabefeldern ausgelesen und 'valueFiltered' zugewiesen
        /// </summary>
        /// <param name="properties">gibt die Propertoes des Knotens an</param>
        /// <param name="element">gibt das AutomationElement des knotens an</param>
        private void setPropertiesOfPattern(ref GeneralProperties properties, AutomationElement element)
        {
            object valuePattern = null;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
            {
                properties.valueFiltered = (valuePattern as ValuePattern).Current.Value;
            }
        }

        /// <summary>
        /// Ändert die <code>GeneralProperties</code> im gespiegelten Baum anhand der angegebenen <code>IdGenerated</code>. (Sollten mehrere Knoten mit der selben Id existieren, so werden alle aktualisiert.)
        /// </summary>
        /// <param name="mirroredTreeGeneratedId">gibt die generierte Id des zu ändernden knotens im gespielgelten Baum an.</param>
        public void updateNodeOfMirroredTree(String mirroredTreeGeneratedId)
        {
            AutomationElement au;
            List<ITreeStrategy<OSMElement.OSMElement>> relatedMirroredTreeObject =  strategyMgr.getSpecifiedTree().getAssociatedNodeList(mirroredTreeGeneratedId); //TODO: in dem Kontext wollen wir eigentlich nur ein Element zurückbekommen
            AutomationElement mainWindowElement;
            foreach (ITreeStrategy<OSMElement.OSMElement> treeElement in relatedMirroredTreeObject)
            {
                Condition cond = setPropertiesCondition(treeElement.Data.properties);
                if (treeElement.Data.properties.hWndFiltered == 0)
                {
                    au = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, cond);
                }
                else
                {//ist der Weg wirklich schneller?
                    IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID((IntPtr)treeElement.Data.properties.hWndFiltered));
                    mainWindowElement = deliverAutomationElementFromHWND(pointer);
                    au = mainWindowElement.FindFirst(TreeScope.Children, cond);
                }
                if (au != null)
                {
                    GeneralProperties propertiesUpdated = setProperties(au);
                    specifiedTree.changePropertiesOfNode(propertiesUpdated);
                    break;
                }
            }
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
            //Falls die "AutomationId" gesetzt wurde, so ist diese Eigenschaft ausreichend um das Element eindeutig zu identifizieren
            if (properties.autoamtionIdFiltered != null && !properties.autoamtionIdFiltered.Equals(""))
            {
                return  new PropertyCondition(AutomationElement.AutomationIdProperty, properties.autoamtionIdFiltered);
            }

            //TODO: Achtung einige Eigenschaften vonrscheinlich GeneralProperties sollten wahrscheinlich nicht genutzt werden
            Condition resultCondition;
            #region von allen auslesbar
            //resultCondition = new PropertyCondition(AutomationElement.NameProperty, properties.nameFiltered);
                resultCondition = new PropertyCondition(AutomationElement.ClassNameProperty, properties.classNameFiltered);
            
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


            //.. 
            return resultCondition;
        }


    }


    #region Events
    /// <summary>
    /// todo
    /// Abfangen des events eines button funktioniert, auch für test-wpf-app
    /// next step: events verallgemeinern + als stratgy-pattern nutzen
    /// </summary>
    public class UIAEventMonitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHWND"></param>
        public void eventsUIA(IntPtr appHWND)
        {
            try
            {
                //die Methode getProcessHwndFromHwnd liefert das GUElemtn der eigentlichen Anwendung
                //appHWND = operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points));

                Console.WriteLine("appHWND: '{0}'", appHWND.ToString());

                //todo apphwnd durchgehen mittels treescope und den ersten button der app geben lassen
                AutomationElement at = FilterStrategyUIA.deliverAutomationElementFromHWND(appHWND);
                
                SubscribeToInvoke(at);

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
        }

        #region UIA_Automation_Events
        //Automation.AddAutomationEventHandler Method (AutomationEvent, AutomationElement, TreeScope, AutomationEventHandler)
        //https://msdn.microsoft.com/en-us/library/system.windows.automation.automation.addautomationeventhandler%28v=vs.110%29.aspx

        // Member Variables
        AutomationElement ElementSubscribeButton;
        AutomationEventHandler UIAeventHandler;

        public void SubscribeToInvoke(AutomationElement elementButton)
        {
            if (elementButton != null)
            {
                // hier wurde auf children geändert und damit sollte alle events der eigentlichen hwnd anwendung berücksichtigt werden
                Automation.AddAutomationEventHandler(InvokePattern.InvokedEvent,
                     elementButton, TreeScope.Children,
                     UIAeventHandler = new AutomationEventHandler(OnUIAutomationEvent));
                ElementSubscribeButton = elementButton;
            }
        }

        /// <summary>
        /// AutomationEventHandler delegate.
        /// </summary>
        /// <param name="src">Object that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void OnUIAutomationEvent(object src, AutomationEventArgs e)
        {
            // Make sure the element still exists. Elements such as tooltips
            // can disappear before the event is processed.
            AutomationElement sourceElement;
            try
            {
                sourceElement = src as AutomationElement;
            }
            catch (ElementNotAvailableException)
            {
                return;
            }
            if (e.EventId == InvokePattern.InvokedEvent)
            {
                // TODO Add handling code.
                Console.WriteLine("InvokedEvent raised '{0}'", sourceElement.ToString());
            }
            else
            {
                // TODO Handle any other events that have been subscribed to.
            }
        }

        private void ShutdownUIA()
        {
            if (UIAeventHandler != null)
            {
                Automation.RemoveAutomationEventHandler(InvokePattern.InvokedEvent,
                    ElementSubscribeButton, UIAeventHandler);
            }
        }
        #endregion

        #region UIA_Automation_Events
        //Automation.AddAutomationPropertyChangedEventHandler Method (AutomationElement, TreeScope, AutomationPropertyChangedEventHandler, AutomationProperty[])
        //https://msdn.microsoft.com/en-us/library/system.windows.automation.automation.addautomationpropertychangedeventhandler%28v=vs.110%29.aspx

        AutomationPropertyChangedEventHandler propChangeHandler;
        /// <summary>
        /// Adds a handler for property-changed event; in particular, a change in the enabled state.
        /// </summary>
        /// <param name="element">The UI Automation element whose state is being monitored.</param>
        public void SubscribePropertyChange(AutomationElement element)
        {
            Automation.AddAutomationPropertyChangedEventHandler(element,
                TreeScope.Element,
                propChangeHandler = new AutomationPropertyChangedEventHandler(OnPropertyChange),
                AutomationElement.IsEnabledProperty);

        }

        /// <summary>
        /// Handler for property changes.
        /// </summary>
        /// <param name="src">The source whose properties changed.</param>
        /// <param name="e">Event arguments.</param>
        private void OnPropertyChange(object src, AutomationPropertyChangedEventArgs e)
        {
            AutomationElement sourceElement = src as AutomationElement;
            if (e.Property == AutomationElement.IsEnabledProperty)
            {
                bool enabled = (bool)e.NewValue;
                // TODO: Do something with the new value. 
                // The element that raised the event can be identified by its runtime ID property.
            }
            else
            {
                // TODO: Handle other property-changed events.
            }
        }

        public void UnsubscribePropertyChange(AutomationElement element)
        {
            if (propChangeHandler != null)
            {
                Automation.RemoveAutomationPropertyChangedEventHandler(element, propChangeHandler);
            }
        }

        #endregion


    }
    #endregion

}



