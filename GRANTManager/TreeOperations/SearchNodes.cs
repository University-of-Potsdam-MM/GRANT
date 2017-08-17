using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GRANTManager.TreeOperations
{
    public class SearchNodes
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;

        public SearchNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        #region search by giving properties

        /// <summary>
        /// depending on the given properties, all nodes with these properties are searched (depth-first search). 
        /// Only properties that have been specified are taken into account.
        /// </summary>
        /// <param name="tree">tree object for search </param>
        /// <param name="generalProperties">properties for the search</param>
        /// <param name="oper">Operator for combining the properties (and, or) </param>
        /// <returns>A list of the found tree objects</returns>
        public List<Object> getNodesByProperties(Object tree, OSMElement.GeneralProperties generalProperties, OperatorEnum oper = OperatorEnum.and)
        {//TODO: many properties are still missing
            List<Object> result = new List<Object>();
            if (tree == null) { return result; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(node);
                Boolean propertieLocalizedControlType = generalProperties.localizedControlTypeFiltered == null || nodeData.properties.localizedControlTypeFiltered.Equals(generalProperties.localizedControlTypeFiltered);
                Boolean propertieName = generalProperties.nameFiltered == null || nodeData.properties.nameFiltered.Equals(generalProperties.nameFiltered);
                Boolean propertieIsEnabled = generalProperties.isEnabledFiltered == null || nodeData.properties.isEnabledFiltered == generalProperties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = generalProperties.boundingRectangleFiltered == new System.Windows.Rect() || nodeData.properties.boundingRectangleFiltered.Equals(generalProperties.boundingRectangleFiltered);
                Boolean propertieIdGenerated = generalProperties.IdGenerated == null || generalProperties.IdGenerated.Equals(nodeData.properties.IdGenerated);
                Boolean propertieAccessKey = generalProperties.accessKeyFiltered == null || generalProperties.accessKeyFiltered.Equals(nodeData.properties.accessKeyFiltered);
                Boolean acceleratorKey = generalProperties.acceleratorKeyFiltered == null || generalProperties.acceleratorKeyFiltered.Equals(nodeData.properties.acceleratorKeyFiltered);
                Boolean runtimeId = generalProperties.runtimeIDFiltered == null || Enumerable.SequenceEqual(generalProperties.runtimeIDFiltered, nodeData.properties.runtimeIDFiltered);
                Boolean automationId = generalProperties.autoamtionIdFiltered == null || generalProperties.autoamtionIdFiltered.Equals(nodeData.properties.autoamtionIdFiltered); //ist zumindest bei Skype für ein UI-Element nicht immer gleich
                Boolean controlType = generalProperties.controlTypeFiltered == null || generalProperties.controlTypeFiltered.Equals(nodeData.properties.controlTypeFiltered);
                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieLocalizedControlType && propertieIdGenerated && propertieAccessKey && acceleratorKey &&
                        runtimeId && controlType && propertieName && propertieIsEnabled)
                    {
                        result.Add(node);
                    }
                }
                if (OperatorEnum.Equals(oper, OperatorEnum.or))
                {//TODO: add properties
                    if ((generalProperties.localizedControlTypeFiltered != null && propertieLocalizedControlType) ||
                        (generalProperties.nameFiltered != null && propertieName) ||
                        (generalProperties.isEnabledFiltered != null && propertieIsEnabled) ||
                        (generalProperties.boundingRectangleFiltered != new System.Windows.Rect() && propertieBoundingRectangle) ||
                        (generalProperties.IdGenerated != null && propertieIdGenerated) ||
                        (generalProperties.accessKeyFiltered != null && propertieAccessKey) ||
                        (generalProperties.acceleratorKeyFiltered != null && acceleratorKey) ||
                        (generalProperties.runtimeIDFiltered != null && runtimeId) ||
                        (generalProperties.controlTypeFiltered != null && controlType)
                        )
                    {
                        result.Add(node);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// depending on the given properties, all nodes with these properties are searched (depth-first search). 
        /// Properties which are null or the default value is set are ignore
        /// </summary>
        /// <param name="tree">tree object for search </param>
        /// <param name="properties">properties for the search</param>
        /// <param name="oper">Operator for combining the properties (and, or) </param>
        /// <returns>A list of the found tree objects</returns>
        public List<Object> getNodesByProperties(Object tree, OSMElement.OSMElement properties, OperatorEnum oper = OperatorEnum.and)
        {
            List<Object> result = new List<Object>();
            if (tree == null) { return result; }
            List<String> listOfUsedProperties = OSMElement.OSMElement.getAllTypes();
            listOfUsedProperties = trimListOfProperties(listOfUsedProperties, properties);
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(node);
                Boolean isToAdd = true;
                foreach (String s in listOfUsedProperties)
                {
                    Boolean resultEquals = false;
                    var data_1 = OSMElement.OSMElement.getElement(s, nodeData);
                    var data_2 = OSMElement.OSMElement.getElement(s, properties);
                    if ((data_1 != null && data_1.Equals(data_2)) || (data_1 == null && data_2 == null))
                    {
                        resultEquals = true;
                    }
                    else
                    {
                        resultEquals = false;
                    }
                    
                    if (OperatorEnum.Equals(oper, OperatorEnum.and))
                    {
                        isToAdd = isToAdd && resultEquals;
                        if (!isToAdd) { break; }
                    }
                    else
                    {
                        if (OperatorEnum.Equals(oper, OperatorEnum.or))
                        {
                            isToAdd = isToAdd || resultEquals;
                        }
                    }
                }
                if (isToAdd)
                {
                    result.Add(node);
                }
            }
            return result;
        }

        private List<String> trimListOfProperties(List<String> listOfProperties, OSMElement.OSMElement osmValues)
        {
            List<String> trimmedList = new List<string>();
            foreach (String s in listOfProperties)
            {
                Type typeOfProperty;
                var data = OSMElement.OSMElement.getElement(s, osmValues, out typeOfProperty);

                if (data != null)
                {
                    var t = ExtObject.GetDefault(typeOfProperty);
                    if (t == null || !t.Equals(data))
                    {
                        trimmedList.Add(s);
                    }
                }
            }
            return trimmedList;
        }
        #endregion

        #region search by id

        /// <summary>
        /// searches for node with the given id in a tree object
        /// </summary>
        /// <param name="idGenereted">id of the searched node</param>
        /// <param name="tree"> Specifies the tree for the search </param>
        /// <returns>List of all found nodes</returns>
        public List<Object> getNodeList(String idGenereted, Object tree)
        {
            List<Object> result = new List<Object>();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                Boolean propertieIdGenerated = idGenereted == null || idGenereted.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                if (propertieIdGenerated)
                {
                    result.Add(node);
                }
            }
            return result;
        }

        /// <summary>
        /// In the filtered tree, searches for node with the given id
        /// </summary>
        /// <param name="idGenerated">id of the searched node</param>
        /// <returns><c>OSMElement.OSMElement</c> with the searched node</returns>
        public OSMElement.OSMElement getFilteredTreeOsmElementById(String idGenerated)
        {
            return getNodeElement(idGenerated, grantTrees.filteredTree);
        }

        /// <summary>
        /// In the braille tree, searches for node with the given id
        /// </summary>
        /// <param name="idGenerated">id of the searched node</param>
        /// <returns><c>OSMElement.OSMElement</c> with the searched node</returns>
        public OSMElement.OSMElement getBrailleTreeOsmElementById(String idGenerated)
        {
            return getNodeElement(idGenerated, grantTrees.brailleTree);
        }

        /// <summary>
        /// searches for node with the given id
        /// </summary>
        /// <param name="idGenerated">id of the searched node</param>
        /// <param name="tree">Specifies the tree for the search</param>
        /// <returns><c>OSMElement.OSMElement</c> with the searched node</returns>
        internal OSMElement.OSMElement getNodeElement(String idGenerated, Object tree)
        {
            object osmObject = getNode(idGenerated, tree);
            if(osmObject != null && !osmObject.Equals(new Object()))
            {
                return strategyMgr.getSpecifiedTree().GetData(osmObject);
            }
            else
            {
                return new OSMElement.OSMElement();
            }
        }

        /// <summary>
        /// searches for node with the given id
        /// </summary>
        /// <param name="idGenerated">id of the searched node</param>
        /// <param name="tree">Specifies the tree for the search </param>
        /// <returns>found tree object</returns>
        public Object getNode(String idGenerated, Object tree)
        {
            if (tree == null || idGenerated == null) { return null; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(idGenerated))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// searches a filtered node (in <c>grantTrees.filteredTree</c> ) by a given HWND
        /// </summary>
        /// <param name="hwnd">a hwnd of a node</param>
        /// <returns>the node object OR <c>null</c></returns>
        public Object getFilteredNodeByHwnd(IntPtr hwnd)
        {
            if (IntPtr.Zero.Equals(hwnd)) { return null; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                OSMElement.OSMElement osmData = strategyMgr.getSpecifiedTree().GetData(node);
                if (hwnd.Equals(osmData.properties.hWndFiltered))
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// searches a filtered node ID (in <c>grantTrees.filteredTree</c> ) by a given HWND
        /// </summary>
        /// <param name="hwnd">a hwnd of a node</param>
        /// <returns>the node ID OR <c>null</c></returns>
        public String getIdFilteredNodeByHwnd(IntPtr hwnd)
        {
            Object node = getFilteredNodeByHwnd(hwnd);
            if(node == null) { return null; }
            OSMElement.OSMElement osmData = strategyMgr.getSpecifiedTree().GetData(node);
            return osmData.properties.IdGenerated;
        }
        #endregion

        #region connected nodes (Filtered tree + braille tree)

        /// <summary>
        /// Retuns all connected Braille nodes to a specified (filtered node) id
        /// </summary>
        /// <param name="idGeneratedFilteredNode">id of the node in the filtered tree</param>
        /// <returns>list with all connected braille nodes or <c>null</c></returns>
        public List<String> getConnectedBrailleTreenodeIds(String idGeneratedFilteredNode)
        {
            List<OsmTreeConnectorTuple<String, String>> osmRelationships = grantTrees.osmTreeConnections.FindAll(r => r.FilteredTree.Equals(idGeneratedFilteredNode));
            if (osmRelationships != null)
            {
                List<String> result = new List<string>();
                foreach (OsmTreeConnectorTuple<String, String> r in osmRelationships)
                {
                    result.Add(r.BrailleTree);
                }
                return result;
            }
            else { return null; }
        }

        /// <summary>
        /// Retuns the connected filtered node to a specified (braille node) id
        /// </summary>
        /// <param name="idGeneratedBrailleNode">id of the node in the  braille tree</param>
        /// <returns>id of the connected filtered tree node or <c>null</c></returns>
        public String getConnectedFilteredTreenodeId(String idGeneratedBrailleNode)
        {
            OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(idGeneratedBrailleNode));
            if (osmRelationship != null) { return osmRelationship.FilteredTree; } else { return null; }
        }
        #endregion

        #region subtree

        /// <summary>
        /// Gives a subtree of a specified screen
        /// </summary>
        /// <param name="screenName">name of the screen</param>
        /// <returns>subtree object or <c>null</c></returns>
        public Object getSubtreeOfScreen(String screenName)
        {
            if (screenName == null || screenName.Equals("")) { return null; }
            if (grantTrees.brailleTree == null ) { return null; }

            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                foreach(Object screenNodes in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                {
                    if (strategyMgr.getSpecifiedTree().GetData(screenNodes).brailleRepresentation.screenName.Equals(screenName))
                    {
                        return screenNodes;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gives a subtree of a specified typeOfView
        /// </summary>
        /// <param name="typeOfViewName">name of the typeOfView</param>
        /// <returns>subtree object or <c>null</c></returns>
        public Object getSubtreeOfTypeOfView(String typeOfViewName)
        {

            if (typeOfViewName == null || typeOfViewName.Equals("")) { return null; }
            Object tree = strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree);
            if (grantTrees.brailleTree == null) { return null; }

            foreach (Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView.Equals(typeOfViewName))
                {
                    return vC;
                }
            }
            return null;
        }

        private Object getSubtreeOfView(String viewName, Object subtree)
        {
            //TODO. hier sollten auch die direkten Kinder von subtree reichen durchzugehen
            if (subtree == null) { return null; }
            if (strategyMgr.getSpecifiedTree().HasChild(subtree))
            {
                subtree = strategyMgr.getSpecifiedTree().Child(subtree);
                if (strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            else { return null; }
            while (strategyMgr.getSpecifiedTree().HasNext(subtree))
            {
                subtree = strategyMgr.getSpecifiedTree().Next(subtree);
                if (strategyMgr.getSpecifiedTree().GetData(subtree).brailleRepresentation.viewName.Equals(viewName))
                {
                    return subtree;
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Gives the names of all existing screens
        /// </summary>
        /// <param name="typeOfView">the type of view (<see cref="Settings.getPossibleTypesOfViews"/></param>
        /// <returns>List with all possible name of screens,
        /// if <para>typeOfView</para> specified, only screens from this <para>typeOfView</para> are returned</returns>
        public List<String> getPosibleScreenNames(String typeOfView = null)
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return null; }
            List<String> screens = new List<string>();
            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if(typeOfView == null || typeOfView.Equals(strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView))
                {
                    foreach(Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                    {
                        screens.Add(strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName);
                    }
                }
            }
            return screens;
        }

        /// <summary>
        /// Determinates the screen name to a given screen id
        /// </summary>
        /// <param name="screenId">the id of the screen</param>
        /// <returns>Returns the screen name OR <c>null</c></returns>
        public String getScreenIdToScreenName(String screenId)
        {
            if(screenId == null) { return null; }
            OSMElement.OSMElement osm = getBrailleTreeOsmElementById(screenId);
            if(osm == null || osm.Equals(new OSMElement.OSMElement()) || osm.brailleRepresentation == null) { return null; }
            return osm.brailleRepresentation.screenName;
        }

        /// <summary>
        /// A List of used typeOfViews (cf. <see cref="GRANTManager.Settings.getPossibleTypesOfViews"/>)
        /// </summary>
        /// <returns> A List of used typeOfViews</returns>
        public List<String> getUsedTypesOfViews()
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return null; }
            List<String> viewCategories = new List<string>();
            foreach (Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                viewCategories.Add(strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView);
            }
            return viewCategories;
        }

        /// <summary>
        /// Calculetes a node (in the braille tree) to a given point 
        /// it will be searched the child node of a "group node" 
        /// </summary>
        /// <param name="pointX">x coordinat of the point</param>
        /// <param name="pointY">y coordinat of the point</param>
        /// <param name="groupViewName">the name of the "group view"</param>
        /// <param name="offsetX">x offset  of the group view </param>
        /// <param name="offsetY">y offset  of the group view </param>
        /// <returns>the searched braille node or <c>null</c></returns>
        public Object getTreeElementOfViewAtPoint(int pointX, int pointY, String groupViewName, int offsetX, int offsetY)
        {
            if (grantTrees == null || groupViewName == null || groupViewName.Equals("")) { return null; }
            String visibleScreen = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreen();
            Object screenTree = getSubtreeOfScreen(visibleScreen);
            screenTree = getSubtreeOfView(groupViewName, screenTree);
            if (screenTree == null || screenTree.Equals(strategyMgr.getSpecifiedTree().NewTree())) { return null; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(screenTree))
            {
                Rect rect = strategyMgr.getSpecifiedTree().GetData(node).properties.boundingRectangleFiltered;
                if (rect.Contains(pointX - offsetX, pointY - offsetY))
                {
                    return node;
                }
            }
            return screenTree;
        }

        /// <summary>
        /// Calculates for every screen the node with the navigationbar 
        /// </summary>
        /// <returns></returns>
        public List<Object> getListOfNavigationbars()
        {
            List<Object> result = new List<Object>();
            // ITreeStrategy<OSMElement.OSMElement> brailleTree = grantTrees.brailleTree;
            if (grantTrees.brailleTree == null) { return result; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName != null && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Contains(Settings.getNavigationbarSubstring()) && strategyMgr.getSpecifiedTree().HasChild(node))
                {
                    result.Add(node);
                }
            }
            return result;
        }

        #region exist: view, screen, typeOfView
        
        internal bool existViewInTree(Object subtree, String viewName)
        {
            if (viewName == null || viewName.Equals("")) { return false; }
            foreach (Object o in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtree))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                if (viewName.Equals(data.brailleRepresentation.viewName))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool existViewInTree(Object subtree, String viewName, out Object view)
        {
            view = null;
            if (viewName == null || viewName.Equals("")) { return false; }
            foreach (Object o in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtree))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                if (viewName.Equals(data.brailleRepresentation.viewName))
                {
                    view = o;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the view is existing in the screen
        /// </summary>
        /// <param name="screenName">name of the screen</param>
        /// <param name="viewName">name of the view</param>
        /// <param name="typeOfView">name of the type of view</param>
        /// <returns><c>true</c> if the view existing, otherwise <c>false</c> </returns>
        public bool existViewInScreen(String screenName, String viewName, String typeOfView)
        {
            if (screenName == null || screenName.Equals("") || viewName == null || viewName.Equals("") || typeOfView == null || typeOfView.Equals("")) { return false; }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            osmScreen.brailleRepresentation = new BrailleRepresentation();
            osmScreen.brailleRepresentation.screenName = screenName;
            osmScreen.brailleRepresentation.typeOfView = typeOfView;
            osmScreen.properties = new GeneralProperties();
            osmScreen.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);
            //osmScreen.properties = prop;
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen)) { return false; }

            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return false; }
            foreach (Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView.Equals(typeOfView))
                {
                    foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(vC))
                    {
                        OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(node);
                        if (nodeData.brailleRepresentation.screenName != null && nodeData.brailleRepresentation.screenName.Equals(screenName))
                        {
                            foreach (Object childScreen in strategyMgr.getSpecifiedTree().AllChildrenNodes(node))
                            {
                                OSMElement.OSMElement childScreenData = strategyMgr.getSpecifiedTree().GetData(childScreen);
                                if (childScreenData.brailleRepresentation.viewName != null && childScreenData.brailleRepresentation.viewName.Equals(viewName))
                                {
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the screen is existing
        /// </summary>
        /// <param name="screenName">the name of the screen</param>
        /// <returns><c>true</c> if the screen existing, otherwise <c>false</c> </returns>
        internal bool existScreenInTree(String screenName)
        {
            object screen;
            return existScreenInTree(screenName, out screen);
        }

        internal bool existScreenInTree( String screenName, out Object screen)
        {
            screen = null;
            if (screenName == null || screenName.Equals("")) { return false; }
            foreach (Object typOfView in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            { //typeOfView-Branch
                foreach (Object screenBranch in strategyMgr.getSpecifiedTree().DirectChildrenNodes(typOfView))
                { // screen-Branch
                    OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(screenBranch);
                    if (screenName.Equals(data.brailleRepresentation.screenName))
                    {
                        screen = screenBranch;
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool existTypeOfViewInTree(String typeOfViewName)
        {
            Object typeOfView;
            return existTypeOfViewInTree(typeOfViewName, out typeOfView);
        }

        internal bool existTypeOfViewInTree(String typeOfViewName, out Object typeOfView)
        {
            typeOfView = null;
            if (typeOfViewName == null || typeOfViewName.Equals("")) { return false; }
            foreach (Object o in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                if (typeOfViewName.Equals(data.brailleRepresentation.typeOfView))
                {
                    typeOfView = o;
                    return true;
                }
            }
            return false;

        }




        #endregion

        public static Boolean existPropertyName(String propertyName)
        {
            List<String> propNames = OSMElement.OSMElement.getAllTypes();
            return propNames.Contains(propertyName);
        }

        /// <summary>
        /// Returns the main filter strategy for filtering nodes
        /// </summary>
        public String getMainFilterstrategyOfTree()
        {
            if (grantTrees == null || grantTrees.filteredTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree))
            {
                Debug.WriteLine("Can't find a filter strategy in this tree!");
                return null;
            }
            return Settings.strategyUserNameToClassName(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.grantFilterStrategy);
        }
    }
}
