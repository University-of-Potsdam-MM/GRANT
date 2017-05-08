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
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;

        public SearchNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        /// <summary>
        /// depending on the given properties, all nodes with these properties are searched (depth-first search). 
        /// Only properties that have been specified are taken into account.
        /// </summary>
        /// <param name="tree">tree object for search </param>
        /// <param name="generalProperties">properties for the search</param>
        /// <param name="oper">Operator for combining the properties (and, or) </param>
        /// <returns>A list of the found tree objects</returns>
        public List<Object> searchProperties(Object tree, OSMElement.GeneralProperties generalProperties, OperatorEnum oper = OperatorEnum.and)
        {//TODO: many properties are still missing
            List<Object> result = new List<Object>();
            if (tree == null) { return result; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(tree))
            {
                Boolean propertieLocalizedControlType = generalProperties.localizedControlTypeFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.localizedControlTypeFiltered.Equals(generalProperties.localizedControlTypeFiltered);
                Boolean propertieName = generalProperties.nameFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.nameFiltered.Equals(generalProperties.nameFiltered);
                Boolean propertieIsEnabled = generalProperties.isEnabledFiltered == null || strategyMgr.getSpecifiedTree().GetData(node).properties.isEnabledFiltered == generalProperties.isEnabledFiltered;
                Boolean propertieBoundingRectangle = generalProperties.boundingRectangleFiltered == new System.Windows.Rect() || strategyMgr.getSpecifiedTree().GetData(node).properties.boundingRectangleFiltered.Equals(generalProperties.boundingRectangleFiltered);
                Boolean propertieIdGenerated = generalProperties.IdGenerated == null || generalProperties.IdGenerated.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                Boolean propertieAccessKey = generalProperties.accessKeyFiltered == null || generalProperties.accessKeyFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.accessKeyFiltered);
                Boolean acceleratorKey = generalProperties.acceleratorKeyFiltered == null || generalProperties.acceleratorKeyFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.acceleratorKeyFiltered);
                Boolean runtimeId = generalProperties.runtimeIDFiltered == null || Enumerable.SequenceEqual(generalProperties.runtimeIDFiltered, strategyMgr.getSpecifiedTree().GetData(node).properties.runtimeIDFiltered);
                Boolean automationId = generalProperties.autoamtionIdFiltered == null || generalProperties.autoamtionIdFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.autoamtionIdFiltered); //ist zumindest bei Skype für ein UI-Element nicht immer gleich
                Boolean controlType = generalProperties.controlTypeFiltered == null || generalProperties.controlTypeFiltered.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.controlTypeFiltered);
                if (OperatorEnum.Equals(oper, OperatorEnum.and))
                {
                    if (propertieBoundingRectangle && propertieLocalizedControlType && propertieIdGenerated && propertieAccessKey && acceleratorKey && runtimeId && controlType)
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
                        (generalProperties.accessKeyFiltered != null && propertieAccessKey)
                        )
                    {
                        result.Add(node);
                    }
                }
            }
            return result;
        }

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
        internal Object getNode(String idGenerated, Object tree)
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
        /// Gives a subtree with all screens of a specified view
        /// </summary>
        /// <param name="screenName">name of the screen</param>
        /// <returns>subtree object or <c>null</c></returns>
        public Object getSubtreeOfScreen(String screenName)
        {

            if (screenName == null || screenName.Equals("")) { return null; }
            Object tree = strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree);
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
        /// Gives the names of the existing screens
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

        /// <summary>
        /// Determines whether the view is existing in the screen
        /// </summary>
        /// <param name="screenName">name of the screen</param>
        /// <param name="viewName">name of the view</param>
        /// <param name="typeOfView">name of the type of view</param>
        /// <returns><c>true</c> if the screen existing, otherwise <c>false</c> </returns>
        public bool existViewInScreen(String screenName, String viewName, String typeOfView)
        {
            if (screenName == null || screenName.Equals("") || viewName == null || viewName.Equals("") || typeOfView == null || typeOfView.Equals("")) { return false; }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            brailleScreen.typeOfView = typeOfView;
            osmScreen.brailleRepresentation = brailleScreen;
            GeneralProperties prop = new GeneralProperties();
            prop.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);
            osmScreen.properties = prop;
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen)) { return false; }

            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return false; }
            foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
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
        /// Retuns all connected Braille nodes to a specified (filtered node) id
        /// </summary>
        /// <param name="idGeneratedFilteredNode">id of the node in the filtered tree</param>
        /// <returns>list with all connected braille nodes or <c>null</c></returns>
        public List<String> getConnectedBrailleTreenodeIds(String idGeneratedFilteredNode)
        {
            List<OsmConnector<String, String>> osmRelationships = grantTrees.osmRelationship.FindAll(r => r.FilteredTree.Equals(idGeneratedFilteredNode));
            if (osmRelationships != null)
            {
                List<String> result = new List<string>();
                foreach(OsmConnector<String, String> r in osmRelationships)
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
            OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(idGeneratedBrailleNode));
            if(osmRelationship != null) { return osmRelationship.FilteredTree; }else { return null; }
        }
    }
}
