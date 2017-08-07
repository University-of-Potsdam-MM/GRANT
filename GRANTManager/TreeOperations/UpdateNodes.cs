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

        #region (update)  filtering

        /// <summary>
        /// Filters a node with the current filter strategy,
        /// thus, the filtering strategy of a node can be changed
        /// </summary>
        /// <param name="filteredTreeGeneratedId">the id of the node in the filtered tree</param>
        public void filterNodeWithCurrentFilterStrategy(String filteredTreeGeneratedId)
        {
            OSMElement.OSMElement relatedFilteredTreeObject = treeOperation.searchNodes.getFilteredTreeOsmElementById(filteredTreeGeneratedId);
            if (relatedFilteredTreeObject.Equals(new OSMElement.OSMElement())) { return; }
            OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(relatedFilteredTreeObject);
            changePropertiesOfFilteredNode(properties);
        }

        /// <summary>
        /// filtered a subtree and updates the tree object in the <c>GrantProjectObject</c> object;
        /// it will be filtering with the current selectet filter library
        /// </summary>
        /// <param name="idGeneratedOfFirstNodeOfSubtree">id of the node of the subtree to be updated</param>
        public void filterSubtreeWithCurrentFilterStrtegy(String idGeneratedOfFirstNodeOfSubtree) // filterSubtreeWithCurrentFilterStrtegy
        {
            OSMElement.OSMElement osmElementOfFirstNodeOfSubtree = treeOperation.searchNodes.getFilteredTreeOsmElementById(idGeneratedOfFirstNodeOfSubtree);
            Object subtree = strategyMgr.getSpecifiedFilter().filtering(idGeneratedOfFirstNodeOfSubtree, TreeScopeEnum.Subtree);
            String idParent = treeOperation.updateNodes.changeSubtreeOfFilteredTree(subtree, idGeneratedOfFirstNodeOfSubtree);
        }

        /// <summary>
        /// Updates the whole filtered tree (e.g. after reload);
        /// First all nodes will be filtered with the main filter strategy and after this it will be checks which node must be filtered withe a special filter (and filters with this)
        /// </summary>
        /// <param name="hwndNew">the handle of the application to filtered</param>
        public void filteredTreeOfApplication(IntPtr hwndNew)
        {
            Object treeLoaded = grantTrees.filteredTree.DeepCopy();
            Object treeNew = strategyMgr.getSpecifiedFilter().filtering(hwndNew, TreeScopeEnum.Application, -1);
            treeOperation.generatedIds.generatedIdsOfFilteredTree(ref treeNew);
            grantTrees.filteredTree = treeNew;

            if (treeNew.Equals(strategyMgr.getSpecifiedTree().NewTree()) || !strategyMgr.getSpecifiedTree().HasChild(treeNew)) { throw new Exception("The application cann't be filtered."); }
            String mainFilterstrategy = treeOperation.searchNodes.getMainFilterstrategyOfTree();
            String mainFS_userName = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(treeLoaded)).properties.grantFilterStrategy;
            List<String> fsChildren = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(treeLoaded)).properties.grantFilterStrategiesChildren;
            filterChild(treeLoaded, mainFS_userName, fsChildren);
        }

        public void filteredTree(String idGenerated, TreeScopeEnum treescope)
        {
            if(grantTrees.filteredTree == null) { return; }
            if (TreeScopeEnum.Application.Equals(treescope))
            {
                //in this case the generatedId will be ignore and the hwnd of the first noe fil be used for filtering
                if (strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree))
                {
                    filteredTreeOfApplication(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.hWndFiltered);
                }
                return;
            }
            if (TreeScopeEnum.Element.Equals(treescope))
            {
                filteredNodeElementOfApplication(idGenerated);
                return;
            }
            if (TreeScopeEnum.Ancestors.Equals( treescope) || TreeScopeEnum.Sibling.Equals(treescope) )
            {
                throw new NotImplementedException();
            }

            Object treeObjectOfIdGenerated = treeOperation.searchNodes.getNode(idGenerated, grantTrees.filteredTree).DeepCopy();

            String mainFS_userName= strategyMgr.getSpecifiedTree().GetData(treeObjectOfIdGenerated).properties.grantFilterStrategy;
            List<String> fsChildren = strategyMgr.getSpecifiedTree().GetData(treeObjectOfIdGenerated).properties.grantFilterStrategiesChildren;
            
            if (TreeScopeEnum.Children.Equals(treescope))
            {
                filterChild2(treeObjectOfIdGenerated);
            }
            else
            {
                bool changeFilter = false;
                if (mainFS_userName != null && !treeOperation.searchNodes.getMainFilterstrategyOfTree().Equals(Settings.strategyUserNameToClassName(mainFS_userName)))
                {
                    changeFilter = true;
                    this.changeFilter(mainFS_userName);
                }
                Object treeNew = strategyMgr.getSpecifiedFilter().filtering(idGenerated, treescope); // treescope == Subtree or Descendants
                if (treeNew == null || treeNew.Equals(strategyMgr.getSpecifiedTree().NewTree())) { throw new Exception("The application cann't be filtered. -- 1"); }
                if (mainFS_userName != null && changeFilter)
                {
                    //resets the filter
                    strategyMgr.setSpecifiedFilter(treeOperation.searchNodes.getMainFilterstrategyOfTree());
                    strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                    strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                }
                changePartOfFilteredTree(treeNew, idGenerated, treescope);
                if (treeNew == null || treeNew.Equals(strategyMgr.getSpecifiedTree().NewTree())) { throw new Exception("The application cann't be filtered. ---2"); }
                filterChild(treeObjectOfIdGenerated, mainFS_userName, fsChildren);
            }
        }

        /// <summary>
        /// Updates a node of the filtered tree;
        /// It is possible that the filter strategy changes temporarily
        /// </summary>
        /// <param name="generatedId">the id of the node in the filtered tree</param>
        public void filteredNodeElementOfApplication(String generatedId)
        {
            bool changeFilter = false;
            OSMElement.OSMElement osmOfNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedId).DeepCopy();
            if (osmOfNode.Equals(new OSMElement.OSMElement())) { return; }
            String mainFilterstrategy = treeOperation.searchNodes.getMainFilterstrategyOfTree();
            String nodeFilterstrategy = treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedId).properties.grantFilterStrategy;
            // if necessary changes the filter strategy
            if (nodeFilterstrategy != null && !mainFilterstrategy.Equals(Settings.strategyUserNameToClassName( nodeFilterstrategy)))
            {
                changeFilter = true;
                this.changeFilter(nodeFilterstrategy);
            }
            // filters and updates the node
            OSMElement.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(osmOfNode);
            changePropertiesOfFilteredNode(properties);

            if (nodeFilterstrategy != null && changeFilter)
            {
                //resets the filter
                strategyMgr.setSpecifiedFilter(mainFilterstrategy);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
            }
        }

        
        private void changePartOfFilteredTree(Object subtree, String idOfFirstNode, TreeScopeEnum treescope)
        {
            if (TreeScopeEnum.Children.Equals(treescope))
            {
                return;
            }
            if (TreeScopeEnum.Ancestors.Equals(treescope) || TreeScopeEnum.Sibling.Equals(treescope) || TreeScopeEnum.Application.Equals(treescope) || TreeScopeEnum.Element.Equals(treescope))
            {
                throw new NotImplementedException();
            }
            //treescope == Subtree or Descendants

            if (subtree == strategyMgr.getSpecifiedTree().NewTree() || strategyMgr.getSpecifiedTree().HasChild(subtree) == false) { Debug.WriteLine("Empty subtree!"); return; }
            if (grantTrees.filteredTree == null) { Debug.WriteLine("It dosn't exist a tree!"); return; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("id missinig"); return; }

            // if treescope == Subtree => the tree will be added at the parent of the node "idOfFirstNode"
            // if treescope == Descendants => the tree will be added at the node "idOfFirstNode"
            if (TreeScopeEnum.Descendants.Equals(treescope)) { changeDescendantsOfFilteredTree(subtree, idOfFirstNode); }
            if (TreeScopeEnum.Subtree.Equals(treescope))
            {
                changeSubtreeOfFilteredTree(subtree, idOfFirstNode);
            }            
        }

        private void changeDescendantsOfFilteredTree(Object subtreeNew, String idOfFirstNode)
        { // => the tree will be added at the node "idOfFirstNode"
            if (subtreeNew == strategyMgr.getSpecifiedTree().NewTree() ) { return; }
            if (grantTrees.filteredTree == null) { Debug.WriteLine("It dosn't exist a tree!"); return; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("id missinig"); return; }
            Object subtreeObjectOld = treeOperation.searchNodes.getNode(idOfFirstNode, grantTrees.filteredTree);
            strategyMgr.getSpecifiedTree().RemoveAllDescendants(grantTrees.filteredTree, subtreeObjectOld);
            strategyMgr.getSpecifiedTree().InsertChild(subtreeObjectOld, subtreeNew);

            

            treeOperation.generatedIds.generatedIdsOfFilteredSubtree(subtreeObjectOld);
            #region grantFilterstrategy
            //TODO: Methode (setGrantFilterStrategiesChildren) erweitern, dass Treescope mit angegeben wird
            foreach (Object child in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtreeObjectOld))
            {
                OSMElement.OSMElement OSM = strategyMgr.getSpecifiedTree().GetData(child);
                treeOperation.updateNodes.setGrantFilterStrategiesChildren(OSM.properties.IdGenerated);
            }
            #endregion

        }

        /// <summary>
        /// Changes a subtree of the filtered tree.
        /// </summary>
        /// <param name="subtree">the subtree</param>
        /// <param name="idOfFirstNode">Id of the first node of the subtree</param>
        /// <returns>die Id des Elternknotens des Teilbaumes oder <c>null</c></returns>
        private String changeSubtreeOfFilteredTree(Object subtree, String idOfFirstNode)
        { //=> the tree will be added at the parent of the node "idOfFirstNode"
            if (subtree == strategyMgr.getSpecifiedTree().NewTree() || strategyMgr.getSpecifiedTree().HasChild(subtree) == false) { Debug.WriteLine("Empty subtree!"); return null; }
            if (grantTrees.filteredTree == null) { Debug.WriteLine("It dosn't exist a tree!"); return null; }
            if (idOfFirstNode == null || idOfFirstNode.Equals("")) { Debug.WriteLine("id missinig"); return null; }
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
                                strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node)); //Don't remove the connection to the Braille Tree
                                strategyMgr.getSpecifiedTree().InsertChild(parentNode, subtree);
                            }
                            else
                            {
                                if (strategyMgr.getSpecifiedTree().BranchIndex(node) + 1 == strategyMgr.getSpecifiedTree().BranchCount(node))
                                {
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node)); //Don't remove the connection to the Braille Tree
                                    strategyMgr.getSpecifiedTree().AddChild(parentNode, subtree);
                                }
                                else
                                {
                                    Object previousNode = strategyMgr.getSpecifiedTree().Previous(node);
                                    strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node)); //Don't remove the connection to the Braille Tree
                                    strategyMgr.getSpecifiedTree().InsertNext(previousNode, subtree);
                                }
                            }

                            #region generatet the ids of the subtree
                            Object searchResultTree = treeOperation.searchNodes.getNode(strategyMgr.getSpecifiedTree().GetData(parentNode).properties.IdGenerated, grantTrees.filteredTree);

                            if (searchResultTree != null)
                            {
                                treeOperation.generatedIds.generatedIdsOfFilteredSubtree(searchResultTree);
                                treeOperation.updateNodes.setGrantFilterStrategiesChildren(idOfFirstNode);
                            }
                            #endregion
                            return strategyMgr.getSpecifiedTree().GetData(parentNode).properties.IdGenerated;
                        }
                    }
                }
            }
            Debug.WriteLine("Cann't find the id in the tree!");
            return null;
        }

        /// <summary>
        /// checks the given filterstrategy and filtered the children with the correct strategy
        /// </summary>
        protected void filterChild2(Object parentObject)
        {
            if (parentObject == null) { return; }
            else
            {
                String mainFS_userName = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.grantFilterStrategy;
                foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(parentObject))
                {
                    OSMElement.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node).DeepCopy();
                    changeFilter(osmNode.properties.grantFilterStrategy);
                    // neu Filtern
                    filteredNodeElementOfApplication(osmNode.properties.IdGenerated);
                }
                changeFilter(mainFS_userName);
            }
        }

        /// <summary>
        /// checks the given filterstrategy and if necessary filtered the node with the correct strategy
        /// </summary>
        /// <param name="subtreeOld">the old subtree object --> this will be importent to get the filterstrategy of each node; the subtreeOld object will be the start point for filtering the children</param>
        /// <param name="fs_main">the main filterstrategy (or the the parent filterstrategy) --> this will be importent to know whether the node was already filtered with the correct strategy </param>
        /// <param name="fs_children"></param>
        private void filterChild(Object subtreeOld, String fs_main, List<String> fs_children)
        {
            if (fs_children == null || fs_children.Count == 0 || (fs_children.Count == 1 && fs_children[0].Equals(fs_main)))
            {
                // all nodes was filtered with the same filterstrategy
                return;
            }
            else
            { //TODO: so werden neue Knoten ignoriert
                foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtreeOld))
                {
                    OSMElement.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node).DeepCopy();
                    if (!osmNode.properties.grantFilterStrategy.Equals(fs_main))
                    {
                        changeFilter(osmNode.properties.grantFilterStrategy);
                        // neu Filtern
                        // müssen die Kinder noch geprüft werden?
                        Object subtree = strategyMgr.getSpecifiedFilter().filtering(osmNode, TreeScopeEnum.Subtree);
                        changeSubtreeOfFilteredTree(subtree, osmNode.properties.IdGenerated);
                        filterChild(node, osmNode.properties.grantFilterStrategy, osmNode.properties.grantFilterStrategiesChildren);
                        changeFilter(fs_main);
                    }
                    else
                    {
                        // check children 
                        filterChild(node, fs_main, osmNode.properties.grantFilterStrategiesChildren);
                    }
                }
            }
        }

        protected void changeFilter(String filterUserName)
        {
            //TODO: check if necessary
            strategyMgr.setSpecifiedFilter(Settings.strategyUserNameToClassName(filterUserName));
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
        }

        #endregion

        #region update Braille node

        /// <summary>
        /// Changes the properties of the braille node
        /// </summary>
        /// <param name="element">the braille node which sould be updated</param>
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

                String text = getTextForView(element);
                element.properties.valueFiltered = text ?? element.properties.valueFiltered; // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-conditional-operator
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
        /// Sets the new braille representaion of the element
        /// </summary>
        /// <param name="element">the new representation</param>
        private void changeBrailleRepresentation(ref OSMElement.OSMElement element)
        {
            Object brailleTree = grantTrees.brailleTree;
            if (brailleTree == null) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(element.properties.IdGenerated))
                {
                    strategyMgr.getSpecifiedTree().SetData(node, element);
                    break;
                }
            }
        }

        #endregion

        #region properties

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
                if (treeOperation.searchNodes.existViewInTree(parent, newProperty as String))
                {
                    return false;
                }
            }
            if (nameOfProperty.Equals("valueFiltered"))
            {
                //if a connection to a filtered exsisting => delete this
                if (node.brailleRepresentation.displayedGuiElementType != null)
                {
                    node.brailleRepresentation.displayedGuiElementType = null;
                    String connectedFilteredTreeId = treeOperation.searchNodes.getConnectedFilteredTreenodeId(node.properties.IdGenerated);
                    if (connectedFilteredTreeId != null)
                    {
                        List<OsmTreeConnectorTuple<String, String>> conector = grantTrees.osmTreeConnections;
                        OsmTreeConnector.removeOsmConnection(connectedFilteredTreeId, node.properties.IdGenerated, ref conector);
                    }
                }
            }
            // https://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
            OSMElement.OSMElement.setElement(nameOfProperty, newProperty, node);
            Object propertyValueNew = OSMElement.OSMElement.getElement(nameOfProperty, node);
            if (propertyValueCurrent == null && propertyValueNew == null) { return false; }
            if (propertyValueCurrent != null && propertyValueCurrent.Equals(propertyValueNew)) { return false; }
            else
            {
                if ((propertyValueNew == null && newProperty == null) || propertyValueNew.ToString().Equals(newProperty.ToString()))
                {
                    updateNodeOfBrailleUi(ref node);
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
        /// Changes the <see cref="GeneralProperties"/> of a node
        /// </summary>
        /// <param name="properties">the new properties</param>
        /// <param name="idGeneratedOld">the old id</param>
        internal void changePropertiesOfFilteredNode(GeneralProperties properties, String idGeneratedOld = null)
        {
            // List<Object> node = treeOperation.searchNodes.searchNodeByProperties(grantTrees.filteredTree, properties);
            Object node = treeOperation.searchNodes.getNode(properties.IdGenerated, grantTrees.filteredTree);
            if (node != null)
            {
                OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                if (osm.properties.grantFilterStrategiesChildren != null && properties.grantFilterStrategiesChildren == null)
                {
                    properties.grantFilterStrategiesChildren = osm.properties.grantFilterStrategiesChildren;
                }
                osm.properties = properties;
                if (idGeneratedOld != null)
                {
                    osm.properties.IdGenerated = idGeneratedOld;
                }

                strategyMgr.getSpecifiedTree().SetData(node, osm);
                return;
            }
            Debug.WriteLine("Can't find the node!");
        }

        /// <summary>
        /// Seeks the displayed text for a (braille) view;
        /// exists an acronym for this text this will be used
        /// </summary>
        /// <param name="osmElementBraille">the osmElement of the Braille Tree</param>
        /// <returns>the text to display</returns>
        private String getTextForView(OSMElement.OSMElement osmElementBraille)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmElementBraille.properties.IdGenerated);
            if (connectedIdFilteredTree == null)
            {
                Console.WriteLine("No matching object found.");
                return null;
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(connectedIdFilteredTree);
            String text = "";
            if (!associatedNode.Equals(new OSMElement.OSMElement()) && !osmElementBraille.brailleRepresentation.displayedGuiElementType.Trim().Equals(""))
            {
                object objectText = GeneralProperties.getPropertyElement(osmElementBraille.brailleRepresentation.displayedGuiElementType, associatedNode.properties);
                text = (objectText != null ? objectText.ToString() : "");
            }
            return useAcronymForText(text);
        }

        /// <summary>
        /// if exists, replaces the text by an acronym 
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns> the acronym of the text or the text </returns>
        public String useAcronymForText(String text)
        {
            if (grantTrees.TextviewObject.Equals(new TextviewElement()) || grantTrees.TextviewObject.acronymsOfPropertyContent == null || text == null || text.Equals("")) { return ""; }
            foreach (AcronymsOfPropertyContent aopc in grantTrees.TextviewObject.acronymsOfPropertyContent)
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
        /// <param name="osmElementBraille">the OSM element of the braille tree</param>
        /// <returns><code>true</code> if the connected element enable; otherwise <code>false</code> (If the value can not be determined, <code>null</code> is returned)</returns>
        private bool? isUiElementEnable(OSMElement.OSMElement osmElementBraille)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmElementBraille.properties.IdGenerated);
            if (connectedIdFilteredTree == null)
            {
                Console.WriteLine("No matching object found.");
                return null;
            }
            OSMElement.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(connectedIdFilteredTree);
            if (!associatedNode.Equals(new OSMElement.OSMElement()))
            {
                return associatedNode.properties.isEnabledFiltered;
            }
            return null;
        }

        private bool changeTypeOfView(String id, String typeOfViewNew)
        {
            Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
            OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            //checks the position of the node

            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject))
            {
                #region the node is a "root" branch of a type of view
                if (!treeOperation.searchNodes.existTypeOfViewInTree(typeOfViewNew))
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
                #endregion
            }
            else
            {
                #region screen OR view branch
                Object existTypeOfView;
                Object parent = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                if (nodeData.brailleRepresentation.viewName == null)
                {
                    #region  screen-branch 
                    //rename this node and all children
                    foreach (Object o in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                    {
                        OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
                        data.brailleRepresentation.typeOfView = typeOfViewNew;
                    }
                    Object removeTypeOfView = null;
                    if (!strategyMgr.getSpecifiedTree().HasPrevious(nodeObject) && !strategyMgr.getSpecifiedTree().HasNext(nodeObject))
                    {
                        // the screen was the last node in this branch
                        removeTypeOfView = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                    }
                    if (!treeOperation.searchNodes.existTypeOfViewInTree(typeOfViewNew, out existTypeOfView))
                    {
                        addTypeOfViewInBrailleTree(typeOfViewNew, out existTypeOfView); // now the typeOfView node exists
                        if (existTypeOfView == null) { return false; }
                    }
                    Boolean result = strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, existTypeOfView);
                    if (result)
                    {
                        RemoveNodeAndConnection(removeTypeOfView);
                        return true;
                    }
                    else { return false; }
                    #endregion
                }
                else
                {
                    #region view-branch
                    if (strategyMgr.getSpecifiedTree().HasNext(nodeObject) || strategyMgr.getSpecifiedTree().HasPrevious(nodeObject))
                    {
                        // there would be in different typeOfViewBranches screens with the same name
                        return false;
                    }
                    else
                    {
                        //  nodeData.brailleRepresentation.typeOfView = typeOfViewNew;
                        Object removeTypeOfView = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                        Object nodeParentObject = strategyMgr.getSpecifiedTree().Parent(nodeObject); //old TypeOfView-Branch
                        if (!strategyMgr.getSpecifiedTree().HasPrevious(nodeParentObject) && !strategyMgr.getSpecifiedTree().HasNext(nodeParentObject))
                        {
                            // the screen was the last node in this branch
                            removeTypeOfView = strategyMgr.getSpecifiedTree().Parent(strategyMgr.getSpecifiedTree().Parent(nodeObject));
                        }
                        if (!treeOperation.searchNodes.existTypeOfViewInTree(typeOfViewNew, out existTypeOfView))
                        {
                            // addScreenInBrailleTree(nodeData.brailleRepresentation.screenName, typeOfViewNew, out existTypOfView);
                            addTypeOfViewInBrailleTree(typeOfViewNew, out existTypeOfView);
                        }
                        foreach (Object n in strategyMgr.getSpecifiedTree().AllNodes(nodeParentObject))
                        {
                            OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(n);
                            osm.brailleRepresentation.typeOfView = typeOfViewNew;
                        }


                        Boolean result = strategyMgr.getSpecifiedTree().moveSubtree(nodeParentObject, existTypeOfView);
                        if (result)
                        {
                            if (removeTypeOfView != null)
                            {
                                strategyMgr.getSpecifiedTree().Remove(removeTypeOfView);
                            }
                            return true;
                        }
                        else { return false; }
                    }
                    #endregion
                }
                #endregion
            }
        }

        private bool changeScreenName(String id, String screenNameNew)
        {
            Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
            OSMElement.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            //checks the position of the node
            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject)) { return false; }
            Object screenBranchNew;
            if (nodeData.brailleRepresentation != null && nodeData.brailleRepresentation.viewName == null && nodeData.brailleRepresentation.isVisible == false)
            {
                #region => 'root' of a screen branch => rename all children

                if (treeOperation.searchNodes.existScreenInTree(screenNameNew, out screenBranchNew))
                {
                    //check whether all views have different names
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
                    foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                        osm.brailleRepresentation.screenName = screenNameNew;
                    }
                    while (strategyMgr.getSpecifiedTree().HasChild(nodeObject))
                    {
                        strategyMgr.getSpecifiedTree().moveSubtree(strategyMgr.getSpecifiedTree().Child(nodeObject), screenBranchNew);
                    }
                    strategyMgr.getSpecifiedTree().Remove(nodeObject);
                    return true;
                }
                else
                {
                    // the screen-branch didn't exist befor
                    //rename the screen-property in each node of this branch
                    foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                    {
                        OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                        osm.brailleRepresentation.screenName = screenNameNew;
                    }
                    return true;
                }

                #endregion
            }
            else
            {
                #region it's a child node of a screen branch (view-node)
                if (treeOperation.searchNodes.existScreenInTree(screenNameNew, out screenBranchNew))
                {
                    //check whether the new screen exist in this typeOfView
                    if (!strategyMgr.getSpecifiedTree().GetData(screenBranchNew).brailleRepresentation.typeOfView.Equals(nodeData.brailleRepresentation.typeOfView))
                    { return false; }
                    // check whether the view exists
                    if (treeOperation.searchNodes.existViewInScreen(screenNameNew, nodeData.brailleRepresentation.viewName, nodeData.brailleRepresentation.typeOfView))
                    {
                        return false;
                    }
                }
                else
                {
                    addScreenInBrailleTree(screenNameNew, nodeData.brailleRepresentation.typeOfView, out screenBranchNew);
                    if (screenBranchNew == null) { return false; }
                }
                Object screenToRemove = null;
                if (!strategyMgr.getSpecifiedTree().HasNext(nodeObject) && !strategyMgr.getSpecifiedTree().HasPrevious(nodeObject))
                {
                    screenToRemove = strategyMgr.getSpecifiedTree().Parent(nodeObject);
                }
                foreach (Object view in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                {
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(view);
                    osm.brailleRepresentation.screenName = screenNameNew;
                }
                strategyMgr.getSpecifiedTree().moveSubtree(nodeObject, screenBranchNew);
                if (screenToRemove != null)
                {
                    strategyMgr.getSpecifiedTree().Remove(screenToRemove);
                }
                return true;

                #endregion
            }
        }
       
        /// <summary>
        /// Determines whether connected checkbox of a braille node is selected 
        /// </summary>
        /// <param name="elementBraille">the OSM element of the braille tree</param>
        /// <returns><c>true</c> if the checkbox in the connected filtered node selected</returns>
        private bool isCheckboxOfAssociatedElementSelected(OSMElement.OSMElement elementBraille)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(elementBraille.properties.IdGenerated);
            if (connectedIdFilteredTree == null)
            {
                Debug.WriteLine("No matching object found.");
                return false;
            }
            List<Object> associatedNodeList = treeOperation.searchNodes.getNodeList(connectedIdFilteredTree, grantTrees.filteredTree);
            if (associatedNodeList != null && associatedNodeList.Count == 1 && strategyMgr.getSpecifiedTree().HasChild(associatedNodeList[0]))
            {
                //the information wether an element is selected will be found in the child element (at least in my example application)
                return strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn != null ? (bool)strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(associatedNodeList[0])).properties.isToggleStateOn : false;
            }
            return false;
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
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(screenTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation.viewName.Contains(Settings.getNavigationbarSubstring()))
                { //node with the navigation bar
                    navigationBarSubtree = node;
                    break;
                }
            }
            if (navigationBarSubtree != null && strategyMgr.getSpecifiedTree().Count(navigationBarSubtree) > 0)
            {
                foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(navigationBarSubtree))
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

        #region filterstrategie
        /// <summary>
        /// Sets the GrantFilterStrategiesChildren by all Parent nodes of the node with the given id
        /// </summary>
        /// <param name="idGeneratedChild">the child id</param>
        internal void setGrantFilterStrategiesChildren(string idGeneratedChild)
        {
            Object childObject = treeOperation.searchNodes.getNode(idGeneratedChild, grantTrees.filteredTree);
            if (childObject == null) { return; }
            List<String> filterStrategiesChildren = new List<string>();
            OSMElement.OSMElement osmChild = strategyMgr.getSpecifiedTree().GetData(childObject);
            if (osmChild.properties.grantFilterStrategiesChildren != null)
            {
                filterStrategiesChildren.AddRange(osmChild.properties.grantFilterStrategiesChildren);
            }
            if (!filterStrategiesChildren.Contains(osmChild.properties.grantFilterStrategy))
            {
                filterStrategiesChildren.Add(osmChild.properties.grantFilterStrategy);
            }
            if (!filterStrategiesChildren.Equals(new List<String>()))
            {
                while (strategyMgr.getSpecifiedTree().HasParent(childObject))
                {
                    childObject = strategyMgr.getSpecifiedTree().Parent(childObject);
                    OSMElement.OSMElement osmParent = strategyMgr.getSpecifiedTree().GetData(childObject);
                    if (osmParent.properties.grantFilterStrategiesChildren == null)
                    {
                        osmParent.properties.grantFilterStrategiesChildren.AddRange(filterStrategiesChildren);
                    }
                    else
                    {
                        if (filterStrategiesChildren.Equals(osmParent.properties.grantFilterStrategiesChildren))
                        { return; }
                        if (osmParent.properties.grantFilterStrategiesChildren.All(p => filterStrategiesChildren.Contains(p))) // check whether 'osmParent.properties.grantFilterStrategiesChildren' is a subset of filterStrategiesChildren
                        { // add all filterStrategies wich aren't in !osmParent.properties.grantFilterStrategiesChildren
                            List<String> fsParent = osmParent.properties.grantFilterStrategiesChildren;
                            addFilterStrategy(ref fsParent, filterStrategiesChildren);
                            osmParent.properties.grantFilterStrategiesChildren = fsParent;
                        }
                        else
                        {
                            // identify wich Filterstrategys in 'osmParent.properties.grantFilterStrategiesChildren' aren't a part of 'filterStrategiesChildren'
                            List<String> fsParent_Copy = osmParent.properties.grantFilterStrategiesChildren.DeepCopy();
                            foreach (String fs in fsParent_Copy)
                            {
                                if (!filterStrategiesChildren.Contains(fs))
                                {
                                    Boolean isFsInsibling = false;
                                    foreach (object sibling in strategyMgr.getSpecifiedTree().DirectChildrenNodes(childObject))
                                    {
                                        OSMElement.OSMElement osmSibling = strategyMgr.getSpecifiedTree().GetData(sibling);

                                        if ((osmSibling.properties.grantFilterStrategiesChildren != null && osmSibling.properties.grantFilterStrategiesChildren.Contains(fs)) ||
                                            (osmSibling.properties.grantFilterStrategy != null && osmSibling.properties.grantFilterStrategy.Equals(fs)))
                                        {
                                            isFsInsibling = true;
                                            filterStrategiesChildren.Add(fs);
                                            break;
                                        }
                                    }
                                    if (!isFsInsibling)
                                    {
                                        osmParent.properties.grantFilterStrategiesChildren.Remove(fs);
                                    }
                                }
                            }
                            List<String> fsParent = osmParent.properties.grantFilterStrategiesChildren;
                            addFilterStrategy(ref fsParent, filterStrategiesChildren);
                            osmParent.properties.grantFilterStrategiesChildren = fsParent;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds the filterstrategies from <para>filterstrategyNew</para> to <para>filterStrategyToAdd</para> if there not contains
        /// </summary>
        /// <param name="filterStrategyToAdd"></param>
        /// <param name="filterstrategyNew"></param>
        private void addFilterStrategy(ref List<String> filterStrategyToAdd, List<String> filterstrategyNew)
        {
            foreach (String fs in filterstrategyNew)
            {
                if (!filterStrategyToAdd.Contains(fs))
                {
                    filterStrategyToAdd.Add(fs);
                }
            }
        }

        #endregion

        #region file path
        /// <summary>
        /// Compares and changes the path of an application if necessary
        /// </summary>
        public void changeFilePath()
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree) || strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.Equals(new GeneralProperties())) { return; }
            String fileNameNew = strategyMgr.getSpecifiedOperationSystem().getFileNameOfApplicationByModulName(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.processName);
            GeneralProperties child = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties;
            if (child.appPath != null && !child.appPath.Equals(fileNameNew))
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
        #endregion
        #endregion


        #region add + remove Node
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
            Object existingScreenNode;
            if(treeOperation.searchNodes.existScreenInTree(brailleNode.brailleRepresentation.screenName, out existingScreenNode))
            {                
                if (!strategyMgr.getSpecifiedTree().GetData(existingScreenNode).brailleRepresentation.typeOfView.Equals(brailleNode.brailleRepresentation.typeOfView))
                {
                    Debug.WriteLine("A screen with the same name exist in a other typeOfView --> it isn't allowed");
                    return null;
                }
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
            if (typeOfViewName == null || typeOfViewName.Equals("")) { return; }
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
        private void addScreenInBrailleTree(String screenName, String typeOfView, out Object subtreeScreen) //TODO in bool ändern?
        {
            if (screenName == null || screenName.Equals(""))
            {
                Debug.WriteLine("No ScreenName is specified. The screen node wasn't added.");
                subtreeScreen = null;
                return;
            }
            Object existingScreenNode;
            if (treeOperation.searchNodes.existScreenInTree(screenName, out existingScreenNode))
            {
                if (!strategyMgr.getSpecifiedTree().GetData(existingScreenNode).brailleRepresentation.typeOfView.Equals(typeOfView))
                {
                    Debug.WriteLine("A screen with the same name exist in a other typeOfView --> it isn't allowed");
                    subtreeScreen = null;
                    return;
                }
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
                if (strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmViewCategory))
                {
                    //searches typeOfView 
                    foreach (Object vC in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
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
                if (typeOfViewSubtree == null) { throw new Exception(); }
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
            RemoveNodeAndConnection(nodeToRemove);
            return true;
          /*  // "if"s are not necessary (but it's more understandable to me)
            if (dataOfNodeToRemove.brailleRepresentation.viewName != null)
            {
                // just remove the view
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            if (dataOfNodeToRemove.brailleRepresentation.screenName != null)
            {
                // remove the screen-node and all children
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            if (dataOfNodeToRemove.brailleRepresentation.typeOfView != null)
            {
                // remove the typeOfView and all children
                strategyMgr.getSpecifiedTree().Remove(nodeToRemove);
                return true;
            }
            return false;
            */
        }
        #endregion

        #region Braille Group
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
                    // strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osm);
                    RemoveNodeAndConnection(osm);
                }
            }
        }


        /// <summary>
        /// updates all groups in the braille tree
        /// </summary>
        public void updateBrailleGroups()
        {
            if (grantTrees == null || grantTrees.brailleTree == null || !strategyMgr.getSpecifiedTree().HasChild(grantTrees.brailleTree)) { return; }
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(strategyMgr.getSpecifiedTree().Copy(grantTrees.brailleTree)))
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
                        String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                        if (connectedIdFilteredTree != null)
                        {
                            Object subtreeFiltered = treeOperation.searchNodes.getNode(connectedIdFilteredTree, grantTrees.filteredTree);
                            if (subtreeFiltered == null) { return; }
                        /*    String id = strategyMgr.getSpecifiedTree().GetData(subtreeFiltered).properties.IdGenerated;
                             subtreeFiltered = strategyMgr.getSpecifiedFilter().filtering(strategyMgr.getSpecifiedTree().GetData(subtreeFiltered), TreeScopeEnum.Subtree);
                             String idParent = changeSubtreeOfFilteredTree(subtreeFiltered, id); */
                            filteredTree(connectedIdFilteredTree, TreeScopeEnum.Subtree);
                            subtreeFiltered = treeOperation.searchNodes.getNode(connectedIdFilteredTree, grantTrees.filteredTree);
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
                    String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                    if (connectedIdFilteredTree != null)
                    {
                        List<OsmTreeConnectorTuple<String, String>> conector = grantTrees.osmTreeConnections;
                        OsmTreeConnector.removeOsmConnection(connectedIdFilteredTree, strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated, ref conector);
                        removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
                    }
                }
            }
        }
        #endregion

        #region connection (filtered + braille tree)

        /// <summary>
        /// Removes a node an its connection 
        /// </summary>
        /// <param name="osmNode">the osm element of the (braile OR filtered) tree</param>
        public void RemoveNodeAndConnection(OSMElement.OSMElement osmNode)
        {
            if (osmNode.brailleRepresentation.Equals(new BrailleRepresentation()))
            {
                // filtered node
                List<String> connectedIdsBrailleTree = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                List<OsmTreeConnectorTuple<String, String>> osmTreeConnections = grantTrees.osmTreeConnections; ;
                if (connectedIdsBrailleTree != null && osmTreeConnections != null)
                {
                    foreach (String id in connectedIdsBrailleTree)
                    {
                        OsmTreeConnector.removeOsmConnection(osmNode.properties.IdGenerated, id, ref osmTreeConnections);
                    }
                }
                strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, osmNode);
            }
            else
            {
                // braille node
                String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmNode.properties.IdGenerated);
                List<OsmTreeConnectorTuple<String, String>> osmTreeConnections = grantTrees.osmTreeConnections; ;
                if (connectedIdFilteredTree != null && osmTreeConnections != null)
                {
                    OsmTreeConnector.removeOsmConnection(osmNode.properties.IdGenerated, connectedIdFilteredTree, ref osmTreeConnections);
                }
                strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osmNode);
            }
        }
        
        /// <summary>
        /// Remove the node, all children and its connections
        /// </summary>
        /// <param name="nodeObject"></param>
        public void RemoveNodeAndConnection(Object nodeObject)
        {
            if (nodeObject == null) { return; }
            {
                //remove the children first

                //determinates which kind of tree 
                OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(nodeObject);
                if (osm == null) { return; }
                List<OsmTreeConnectorTuple<String, String>> existingConnections = grantTrees.osmTreeConnections;
                List<OsmTreeConnectorTuple<String, String>> connectionsToDel;
                if (osm.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    // filtered tree
                    connectionsToDel = getAllConnectionsOfSubtree(nodeObject, true);
                }
                else
                {
                    // braille tree
                    connectionsToDel = getAllConnectionsOfSubtree(nodeObject, false);
                }
                if (connectionsToDel != null && !connectionsToDel.Equals(new List<OsmTreeConnectorTuple<String, String>>()))
                {
                    foreach (OsmTreeConnectorTuple<String, String> connection in connectionsToDel)
                    {
                        OsmTreeConnector.removeOsmConnection(connection, ref existingConnections);
                    }
                }
                strategyMgr.getSpecifiedTree().Remove(nodeObject);

            }
        }
        private List<OsmTreeConnectorTuple<String, String>> getAllConnectionsOfSubtree(Object subtree, bool isFilteredTree)
        {
            List<OsmTreeConnectorTuple<String, String>> connections = new List<OsmTreeConnectorTuple<string, string>>();
            if (isFilteredTree) {
                foreach (Object obj in strategyMgr.getSpecifiedTree().AllNodes(subtree))
                {
                    //treeOperation.searchNodes.getConnectedBrailleTreenodeIds(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated);
                    List<OsmTreeConnectorTuple<String, String>> tmpConnections = grantTrees.osmTreeConnections.FindAll(r => r.FilteredTree.Equals(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated));
                    if (tmpConnections != null)
                    {
                        connections.AddRange(tmpConnections);
                    }
                }
            }
            else
            {
                foreach (Object obj in strategyMgr.getSpecifiedTree().AllNodes(subtree))
                {
                    OsmTreeConnectorTuple<String, String> tmpConnection = grantTrees.osmTreeConnections.Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated));
                    if (tmpConnection != null && !tmpConnection.Equals(new OsmTreeConnectorTuple<String, String>()))
                    {
                        connections.Add(tmpConnection);
                    }
                }
            }
            return connections;
        }
        #endregion

    }
}
