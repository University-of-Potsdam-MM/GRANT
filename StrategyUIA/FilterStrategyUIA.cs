using System;
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
using GRANTManager.TreeOperations;

//using Microsoft.Practices.Prism;
//using Microsoft.Practices.Prism.PubSubEvents;
using Prism.Events;
//https://msdn.microsoft.com/en-us/library/ff649187.aspx
//https://github.com/PrismLibrary/Prism/blob/master/Documentation/WPF/70-CommunicatingBetweenLooselyCoupledComponents.md

//http://www.codeproject.com/Articles/355473/Prism-EventAggregator-Sample


namespace StrategyUIA
{
    public class FilterStrategyUIA : IFilterStrategy
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperation = treeOperation; }
        public void setStrategyMgr(StrategyManager manager) { strategyMgr = manager; }
        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees) { this.grantTrees = grantTrees; }
        
        public StrategyManager getStrategyMgr() { return strategyMgr; }

        #region filtering
        /// <summary>
        /// filters an application depending on a given hwnd
        /// </summary>
        /// <param name="hwnd">the process handle of applicatio/element</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Parent', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <returns>the filtered (sub-)tree</returns>
        public Object filtering(IntPtr hwnd, TreeScopeEnum treeScope = TreeScopeEnum.Application, int depth = -1)
        {            
            AutomationElement mainElement = deliverAutomationElementFromHWND(hwnd);
            return filtering(mainElement, treeScope, depth);            
        }

        /// <summary>
        /// filters an application depending on a given point (<paramref name="pointX"/>, <paramref name="pointY"/>) and the choosen <paramref name="treeScope"/>
        /// </summary>
        /// <param name="pointX">x coordinate of the element to filtering</param>
        /// <param name="pointY">y coordinate of the element to filtering</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Parent', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <returns>the filtered (sub-)tree</returns>
        public Object filtering(int pointX, int pointY, TreeScopeEnum treeScope, int depth = -1)
        {
            AutomationElement mainElement = deliverAutomationElementFromCursor(pointX, pointY);
            if (mainElement == null)
            {
                throw new ArgumentException("The AutomationElement is 'null'!");
            }
           Object tree = getStrategyMgr().getSpecifiedTree().NewTree();

            UIAEventsMonitor uiaEvents = new UIAEventsMonitor();
            uiaEvents.eventsUIA_withAutomationElement(mainElement);
            return filtering(mainElement, treeScope, depth);
        }

        /// <summary>
        /// filters an application depending on a given OSM element
        /// </summary>
        /// <param name="osmElementOfFirstNodeOfSubtree">osm element of the to filtered application</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <returns>the filtered (sub-)tree</returns>
        public Object filtering(OSMElement.OSMElement osmElementOfFirstNodeOfSubtree, TreeScopeEnum treeScope)
        {
            AutomationElement au = getAutomationelementOfOsmElement(osmElementOfFirstNodeOfSubtree);
            if (au != null)
            {
                return filtering(au, treeScope, -1);
            }
            return null;
        }

        public Object filtering(String generatedNodeId, TreeScopeEnum treeScope)
        {
            if (generatedNodeId == null) { return null; }
            return filtering(treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedNodeId), treeScope);
        }

        /// <summary>
        /// filters an application depending on a given AutomationElement;
        /// only if the whole tree is filtered (TreeScopeEnum.Application) the ids for each node will be generated and set
        /// </summary>
        /// <param name="automationElement"> the AutomationElement of the filtered element</param>
        /// <param name="treeScope">kind of filtering</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Ancestors', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <returns>the filtered (sub-)tree</returns>
        private Object filtering(AutomationElement automationElement, TreeScopeEnum treeScope, int depth)
        {
            Object tree = getStrategyMgr().getSpecifiedTree().NewTree();
            if (automationElement == null)
            {
                throw new ArgumentException("The AutomationElement is 'null'!");
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
                    // same like as 'Children' but also all "child children"
                    filterChildren(automationElement, -1, ref tree);
                    break;
                case TreeScopeEnum.Subtree:
                    filterSubtree(automationElement, ref tree);
                    break;
                case TreeScopeEnum.Element:
                    filterElement(automationElement, ref tree);
                    setSpecialPropertiesOfFirstNode(ref tree);
                    break;
                case TreeScopeEnum.Ancestors:
                    // same like parents
                    filterParents(automationElement, ref tree);
                    break;
                case TreeScopeEnum.Application:
                    filterApplication(automationElement, depth, ref tree);
                    // note by the first node the filter strategy and ModulName
                    setSpecialPropertiesOfFirstNode(ref tree);
                    treeOperation.generatedIds.generatedIdsOfFilteredTree(ref tree);
                    break;
            }
            return tree;
        }

        /// <summary>
        /// filters an application deppending on a given AutomationElement (to the specified depth)
        /// </summary>
        /// <param name="element">the AutomationElement of the filtered element</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/> of 'Parent', 'Children' and 'Application';  <code>-1</code> means the whole depth</param>
        /// <param name="tree">the tree object</param>
        private void filterApplication(AutomationElement element, int depth, ref Object tree)
        {
            IntPtr mainAppHwdn = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(element.Current.ProcessId);
            AutomationElement mainAppAutomationelement =  deliverAutomationElementFromHWND(mainAppHwdn);

            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(mainAppAutomationelement);

            Object top  = strategyMgr.getSpecifiedTree().AddChild(tree, osmElement);
            filterChildren(mainAppAutomationelement, -1, ref top);
        }

        /// <summary>
        /// filters a subtree
        /// </summary>
        /// <param name="element">the AutomationElement of the filtered element</param>
        /// <param name="tree">the tree object</param>
        private void filterSubtree(AutomationElement element, ref Object tree)
        {
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(element);
            
            Object treeTop = strategyMgr.getSpecifiedTree().AddChild(tree, osmElement);

            filterChildren(element, -1, ref treeTop);
            tree = strategyMgr.getSpecifiedTree().Root(treeTop);
        }

        /// <summary>
        /// filters an element
        /// </summary>
        /// <param name="automationElement">the AutomationElement of the filtered element</param>
        /// <param name="tree">the tree object</param>
        private void filterElement(AutomationElement automationElement, ref Object tree)
        {
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(automationElement);
            strategyMgr.getSpecifiedTree().AddChild(tree, osmElement);
        }

        /// <summary>
        /// filters the children of the AutomationElement
        /// </summary>
        /// <param name="automationElement">the AutomationElement</param>
        /// <param name="depth">depth of filtering for the <paramref name="treeScope"/>;  <code>-1</code> means the whole depth</param>
        /// <param name="tree">the tree object</param>
        private void filterChildren(AutomationElement automationElement, int depth, ref Object tree)
        {
           //TODO: oder auch über TreeWalker?
            AutomationElementCollection collection = automationElement.FindAll(TreeScope.Children, Condition.TrueCondition);
            findChildrenOfNode(tree, collection, depth);
        }

        /// <summary>
        /// filters the siblings of the AutomationElement
        /// </summary>
        /// <param name="automationElement">the AutomationElementn</param>
        /// <param name="tree">the tree object</param>
        private void filterSibling(AutomationElement automationElement, ref Object tree)
        {
            //TreeWalker walker = TreeWalker.ControlViewWalker;
            TreeWalker walker = TreeWalker.ContentViewWalker;
            AutomationElement elementParent = walker.GetParent(automationElement);
            int depth = 0;
      //      filterChildren(elementParent, 0, ref tree); // depth is null  ==> all siblings will be "top nodes" (see https://www.codeproject.com/Articles/12476/A-Generic-Tree-Collection )
            //oder             walker.GetPreviousSibling(mainElement);             walker.GetNextSibling(mainElement);

            AutomationElementCollection collection = elementParent.FindAll(TreeScope.Children, Condition.TrueCondition);
            //findChildrenOfNode(tree, collection, depth);

            foreach (AutomationElement element in collection)
            {
                if (!element.Equals(automationElement) && (strategyMgr.getSpecifiedTree().Depth(tree) < depth || depth <= -1))
                {
                    OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
                    osmElement.properties = setProperties(element);
                    Object node = strategyMgr.getSpecifiedTree().AddChild(tree, osmElement);

                    AutomationElementCollection c = element.FindAll(TreeScope.Children, Condition.TrueCondition);
                }
            }
        }

        /// <summary>
        /// filters the parents of the AutomationElement
        /// </summary>
        /// <param name="automationElement">the AutomationElement</param>
        /// <param name="tree">the tree object</param>
        private void filterParents(AutomationElement automationElement, ref Object tree)
        {
            if (!(automationElement.Current.ControlType.LocalizedControlType).Equals("Fenster"))// TODO: Root bestimmen
            {
               // TreeWalker walker = TreeWalker.ControlViewWalker;
                TreeWalker walker = TreeWalker.ContentViewWalker;
                AutomationElement elementParent = walker.GetParent(automationElement);
                addParentOfNode(elementParent, ref tree);
                filterParents(elementParent, ref  tree);                
            }
        }
        #endregion
        
        #region properties

        /// <summary>
        /// todo
        /// AutomationElement-properties to <see cref="OSMElement.GeneralProperties"/>
        /// Desweiteren stellt sich die frage, ob cached abgefragt wird, oder current, wegen geschwindigekti der abfrage
        /// </summary>
        /// <param name="automationElement">the AutomationElement</param>
        /// <returns>A <see cref="GeneralProperties"/> object with the properties of the AutomationELement</returns>
        private GeneralProperties setProperties(AutomationElement automationElement)
        {
            GeneralProperties elementP = new GeneralProperties();
            elementP.grantFilterStrategy = Settings.filterStrategyTypeToUserName(this.GetType());
            /*     elementP.grantFilterStrategiesChildren = new List<string>();
                 elementP.grantFilterStrategiesChildren.Add(elementP.grantFilterStrategy);*/
            try
            {
            elementP.acceleratorKeyFiltered = automationElement.Current.AcceleratorKey;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (Accelerator) '{0}'", a.ToString());
            }
            try
            {
            elementP.accessKeyFiltered = automationElement.Current.AccessKey;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (AccessKey) '{0}'", a.ToString());
            }
            try
            {
            elementP.autoamtionIdFiltered = automationElement.Current.AutomationId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (AutomationId) '{0}'", a.ToString());
            }

            try {
            if (!automationElement.Current.BoundingRectangle.IsEmpty) //Note: if BoundingRectangle == Empty =>There are problems reading the data from the XML file (XmlDeserialize)
                {
                elementP.boundingRectangleFiltered = automationElement.Current.BoundingRectangle;
                }
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (BoundingRectangle) '{0}'", a.ToString());
            }
            try {
            elementP.classNameFiltered = automationElement.Current.ClassName;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ClassName) '{0}'", a.ToString());
            }
            try {
                String[] t = automationElement.Current.ControlType.ProgrammaticName.Split(new String[]{"."}, StringSplitOptions.RemoveEmptyEntries);
                elementP.controlTypeFiltered = t[1];
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try {
            elementP.frameWorkIdFiltered = automationElement.Current.FrameworkId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (FrameworkId) '{0}'", a.ToString());
            }
            try {
            elementP.hasKeyboardFocusFiltered = automationElement.Current.HasKeyboardFocus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HasKeyboardFocus) '{0}'", a.ToString());
            }
            try {
            elementP.helpTextFiltered = automationElement.Current.HelpText;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (HelpText) '{0}'", a.ToString());
            }
            try {
            elementP.hWndFiltered = new IntPtr(automationElement.Current.NativeWindowHandle);
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (NativeWindowHandle) '{0}'", a.ToString());
            }
            try {
            elementP.isContentElementFiltered = automationElement.Current.IsContentElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsContentElement) '{0}'", a.ToString());
            }
            try {
            elementP.isControlElementFiltered = automationElement.Current.IsControlElement;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsControlElement) '{0}'", a.ToString());
            }
            try {
            elementP.isEnabledFiltered = automationElement.Current.IsEnabled;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsEnabled) '{0}'", a.ToString());
            }
            try {
            elementP.isKeyboardFocusableFiltered = automationElement.Current.IsKeyboardFocusable;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsKeyboardFocusable) '{0}'", a.ToString());
            }
            try {
            elementP.isOffscreenFiltered = automationElement.Current.IsOffscreen;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsOffscreen) '{0}'", a.ToString());
            }
            try {
            elementP.isPasswordFiltered = automationElement.Current.IsPassword;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsPassword) '{0}'", a.ToString());
            }
            try {
            elementP.isRequiredForFormFiltered = automationElement.Current.IsRequiredForForm;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (IsRequiredForForm) '{0}'", a.ToString());
            }
            try {
            elementP.itemStatusFiltered = automationElement.Current.ItemStatus;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemStatus) '{0}'", a.ToString());
            }
            try {
            elementP.itemTypeFiltered = automationElement.Current.ItemType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ItemType) '{0}'", a.ToString());
            }
            try
            {
                elementP.runtimeIDFiltered = automationElement.GetRuntimeId();
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (runtime) '{0}'", a.ToString());
            }
            try {
            elementP.localizedControlTypeFiltered = automationElement.Current.LocalizedControlType;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (LocalizedControlType) '{0}'", a.ToString());
            }
            try
            {
                elementP.nameFiltered = automationElement.Current.Name;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (Name) '{0}'", a.ToString());
            }
            try
            {
            elementP.processIdFiltered = automationElement.Current.ProcessId;
            }
            catch (Exception a)
            {
                Console.WriteLine("Property: (ProcessId) '{0}'", a.ToString());
            }
            setPropertiesOfPattern(ref elementP, automationElement);
            setSupportedPatterns(ref elementP, automationElement);
            return elementP;
        }

        /// <summary>
        /// Sets some pattern of the UI element
        /// </summary>
        /// <param name="properties">the properties of the element</param>
        /// <param name="automationElement">the AutomationElement</param>
        private void setPropertiesOfPattern(ref GeneralProperties properties, AutomationElement automationElement)
        {//https://msdn.microsoft.com/de-de/library/ms750574(v=vs.110).aspx

            object valuePattern = null;
            if (automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out valuePattern))
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
            if(automationElement.TryGetCurrentPattern(RangeValuePattern.Pattern, out rangeValuePattern))
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
            object togglePattern;
            if(automationElement.TryGetCurrentPattern(TogglePattern.Pattern, out togglePattern))
            {
                ToggleState state = (togglePattern as TogglePattern).Current.ToggleState;
                if (state == ToggleState.On) { properties.isToggleStateOn = true; }
                else
                {
                    if (state == ToggleState.Off) { properties.isToggleStateOn = false; }
                }
            }
        }

        /// <summary>
        /// Sets the supported pattern of the UI element
        /// </summary>
        /// <param name="properties">the properties of the element</param>
        /// <param name="automationElement">the AutomationElement</param>
        private void setSupportedPatterns(ref GeneralProperties properties, AutomationElement automationElement)
        {
            properties.suportedPatterns = automationElement.GetSupportedPatterns().Select(p => p.ProgrammaticName).ToArray();  //  automationElement.GetSupportedPatterns().ToArray();
        }

        /// <summary>
        /// Sets some "special" properties for the first node
        /// </summary>
        /// <param name="tree">the tree object</param>
        private void setSpecialPropertiesOfFirstNode(ref Object tree)
        {
            if (strategyMgr.getSpecifiedTree().HasChild(tree))
            {
              //  Settings settings = new Settings();
                OSMElement.OSMElement osm = new OSMElement.OSMElement();
                osm.brailleRepresentation = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).brailleRepresentation;
                osm.properties = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties;
             //   osm.properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(this.GetType());
                osm.properties.processName = strategyMgr.getSpecifiedOperationSystem().getProcessNameOfApplication(osm.properties.processIdFiltered);
                osm.properties.appPath = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByMainWindowTitle(osm.properties.processIdFiltered);
                strategyMgr.getSpecifiedTree().SetData(strategyMgr.getSpecifiedTree().Child(tree), osm);
            }
        }


        /// <summary>
        /// Creats the condition to search an AutomationElemente depending on <see cref="GeneralProperties"/> object
        /// </summary>
        /// <param name="properties">the properties to search</param>
        /// <returns>a Condition object</returns>
        private Condition setPropertiesCondition(GeneralProperties properties)
        {
            Condition resultCondition;
            #region Readable from all
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
            if (properties.labeledByFiltered != null)
            {
                resultCondition = new AndCondition(new PropertyCondition(AutomationElement.LabeledByProperty, properties.labeledByFiltered), resultCondition);
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

        #endregion

        /// <summary>
        /// Seeks new data for a node
        /// </summary>
        /// <param name="osmElementFilteredNode">OSM element to update</param>
        /// <returns>new properties for a node</returns>
        public GeneralProperties updateNodeContent(OSMElement.OSMElement osmElementFilteredNode)
        {
            GeneralProperties propertiesUpdated = new GeneralProperties();
            AutomationElement au = getAutomationelementOfOsmElement(osmElementFilteredNode);
            if (au != null)
            {
                propertiesUpdated = setProperties(au);
                propertiesUpdated.IdGenerated = osmElementFilteredNode.properties.IdGenerated;
            }
            return propertiesUpdated;
        }

        /// <summary>
        /// Seeks to an <see cref="OSMElement.OSMElement"/> the appropriate <see cref="AutomationElement"/>
        /// </summary>
        /// <param name="osmElement">OSM element</param>
        /// <returns>the appropriate <see cref="AutomationElement"</returns>
        private AutomationElement getAutomationelementOfOsmElement(OSMElement.OSMElement osmElement)
        {
            AutomationElement au;
            Condition cond = setPropertiesCondition(osmElement.properties);
            if (osmElement.properties.hWndFiltered != IntPtr.Zero)
            {
                //ist der Weg wirklich schneller?
                IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(osmElement.properties.hWndFiltered));
                AutomationElement mainWindowElement = deliverAutomationElementFromHWND(pointer);
                //au = mainWindowElement.FindFirst(TreeScope.Children, cond);
                if(mainWindowElement == null) { return null; }
                au = mainWindowElement.FindFirst(TreeScope.Subtree, cond); 
            }
            else
            {
                if (grantTrees.filteredTree != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.hWndFiltered != IntPtr.Zero)
                {
                    IntPtr hwnd = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.hWndFiltered;
                    //IntPtr pointer = strategyMgr.getSpecifiedOperationSystem().getProcessHwndFromHwnd(deliverElementID(strategyMgr.filteredTree.Child.Data.properties.hWndFiltered));
                    AutomationElement element = AutomationElement.FromHandle(hwnd);
                    au = element.FindFirst(TreeScope.Descendants, cond);
                }
                else
                {
                    au = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, cond); //Attention: it is possible that another element will be found
                }
            }
            return au;
        }

        /// <summary>
        /// todo bzw. nachfrage: geht die methode findall nicht alleine alle elemente der collection durch? wird in der foreach-schleife dadurch nicht doppelt gearbeitet?
        /// Sucht rekusiv alle Kindelemente eines Knotens und ermittelt dessen Eingenschaften
        /// </summary>
        /// <param name="top">teh current tree object</param>
        /// <param name="collection">gibt die AutomationElement-Collection an</param>
        /// <param name="depth">gibt an wie tief der Suche ausgehend vom Root-Element an.</param>
        private void findChildrenOfNode(Object top, AutomationElementCollection collection,  int depth)
        {
            #region set grantFilterStrategiesChildren
            if (collection != null && collection.Count > 0 && !strategyMgr.getSpecifiedTree().IsRoot(top))
            {
                OSMElement.OSMElement osmTop = strategyMgr.getSpecifiedTree().GetData(top);
                if (osmTop.properties.grantFilterStrategiesChildren == null)
                {
                    osmTop.properties.grantFilterStrategiesChildren = new List<string>();
                }
                if (!osmTop.properties.grantFilterStrategiesChildren.Contains(Settings.filterStrategyTypeToUserName(this.GetType())))
                {
                    osmTop.properties.grantFilterStrategiesChildren.Add(Settings.filterStrategyTypeToUserName(this.GetType()));
                }
            }
            #endregion
            foreach (AutomationElement element in collection)
            {
                if (strategyMgr.getSpecifiedTree().Depth(top) < depth || depth <= -1)
                {
                    OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
                    osmElement.properties = setProperties(element);
                    Object node = strategyMgr.getSpecifiedTree().AddChild(top, osmElement);

                    AutomationElementCollection c = element.FindAll(TreeScope.Children, Condition.TrueCondition);
                    findChildrenOfNode(node, c, depth);
                }
            }
        }

        /// <summary>
        /// Seeks the AutomationElement depending on a given <paramref name="hwnd"/>
        /// </summary>
        /// <param name="hwnd">the process handle of an element of a application</param>
        /// <returns>a AutomationElement</returns>
        internal static AutomationElement deliverAutomationElementFromHWND(IntPtr hwnd)
        {
            if(IntPtr.Zero.Equals(hwnd) || hwnd == null) { return null; }
            AutomationElement element;
            try
            {
                element = AutomationElement.FromHandle(hwnd);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new System.ComponentModel.Win32Exception("Access denied: Can't seeks the AutomationElement");
            }
            return element;
        }


        /// <summary>
        /// Seeks the process id of a given handle
        /// </summary>
        /// <param name="hwnd">the process handle of an element of a application</param>
        /// <returns>the process id</returns>
        public int deliverElementID(IntPtr hwnd)
        {
            //window = WindowFromPoint(cp);
            AutomationElement element;
            try{
                element =  AutomationElement.FromHandle(hwnd);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                throw new System.ComponentModel.Win32Exception("Access denied: Can't seeks the AutomationElement");
            }catch(ElementNotAvailableException e)
            {
                Debug.WriteLine("The element isn't available.");
                return -1;
            }
            int processIdentifier = (int)element.GetCurrentPropertyValue(AutomationElement.ProcessIdProperty);
            return processIdentifier;
        }

        private AutomationElement deliverAutomationElementFromCursor(int x, int y)
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
                throw new System.ComponentModel.Win32Exception("Access denied: Can't seeks the AutomationElement");
            }
            return element;
        }

        /// <summary>
        /// Seeks an OSM element to a given point
        /// </summary>
        /// <param name="pointX">x coordinate of the element to filtering</param>
        /// <param name="pointY">y coordinate of the element to filtering</param>
        /// <returns>the OSM element of the point</returns>
        public OSMElement.OSMElement getOSMElement(int pointX, int pointY)
        {            
            AutomationElement mouseElement = deliverAutomationElementFromCursor(pointX, pointY);
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(mouseElement);
            //set Id 
            List<Object> node = treeOperation.searchNodes.getNodesByProperties(grantTrees.filteredTree, osmElement.properties, OperatorEnum.and);
            if (node.Count == 1)
            {
                return strategyMgr.getSpecifiedTree().GetData(node[0]);
            }
            else
            {
                Debug.WriteLine("Can't find this element in the filtered tree!");
                return osmElement;
            }            
        }


        /// <summary>
        /// Adds a parent element in the tree.
        /// </summary>
        /// <param name="parentElement">the parent Automation element</param>
        /// <param name="tree">the tree</param>
        private void addParentOfNode(AutomationElement parentElement, ref Object tree)
        {
            Object tree2 = getStrategyMgr().getSpecifiedTree().NewTree();
            OSMElement.OSMElement osmElement = new OSMElement.OSMElement();
            osmElement.properties = setProperties(parentElement);
            #region set grantFilterStrategiesChildren
            if (tree != null)
            {
                OSMElement.OSMElement osmChild = strategyMgr.getSpecifiedTree().GetData(tree);
                if(osmChild !=null && !osmChild.Equals(new OSMElement.OSMElement()))
                {
                    if(osmChild.properties.grantFilterStrategiesChildren != null)
                    {
                        osmElement.properties.grantFilterStrategiesChildren = osmChild.properties.grantFilterStrategiesChildren;
                    }
                    else
                    {
                        osmElement.properties.grantFilterStrategiesChildren = new List<string>();
                        osmElement.properties.grantFilterStrategiesChildren.Add(osmChild.properties.grantFilterStrategy);
                    }
                }
            }
            #endregion
            Object node = strategyMgr.getSpecifiedTree().AddChild(tree2, osmElement);
            strategyMgr.getSpecifiedTree().AddChild(node, tree);
            strategyMgr.getSpecifiedTree().Clear(tree);
            strategyMgr.getSpecifiedTree().AddChild(tree, tree2);
        }
   }        
}