using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;

namespace GRANTManager.TreeOperations
{
    /// <summary>
    /// Methods for updating nodes in the trees
    /// </summary>
    public class UpdateNodes
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperation;

        public UpdateNodes(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        /// <summary>
        /// Updates a node of the filtered tree;
        /// It is possible that the filter strategy changes temporarily
        /// </summary>
        /// <param name="generatedId">the id of the node in the filtered tree</param>
        public void updateNodeOfFilteredTree(String generatedId)
        {
            bool changeFilter = false;
            OSMElement.OSMElement osmOfNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedId);
            if (osmOfNode.Equals(new OSMElement.OSMElement())) { return; }
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.filteredTree, grantTrees.filterstrategiesOfNodes, strategyMgr.getSpecifiedTree());
            FilterstrategyOfNode<String, String, String> nodeFilterstrategy = FilterstrategiesOfTree.getFilterstrategyOfNode(generatedId, grantTrees.filterstrategiesOfNodes);
            // if necessary changes the filter strategy
            if (nodeFilterstrategy != null && !mainFilterstrategy.Equals(nodeFilterstrategy))
            {
                changeFilter = true;
                strategyMgr.setSpecifiedFilter(nodeFilterstrategy.FilterstrategyFullName + ", " + nodeFilterstrategy.FilterstrategyDll);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            }
            // filters and updates the node
            OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(osmOfNode);
            changePropertiesOfFilteredNode(properties);

            if (nodeFilterstrategy != null && changeFilter)
            {
                //resets the filter
                strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName + ", " + mainFilterstrategy.FilterstrategyDll);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            }
        }

        /// <summary>
        /// Filters a node with the current filter strategy,
        /// thus, the filtering strategy of a node can be changed
        /// </summary>
        /// <param name="filteredTreeGeneratedId">the id of the node in the filtered tree</param>
        public void filterNodeWithNewStrategy(String filteredTreeGeneratedId)
        {
            OSMElement.OSMElement relatedFilteredTreeObject = treeOperation.searchNodes.getFilteredTreeOsmElementById(filteredTreeGeneratedId);
            if (relatedFilteredTreeObject.Equals(new OSMElement.OSMElement())) { return; }
            //checks whether the node sould be filtered with the "standard" filter --> temporary chnages the filter 
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.filteredTree, grantTrees.filterstrategiesOfNodes, strategyMgr.getSpecifiedTree());
            OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(relatedFilteredTreeObject);

            if (!(mainFilterstrategy.FilterstrategyFullName.Equals(strategyMgr.getSpecifiedFilter().GetType().FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyMgr.getSpecifiedFilter().GetType().Namespace)))
            {
                List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.filterstrategiesOfNodes;
                FilterstrategiesOfTree.addFilterstrategyOfNode(filteredTreeGeneratedId, strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                Settings settings = new Settings();
                properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyMgr.getSpecifiedFilter().GetType());
            }
            else
            {
                if (FilterstrategiesOfTree.getFilterstrategyOfNode(filteredTreeGeneratedId, grantTrees.filterstrategiesOfNodes) != null)
                {
                    List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.filterstrategiesOfNodes;
                    bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(filteredTreeGeneratedId, strategyMgr.getSpecifiedFilter().GetType(), ref filterstrategies);
                    if (isRemoved)
                    {
                        properties.grantFilterStrategy = "";
                    }
                }
            }
            changePropertiesOfFilteredNode(properties);
        }

        /// <summary>
        /// Compares and changes the path of an application if necessary
        /// </summary>
        public void changeFilePath()
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( grantTrees.filteredTree)).properties.Equals(new GeneralProperties())) { return; }
            String fileNameNew = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByModulName(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.processName);
            GeneralProperties child = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties;
            if (child.appPath != null  && !child.appPath.Equals(fileNameNew))
            {
                changeFilePath(fileNameNew);
            }
        }

        /// <summary>
        /// changes the path of an application
        /// </summary>
        /// <param name="fileNameNew">the new path of the application</param>
        public void changeFilePath(String fileNameNew)
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.Equals(new GeneralProperties())) { return; }
            GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties;
            properties.appPath = fileNameNew;
            changePropertiesOfFilteredNode(properties);
        }

        /// <summary>
        /// Changes the <see cref="GeneralProperties"/> of a node
        /// </summary>
        /// <param name="properties">the new properties</param>
        /// <param name="idGeneratedOld">the old id</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties, String idGeneratedOld = null)
        {
            // List<Object> node = treeOperation.searchNodes.searchNodeByProperties(grantTrees.filteredTree, properties);
            Object node = treeOperation.searchNodes.getNode(properties.IdGenerated, grantTrees.filteredTree);
            if (node != null)
            {
                OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                osm.properties = properties;
                if(idGeneratedOld != null)
                {
                    osm.properties.IdGenerated = idGeneratedOld;
                }
                strategyMgr.getSpecifiedTree().SetData(node, osm);
                return;
            }
            Debug.WriteLine("Can't find the node!");
        }

        /// <summary>
        /// Sets the new braille representaion of the element
        /// </summary>
        /// <param name="osmElement">the new representation</param>
        public void setBrailleTreeOsmElement(OSMElement.OSMElement osmElement)
        {
            changeBrailleRepresentation(ref osmElement);
        }

        /// <summary>
        /// Sets/Changes a property of a node in the braille tree
        /// </summary>
        /// <param name="id">the id of the node to change his property</param>
        /// <param name="nameOfProperty">name of the property (<see cref="OSMElement.OSMElement.getAllTypes()"/>)</param>
        /// <param name="newProperty">the new value</param>
        /// <returns><c>true</c> if the property sets, otherwiese <c>false</c></returns>
        public bool setBrailleTreeProperty(String id, String nameOfProperty, Object newProperty)
        {
            if (nameOfProperty == null || !OSMElement.OSMElement.getAllTypes().Contains(nameOfProperty) || id == null || grantTrees.brailleTree == null)
            {
                return false;
            }
            OSMElement.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(id);
            if (node == new OSMElement.OSMElement()) { return false; }
            Object propertyValueCurrent = OSMElement.OSMElement.getElement(nameOfProperty, node);
            if(nameOfProperty.Equals("typeOfView"))
            {
                return changeTypeOfView(id, newProperty as String);
            }
            if (nameOfProperty.Equals("screenName"))
            {
            return changeScreenName(id, newProperty as String);
            }
            if (nameOfProperty.Equals("viewName"))
            {
                Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
                Object parent = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                if(existViewInTree(parent, newProperty as String))
                {
                    return false;
                }
            }
            // https://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
            OSMElement.OSMElement.setElement(nameOfProperty, newProperty, node);
            Object propertyValueNew = OSMElement.OSMElement.getElement(nameOfProperty, node);
            if (propertyValueCurrent == null && propertyValueNew == null) { return false; }
            if (propertyValueCurrent != null && propertyValueCurrent.Equals(propertyValueNew)) { return false; }
            else
            {
                if ((propertyValueNew == null && newProperty == null) || propertyValueNew.ToString().Equals(newProperty.ToString())) { return true; }
            }
            return false;
        }

        private bool changeTypeOfView(String id, String typeOfViewNew)
        {
            Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
            OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            //checks the position of the node
            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject))
            {
                // the node is a "root" branch of a type of view
                if (!existTypeOfViewInTree(grantTrees.brailleTree, typeOfViewNew))
                {
                    //rename this node and all children
                    foreach (Object o in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                        data.brailleRepresentation.typeOfView = typeOfViewNew;
                    }
                    return true;
                }
                else { return false; }
            }
            else
            {
                Object existTypeOfView;
                if (!existTypeOfViewInTree(grantTrees.brailleTree, typeOfViewNew, out existTypeOfView))
                {
                    //add typeOfView, rename & move
                    addScreenInBrailleTree(nodeData.brailleRepresentation.screenName, typeOfViewNew);
                    existTypeOfViewInTree(grantTrees.brailleTree, typeOfViewNew, out existTypeOfView); // now the typeOfView node exists
                }
                Object existScreen;
                if (!existScreenInTree(existTypeOfView, nodeData.brailleRepresentation.screenName, out existScreen) && nodeData.brailleRepresentation.viewName == null)
                {
                    // viewName == null
                    // screen dosn't exist in the 'new' typeOfView => rename & move
                    foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(node);
                        data.brailleRepresentation.typeOfView = typeOfViewNew;
                    }
                    strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, existTypeOfView);
                    return true;
                }
                Object existView = null;
                if (nodeData.brailleRepresentation.viewName != null && !existViewInTree(existTypeOfView, nodeData.brailleRepresentation.viewName, out existView) && existScreen == null)
                {
                    // viewName != null
                    // screen dosn't exist in the 'new' typeOfView => rename & move
                    nodeData.brailleRepresentation.typeOfView = typeOfViewNew;
                    Object removeScreenOfView = null;
                    if (!strategyMgr.getSpecifiedTree().HasPrevious(nodeObject) && !strategyMgr.getSpecifiedTree().HasNext(nodeObject))
                    {
                        removeScreenOfView = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                    }
                    //Add screen
                    Object screenNew;
                    addScreenInBrailleTree(nodeData.brailleRepresentation.screenName, typeOfViewNew, out screenNew);
                    strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, screenNew);
                    //remove the screen-node if no sisters exists
                    if (removeScreenOfView != null)
                    {
                        strategyMgr.getSpecifiedTree().Remove(removeScreenOfView);
                    }
                    return true;
                }
                if (existScreen != null && nodeData.brailleRepresentation.viewName != null && existView == null)
                {
                    // typeOfView + Screen exist BUT view doesn't exist
                    // rename & move
                    nodeData.brailleRepresentation.typeOfView = typeOfViewNew;
                    Boolean b = strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, existScreen);
                    return true;
                }
                if (existScreen != null && nodeData.brailleRepresentation.viewName == null) 
                {
                    // checks whether no view (of the screen-branch) exist in the new screen
                    foreach (object v in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElement.OSMElement viewData = strategyMgr.getSpecifiedTree().GetData(v);
                        if (treeOperation.searchNodes.existViewInScreen(viewData.brailleRepresentation.screenName, viewData.brailleRepresentation.viewName, typeOfViewNew))
                        {
                            return false;
                        }
                    }

                    // typeOfView + Screen exist BUT the viewS doesn't exist
                    foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(node);
                        data.brailleRepresentation.typeOfView = typeOfViewNew;
                    }
                    while (strategyMgr.getSpecifiedTree().HasChild(nodeObject))
                    {
                        strategyMgr.getSpecifiedTree().moveSubtree(strategyMgr.getSpecifiedTree().Child(nodeObject), existScreen);
                    }
                    if (!strategyMgr.getSpecifiedTree().HasPrevious(nodeObject) && !strategyMgr.getSpecifiedTree().HasNext(nodeObject) && strategyMgr.getSpecifiedTree().HasParent(nodeObject))
                    {
                        strategyMgr.getSpecifiedTree().Remove(strategyMgr.getSpecifiedTree().Parent(nodeObject));
                    }
                    else
                    {
                        strategyMgr.getSpecifiedTree().Remove(nodeObject);
                    }
                    return true;
                }
            }
            return false;
        }

        private bool changeScreenName(String id, String screenNameNew)
        {
            Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
            OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            //checks the position of the node
            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject)) { return false; }
            Object parent = strategyMgr.getSpecifiedTree().Parent(nodeObject);
            if (nodeData.brailleRepresentation != null && nodeData.brailleRepresentation.viewName == null && nodeData.brailleRepresentation.isVisible == false)
            {
                Object existScreen;
                // => 'root' of a screen branch => rename all children
                if (existScreenInTree(parent, screenNameNew, out existScreen))
                {
                    foreach (Object view in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(view);
                        if (treeOperation.searchNodes.existViewInScreen(screenNameNew, data.brailleRepresentation.viewName, data.brailleRepresentation.typeOfView))
                        {
                            return false;
                        }
                    }
                    //  Screen exist BUT the viewS doesn't exist
                    //rename + move every view
                    foreach (Object view in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(view);
                        data.brailleRepresentation.screenName = screenNameNew;
                    }
                    while (strategyMgr.getSpecifiedTree().HasChild(nodeObject))
                    {
                        strategyMgr.getSpecifiedTree().moveSubtree(strategyMgr.getSpecifiedTree().Child(nodeObject), existScreen);
                    }
                    if (!strategyMgr.getSpecifiedTree().HasPrevious(nodeObject) && !strategyMgr.getSpecifiedTree().HasNext(nodeObject) && strategyMgr.getSpecifiedTree().HasParent(nodeObject))
                    {
                        strategyMgr.getSpecifiedTree().Remove(strategyMgr.getSpecifiedTree().Parent(nodeObject));
                    }
                    else
                    {
                        strategyMgr.getSpecifiedTree().Remove(nodeObject);
                    }
                    return true;
                }
                foreach (Object o in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                {
                    OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                    data.brailleRepresentation.screenName = screenNameNew;
                }
                return true;
            }
            // it's a child node of a (screen) branch
            Object existScreenWitheName;
            if (!strategyMgr.getSpecifiedTree().HasParent(parent)) { return false; }
            Object parentsParent = strategyMgr.getSpecifiedTree().Parent(parent);
            if (existScreenInTree(parentsParent, screenNameNew, out existScreenWitheName))
            {
                if(existViewInTree(existScreenWitheName, nodeData.brailleRepresentation.viewName))
                {
                    return false;
                }else
                {
                    //rename & move
                    nodeData.brailleRepresentation.screenName = screenNameNew;
                 return   strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, existScreenWitheName);
                }
            }
            else
            {
                // rename, create screen, move view
                nodeData.brailleRepresentation.screenName = screenNameNew;
                Object subtreeNewScreen;
                addScreenInBrailleTree(screenNameNew, nodeData.brailleRepresentation.typeOfView, out subtreeNewScreen);
                if(subtreeNewScreen == null)
                {
                    Debug.WriteLine("The new screen couldn't be created.");
                    return false;
                }
                return strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, subtreeNewScreen);
            }
        }

        private bool existViewInTree(Object subtree, String viewName)
        {
            if(viewName == null || viewName.Equals("")) { return false; }
            foreach(Object o  in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtree))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                if(viewName.Equals(data.brailleRepresentation.viewName))
                {
                    return true;
                }
            }
            return false;
        }

        private bool existViewInTree(Object subtree, String viewName, out Object view)
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

        private bool existScreenInTree(Object subtree, String screenName)
        {
            object screen;
            return existScreenInTree(subtree, screenName, out screen);
        }

        private bool existScreenInTree(Object subtree, String screenName, out Object screen)
        {
            screen = null;
            if (screenName == null || screenName.Equals("")) { return false; }
            foreach (Object o in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtree))
            {
                OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                if (screenName.Equals(data.brailleRepresentation.screenName))
                {
                    screen = o;
                    return true;
                }
            }
            return false;
        }

        private bool existTypeOfViewInTree(Object tree, String typeOfViewName)
        {
            Object typeOfView;
            return existTypeOfViewInTree(tree, typeOfViewName, out typeOfView);
        }

        private bool existTypeOfViewInTree(Object tree, String typeOfViewName, out Object typeOfView)
        {
            typeOfView = null;
            if(typeOfViewName == null || typeOfViewName.Equals("")) { return false; }
            foreach (Object o in strategyMgr.getSpecifiedTree().DirectChildrenNodes(tree))
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

        /// <summary>
        /// Value of a given Property
        /// </summary>
        /// <param name="elementName">the name of the property (<see cref="OSMElement.OSMElement.getAllTypes()"/>)</param>
        /// <param name="osmElement"></param>
        /// <returns>Value of a given Property; if <c>value == null</c> the result will be an empty String</returns>
        public static String getProperty(String elementName, OSMElement.OSMElement osmElement)
        {
            Object value = OSMElement.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }

        /// <summary>
        /// Value of a given Property
        /// </summary>
        /// <param name="elementName">the name of the property (<see cref="OSMElement.OSMElement.getAllTypes()"/>)</param>
        /// <param name="idOsmElement">Id of the OSM element</param>
        /// <returns>Value of a given Property; if the id dosen't exist or <c>value == null</c> the result will be an empty String</returns>
        public String getPropertyofBrailleTree(String elementName, String idOsmElement)
        {
            OSMElement.OSMElement osmElement = treeOperation.searchNodes.getBrailleTreeOsmElementById(idOsmElement);
            if (osmElement == new OSMElement.OSMElement()) { return ""; }
            Object value = OSMElement.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }

        /// <summary>
        /// Value of a given Property
        /// </summary>
        /// <param name="elementName">the name of the property (<see cref="OSMElement.OSMElement.getAllTypes()"/>)</param>
        /// <param name="idOsmElement">Id of the OSM element</param>
        /// <returns>Value of a given Property; if the id dosen't exist or <c>value == null</c> the result will be an empty String</returns>
        public String getPropertyofFilteredTree(String elementName, String idOsmElement)
        {
            OSMElement.OSMElement osmElement = treeOperation.searchNodes.getFilteredTreeOsmElementById(idOsmElement);
            if (osmElement == new OSMElement.OSMElement()) { return ""; }
            Object value = OSMElement.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }


        /// <summary>
        /// Sets the new braille representaion of the element
        /// </summary>
        /// <param name="element">the new representation</param>
        private void changeBrailleRepresentation(ref OSMElement.OSMElement element)
        {
            Object brailleTree = grantTrees.brailleTree;
            if(brailleTree == null) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(element.properties.IdGenerated))
                {
                    strategyMgr.getSpecifiedTree().SetData(node, element);
                    break;
                }
            }
        }

        /// <summary>
        /// Changes the properties of the braille node
        /// </summary>
        /// <param name="element">the node which sould be updated</param>
        public void updateNodeOfBrailleUi(ref OSMElement.OSMElement element)
        {            
            if (element.brailleRepresentation.isGroupChild && element.brailleRepresentation.groupelementsOfSameType.renderer != null)
            {
                element.properties.controlTypeFiltered = element.brailleRepresentation.groupelementsOfSameType.renderer; //TODO: passt das so?
            }
            if (!strategyMgr.getSpecifiedBrailleDisplay().getAllUiElementRenderer().Contains(element.properties.controlTypeFiltered))
            {
                Debug.WriteLine("Attention: The chosen renderer dosn't exist. Now the renderer is 'Text'.");
                element.properties.controlTypeFiltered = "Text";
            }
            if (element.brailleRepresentation.displayedGuiElementType != null)
            {
                element.properties.valueFiltered = getTextForView(element);
            }
            if (element.brailleRepresentation.uiElementSpecialContent != null && element.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                ListMenuItem listMenuItem = (ListMenuItem)element.brailleRepresentation.uiElementSpecialContent;
                element.properties.isToggleStateOn = isCheckboxOfAssociatedElementSelected(element);
            }            
            bool? isEnable = isUiElementEnable(element);
            if (isEnable != null)
            {
                element.properties.isEnabledFiltered = (bool)isEnable;
            }
            if (element.brailleRepresentation.displayedGuiElementType != null && element.brailleRepresentation.displayedGuiElementType != "" && !GeneralProperties.getAllTypes().Contains(element.brailleRepresentation.displayedGuiElementType))
            {
                Debug.WriteLine("Attention: The chosen property for 'displayedGuiElementType' dosn't exist. Therefore, 'nameFiltered' is set.");
                element.brailleRepresentation.displayedGuiElementType = "nameFiltered";
            }
            
            changeBrailleRepresentation(ref element);// here is the element already changed
        }

        /// <summary>
        /// Determines whether connected checkbox of a braille node is selected 
        /// </summary>
        /// <param name="element">the OSM element of the braille tree</param>
        /// <returns><c>true</c> if the checkbox in the connected filtered node selected</returns>
        private bool isCheckboxOfAssociatedElementSelected(OSMElement.OSMElement element)
        {
            OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(element.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Debug.WriteLine("No matching object found.");
                return false;
            }
            List<Object> associatedNodeList = treeOperation.searchNodes.getNodeList(osmRelationship.FilteredTree, grantTrees.filteredTree);
            if (associatedNodeList != null && associatedNodeList.Count == 1 && strategyMgr.getSpecifiedTree().HasChild(associatedNodeList[0]))
            {
                //the information wether an element is selected will be found in the child element (at least in my example application)
                return strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn != null ? (bool)strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn : false;
            }
            return false;
        }


        /// <summary>
        /// Seeks the displayed text for a (braille) view;
        /// exists an acronym for this text this will be used
        /// </summary>
        /// <param name="filteredSubtree">gibt das OSM-Element des anzuzeigenden GUI-Elementes an</param>
        /// <returns>the text to display</returns>
        private String getTextForView(OSMElement.OSMElement osmElement)
        {
            OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("No matching object found.");
                return "";
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
            String text = "";
            if (!associatedNode.Equals(new OSMElement.OSMElement()) && !osmElement.brailleRepresentation.displayedGuiElementType.Trim().Equals(""))
            {
                object objectText = GeneralProperties.getPropertyElement(osmElement.brailleRepresentation.displayedGuiElementType, associatedNode.properties);
                text = (objectText != null ? objectText.ToString() : "");
            }
            return useAcronymForText( text);
        }

        /// <summary>
        /// if exists, replaces the text by an acronym 
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns> the acronym of the text or the text </returns>
        public String useAcronymForText(String text)
        {
            if (grantTrees.TextviewObject.Equals(new TextviewElement()) || grantTrees.TextviewObject.acronymsOfPropertyContent == null || text == null || text.Equals("")){ return ""; }
            foreach(AcronymsOfPropertyContent aopc in grantTrees.TextviewObject.acronymsOfPropertyContent)
            {
                if (aopc.name.Equals(text))
                {
                    return aopc.acronym;
                }
            }
            return text;
        }

        /// <summary>
        /// Determines whether an connected element is enable
        /// </summary>
        /// <param name="osmElement">the OSM element of the braille tree</param>
        /// <returns><code>true</code> if the connected element enable; otherwise <code>false</code> (If the value can not be determined, <code>null</code> is returned)</returns>
        private bool? isUiElementEnable(OSMElement.OSMElement osmElement)
        {
            OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
            if (osmRelationship == null)
            {
                Console.WriteLine("No matching object found.");
                return null;
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(osmRelationship.FilteredTree);
            if (!associatedNode.Equals(new OSMElement.OSMElement()))
            {                
                return associatedNode.properties.isEnabledFiltered;
            }
            return null;
        }

        /// <summary>
        /// Adds a node in the braille tree;
        /// if a node with this id already exists, the node will be updated
        /// </summary>
        /// <param name="brailleNode">the OSM element of the new node</param>
        /// <param name="parentId">The new node is added as a child of this node</param>
        /// <returns>the Id of the new/updated node or <c>null</c></returns>
        public String addNodeInBrailleTree(OSMElement.OSMElement brailleNode, String parentId = null)
        {
            if (grantTrees.brailleTree == null) { grantTrees.brailleTree = strategyMgr.getSpecifiedTree().NewTree(); }
            //checks wether the node already exists
            if (brailleNode.properties.IdGenerated != null && !brailleNode.properties.IdGenerated.Equals(""))
            {
                OSMElement.OSMElement nodeToRemove = treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
                if (!nodeToRemove.Equals(new OSMElement.OSMElement()))
                {
                    changeBrailleRepresentation(ref brailleNode);
                    return brailleNode.properties.IdGenerated;
                }
            }

            if (brailleNode.brailleRepresentation.screenName == null || brailleNode.brailleRepresentation.screenName.Equals(""))
            {
                Debug.WriteLine("Attention: No ScreenName is specified in the node. The node wasn't added."); return null;
            }
            // prüfen, ob es die View auf dem Screen schon gibt bzw:
            // Boolean isContains= strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, brailleNodeWithId);
            if (treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.viewName, brailleNode.brailleRepresentation.typeOfView))
            {
                Debug.WriteLine("Attention: The view is already exists. The node wasn't added."); return null;
            }
            addScreenInBrailleTree(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.typeOfView);

            //1. find correct ViewCategorie
            foreach (Object viewCategoryNode in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(viewCategoryNode).brailleRepresentation.typeOfView.Equals(brailleNode.brailleRepresentation.typeOfView))
                {
                    foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(viewCategoryNode))
                    {
                        //2. find parent screen node
                        if (!strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.Equals(new BrailleRepresentation()) && strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName.Equals(brailleNode.brailleRepresentation.screenName))
                        {
                            //   OSMElement.OSMElement brailleNodeWithId = brailleNode;
                            brailleNode.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(brailleNode);
                            if (parentId == null)
                            {
                                strategyMgr.getSpecifiedTree().AddChild(node, brailleNode);
                                return brailleNode.properties.IdGenerated;
                            }
                            else
                            {
                                foreach (object childOfNode in strategyMgr.getSpecifiedTree().DirectChildrenNodes(node))
                                {
                                    OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(childOfNode);
                                    
                                    if (!data.properties.Equals(new GeneralProperties()) && data.properties.IdGenerated != null && data.properties.IdGenerated.Equals(parentId))
                                    {
                                        strategyMgr.getSpecifiedTree().AddChild(childOfNode, brailleNode);
                                        return brailleNode.properties.IdGenerated;
                                    }
                                }
                                strategyMgr.getSpecifiedTree().AddChild(node, brailleNode);
                                return brailleNode.properties.IdGenerated;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private void addTypeOfViewInBrailleTree(String typeOfViewName)
        {
            Object typeOfViewSubtree;
            addTypeOfViewInBrailleTree(typeOfViewName, out typeOfViewSubtree);
        }

        private void addTypeOfViewInBrailleTree(String typeOfViewName, out Object typeOfViewSubtree)
        {
            typeOfViewSubtree = null;
            if(typeOfViewName == null || typeOfViewName.Equals("")) { return; }
            OSMElement.OSMElement osmTypeOfView = new OSMElement.OSMElement();
            osmTypeOfView.brailleRepresentation.typeOfView = typeOfViewName;
            osmTypeOfView.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmTypeOfView);
            if (strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmTypeOfView))
            {
                return;
            }
            typeOfViewSubtree = strategyMgr.getSpecifiedTree().AddChild(grantTrees.brailleTree, osmTypeOfView);
        }

        private void addScreenInBrailleTree(String screenName, String typeOfView)
        {
            Object subtreeScreen;
            addScreenInBrailleTree(screenName, typeOfView, out subtreeScreen);
        }

        /// <summary>
        /// Adds a branch for this Screen to the Root node, if the screen doesn't exists
        /// </summary>
        /// <param name="screenName">the name of the screen</param>
        /// <param name="typeOfView">the type of the view</param>
        private void addScreenInBrailleTree(String screenName, String typeOfView, out Object subtreeScreen)
        {
            if (screenName == null || screenName.Equals(""))
            {
                Debug.WriteLine("No ScreenName is specified. The screen node wasn't added.");
                subtreeScreen = null;
                return;
            }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            osmScreen.brailleRepresentation.screenName = screenName;
            osmScreen.brailleRepresentation.typeOfView = typeOfView;
            osmScreen.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);            

            //the screen dosn't exist
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen))
            {
                OSMElement.OSMElement osmViewCategory = new OSMElement.OSMElement();
                osmViewCategory.brailleRepresentation = new BrailleRepresentation();
                osmViewCategory.brailleRepresentation.typeOfView = typeOfView;
                osmViewCategory.properties = new GeneralProperties();
                osmViewCategory.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmViewCategory);
                
                Object typeOfViewSubtree = null;
                //the typeOfView exist
                if(strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmViewCategory))
                {
                    //searches typeOfView 
                    foreach(Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes( grantTrees.brailleTree))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(vC).brailleRepresentation.typeOfView.Equals(typeOfView))
                        {
                            typeOfViewSubtree = vC;
                        }
                    }
                }
                //the typeOfView dosn't exist
                else
                {
                    typeOfViewSubtree = strategyMgr.getSpecifiedTree().AddChild(grantTrees.brailleTree, osmViewCategory);
                }                
                if(typeOfViewSubtree == null ) { throw new Exception(); }                
                subtreeScreen = strategyMgr.getSpecifiedTree().AddChild(typeOfViewSubtree, osmScreen);
                return;
            }
            subtreeScreen = null;
        }

        /// <summary>
        /// Remove a node (and his children) in the Braille tree
        /// </summary>
        /// <param name="nodeId">the id of the node to remove</param>
        /// <returns><c>true</c> if the was removed, otherwise <c>false</c></returns>
        public bool removeNodeInBrailleTree(String nodeId)
        {
            Object nodeToRemove = treeOperation.searchNodes.getNode(nodeId, grantTrees.brailleTree);
            if (nodeToRemove == null) { return false; }
            OSMElement.OSMElement dataOfNodeToRemove = strategyMgr.getSpecifiedTree().GetData(nodeToRemove);
            if (dataOfNodeToRemove.Equals(new OSMElement.OSMElement())) { return false; }
            // "if"s are not necessary (but it's more understandable to me)
            if (dataOfNodeToRemove.brailleRepresentation.viewName != null)
            {
                // just remove the view
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            if(dataOfNodeToRemove.brailleRepresentation.screenName != null)
            {
                // remove the screen-node and all children
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            if(dataOfNodeToRemove.brailleRepresentation.typeOfView != null)
            {
                // remove the typeOfView and all children
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all children of the node (subtree), but not the node itself
        /// </summary>
        /// <param name="parentSubtree">the subtree (parent) to removes the children</param>
        public void removeChildNodeInBrailleTree(Object parentSubtree)
        {
            if (parentSubtree != null && strategyMgr.getSpecifiedTree().HasChild(parentSubtree) && grantTrees.brailleTree != null)
            {
                List<OSMElement.OSMElement> listToRemove = new List<OSMElement.OSMElement>();
                Object childeens = strategyMgr.getSpecifiedTree().Child(parentSubtree);
                //grantTrees.brailleTree.Remove(childeens.Data);
                listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                while (strategyMgr.getSpecifiedTree().HasNext(childeens))
                {
                    childeens = strategyMgr.getSpecifiedTree().Next(childeens);
                    //grantTrees.brailleTree.Remove(childeens.Data);
                    listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                }
                foreach (OSMElement.OSMElement osm in listToRemove)
                {
                    strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osm);
                }
            }
        }

        /// <summary>
        /// Updates the whole filtered tree (after reload);
        /// First all nodes will be filtered with the main filter strategy and after this it will be checks which node must be filtered withe a special filter (and filters with this)
        /// </summary>
        /// <param name="hwndNew">the handle of the application to filtered</param>
        public void updateFilteredTree(IntPtr hwndNew)
        {
            Object treeLoaded = strategyMgr.getSpecifiedTree().Copy(grantTrees.filteredTree);
            Object treeNew = strategyMgr.getSpecifiedFilter().filtering(hwndNew, TreeScopeEnum.Application, -1);
            grantTrees.filteredTree= treeNew;

            if (treeNew.Equals(strategyMgr.getSpecifiedTree().NewTree()) || !strategyMgr.getSpecifiedTree().HasChild(treeNew)) { throw new Exception("The application cann't be filtered."); }
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.filteredTree, grantTrees.filterstrategiesOfNodes, strategyMgr.getSpecifiedTree());
            foreach (FilterstrategyOfNode<String, String, String> nodeStrategy in grantTrees.filterstrategiesOfNodes)
            {
                //exclude the fist node --> the strategy is correct
                if (!nodeStrategy.IdGenerated.Equals(mainFilterstrategy.IdGenerated))
                {
                    //change the filter
                    strategyMgr.setSpecifiedFilter(nodeStrategy.FilterstrategyFullName + ", " + nodeStrategy.FilterstrategyDll);
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                    // searches the node in new tree + filtering and updates
                    OSMElement.OSMElement foundNewNode = treeOperation.searchNodes.getNodeElement(nodeStrategy.IdGenerated, grantTrees.filteredTree);
                    if (!foundNewNode.Equals(new OSMElement.OSMElement()))
                    {
                        OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(foundNewNode);
                        Settings settings = new Settings();
                        properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyMgr.getSpecifiedFilter().GetType());
                        changePropertiesOfFilteredNode(properties, foundNewNode.properties.IdGenerated);
                    }
                }
            }
            IFilterStrategy filter = strategyMgr.getSpecifiedFilter();
            if (!mainFilterstrategy.FilterstrategyFullName.Equals(filter.GetType().FullName) || !mainFilterstrategy.FilterstrategyDll.Equals(filter.GetType().Namespace))
            {
                strategyMgr.setSpecifiedFilter(mainFilterstrategy.FilterstrategyFullName + ", " + mainFilterstrategy.FilterstrategyDll);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            }
        }

        /// <summary>
        /// Changes a subtree of the filtered tree.
        /// </summary>
        /// <param name="subtree">the subtree</param>
        /// <param name="idOfFirstNode">Id of the first node of the subtree</param>
        /// <returns>die Id des Elternknotens des Teilbaumes oder <c>null</c></returns>
        public String changeSubtreeOfFilteredTree(Object subtree, String idOfFirstNode)
        {
            if (subtree == strategyMgr.getSpecifiedTree().NewTree() || strategyMgr.getSpecifiedTree().HasChild(subtree) == false) { Debug.WriteLine("Empty subtree!"); return null; }
            if (grantTrees.filteredTree == null) { Debug.WriteLine("It dosn't exist a tree!"); return null; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("Parent id missinig"); return null; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            { 
                if (idOfFirstNode.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated))
                {
                    if (strategyMgr.getSpecifiedTree().HasParent(node))
                    {
                        //  if (subTree.Child.HasNext || subTree.Child.HasChild)
                        {
                            Object parentNode = strategyMgr.getSpecifiedTree().Parent(node);

                            if (strategyMgr.getSpecifiedTree().BranchIndex(node) == 0)
                            {
                                //   Object copy = strategyMgr.getSpecifiedTree().DeepCopy(grantTrees.filteredTree);
                              bool reslut =  strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node));
                                strategyMgr.getSpecifiedTree().InsertChild( parentNode, subtree);
                            }
                            else
                            {
                                if (strategyMgr.getSpecifiedTree().BranchIndex(node) +1 == strategyMgr.getSpecifiedTree().BranchCount(node))
                                {
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node));
                                    strategyMgr.getSpecifiedTree().AddChild(parentNode, subtree);
                                }
                                else
                                {
                                    Object previousNode = strategyMgr.getSpecifiedTree().Previous(node);
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node));
                                    strategyMgr.getSpecifiedTree().InsertNext(previousNode, subtree);
                                }
                            }
                            return strategyMgr.getSpecifiedTree().GetData(parentNode).properties.IdGenerated;
                        }
                    }
                }
            }
            Debug.WriteLine("Cann't find the id in the tree!");
            return null;
        }

        /// <summary>
        /// Sets the filterstrategy in the subtree
        /// </summary>
        /// <param name="strategyType">the filterstrategy</param>
        /// <param name="subtree">subtree to change the filterstrategy</param>
        public void setFilterstrategyInPropertiesAndObject(Type strategyType, Object subtree)
        {
            FilterstrategyOfNode<String, String, String> mainFilterstrategy = FilterstrategiesOfTree.getMainFilterstrategyOfTree(grantTrees.filteredTree, grantTrees.filterstrategiesOfNodes, strategyMgr.getSpecifiedTree());

            if (mainFilterstrategy.FilterstrategyFullName.Equals(strategyType.FullName) && mainFilterstrategy.FilterstrategyDll.Equals(strategyType.Namespace))
            {
                removeFilterStrategyOfSubtree(strategyType, ref subtree);
                grantTrees.filteredTree = strategyMgr.getSpecifiedTree().Root(subtree);
                return;
            }
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.filterstrategiesOfNodes;
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                bool isAdded = FilterstrategiesOfTree.addFilterstrategyOfNode(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, strategyType, ref filterstrategies);
                if (isAdded)
                {
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.grantFilterStrategy = settings.filterStrategyTypeToUserName(strategyType);
                    changePropertiesOfFilteredNode(properties);
                }
            }
            grantTrees.filteredTree = strategyMgr.getSpecifiedTree().Root(subtree);
        }

        private void removeFilterStrategyOfSubtree(Type strategyType, ref Object subtree)
        {
            Settings settings = new Settings();
            List<FilterstrategyOfNode<String, String, String>> filterstrategies = grantTrees.filterstrategiesOfNodes;
            if (!strategyMgr.getSpecifiedTree().HasChild(subtree)) { return; }
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Child(subtree)))
            {
                bool isRemoved = FilterstrategiesOfTree.removeFilterstrategyOfNode(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, strategyType, ref filterstrategies);
                if (isRemoved)
                {
                    GeneralProperties properties = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    properties.grantFilterStrategy = "";
                    changePropertiesOfFilteredNode(properties);
                }
            }
        }
        
        /// <summary>
        /// updates all groups in the braille tree
        /// </summary>
        public void updateBrailleGroups()
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree)) )
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.isContentElementFiltered == false && !strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.isGroupChild)
                {
                    Type typeOfTemplate = Type.GetType(strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.templateFullName + ", " + strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.templateNamspace);
                    if (typeOfTemplate != null)
                    {
                        TemplateUiObject templateobject = new TemplateUiObject();
                        templateobject.osm = strategyMgr.getSpecifiedTree().GetData(node);
                        templateobject.Screens = new List<string>();
                        templateobject.Screens.Add(strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.screenName);
                        OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                        if (osmRelationship != null)
                        {
                            Object subtreeFiltered = treeOperation.searchNodes.getNode(osmRelationship.FilteredTree, grantTrees.filteredTree);
                            if (subtreeFiltered == null) { return; }
                            String id = strategyMgr.getSpecifiedTree().GetData(subtreeFiltered).properties.IdGenerated;
                            subtreeFiltered = strategyMgr.getSpecifiedFilter().filtering(strategyMgr.getSpecifiedTree().GetData(subtreeFiltered), TreeScopeEnum.Subtree);
                            String idParent = changeSubtreeOfFilteredTree(subtreeFiltered, id);
                            Object tree = grantTrees.filteredTree;
                            if (idParent == null || tree == null) { return; }
                            tree = treeOperation.generatedIds.generatedIdsOfFilteredSubtree(treeOperation.searchNodes.getNode(idParent, tree));
                            grantTrees.filteredTree = tree;
                            subtreeFiltered = treeOperation.searchNodes.getNode(osmRelationship.FilteredTree, grantTrees.filteredTree);
                            if (strategyMgr.getSpecifiedTree().HasParent(node))
                            {
                                removeChildNodeInBrailleTree(node);
                            }
                            if (subtreeFiltered != null)
                            {
                                strategyMgr.getSpecifiedGeneralTemplateUi().createUiElementFromTemplate(subtreeFiltered, templateobject, strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes children and its OSM conneection in the braille tree
        /// </summary>
        public void deleteChildsOfBrailleGroups()
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree)))
            {
                if (strategyMgr.getSpecifiedTree().HasParent(node) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(node)).properties.isContentElementFiltered == false)
                {
                    OsmTreeConnectorTuple<String, String> osmRelationship = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                    if (osmRelationship != null)
                    {
                        List<OsmTreeConnectorTuple<String, String>> conector = grantTrees.osmTreeConnections;
                        OsmTreeConnector.removeOsmConnection(osmRelationship.FilteredTree, osmRelationship.BrailleTree, ref conector);
                        removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                    }
                }
            }
        }

        /// <summary>
        /// Ändert die Eigenschaft beim Braille-Knoten, welcher dem Screen entspricht,
        /// </summary>
        /// <param name="screenName">gibt den namen des Screens an</param>
        /// <param name="isActiv"></param>
        public void setPropertyInNavigationbarForScreen(string screenName, bool isActiv)
        {
            /*
             * 1. Searches Screens-nodes
             * 2. in the screen-subtree searches the navigationbar
             * 3. changes the properties
             */
            Object screenTree = treeOperation.searchNodes.getSubtreeOfScreen(screenName);
            if (screenTree == null)
            {
                Debug.WriteLine("Can't found the screen, that's why the properties of the navigation bar couldn't be changed.");
                return;
            }
            Object navigationBarSubtree = strategyMgr.getSpecifiedTree().NewTree();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(screenTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Contains(Settings.getNavigationbarSubstring()))
                { //node with the navigation bar
                    navigationBarSubtree = node;
                    break;
                }
            }
            if (navigationBarSubtree != null && strategyMgr.getSpecifiedTree().Count(navigationBarSubtree) > 0)
            {
                foreach( Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(navigationBarSubtree))
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    if (strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered.Equals(screenName))
                    {
                        osm.properties.isEnabledFiltered = false;
                    }
                    else
                    {
                        osm.properties.isEnabledFiltered = true;
                    }
                    
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                }
                OSMElement.OSMElement osmScreen = strategyMgr.getSpecifiedTree().GetData(navigationBarSubtree);
                grantTrees.brailleTree = strategyMgr.getSpecifiedTree().Root(navigationBarSubtree);
                //removeChildNodeInBrailleTree

                strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmScreen);
                strategyMgr.getSpecifiedTree().SetData(navigationBarSubtree, osmScreen);
                grantTrees.brailleTree = strategyMgr.getSpecifiedTree().Root(navigationBarSubtree);
            }
        }
    }
}
