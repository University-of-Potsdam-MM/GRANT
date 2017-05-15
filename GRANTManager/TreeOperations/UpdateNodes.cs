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
            if (child.appPath != null && !fileNameNew.Equals(child.appPath))
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
        /// Changes the <see cref="GeneralProperties"/> of a node;
        /// but not the "IdGenerated" 
        /// </summary>
        /// <param name="properties">the new properties</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties)
        {
            List<Object> node = treeOperation.searchNodes.searchProperties(grantTrees.filteredTree, properties);
            if (node.Count == 1)
            {
                OSMElement.OSMElement osm = new OSMElement.OSMElement();
                osm.brailleRepresentation = strategyMgr.getSpecifiedTree().GetData(node[0]).brailleRepresentation;
                osm.events = strategyMgr.getSpecifiedTree().GetData(node[0]).events;
                properties.IdGenerated = strategyMgr.getSpecifiedTree().GetData(node[0]).properties.IdGenerated;
                osm.properties = properties;
                strategyMgr.getSpecifiedTree().SetData(node[0], osm);
                return;
            }
            Debug.WriteLine("TODO");
        }

        /// <summary>
        /// Changes the <see cref="GeneralProperties"/> of a node and also the "IdGenerated"
        /// </summary>
        /// <param name="properties">the new properties</code></param>
        /// <param name="idGeneratedOld">the old id</param>
        public void changePropertiesOfFilteredNode(GeneralProperties properties, String idGeneratedOld)
        {
            foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.filteredTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated.Equals(idGeneratedOld))
                {
                    OSMElement.OSMElement osm = new OSMElement.OSMElement();
                    osm.brailleRepresentation = strategyMgr.getSpecifiedTree().GetData(node).brailleRepresentation;
                    osm.events = strategyMgr.getSpecifiedTree().GetData(node).events;
                    properties.IdGenerated = idGeneratedOld;
                    osm.properties = properties;
                    strategyMgr.getSpecifiedTree().SetData(node, osm);
                    break;
                }
            }
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
            BrailleRepresentation updatedContentBR = element.brailleRepresentation;
            GeneralProperties updatedContentGP = element.properties;
            if (element.brailleRepresentation.isGroupChild && element.brailleRepresentation.groupelementsOfSameType.renderer != null)
            {
                updatedContentGP.controlTypeFiltered = element.brailleRepresentation.groupelementsOfSameType.renderer; //TODO: passt das so?
            }
            if (!strategyMgr.getSpecifiedBrailleDisplay().getAllUiElementRenderer().Contains(updatedContentGP.controlTypeFiltered))
            {
                Debug.WriteLine("Attention: The chosen renderer dosn't exist. Now the renderer is 'Text'.");
                updatedContentGP.controlTypeFiltered = "Text";
            }
            if (element.brailleRepresentation.displayedGuiElementType != null)
            {
                updatedContentGP.valueFiltered = getTextForView(element);
            }
            if (element.brailleRepresentation.uiElementSpecialContent != null && element.brailleRepresentation.uiElementSpecialContent.GetType().Equals(typeof(OSMElement.UiElements.ListMenuItem)))
            {
                ListMenuItem listMenuItem = (ListMenuItem)element.brailleRepresentation.uiElementSpecialContent;
                updatedContentGP.isToggleStateOn = isCheckboxOfAssociatedElementSelected(element);
            }            
            bool? isEnable = isUiElementEnable(element);
            if (isEnable != null)
            {
                updatedContentGP.isEnabledFiltered = (bool)isEnable;
            }
            if (updatedContentBR.displayedGuiElementType != null && updatedContentBR.displayedGuiElementType != "" && !GeneralProperties.getAllTypes().Contains(updatedContentBR.displayedGuiElementType))
            {
                Debug.WriteLine("Attention: The chosen property for 'displayedGuiElementType' dosn't exist. Therefore, 'nameFiltered' is set.");
                updatedContentBR.displayedGuiElementType = "nameFiltered";
            }

            element.brailleRepresentation = updatedContentBR;
            element.properties = updatedContentGP;
            changeBrailleRepresentation(ref element);// here is the element already changed
        }

        /// <summary>
        /// Determines whether connected checkbox of a braille node is selected 
        /// </summary>
        /// <param name="element">the OSM element of the braille tree</param>
        /// <returns><c>true</c> if the checkbox in the connected filtered node selected</returns>
        private bool isCheckboxOfAssociatedElementSelected(OSMElement.OSMElement element)
        {
            OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(element.properties.IdGenerated));
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
            OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
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
            OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(osmElement.properties.IdGenerated));
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
        /// if a node with this d already exists, the node will be updated
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
            // prüfen, ob es die View auf dem Screen schon gibt
            if (treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.viewName, brailleNode.brailleRepresentation.typeOfView))
            {
                Debug.WriteLine("Attention: The view is already exists. The node wasn't added."); return null;
            }
            addSubtreeOfScreen(brailleNode.brailleRepresentation.screenName, brailleNode.brailleRepresentation.typeOfView);

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
                            OSMElement.OSMElement brailleNodeWithId = brailleNode;
                            GeneralProperties prop = brailleNode.properties;
                            prop.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(brailleNode);
                            brailleNodeWithId.properties = prop;
                            if (parentId == null)
                            {
                                strategyMgr.getSpecifiedTree().AddChild(node, brailleNodeWithId);
                                return prop.IdGenerated;
                            }
                            else
                            {
                                foreach (object childOfNode in strategyMgr.getSpecifiedTree().DirectChildrenNodes(node))
                                {
                                    OSMElement.OSMElement data = strategyMgr.getSpecifiedTree().GetData(childOfNode);
                                    
                                    if (!data.properties.Equals(new GeneralProperties()) && data.properties.IdGenerated != null && data.properties.IdGenerated.Equals(parentId))
                                    {
                                        strategyMgr.getSpecifiedTree().AddChild(childOfNode, brailleNodeWithId);
                                        return prop.IdGenerated;
                                    }
                                }
                                strategyMgr.getSpecifiedTree().AddChild(node, brailleNodeWithId);
                                return prop.IdGenerated;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Adds a branch for this Screen to the Root node, if the screen doesn't exists
        /// </summary>
        /// <param name="screenName">the name of the screen</param>
        /// <param name="typeOfView">the type of the view</param>
        private void addSubtreeOfScreen(String screenName, String typeOfView)
        {
            if (screenName == null || screenName.Equals(""))
            {
                Debug.WriteLine("No ScreenName is specified. The screen node wasn't added.");
                return;
            }
            OSMElement.OSMElement osmScreen = new OSMElement.OSMElement();
            BrailleRepresentation brailleScreen = new BrailleRepresentation();
            brailleScreen.screenName = screenName;
            brailleScreen.typeOfView = typeOfView;
            osmScreen.brailleRepresentation = brailleScreen;
            GeneralProperties propOsmScreen = new GeneralProperties();
            propOsmScreen.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmScreen);
            osmScreen.properties = propOsmScreen;

            //the scree dosn't exist
            if (!strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osmScreen))
            {
                OSMElement.OSMElement osmViewCategory = new OSMElement.OSMElement();
                BrailleRepresentation brailleViewCategory = new BrailleRepresentation();
                brailleViewCategory.typeOfView = typeOfView;
                osmViewCategory.brailleRepresentation = brailleViewCategory;
                GeneralProperties propViewCategory = new GeneralProperties();
                propViewCategory.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmViewCategory);
                osmViewCategory.properties = propViewCategory;
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
                strategyMgr.getSpecifiedTree().AddChild(typeOfViewSubtree, osmScreen);
            }
        }


        /// <summary>
        /// Removes a node of the braille tree
        /// </summary>
        /// <param name="brailleNode">the OSM element of the node to remove</param>
        public void removeNodeInBrailleTree(OSMElement.OSMElement brailleNode)
        {
            if (grantTrees.brailleTree == null)
            {
                Debug.WriteLine("The tree is empty"); return;
            }

            //checks whether the node exist
            OSMElement.OSMElement nodeToRemove = treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNode.properties.IdGenerated);
            if (nodeToRemove.Equals(new OSMElement.OSMElement()))
            {
                Debug.WriteLine("The node dosen't exist!"); return;
            }
            strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, nodeToRemove);
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
                                strategyMgr.getSpecifiedTree().Remove(grantTrees.filteredTree, strategyMgr.getSpecifiedTree().GetData(node));
                                strategyMgr.getSpecifiedTree().InsertChild(parentNode, subtree);
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
                        OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
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
                    OsmConnector<String, String> osmRelationship = grantTrees.osmRelationship.Find(r => r.BrailleTree.Equals(strategyMgr.getSpecifiedTree().GetData(node).properties.IdGenerated));
                    if (osmRelationship != null)
                    {
                        List<OsmConnector<String, String>> conector = grantTrees.osmRelationship;
                        OsmTreeConnector.removeOsmConnection(osmRelationship.FilteredTree, osmRelationship.BrailleTree, ref conector);
                        removeNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(node));
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
                    GeneralProperties prop = strategyMgr.getSpecifiedTree().GetData(node).properties;
                    if (strategyMgr.getSpecifiedTree().GetData(node).properties.valueFiltered.Equals(screenName))
                    {
                        prop.isEnabledFiltered = false;
                    }
                    else
                    {
                        prop.isEnabledFiltered = true;
                    }
                    OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(node);
                    osm.properties = prop;
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
