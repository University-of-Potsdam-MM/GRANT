using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTApplication;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;
using GRANTManager.TreeOperations;

namespace GRANTExample
{
    public class ExampleTree
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTree;
        TreeOperation treeOperation;

        public ExampleTree(StrategyManager mgr, GeneratedGrantTrees tree, TreeOperation treeOperation)
        {
            strategyMgr = mgr;
            grantTree = tree;
            this.treeOperation = treeOperation;
        }

        public String filterNodeOfApplicatione()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IntPtr points = strategyMgr.getSpecifiedOperationSystem().getHWND();
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                    int pointX;
                    int pointY;
                    strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                    Object tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Element, 0);
                  /*  if (grantTree.filteredTree != null)
                    {
                        treeOperation.updateNodes.changePropertiesOfFilteredNode(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties);
                    }*/
                   // strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                    if (strategyMgr.getSpecifiedTree().HasChild(tree)== true)
                    {
                        return printProperties(strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties);
                    }
                    return "";
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }               
            }
            return "";
        }

        public String filterSubtreeOfApplication()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IntPtr points = strategyMgr.getSpecifiedOperationSystem().getHWND();
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                    int pointX;
                    int pointY;
                    strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                    Object tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Descendants, -1);
                    if (grantTree.filteredTree != null)
                    {
                        List<Object> result = treeOperation.searchNodes.getNodesByProperties(grantTree.filteredTree, strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(tree)).properties, OperatorEnum.and);
                        if (result.Count == 1)
                        {
                            GuiFunctions guiFunctions = new GuiFunctions(strategyMgr, grantTree, treeOperation);
                            treeOperation.updateNodes.filterSubtreeWithCurrentFilterStrtegy(strategyMgr.getSpecifiedTree().GetData(result[0]).properties.IdGenerated);

                          // guiFunctions.filterAndAddSubtreeOfApplication(strategyMgr.getSpecifiedTreeOperations().getFilteredTreeOsmElementById("7CA0B5B9845D7906E3BD235A600F3546"));
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
            return "";
        }



        private String printProperties(GeneralProperties properties)
        {
            String resultString = "Knoten: \n";
            Console.WriteLine("\nProperties:");
            if (properties.localizedControlTypeFiltered != null)
            {
                resultString = resultString + " localizedControlTypeFiltered: " + properties.localizedControlTypeFiltered +"\n";
            }
            if (properties.nameFiltered != null)
            {
                resultString = resultString + "nameFiltered: " + properties.nameFiltered + "\n";
            }
            if (properties.boundingRectangleFiltered != null && properties.boundingRectangleFiltered != new System.Windows.Rect())
            {
                resultString = resultString + "boundingRectangleFiltered: " + properties.boundingRectangleFiltered + "\n";
            }
            if (properties.helpTextFiltered != null)
            {
                resultString = resultString + "helpTextFiltered: " + properties.helpTextFiltered + "\n";
            }
            if (properties.IdGenerated != null)
            {
                resultString = resultString + "IdGenerated: " + properties.IdGenerated + "\n";
            }
            if (properties.hWndFiltered != null)
            {
                resultString = resultString + "hWndFiltered: " + properties.hWndFiltered + "\n";
            }
            if (properties.isContentElementFiltered != null)
            {
                resultString = resultString + "isContentElementFiltered: " + properties.isContentElementFiltered + "\n";
            }
            if (properties.valueFiltered != null)
            {
                resultString = resultString + "valueFiltered: " + properties.valueFiltered + "\n";
            }
            if(properties.grantFilterStrategiesChildren != null)
            {
                resultString = resultString + "grantFilterStrategiesChildren" + String.Join(" : ", properties.grantFilterStrategiesChildren.Select(p => p.ToString()).ToArray());
            }
            return resultString;
        }


        public void filterTreeOfApplication()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IntPtr points = strategyMgr.getSpecifiedOperationSystem().getHWND();
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();
                   // filterStrategy.setStrategyMgr(strategyMgr);
                    //ITreeStrategy<OSMElement.OSMElement> tree1 = filterStrategy.filtering(operationSystemStrategy.getProcessHwndFromHwnd(filterStrategy.deliverElementID(points)));
                    // strategyMgr.setFilteredTree(tree1);
                    int pointX;
                    int pointY;
                    strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                    Object tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Application);
                   // strategyMgr.getSpecifiedTreeOperations().printTreeElements(parentBrailleTreeNode, -1);
                    grantTree.filteredTree = tree;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

        public void searchPropertie(String localizedControlTypeFiltered)
        {
            if (grantTree.filteredTree == null)
            {
                    filterTreeOfApplication();
            }
            GeneralProperties searchedProperties = new GeneralProperties();
            searchedProperties.localizedControlTypeFiltered = localizedControlTypeFiltered.Equals("") ? "Schaltfläche" : localizedControlTypeFiltered;
            //  searchedProperties.nameFiltered = "";

            Console.Write("Gesuchte Eigenschaften ");
            treeOperation.searchNodes.getNodesByProperties(grantTree.filteredTree, searchedProperties, OperatorEnum.or);

        }

        /// <summary>
        /// Setzt die Beziehung von dem "realen" GUI Element zu dem UI-Element auf der Stiftplatte umd die Darstellung eines Screenshots zu testen
        /// falls noch kein Baum gefiltert wurde, so wird die Anwendung gefiltert
        /// </summary>
        public void setOSMRelationshipImg()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                    if (grantTree.filteredTree == null)
                    {
                        filterTreeOfApplication();
                    }
                    if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
                    {
                        int pointX;
                        int pointY;

                        strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                        OSMElement.OSMElement osmElement = filterStrategy.getOSMElement(pointX, pointY);
                        GeneralProperties propertiesForSearch = new GeneralProperties();
                        propertiesForSearch.controlTypeFiltered = "Screenshot";
                        List<Object> treeElement = treeOperation.searchNodes.getNodesByProperties(grantTree.brailleTree, propertiesForSearch, OperatorEnum.and);
                        if (treeElement.Count > 0)
                        {
                            treeOperation.osmTreeConnector.addOsmConnection(osmElement.properties.IdGenerated, strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

        /// <summary>
        /// Setzt die Beziehung von dem "realen" GUI Element zu dem UI-Element auf der Stiftplatte mit der Id = "braille123_3"
        /// falls noch kein Baum gefiltert wurde, so wird die Anwendung gefiltert
        /// </summary>
        public void setOSMRelationship()
        {
            if (grantTree.brailleTree == null) { return; }
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                    if (grantTree.filteredTree == null)
                    {
                        filterTreeOfApplication();
                    }
                    if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
                    {
                        int pointX;
                        int pointY;

                        strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                        OSMElement.OSMElement osmElement = filterStrategy.getOSMElement(pointX, pointY);
                        GeneralProperties propertiesForSearch = new GeneralProperties();
                        propertiesForSearch.controlTypeFiltered = "TextBox";
                        List<Object> treeElement = treeOperation.searchNodes.getNodesByProperties(grantTree.brailleTree, propertiesForSearch, OperatorEnum.and);
                        if (treeElement.Count > 0)
                        { //für Testzwecke wird einfach das erste Element genutzt
                            //   OsmTreeRelationship.addOsmConnection(filteredSubtree.properties.IdGenerated, "braille123_3", ref relationship);
                            //  OsmTreeRelationship.addOsmConnection(filteredSubtree.properties.IdGenerated, "braille123_5", ref relationship);
                            treeOperation.osmTreeConnector.setOsmConnection(osmElement.properties.IdGenerated, strategyMgr.getSpecifiedTree().GetData(treeElement[0]).properties.IdGenerated);
                            //  OsmTreeRelationship.setOsmConnection(filteredSubtree.properties.IdGenerated, "braille123_11", ref relationshipList);
                        }

                      
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

        /// <summary>
        /// Wechselt zwischen dem UIA-Filter und dem UIA2-displayStrategy
        /// </summary>
        public void changeFilter()
        {
            Type currentFilter = strategyMgr.getSpecifiedFilter().GetType();
            List<Strategy> possibleFilter = Settings.getPossibleFilters();
            if (currentFilter == Type.GetType(possibleFilter[0].className))
            {
                strategyMgr.setSpecifiedFilter(possibleFilter[2].className);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                Console.WriteLine("Die Filter-Strategy wurde auf {0} gewechselt", possibleFilter[2].userName);
            }
            else
            {
                strategyMgr.setSpecifiedFilter(possibleFilter[0].className);
                strategyMgr.getSpecifiedFilter().setGeneratedGrantTrees(grantTree);
                strategyMgr.getSpecifiedFilter().setTreeOperation(treeOperation);
                Console.WriteLine("Die Filter-Strategy wurde auf {0} gewechselt", possibleFilter[0].userName);
            }

            //String cUserFilterName = possibleFilter[2].userName; // der Filter muss dynamisch ermittelt werden
            //strategyMgr.setSpecifiedFilter(settings.strategyUserNameToClassName(cUserFilterName));
            //Console.WriteLine("Die Filter-Strategy wurde auf {0} gewechselt", cUserFilterName);
        }

       /* private List<OsmConnector<String, String>> setOsmConnection(String guiID)
        {
            if (strategyMgr.osmRelationship == null)
            {
                //List<OsmConnector<String, String>> relationships = new List<OsmConnector<String, String>>();
                strategyMgr.setOsmConnection(new List<OsmConnector<String, String>>());
            }
            OsmConnector<String, String> r3 = new OsmConnector<String, String>();
            r3.FilteredTree = guiID;
            r3.BrailleTree =  "braille123_3";
            strategyMgr.osmRelationship.Add(r3);
           // relationships.Add(r3);
            return strategyMgr.osmRelationship;
        }*/

        public static List<OsmTreeConnectorTuple> setOsmRelationship()
        {
            List<OsmTreeConnectorTuple> relationships = new List<OsmTreeConnectorTuple>();
            OsmTreeConnectorTuple r1 = new OsmTreeConnectorTuple();
            r1.FilteredTreeId = "461FD37218F2E2BCBE4C5486629A2FC6"; //Notepad;
            r1.BrailleTreeId = "braille123_1";
            OsmTreeConnectorTuple r2 = new OsmTreeConnectorTuple();
            r2.FilteredTreeId = "gui123_2";
            r2.BrailleTreeId = "braille123_2";
            OsmTreeConnectorTuple r3 = new OsmTreeConnectorTuple();
            r3.FilteredTreeId = "6941463181BDAA498DBC02B4164EF1AA";
            r3.BrailleTreeId = "braille123_3";

            relationships.Add(r1);
            // relationships.Add(r2);
            relationships.Add(r3);
            return relationships;
        }

    }
}
