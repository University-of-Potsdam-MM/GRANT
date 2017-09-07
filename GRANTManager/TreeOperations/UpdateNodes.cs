using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager.Interfaces;
using OSMElements;
using OSMElements.UiElements;

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
        /// Adds a node withe data from an external screenreader to the filtered tree
        /// </summary>
        /// <param name="osm">the OSMElement</param>
        public void addNodeExternalScreenreaderInFilteredTree(OSMElement osm)
        {
            if(osm == null || osm.Equals(new OSMElement()) || grantTrees.filteredTree == null || strategyMgr.getSpecifiedExternalScreenreader() == null) { return; }
            // TODO: ggf. prüfen, ob der Knoten schon existiert, dann nur aktualisieren!
            if (osm.properties.grantFilterStrategy != null && isFilteredWithExternalScreenreader(osm.properties.grantFilterStrategy) && grantTrees.filteredTree != null && strategyMgr.getSpecifiedTree().HasChild(grantTrees.filteredTree))
            {
                if (treeOperation.searchNodes.getFilteredTreeOsmElementById(osm.properties.IdGenerated).Equals(new OSMElements.OSMElement()))
                {
                    strategyMgr.getSpecifiedTree().Add(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree), osm);
                    //strategyMgr.getSpecifiedTree().Add(strategyMgr.getSpecifiedTree().Child(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)), osm);
                }
                else
                {
                    updateNodeExternalScreenreaderInFilteredTree(osm);
                }
            }
        }

        /// <summary>
        /// Updates the node of an external screenreader in the filtered tree
        /// </summary>
        /// <param name="osm">the OSMElement</param>
        public void updateNodeExternalScreenreaderInFilteredTree(OSMElement osm)
        {
            if (isFilteredWithExternalScreenreader(osm.properties.grantFilterStrategy))
            {
                changePropertiesOfFilteredNode(osm.properties);
            }
        }

        internal Boolean isFilteredWithExternalScreenreader(String filterstrategy)
        {
            Settings settings = new Settings();
            return settings.getPossibleExternalScreenreaders().Exists(p => p.userName.Equals(filterstrategy));
        }

        /// <summary>
        /// Filters a node with the current filter strategy,
        /// thus, the filtering strategy of a node can be changed
        /// </summary>
        /// <param name="filteredTreeGeneratedId">the id of the node in the filtered tree</param>
        public void filterNodeWithCurrentFilterStrategy(String filteredTreeGeneratedId)
        {
            OSMElements.OSMElement relatedFilteredTreeObject = treeOperation.searchNodes.getFilteredTreeOsmElementById(filteredTreeGeneratedId);
            if (relatedFilteredTreeObject.Equals(new OSMElements.OSMElement())) { return; }
            OSMElements.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(relatedFilteredTreeObject);
            changePropertiesOfFilteredNode(properties);
        }

        /// <summary>
        /// filtered a subtree and updates the tree object in the <c>GrantProjectObject</c> object;
        /// it will be filtering with the current selectet filter library
        /// </summary>
        /// <param name="idGeneratedOfFirstNodeOfSubtree">id of the node of the subtree to be updated</param>
        public void filterSubtreeWithCurrentFilterStrtegy(String idGeneratedOfFirstNodeOfSubtree) // filterSubtreeWithCurrentFilterStrtegy
        {
            OSMElements.OSMElement osmElementOfFirstNodeOfSubtree = treeOperation.searchNodes.getFilteredTreeOsmElementById(idGeneratedOfFirstNodeOfSubtree);
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

        /// <summary>
        /// Filtered an OSM node (and depending on the <para>treescope</para> his sibilings, parents children etc. ) and updates the filtered tree object
        /// </summary>
        /// <param name="generatedId">the generated id of a node in the filtered tree</param>
        /// <param name="treescope">the scope for filtering</param>
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
            if (TreeScopeEnum.Ancestors.Equals( treescope))
            {
                throw new NotImplementedException();
            }
            if (TreeScopeEnum.Sibling.Equals(treescope))
            {
                filteredSiblingsOfNode(idGenerated);
                return;
            }

            Object treeObjectOfIdGenerated = treeOperation.searchNodes.getNode(idGenerated, grantTrees.filteredTree).DeepCopy();

            String mainFS_userName= strategyMgr.getSpecifiedTree().GetData(treeObjectOfIdGenerated).properties.grantFilterStrategy;
            List<String> fsChildren = strategyMgr.getSpecifiedTree().GetData(treeObjectOfIdGenerated).properties.grantFilterStrategiesChildren;
            
            if (TreeScopeEnum.Children.Equals(treescope))
            {
                filterChild2(treeObjectOfIdGenerated);
            }
            else
            { // treescope == Subtree or Descendants
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
            OSMElements.OSMElement osmOfNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedId).DeepCopy();
            if (osmOfNode.Equals(new OSMElements.OSMElement())) { return; }
            String mainFilterstrategy = treeOperation.searchNodes.getMainFilterstrategyOfTree();
            String nodeFilterstrategy = treeOperation.searchNodes.getFilteredTreeOsmElementById(generatedId).properties.grantFilterStrategy;
            // if necessary changes the filter strategy
            if (nodeFilterstrategy != null && !mainFilterstrategy.Equals(Settings.strategyUserNameToClassName( nodeFilterstrategy)))
            {
                changeFilter = true;
                this.changeFilter(nodeFilterstrategy);
            }
            // filters and updates the node
            OSMElements.GeneralProperties properties = strategyMgr.getSpecifiedFilter().updateNodeContent(osmOfNode);
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
                OSMElements.OSMElement OSM = strategyMgr.getSpecifiedTree().GetData(child);
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
                    OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node).DeepCopy();
                    changeFilter(osmNode.properties.grantFilterStrategy);
                    // new filtering
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
        /// <param name="fs_children">a list of the filter strategies of the children nodes</param>
        private void filterChild(Object subtreeOld, String fs_main, List<String> fs_children)
        {
            if (fs_children == null || fs_children.Count == 0 || (fs_children.Count == 1 && fs_children[0].Equals(fs_main)))
            {
                // all nodes was filtered with the same filterstrategy
                return;
            }
            else
            {
                foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtreeOld))
                {
                    OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node).DeepCopy();
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

        private void filteredSiblingsOfNode(String nodeIdGenerated)
        {
            // use first the filterstrategy of the parent node for filtering
            object nodeObject = treeOperation.searchNodes.getNode(nodeIdGenerated, grantTrees.filteredTree);
            if(nodeObject == null) { return; }
            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject)) { return; }
            int branchIndexNode = strategyMgr.getSpecifiedTree().BranchIndex(nodeObject);
            Object parent = strategyMgr.getSpecifiedTree().Parent(nodeObject);
            Object parent_Old = strategyMgr.getSpecifiedTree().Parent(nodeObject).DeepCopy();
            OSMElements.OSMElement osmParent = strategyMgr.getSpecifiedTree().GetData(parent);
            String fs_parent = osmParent.properties.grantFilterStrategy;
            changeFilter(fs_parent);
            Object siblingNodesNew = strategyMgr.getSpecifiedFilter().filtering(nodeIdGenerated, TreeScopeEnum.Sibling);
            
            int depth = strategyMgr.getSpecifiedTree().Depth(parent) +1;

            foreach( Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(siblingNodesNew))
            { 
                OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                List<object> possibleOldNodes = treeOperation.searchNodes.getNodesByProperties(parent_Old, osm.properties);
                if (possibleOldNodes != null && possibleOldNodes.Count == 1)
                {
                    //use the old id
                    osm.properties.IdGenerated = strategyMgr.getSpecifiedTree().GetData(possibleOldNodes[0]).properties.IdGenerated;
                }
                else
                {
                    int bi = strategyMgr.getSpecifiedTree().BranchIndex(node);
                    if (bi >= branchIndexNode) { bi++; }
                    osm.properties.IdGenerated = treeOperation.generatedIds.generatedIdFilteredNode(node, depth, bi, osmParent.properties.IdGenerated);
                }
                Object oldNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(osm.properties.IdGenerated);
                if (oldNode != null && !oldNode.Equals(new OSMElements.OSMElement()))
                {
                    changePropertiesOfFilteredNode(osm.properties);
                }else
                {
                    //Attention: new siblings are possible                    
                    strategyMgr.getSpecifiedTree().AddChild(parent, osm); // TODO: ggf. muss noch die richtige "Stelle" (BranchIndex) bestimmt werden
                }
            }
            if (osmParent.properties.grantFilterStrategiesChildren.Count == 1 && osmParent.properties.grantFilterStrategiesChildren[0].Equals(fs_parent))
            {
                // all siblings should be filteredt with the same filterstrategy like his parent node --> :)
            }
            else
            {
                // new filtering
                foreach (Object nodeOld in strategyMgr.getSpecifiedTree().DirectChildrenNodes(parent_Old))
                { //Attention: it could be a problem to identify the correct node
                    OSMElements.OSMElement osmOld = strategyMgr.getSpecifiedTree().GetData(nodeOld);
                    if (!fs_parent.Equals(osmOld.properties.grantFilterStrategy))
                    {
                        changeFilter(osmOld.properties.grantFilterStrategy);

                        GeneralProperties prop = strategyMgr.getSpecifiedFilter().updateNodeContent(osmOld);
                        prop.IdGenerated = osmOld.properties.IdGenerated;
                        if (osmOld.properties.grantFilterStrategiesChildren != null && prop.grantFilterStrategiesChildren == null)
                        {
                            prop.grantFilterStrategiesChildren = osmOld.properties.grantFilterStrategiesChildren;
                        }
                        changePropertiesOfFilteredNode(prop);
                    }
                }
            }
            changeFilter(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(grantTrees.filteredTree)).properties.grantFilterStrategy); // main FilterStrategy

            setGrantFilterStrategiesChildren(nodeIdGenerated);
        }

        protected void changeFilter(String filterUserName)
        {
            // check if necessary
            String strategyClassName = Settings.strategyUserNameToClassName(filterUserName);
            String currentStrategy = strategyMgr.getSpecifiedFilter().GetType().FullName + ", " + strategyMgr.getSpecifiedFilter().GetType().Namespace;
            if (currentStrategy.Equals(strategyClassName)) { return; }
            strategyMgr.setSpecifiedFilter(strategyClassName);
            strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTrees);
            strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
        }

        #endregion

        #region update Braille node

        /// <summary>
        /// Changes the properties of the braille node
        /// </summary>
        /// <param name="element">the braille node which sould be updated</param>
        public void updateNodeOfBrailleUi(ref OSMElements.OSMElement element)
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
            if (element.brailleRepresentation.uiElementSpecialContent != null && element.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElements.UiElements.ListMenuItem)))
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
        
        #region refreshBrailleOSM
        /// <summary>
        /// Refreshs all Braille OSM elements depending on the given id of a node in the filtered tree
        /// </summary>
        /// <param name="idGeneratedOfFilteredNode">a node id of the filtered tree; the connected (Braille) nodes will be refreshed</param>
        /// <param name="treescopeOfFilteredNode">the tree scope for updating based on the filtered tree</param>
        /// <param name="onlyActiveScreen"><c>true</c> if only the Braille nodes on the active scree shold be updated, otherwise all connected Braille nodes will be updated</param>
        public void refreshBrailleOSM(string idGeneratedOfFilteredNode, TreeScopeEnum treescopeOfFilteredNode, bool onlyActiveScreen = true)
        {
            switch (treescopeOfFilteredNode)
            {
                case TreeScopeEnum.Ancestors:
                    throw new NotImplementedException();
                case TreeScopeEnum.Application:
                    refreshBrailleOSM_Application(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                case TreeScopeEnum.Children:
                    refreshBrailleOSM_Children(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                case TreeScopeEnum.Descendants:
                    refreshBrailleOSM_Descendants(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                case TreeScopeEnum.Element:
                    refreshBrailleOSM_Element(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                case TreeScopeEnum.Sibling:
                    refreshBrailleOSM_Siblings(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                case TreeScopeEnum.Subtree:
                    refreshBrailleOSM_subtree(idGeneratedOfFilteredNode, onlyActiveScreen);
                    break;
                default:
                    throw new Exception("For the TreeScopeEnum '" + treescopeOfFilteredNode + "' didn't exist a refresh methode.");
            }
        }

        private void refreshBrailleOSM_Application(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtreeBraille = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtreeBraille = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            Object subtreeFiltered = treeOperation.searchNodes.getNode(idGeneratedOfFilteredNode, grantTrees.filteredTree);
            List<String> connectedBrailleNodes = new List<string>();
            foreach(var con in grantTrees.osmTreeConnections)
            {
                connectedBrailleNodes.Add(con.BrailleTreeId);
            }
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtreeBraille ?? grantTrees.brailleTree);
        }

        private void refreshBrailleOSM_Children(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtreeBraille = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtreeBraille = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            Object subtreeFiltered = treeOperation.searchNodes.getNode(idGeneratedOfFilteredNode, grantTrees.filteredTree);
            List<String> connectedBrailleNodes = new List<string>();
            foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtreeFiltered))
            {
                OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node);
                List<String> tmpConnectetList = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                if (tmpConnectetList != null)
                {
                    connectedBrailleNodes.AddRange(tmpConnectetList);
                }
            }
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtreeBraille ?? grantTrees.brailleTree);
        }

        private void refreshBrailleOSM_Siblings(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtreeBraille = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtreeBraille = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            Object subtreeFiltered = treeOperation.searchNodes.getNode(idGeneratedOfFilteredNode, grantTrees.filteredTree);
            if (!strategyMgr.getSpecifiedTree().HasParent(subtreeFiltered)) { return; }
            Object subtreeFilteredParent = strategyMgr.getSpecifiedTree().Parent(subtreeFiltered);
            List <String> connectedBrailleNodes = new List<string>();
            foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtreeFilteredParent))
            {
                if (!node.Equals(subtreeFiltered))
                {
                    OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node);
                    List<String> tmpConnectetList = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                    if (tmpConnectetList != null)
                    {
                        connectedBrailleNodes.AddRange(tmpConnectetList);
                    }
                }
            }
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtreeBraille ?? grantTrees.brailleTree);
        }


        private void refreshBrailleOSM_Descendants(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtreeBraille = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtreeBraille = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            Object subtreeFiltered = treeOperation.searchNodes.getNode(idGeneratedOfFilteredNode, grantTrees.filteredTree);
            List<String> connectedBrailleNodes = new List<string>();
            foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtreeFiltered))
            {
                OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node);
                List<String> tmpConnectetList = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                if (tmpConnectetList != null)
                {
                    connectedBrailleNodes.AddRange(tmpConnectetList);
                }
            }
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtreeBraille ?? grantTrees.brailleTree);
        }

        private void refreshBrailleOSM_subtree(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtreeBraille = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtreeBraille = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            Object subtreeFiltered = treeOperation.searchNodes.getNode(idGeneratedOfFilteredNode, grantTrees.filteredTree);
            List<String> connectedBrailleNodes = new List<string>();
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtreeFiltered))
            {
                OSMElements.OSMElement osmNode = strategyMgr.getSpecifiedTree().GetData(node);
                List<String> tmpConnectetList = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                if(tmpConnectetList != null)
                {
                    connectedBrailleNodes.AddRange(tmpConnectetList);
                }
            }
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtreeBraille ?? grantTrees.brailleTree);
        }

        private void refreshBrailleOSM_Element(string idGeneratedOfFilteredNode, bool onlyActiveScreen = true)
        {
            Object subtree = null;
            if (onlyActiveScreen)
            {
                String activeScreenId = strategyMgr.getSpecifiedBrailleDisplay().getVisibleScreenId();
                subtree = treeOperation.searchNodes.getNode(activeScreenId, grantTrees.brailleTree);
            }
            List<String> connectedBrailleNodes = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(idGeneratedOfFilteredNode);
            if (connectedBrailleNodes == null || connectedBrailleNodes == new List<string>()) { Debug.WriteLine("There wasn't a connected Braille node!"); return; }
            refreshBrailleOSM(connectedBrailleNodes, subtree ?? grantTrees.brailleTree); // ??-Operator https://docs.microsoft.com/dotnet/csharp/language-reference/operators/null-conditional-operator
        }

        private void refreshBrailleOSM(List<String> brailleNodeIds, Object brailleSubtree)
        {
            if(brailleNodeIds == null || brailleSubtree == null) { return; }
            foreach(String BId in brailleNodeIds)
            {
                OSMElements.OSMElement osmNode = treeOperation.searchNodes.getNodeElement(BId, brailleSubtree);
                if(osmNode != null && !osmNode.Equals(new OSMElements.OSMElement()))
                {
                    updateNodeOfBrailleUi(ref osmNode);
                    strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(ref osmNode);
                }
            }
        }
        #endregion

        /// <summary>
        /// Sets the new braille representaion of the element
        /// </summary>
        /// <param name="element">the new representation</param>
        private void changeBrailleRepresentation(ref OSMElements.OSMElement element)
        {
            if (grantTrees.brailleTree == null) { return; }
            Object node = treeOperation.searchNodes.getNode(element.properties.IdGenerated, grantTrees.brailleTree);
            if(node != null)
            {
                strategyMgr.getSpecifiedTree().SetData(node, element);
            }
        }
        #endregion

        #region properties

        /// <summary>
        /// Sets/Changes a property of a node in the braille tree
        /// </summary>
        /// <param name="id">the id of the node to change his property</param>
        /// <param name="nameOfProperty">name of the property (<see cref="OSMElements.OSMElement.getAllTypes()"/>)</param>
        /// <param name="newProperty">the new value</param>
        /// <returns><c>true</c> if the property sets, otherwiese <c>false</c></returns>
        public bool setBrailleTreeProperty(String id, String nameOfProperty, Object newProperty)
        {
            if (nameOfProperty == null || !OSMElements.OSMElement.getAllTypes().Contains(nameOfProperty) || id == null || grantTrees.brailleTree == null)
            {
                return false;
            }
            OSMElements.OSMElement node = treeOperation.searchNodes.getBrailleTreeOsmElementById(id);
            if (node == new OSMElements.OSMElement()) { return false; }
            Object propertyValueCurrent = OSMElements.OSMElement.getElement(nameOfProperty, node);
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
                        treeOperation.osmTreeConnector.removeOsmConnection(connectedFilteredTreeId, node.properties.IdGenerated);
                    }
                }
            }
            // https://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
            OSMElements.OSMElement.setElement(nameOfProperty, newProperty, node);
            Object propertyValueNew = OSMElements.OSMElement.getElement(nameOfProperty, node);
            if (propertyValueCurrent == null && propertyValueNew == null) { return false; }
            if (propertyValueCurrent != null && propertyValueCurrent.Equals(propertyValueNew)) { return false; }
            else
            {
                if ((propertyValueNew == null && (newProperty == null || newProperty.ToString().Trim().Equals(""))) || propertyValueNew.ToString().Equals(newProperty.ToString()))
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
        /// <param name="elementName">the name of the property (<see cref="OSMElements.OSMElement.getAllTypes()"/>)</param>
        /// <param name="osmElement"></param>
        /// <returns>Value of a given Property; if <c>value == null</c> the result will be an empty String</returns>
        public static String getProperty(String elementName, OSMElements.OSMElement osmElement)
        {
            Object value = OSMElements.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }

        /// <summary>
        /// Value of a given Property
        /// </summary>
        /// <param name="elementName">the name of the property (<see cref="OSMElements.OSMElement.getAllTypes()"/>)</param>
        /// <param name="idOsmElement">Id of the OSM element</param>
        /// <returns>Value of a given Property; if the id dosen't exist or <c>value == null</c> the result will be an empty String</returns>
        public String getPropertyofBrailleTree(String elementName, String idOsmElement)
        {
            OSMElements.OSMElement osmElement = treeOperation.searchNodes.getBrailleTreeOsmElementById(idOsmElement);
            if (osmElement == new OSMElements.OSMElement()) { return ""; }
            Object value = OSMElements.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }

        /// <summary>
        /// Value of a given Property
        /// </summary>
        /// <param name="elementName">the name of the property (<see cref="OSMElements.OSMElement.getAllTypes()"/>)</param>
        /// <param name="idOsmElement">Id of the OSM element</param>
        /// <returns>Value of a given Property; if the id dosen't exist or <c>value == null</c> the result will be an empty String</returns>
        public String getPropertyofFilteredTree(String elementName, String idOsmElement)
        {
            OSMElements.OSMElement osmElement = treeOperation.searchNodes.getFilteredTreeOsmElementById(idOsmElement);
            if (osmElement == new OSMElements.OSMElement()) { return ""; }
            Object value = OSMElements.OSMElement.getElement(elementName, osmElement);
            return value != null ? value.ToString() : "";
        }

        /// <summary>
        /// Changes the <see cref="GeneralProperties"/> of a node
        /// </summary>
        /// <param name="properties">the new properties</param>
        internal void changePropertiesOfFilteredNode(GeneralProperties properties)
        {
            // List<Object> node = treeOperation.searchNodes.searchNodeByProperties(grantTrees.filteredTree, properties);
            Object node = treeOperation.searchNodes.getNode(properties.IdGenerated, grantTrees.filteredTree);
            if (node != null)
            {
                OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                if (osm.properties.grantFilterStrategiesChildren != null && properties.grantFilterStrategiesChildren == null)
                {
                    properties.grantFilterStrategiesChildren = osm.properties.grantFilterStrategiesChildren;
                }
                osm.properties = properties;

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
        private String getTextForView(OSMElements.OSMElement osmElementBraille)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmElementBraille.properties.IdGenerated);
            if (connectedIdFilteredTree == null)
            {
                Console.WriteLine("No matching object found.");
                return null;
            }
            OSMElements.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(connectedIdFilteredTree);
            String text = "";
            if (!associatedNode.Equals(new OSMElements.OSMElement()) && !osmElementBraille.brailleRepresentation.displayedGuiElementType.Trim().Equals(""))
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
        private bool? isUiElementEnable(OSMElements.OSMElement osmElementBraille)
        {
            String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmElementBraille.properties.IdGenerated);
            if (connectedIdFilteredTree == null)
            {
                Console.WriteLine("No matching object found.");
                return null;
            }
            OSMElements.OSMElement associatedNode = treeOperation.searchNodes.getFilteredTreeOsmElementById(connectedIdFilteredTree);
            if (!associatedNode.Equals(new OSMElements.OSMElement()))
            {
                return associatedNode.properties.isEnabledFiltered;
            }
            return null;
        }

        private bool changeTypeOfView(String id, String typeOfViewNew)
        {
            Object nodeObject = treeOperation.searchNodes.getNode(id, grantTrees.brailleTree);
            OSMElements.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
            //checks the position of the node

            if (!strategyMgr.getSpecifiedTree().HasParent(nodeObject))
            {
                #region the node is a "root" branch of a type of view
                if (!treeOperation.searchNodes.existTypeOfViewInTree(typeOfViewNew))
                {
                    //rename this node and all children
                    foreach (Object o in strategyMgr.getSpecifiedTree().AllNodes(nodeObject))
                    {
                        OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
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
                        OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(o);
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
                            OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(n);
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
            OSMElements.OSMElement nodeData = strategyMgr.getSpecifiedTree().GetData(nodeObject);
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
                        OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(view);
                        if (treeOperation.searchNodes.existViewInScreen(screenNameNew, data.brailleRepresentation.viewName, data.brailleRepresentation.typeOfView))
                        {
                            return false;
                        }
                    }
                    //  Screen exist BUT the viewS doesn't exist
                    //rename + move every view
                    foreach (Object node in strategyMgr.getSpecifiedTree().AllChildrenNodes(nodeObject))
                    {
                        OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
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
                        OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
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
                    OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(view);
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
        private bool isCheckboxOfAssociatedElementSelected(OSMElements.OSMElement elementBraille)
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
                    OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
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
                OSMElements.OSMElement osmScreen = strategyMgr.getSpecifiedTree().GetData(navigationBarSubtree);
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
            OSMElements.OSMElement osmChild = strategyMgr.getSpecifiedTree().GetData(childObject);
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
                    OSMElements.OSMElement osmParent = strategyMgr.getSpecifiedTree().GetData(childObject);
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
                                        OSMElements.OSMElement osmSibling = strategyMgr.getSpecifiedTree().GetData(sibling);

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
        public String addNodeInBrailleTree(OSMElements.OSMElement brailleNode, String parentId = null)
        {
            if (grantTrees.brailleTree == null) { grantTrees.brailleTree = strategyMgr.getSpecifiedTree().NewTree(); }
            //checks wether the node already exists
            if (brailleNode.properties.IdGenerated != null && !brailleNode.properties.IdGenerated.Equals(""))
            {
                OSMElements.OSMElement nodeToRemove = treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
                if (!nodeToRemove.Equals(new OSMElements.OSMElement()))
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
                                    OSMElements.OSMElement data = strategyMgr.getSpecifiedTree().GetData(childOfNode);

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
            OSMElements.OSMElement osmTypeOfView = new OSMElements.OSMElement();
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

            OSMElements.OSMElement osmScreen = new OSMElements.OSMElement();
            osmScreen.brailleRepresentation.screenName = screenName;
            osmScreen.brailleRepresentation.typeOfView = typeOfView;
            osmScreen.properties.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);

            //the screen dosn't exist
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen))
            {
                OSMElements.OSMElement osmViewCategory = new OSMElements.OSMElement();
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
            OSMElements.OSMElement dataOfNodeToRemove = strategyMgr.getSpecifiedTree().GetData(nodeToRemove);
            if (dataOfNodeToRemove.Equals(new OSMElements.OSMElement())) { return false; }
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
                List<OSMElements.OSMElement> listToRemove = new List<OSMElements.OSMElement>();
                Object childeens = strategyMgr.getSpecifiedTree().Child(parentSubtree);
                //grantTrees.brailleTree.Remove(childeens.Data);
                listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                while (strategyMgr.getSpecifiedTree().HasNext(childeens))
                {
                    childeens = strategyMgr.getSpecifiedTree().Next(childeens);
                    //grantTrees.brailleTree.Remove(childeens.Data);
                    listToRemove.Add(strategyMgr.getSpecifiedTree().GetData(childeens));
                }
                foreach (OSMElements.OSMElement osm in listToRemove)
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
                        treeOperation.osmTreeConnector.removeOsmConnection(connectedIdFilteredTree, strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated);
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
        public void RemoveNodeAndConnection(OSMElements.OSMElement osmNode)
        {
            if (osmNode.brailleRepresentation.Equals(new BrailleRepresentation()))
            {
                // filtered node
                List<String> connectedIdsBrailleTree = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(osmNode.properties.IdGenerated);
                if (connectedIdsBrailleTree != null && treeOperation.osmTreeConnector != null)
                {
                    foreach (String id in connectedIdsBrailleTree)
                    {
                        treeOperation.osmTreeConnector.removeOsmConnection(osmNode.properties.IdGenerated, id);
                    }
                }
                strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, osmNode);
            }
            else
            {
                // braille node
                String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(osmNode.properties.IdGenerated);
                if (connectedIdFilteredTree != null && treeOperation.osmTreeConnector != null)
                {
                    treeOperation.osmTreeConnector.removeOsmConnection(osmNode.properties.IdGenerated, connectedIdFilteredTree);
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
                OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(nodeObject);
                if (osm == null) { return; }
                List<OsmTreeConnectorTuple> connectionsToDel;
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
                if (connectionsToDel != null && !connectionsToDel.Equals(new List<OsmTreeConnectorTuple>()))
                {
                    foreach (OsmTreeConnectorTuple connection in connectionsToDel)
                    {
                        treeOperation.osmTreeConnector.removeOsmConnection(connection);
                    }
                }
                strategyMgr.getSpecifiedTree().Remove(nodeObject);

            }
        }
        private List<OsmTreeConnectorTuple> getAllConnectionsOfSubtree(Object subtree, bool isFilteredTree)
        {
            List<OsmTreeConnectorTuple> connections = new List<OsmTreeConnectorTuple>();
            if (isFilteredTree) {
                foreach (Object obj in strategyMgr.getSpecifiedTree().AllNodes(subtree))
                {
                    //treeOperation.searchNodes.getConnectedBrailleTreenodeIds(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated);
                    List<OsmTreeConnectorTuple> tmpConnections = grantTrees.osmTreeConnections.FindAll(r => r.FilteredTreeId.Equals(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated));
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
                    OsmTreeConnectorTuple tmpConnection = grantTrees.osmTreeConnections.Find(r => r.BrailleTreeId.Equals(strategyMgr.getSpecifiedTree().GetData(obj).properties.IdGenerated));
                    if (tmpConnection != null && !tmpConnection.Equals(new OsmTreeConnectorTuple()))
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
