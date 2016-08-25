﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using System.Windows.Automation;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;
using System.Windows;

//using Microsoft.Practices.Prism;
//using Microsoft.Practices.Prism.PubSubEvents;
//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md

//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample


namespace StrategyUIA2
{
    #region filterStrategyUIAClass
    /// <summary>
    /// Das ist eine Kopie von StrategyUIA.FilterStrategyUIA um das Filtern von Knoten mit unterschiedlichen Filtern zu testen
    /// </summary>
    public class FilterStrategyUIA2 : IFilterStrategy
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        public StrategyManager getStrategyMgr() { return strategyMgr; }

        /// <summary>
        /// Erstellt anhand des Handles einer Anwendung den zugehörigen Baum
        /// Variable mainWindowElement sollte umbenannt werden, da es nicht das mainwindow sein muss, es ist einfach nur ein automationelement, die übergabe des mainwindow erfolgt beim aufruf von filtering
        /// </summary>
        /// <param name="hwnd">den handle der Anwendung an</param>
        /// <returns>ein <code>ITree<GeneralProperties></code>-Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd)
        {
            ////alter Code, geht nicht mehr, prbl abarbeitung ganzer baum
            //UIAEventsMonitor uiaEvents = new UIAEventsMonitor();
            return filtering(hwnd, TreeScopeEnum.Application, -1);
        }

        /// <summary>
        /// Filtert eine Anwendung/Teilanwendung ausgehend vom AutomationElement;
        /// nur wenn der ganze Baum gefiltert wird (TreeScopeEnum.Application) werden die IdGenerated gesetzt
        /// </summary>
        /// <param name="automationElement">gibt das AutomationElement an von dem die Filterung ausgeht</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        private ITreeStrategy<OSMElement.OSMElement> filtering(AutomationElement automationElement, TreeScopeEnum treeScope, int depth)
        {
            ITreeStrategy<OSMElement.OSMElement> tree = getStrategyMgr().getSpecifiedTree().NewNodeTree();
            if (automationElement == null)
            {
                throw new ArgumentException("Main Element in FilterStrategyUIA.filtering nicht gefunden!");
            }
            switch (treeScope)
            {
                case TreeScopeEnum.Sibling:
                    filterSibling(automationElement, ref tree);
                    break;
                case TreeScopeEnum.Children:
                    filterChildren(automationElement, depth, ref tree);
                    break;
                case TreeScopeEnum.Descendants:
                    // selbe wie Children bloß alle Kindeskinder
                    filterChildren(automationElement, -1, ref tree);
                    // strategyMgr.getSpecifiedTreeOperations().setFilterstrategyInPropertiesAndObject(this.GetType(), ref parentNode);
                    break;
                case TreeScopeEnum.Subtree:
                    filterSubtree(automationElement, ref tree);
                    break;
                case TreeScopeEnum.Element:
                    filterElement(automationElement, ref tree);
                    setSpecialPropertiesOfFirstNode(ref tree);
                    break;
                case TreeScopeEnum.Ancestors:
                    //selbe wie Parent
                    filterParents(automationElement, ref tree);
                    break;
                case TreeScopeEnum.Application:
                    filterApplication(automationElement, depth, ref tree);
                    //beim ersten Knoten die Strategy mit ranschreiben + ModulName
                    setSpecialPropertiesOfFirstNode(ref tree);
                    strategyMgr.getSpecifiedTreeOperations().generatedIdsOfFilteredTree(ref tree);
                    List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.getFilterstrategiesOfNodes();
                    FilterstrategiesOfTree.addFilterstrategyOfNode(tree.Child.Data.properties.IdGenerated, this.GetType(), ref filterstrategies);
                    grantTrees.setFilterstrategiesOfNodes(filterstrategies);
                    break;
            }

            return tree;
        }



        /// <summary>
        /// Filtert eine Anwendung/Teilanwendung ausgehend vom hwnd
        /// </summary>
        /// <param name="hwnd">gibt den Handle der zu filternden Anwendung/Element an</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd, TreeScopeEnum treeScope, int depth)
        {
            AutomationElement mainElement = deliverAutomationElementFromHWND(hwnd);
            return filtering(mainElement, treeScope, depth);
        }

        /// <summary>
        /// Filtert ausgehend vom angegebenen Punkt (<paramref name="pointX"/>, <paramref name="pointY"/>) unter Berücksichtigung des angegebenen <code>StrategyManager.TreeScopeEnum</code> Baum
        /// </summary>
        /// <param name="pointX">gibt die x-koordinate des zu filternden Elements an</param>
        /// <param name="pointY">gibt die Y-Koordinate des zu filternden Elements an</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <param name="depth">gibt für den <paramref name="treeScope"/> von 'Parent', 'Children' und 'Application' die Tiefe an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> filtering(int pointX, int pointY, TreeScopeEnum treeScope, int depth)
        {
            AutomationElement mainElement = deliverAutomationElementFromCursor(pointX, pointY);
            if (mainElement == null)
            {
                throw new ArgumentException("Main Element in FilterStrategyUIA.filtering nicht gefunden!");
            }
            ITreeStrategy<OSMElement.OSMElement> tree = getStrategyMgr().getSpecifiedTree().NewNodeTree();

           // UIAEventsMonitor uiaEvents = new UIAEventsMonitor();
           // uiaEvents.eventsUIA_withAutomationElement(mainElement);
            return filtering(mainElement, treeScope, depth);
        }

        /// <summary>
        /// Filtert ausgehend von einem <code>AutomationElement</code> der Anwendung, die zugehörige Anwendung bis zur angegebenen Tiefe
        /// </summary>
        /// <param name="element">gibt ein <code>AutomationElement</code> der zu filternden Anwendung an</param>
        /// <param name="depth">gibt die Tiefe der Filterung an, <code>-1</code> steht dabei für die 'komplette' Tiefe </param>
        /// <param name="parentNode">referenziert den gefilterten Baum</param>
        private void filterApplication(AutomationElement element, int depth, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            IntPtr mainAppHwdn = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(element.Current.ProcessId);
            AutomationElement mainAppAutomationelement = deliverAutomationElementFromHWND(mainAppHwdn);

            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(mainAppAutomationelement);
            ITreeStrategy<OSMElement.OSMElement> top = tree.AddChild(osmElement);
            filterChildren(mainAppAutomationelement, -1, ref top);
        }

        private void filterSubtree(AutomationElement element, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            //ITreeStrategy<OSMElement.OSMElement> treeTop = parentNode.Copy();
            //filterElement(element, ref parentNode);
            //treeTop = parentNode.Root;

            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(element);
            ITreeStrategy<OSMElement.OSMElement> treeTop = tree.AddChild(osmElement);

            filterChildren(element, -1, ref treeTop);
            tree = treeTop.Root;
        }

        /// <summary>
        /// Filtert/Ermittelt die Daten des angegeben Elements
        /// </summary>
        /// <param name="mainElement">gibt das gewünschte <code>AutomationElement</code> an</param>
        /// <param name="parentNode">referenziert den gefilterten Baum</param>
        private void filterElement(AutomationElement mainElement, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(mainElement);
            tree.AddChild(osmElement);
        }

        /// <summary>
        /// Filtert die Kinder des angegebenen <code>AutomationElements</code>
        /// </summary>
        /// <param name="mainElement">gibt das <code>AutomationElement</code> an, von dem die Kinder gefiltert werden sollen</param>
        /// <param name="depth">gibt die Tiefe der Filterung an, <code>-1</code> steht dabei für die 'komplette' Tiefe</param>
        /// <param name="parentNode">referenziert den gefilterten Baum</param>
        private void filterChildren(AutomationElement mainElement, int depth, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            //TODO: oder auch über TreeWalker?
            AutomationElementCollection collection = mainElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            findChildrenOfNode(tree, collection, TreeScope.Children, depth);
        }

        /// <summary>
        /// Filtert die Geschwister des angegebenen <code>AutomationElements</code>
        /// </summary>
        /// <param name="mainElement">gibt das <code>AutomationElement</code> an, von dem die geschwister gefiltert werden sollen</param>
        /// <param name="parentNode">referenziert den gefilterten Baum</param>
        private void filterSibling(AutomationElement mainElement, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementParent = walker.GetParent(mainElement);
            filterChildren(elementParent, 1, ref tree);
            Console.WriteLine();
            //oder             walker.GetPreviousSibling(mainElement);             walker.GetNextSibling(mainElement);
        }

        /// <summary>
        /// Filtert die Eltern des angegebenen <code>AutomationElements</code>
        /// </summary>
        /// <param name="mainElement">gibt das <code>AutomationElement</code> an, von dem die Eltern ermittelt werden sollen</param>
        /// <param name="parentNode">referenziert den gefilterten Baum</param>
        private void filterParents(AutomationElement mainElement, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            if (!(mainElement.Current.ControlType.LocalizedControlType).Equals("Fenster"))// TODO: Root bestimmen
            {
                TreeWalker walker = TreeWalker.ControlViewWalker;
                AutomationElement elementParent = walker.GetParent(mainElement);
                addParentOfNode(elementParent, ref tree);
                filterParents(elementParent, ref  tree);

            }
        }


        /// <summary>
        /// todo
        /// Ordnet die Eigenschaften eines AutomationElements dem <typeparamref name="GeneralProperties"/>-Objekt zu
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

            try
            {
                if (!element.Current.BoundingRectangle.IsEmpty) //Anmerkung: Wenn BoundingRectangle == Empty, dann gibt es Probleme beim Einlesen einer erstellten XML (XmlDeserialize)
                {
                    elementP.boundingRectangleFiltered = element.Current.BoundingRectangle;
                }
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (BoundingRectangle) '{0}'", a.ToString());
            }
            try
            {
                elementP.classNameFiltered = element.Current.ClassName;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ClassName) '{0}'", a.ToString());
            }
            try
            {
                String tmp = element.Current.ControlType.ProgrammaticName;
                String[] t = tmp.Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                elementP.controlTypeFiltered = t[1];
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try
            {
                elementP.frameWorkIdFiltered = element.Current.FrameworkId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (FrameworkId) '{0}'", a.ToString());
            }
            try
            {
                elementP.hasKeyboardFocusFiltered = element.Current.HasKeyboardFocus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HasKeyboardFocus) '{0}'", a.ToString());
            }
            try
            {
                elementP.helpTextFiltered = element.Current.HelpText;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HelpText) '{0}'", a.ToString());
            }
            try
            {
                elementP.hWndFiltered = new IntPtr(element.Current.NativeWindowHandle);
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (NativeWindowHandle) '{0}'", a.ToString());
            }
            try
            {
                elementP.isContentElementFiltered = element.Current.IsContentElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsContentElement) '{0}'", a.ToString());
            }
            try
            {
                elementP.isControlElementFiltered = element.Current.IsControlElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsControlElement) '{0}'", a.ToString());
            }
            try
            {
                elementP.isEnabledFiltered = element.Current.IsEnabled;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsEnabled) '{0}'", a.ToString());
            }
            try
            {
                elementP.isKeyboardFocusableFiltered = element.Current.IsKeyboardFocusable;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsKeyboardFocusable) '{0}'", a.ToString());
            }
            try
            {
                elementP.isOffscreenFiltered = element.Current.IsOffscreen;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsOffscreen) '{0}'", a.ToString());
            }
            try
            {
                elementP.isPasswordFiltered = element.Current.IsPassword;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsPassword) '{0}'", a.ToString());
            }
            try
            {
                elementP.isRequiredForFormFiltered = element.Current.IsRequiredForForm;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsRequiredForForm) '{0}'", a.ToString());
            }
            try
            {
                elementP.itemStatusFiltered = element.Current.ItemStatus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemStatus) '{0}'", a.ToString());
            }
            try
            {
                elementP.itemTypeFiltered = element.Current.ItemType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemType) '{0}'", a.ToString());
            }
            try
            {
                elementP.runtimeIDFiltered = element.GetRuntimeId();
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (runtime) '{0}'", a.ToString());
            }
            try
            {
                elementP.localizedControlTypeFiltered = element.Current.LocalizedControlType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try
            {
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
            setSupportedPatterns(ref elementP, element);
            /*if (elementP.IdGenerated == null)
            {
                elementP.IdGenerated = OSMElement.Helper.generatedIdFilteredNode(elementP); //TODO: bessere Stelle für den Aufruf?
                //Console.WriteLine("hash = " + elementP.IdGenerated);
            }*/
            //prüfen, ob es jetzt eine andere Filter-Strategy ist
            /*if (grantTrees != null && grantTrees.getFilteredTree() != null && grantTrees.getFilteredTree().HasChild)
            {
                Type interfaceOfClass = this.GetType().GetInterfaces()[0]; // das diese Klasse ein interface hat wissen wir hier
                // wenn das angegebene Interface nicht gefunden wird ist der Wert hier null
                if (grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName != null && grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace != null)
                {
                    Type filterStrategyTypeTree = getTypeOfStrategy(grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyFullName, grantTrees.getFilteredTree().Child.Data.properties.grantFilterStrategyNamespace);
                    Type interfacesOfTree = filterStrategyTypeTree.GetInterface(interfaceOfClass.Name);
                    if (interfacesOfTree != null)
                    {
                        if (filterStrategyTypeTree != this.GetType())
                        {//wir haben hier nicht die Standard-Filter-Methode
                            elementP.grantFilterStrategyFullName = this.GetType().FullName;
                            elementP.grantFilterStrategyNamespace = this.GetType().Namespace;
                        }
                    }
                }
            }*/
            return elementP;
        }

        /// <summary>
        /// Die Mehtode behhandelt die verschiedenen Pattern
        /// </summary>
        /// <param name="properties">gibt die Propertoes des Knotens an</param>
        /// <param name="element">gibt das AutomationElement des Knotens an</param>
        private void setPropertiesOfPattern(ref GeneralProperties properties, AutomationElement element)
        {//https://msdn.microsoft.com/de-de/library/ms750574(v=vs.110).aspx

            object valuePattern = null;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
            {/* 
              * Conditional Support: Combo Box, Data Item, Edit,Hyperlink, List Item, Progress Bar, Slider,Spinner
              */
                try
                {
                    properties.valueFiltered = (valuePattern as ValuePattern).Current.Value;
                }
                catch (System.NullReferenceException) { }

            }
            object rangeValuePattern = null;
            if (element.TryGetCurrentPattern(RangeValuePattern.Pattern, out rangeValuePattern))
            {
                /*
                 * Conditional Support: Edit, Progress Bar, Scroll Bar, Slider, Spinner
                 */
                RangeValue rangeValue = new RangeValue();
                rangeValue.isReadOnly = (rangeValuePattern as RangeValuePattern).Current.IsReadOnly;
                rangeValue.largeChange = (rangeValuePattern as RangeValuePattern).Current.LargeChange;
                rangeValue.maximum = (rangeValuePattern as RangeValuePattern).Current.Maximum;
                rangeValue.minimum = (rangeValuePattern as RangeValuePattern).Current.Minimum;
                rangeValue.smallChange = (rangeValuePattern as RangeValuePattern).Current.SmallChange;
                rangeValue.currentValue = (rangeValuePattern as RangeValuePattern).Current.Value;
                properties.rangeValue = rangeValue;
            }

        }

        /// <summary>
        /// Ermittelt die unterstützten Pattern;
        /// Das zugewiesene Object ist dabei vom Type <code>AutomationPattern[]</code>
        /// </summary>
        /// <param name="properties">Eine Referenz zu den gesetzten Properties des Elements</param>
        /// <param name="element">gibt das AutomationElement des Knotens an</param>
        private void setSupportedPatterns(ref GeneralProperties properties, AutomationElement element)
        {
            properties.suportedPatterns = element.GetSupportedPatterns().ToArray();
        }

        /// <summary>
        /// Setzt für den ersten Knoten spezielle Eigenschaften
        /// </summary>
        /// <param name="parentNode">gibt eine Referenz auf den bisherigen Baum an</param>
        private void setSpecialPropertiesOfFirstNode(ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            if (tree.HasChild)
            {
                GeneralProperties prop = tree.Child.Data.properties;
                //   prop.grantFilterStrategyFullName = this.GetType().FullName;
                //   prop.grantFilterStrategyNamespace = this.GetType().Namespace;
                Settings settings = new Settings();
                prop.grantFilterStrategy = settings.filterStrategyTypeToUserName(this.GetType());
                prop.moduleName = strategyMgr.getSpecifiedOperationSystem().getModulNameOfApplication(prop.nameFiltered);
                prop.fileName = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByMainWindowTitle(prop.nameFiltered);
                OSMElement.OSMElement osm = new OSMElement.OSMElement();
                osm.brailleRepresentation = tree.Child.Data.brailleRepresentation;
                osm.events = tree.Child.Data.events;
                osm.properties = prop;
                tree.Child.Data = osm;
            }
        }

        /// <summary>
        /// Ermittelt aus dem alten <code>OSMElement</code> eines Knotens die aktualisierten Properties
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element an welches aktualisiert werden soll</param>
        /// <returns>gibt für einen Knoten die aktualisierten Properties zurück</returns>
        public GeneralProperties updateNodeContent(OSMElement.OSMElement osmElementFilteredNode)
        {
            GeneralProperties propertiesUpdated = new GeneralProperties();
            AutomationElement au;
            Condition cond = setPropertiesCondition(osmElementFilteredNode.properties);

            if (osmElementFilteredNode.properties.hWndFiltered != IntPtr.Zero)
            {
                //ist der Weg wirklich schneller?
                IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(osmElementFilteredNode.properties.hWndFiltered));
                AutomationElement mainWindowElement = deliverAutomationElementFromHWND(pointer);
                //au = mainWindowElement.FindFirst(TreeScope.Children, cond);
                au = mainWindowElement.FindFirst(TreeScope.Subtree, cond);
            }
            else
            {
                if (grantTrees.getFilteredTree() != null && grantTrees.getFilteredTree().HasChild && grantTrees.getFilteredTree().Child.Data.properties.hWndFiltered != IntPtr.Zero)
                {
                    IntPtr hwnd = grantTrees.getFilteredTree().Child.Data.properties.hWndFiltered;
                    //IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(strategyMgr.getFilteredTree().Child.Data.properties.hWndFiltered));
                    AutomationElement element = AutomationElement.FromHandle(hwnd);
                    au = element.FindFirst(TreeScope.Descendants, cond);
                }
                else
                {
                    au = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, cond); //Achtung hier könnte auch ein anderes Element gefunden werden
                }
            }
            if (au != null)
            {
                propertiesUpdated = setProperties(au);
            }
            return propertiesUpdated;
        }


        /// <summary>
        /// Filtert ausgehend vom angegebenen OSMElement
        /// </summary>
        /// <param name="osmElementOfFirstNodeOfSubtree">gibt das (alt) OSM-Element an, von dem die neue Filterung ausgeht</param>
        /// <param name="treeScope">gibt die 'Art' der Filterung an</param>
        /// <returns>der gefilterte (Teil-)Baum</returns>
        public ITreeStrategy<OSMElement.OSMElement> updateFiltering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScope)
        {
            AutomationElement au;
            Condition cond = setPropertiesCondition(osmElementOfFirstNodeOfSubtree.properties);

            if (osmElementOfFirstNodeOfSubtree.properties.hWndFiltered != IntPtr.Zero)
            {
                //ist der Weg wirklich schneller?
                IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(osmElementOfFirstNodeOfSubtree.properties.hWndFiltered));
                AutomationElement mainWindowElement = deliverAutomationElementFromHWND(pointer);
                //au = mainWindowElement.FindFirst(TreeScope.Children, cond);
                au = mainWindowElement.FindFirst(TreeScope.Subtree, cond);
            }
            else
            {
                if (grantTrees.getFilteredTree() != null && grantTrees.getFilteredTree().HasChild && grantTrees.getFilteredTree().Child.Data.properties.hWndFiltered != IntPtr.Zero)
                {
                    IntPtr hwnd = grantTrees.getFilteredTree().Child.Data.properties.hWndFiltered;
                    //IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(strategyMgr.getFilteredTree().Child.Data.properties.hWndFiltered));
                    AutomationElement element = AutomationElement.FromHandle(hwnd);
                    au = element.FindFirst(TreeScope.Descendants, cond);
                }
                else
                {
                    au = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, cond); //Achtung hier könnte auch ein anderes Element gefunden werden
                }
            }
            if (au != null)
            {
                return filtering(au, treeScope, -1);
            }
            return null;
        }

        /// <summary>
        /// todo bzw. nachfrage: geht die methode findall nicht alleine alle elemente der collection durch? wird in der foreach-schleife dadurch nicht doppelt gearbeitet?
        /// Sucht rekusiv alle Kindelemente eines Knotens und ermittelt dessen Eingenschaften
        /// </summary>
        /// <param name="top">gibt den Namen des Kindelementes an</param>
        /// <param name="collection">gibt die AutomationElement-Collection an</param>
        /// <param name="depth">gibt an wie tief der Suche ausgehend vom Root-Element an.</param>
        private void findChildrenOfNode(ITreeStrategy<OSMElement.OSMElement> top, AutomationElementCollection collection, TreeScope treeScorpe, int depth)
        {
            foreach (AutomationElement element in collection)
            {
                if (top.Depth < depth || depth <= -1)
                {
                    OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
                    osmElement.properties = setProperties(element);
                    ITreeStrategy<OSMElement.OSMElement> node = top.AddChild(osmElement);

                    AutomationElementCollection c = element.FindAll(treeScorpe, Condition.TrueCondition);
                    findChildrenOfNode(node, c, treeScorpe, depth);
                }
            }
        }

        //  AutomationElement vom HWND
        public static AutomationElement deliverAutomationElementFromHWND(IntPtr hwnd)
        {
            AutomationElement element;
            try
            {
                element = AutomationElement.FromHandle(hwnd);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new System.ComponentModel.Win32Exception("AutomationElement kann nicht ermittelt werden -- Zugriff verweigert");
            }
            //element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return element;
        }


        // ProzessID vom AutomationElement
        public int deliverElementID(IntPtr hwnd)
        {
            //window = WindowFromPoint(cp);
            AutomationElement element;
            try
            {
                element = AutomationElement.FromHandle(hwnd);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new System.ComponentModel.Win32Exception("AutomationElement kann nicht ermittelt werden -- Zugriff verweigert");
            }
            int processIdentifier = (int)element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return processIdentifier;
        }

        public AutomationElement deliverAutomationElementFromCursor(int x, int y)
        {
            // Convert mouse position from System.Drawing.Point to System.Windows.Point.
            System.Windows.Point point = new System.Windows.Point(x, y);

            AutomationElement element;
            try
            {
                element = AutomationElement.FromPoint(point);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new System.ComponentModel.Win32Exception("AutomationElement kann nicht ermittelt werden -- Zugriff verweigert");
            }
            return element;
        }

        public OSMElement.OSMElement setOSMElement(int pointX, int pointY)
        {

            AutomationElement mouseElement = deliverAutomationElementFromCursor(pointX, pointY);

            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();

            osmElement.properties = setProperties(mouseElement);

            //Id setzen
            List<ITreeStrategy<OSMElement.OSMElement>> node = strategyMgr.getSpecifiedTreeOperations().searchProperties(grantTrees.getFilteredTree(), osmElement.properties, OperatorEnum.and);
            if (node.Count == 1)
            {
                return node[0].Data;
            }
            else
            {
                Debug.WriteLine("Element im Baum nicht gefunden");
                return osmElement;
            }

        }

        /* public void getMouseRect(IntPtr hwnd, int pointX, int pointY, out int x, out int y, out int width, out int height)
         {
             //AutomationElement mouseElement = deliverAutomationElementFromHWND(hwnd);
             AutomationElement mouseElement = deliverAutomationElementFromCursor(pointX, pointY);

             OSMElement.OSMElement filteredSubtree = new OSMElement.OSMElement();

             filteredSubtree.properties = setProperties(mouseElement);
            
             //Rect mouseRect = mouseElement.Current.BoundingRectangle;
             x = (int)filteredSubtree.properties.boundingRectangleFiltered.TopLeft.X;
             y = (int)mouseElement.Current.BoundingRectangle.TopLeft.Y;
             int x2 = (int)mouseElement.Current.BoundingRectangle.TopRight.X;
             int y2 = (int)mouseElement.Current.BoundingRectangle.BottomLeft.Y;
             int[] runtimes= mouseElement.GetRuntimeId();
             height = y2 - y;
             width = x2 - x;
          
             Console.WriteLine("hier x: " + x);
             Console.WriteLine("hier y: " + y);
             Console.WriteLine("hier w: " + width);
             Console.WriteLine("hier h: " + height);
             Console.WriteLine("ElnazHWND: '{0}'", hwnd.ToString());
         }*/

        /// <summary>
        /// Ermittelt das zugehörige AutomationElement eines Knotens aus dem gefilterten Baum
        /// </summary>
        /// <param name="node">gibt den Knoten an, von dem das zugehörige AutomationElement ermittelt werden soll</param>
        /// <param name="rootElement"></param> --- hier erst ermitteln
        /// <returns>das zugehörige AutomationElement des Knotens</returns>
        private AutomationElement getAutomationElementFromFilteredTree(ITreeStrategy<OSMElement.OSMElement> node, AutomationElement rootElement)
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
        /// Fügt ein neues Elternelement einen Baum hinzu.
        /// </summary>
        /// <param name="parentElement">gibt das neue Elternelement an</param>
        /// <param name="parentNode">gibt den "alten" Baum an</param>
        private void addParentOfNode(AutomationElement parentElement, ref ITreeStrategy<OSMElement.OSMElement> tree)
        {
            ITreeStrategy<OSMElement.OSMElement> tree2 = getStrategyMgr().getSpecifiedTree().NewNodeTree();
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
            Condition resultCondition;
            #region von allen auslesbar
            resultCondition = new PropertyCondition(AutomationElement.ClassNameProperty, properties.classNameFiltered);
            // ... ?
            #endregion
            if (properties.localizedControlTypeFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, properties.localizedControlTypeFiltered), resultCondition);
            }
            if (properties.acceleratorKeyFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AcceleratorKeyProperty, properties.acceleratorKeyFiltered), resultCondition);
            }
            if (properties.accessKeyFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AccessKeyProperty, properties.accessKeyFiltered), resultCondition);
            }
            if (properties.runtimeIDFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.RuntimeIdProperty, properties.runtimeIDFiltered), resultCondition);
            }
            if (properties.frameWorkIdFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.FrameworkIdProperty, properties.frameWorkIdFiltered), resultCondition);
            }
            if (properties.isContentElementFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.IsContentElementProperty, properties.isContentElementFiltered), resultCondition);
            }
            if (properties.labeledbyFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.LabeledByProperty, properties.labeledbyFiltered), resultCondition);
            }
            if (properties.isControlElementFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.IsControlElementProperty, properties.isControlElementFiltered), resultCondition);
            }
            if (properties.isPasswordFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.IsPasswordProperty, properties.isPasswordFiltered), resultCondition);
            }
            if (properties.itemTypeFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.ItemTypeProperty, properties.itemTypeFiltered), resultCondition);
            }
            if (properties.itemStatusFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.ItemStatusProperty, properties.itemStatusFiltered), resultCondition);
            }
            if (properties.isRequiredForFormFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.IsRequiredForFormProperty, properties.isRequiredForFormFiltered), resultCondition);
            }
            if (properties.autoamtionIdFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, properties.autoamtionIdFiltered), resultCondition);
            }
            //.. 
            return resultCondition;
        }

        private Type getTypeOfStrategy(String fullName, String ns)
        {
            return Type.GetType(fullName + ", " + ns);
        }

    }
    #endregion

}