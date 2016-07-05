using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTApplication;
using StrategyManager;
using StrategyManager.Interfaces;
using OSMElement;

namespace GRANTExample
{
    public class ExampleTree
    {
        StrategyMgr strategyMgr;
        public ExampleTree(StrategyMgr mgr)
        {
            strategyMgr = mgr;
        }

        public String filterNodOfApplicatione()
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
                    ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Element, 0);
                    strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                    if (tree.HasChild != null)
                    {
                        return printProperties(tree.Child.Data.properties);
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
                    ITreeStrategy<OSMElement.OSMElement> tree = filterStrategy.filtering(pointX, pointY, TreeScopeEnum.Application, 0);
                    strategyMgr.getSpecifiedTreeOperations().printTreeElements(tree, -1);
                    strategyMgr.setFilteredTree(tree);

                  //  baumSchleife(tree);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

        private void baumSchleife(ITreeStrategy<OSMElement.OSMElement> tree)
        {
            
            ITreeStrategy<OSMElement.OSMElement> node1;
            while (tree.HasChild && !(tree.Count == 1 && tree.Depth == -1))
            {
                node1 = tree.Child;
                Console.WriteLine("Name: {0}, Type: {1}", node1.Data.properties.nameFiltered, tree.Data.properties.localizedControlTypeFiltered);
                baumSchleife(node1);
            }
            while (tree.HasNext)
            {
                node1 = tree.Next;
                Console.WriteLine("Name: {0}, Type: {1}", node1.Data.properties.nameFiltered, tree.Data.properties.localizedControlTypeFiltered);
                baumSchleife(node1);
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                //baumSchleife(tree);
                return;
            }
            if (!tree.HasChild)
            {
                node1 = tree;
                if (tree.HasParent)
                {
                    node1.Remove();
                }
            }
            if (!tree.HasNext && !tree.HasParent)
            {
                if (tree.HasPrevious)
                {
                    node1 = tree;
                    node1.Remove();
                }
            }
        }

        public void searchPropertie(String localizedControlTypeFiltered)
        {
            if (strategyMgr.getFilteredTree() == null)
            {
                    filterTreeOfApplication();
            }
            GeneralProperties searchedProperties = new GeneralProperties();
            searchedProperties.localizedControlTypeFiltered = localizedControlTypeFiltered.Equals("") ? "Schaltfläche" : localizedControlTypeFiltered;
            //  searchedProperties.nameFiltered = "";

            Console.Write("Gesuchte Eigenschaften ");
            strategyMgr.getSpecifiedTreeOperations().searchProperties(strategyMgr.getFilteredTree(), searchedProperties, OperatorEnum.or);

        }

        /// <summary>
        /// Setzt die Beziehung von dem "realen" GUI Element zu dem UI-Element auf der Stiftplatte mit der Id = "braille123_3"
        /// falls noch kein Baum gefiltert wurde, so wird die Anwendung gefiltert
        /// </summary>
        public void setOSMRelationship()
        {
            if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
            {
                try
                {
                    IFilterStrategy filterStrategy = strategyMgr.getSpecifiedFilter();

                    if (strategyMgr.getFilteredTree() == null)
                    {
                        filterTreeOfApplication();
                    }
                    if (strategyMgr.getSpecifiedOperationSystem().deliverCursorPosition())
                    {
                        int pointX;
                        int pointY;

                        strategyMgr.getSpecifiedOperationSystem().getCursorPoint(out pointX, out pointY);
                        OSMElement.OSMElement osmElement = filterStrategy.setOSMElement(pointX, pointY);

                        List<OsmRelationship<String, String>> relationshipList = strategyMgr.getOsmRelationship();
                     //   OsmTreeRelationship.addOsmRelationship(osmElement.properties.IdGenerated, "braille123_3", ref relationship);
                      //  OsmTreeRelationship.addOsmRelationship(osmElement.properties.IdGenerated, "braille123_5", ref relationship);
                        OsmTreeRelationship.setOsmRelationship(osmElement.properties.IdGenerated, "braille123_5", ref relationshipList);
                        strategyMgr.setOsmRelationship(relationshipList);
                      
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: '{0}'", ex);
                }
            }
        }

       /* private List<OsmRelationship<String, String>> setOsmRelationship(String guiID)
        {
            if (strategyMgr.getOsmRelationship() == null)
            {
                //List<OsmRelationship<String, String>> relationships = new List<OsmRelationship<String, String>>();
                strategyMgr.setOsmRelationship(new List<OsmRelationship<String, String>>());
            }
            OsmRelationship<String, String> r3 = new OsmRelationship<String, String>();
            r3.FilteredTree = guiID;
            r3.BrailleTree =  "braille123_3";
            strategyMgr.getOsmRelationship().Add(r3);
           // relationships.Add(r3);
            return strategyMgr.getOsmRelationship();
        }*/

        public static List<OsmRelationship<String, String>> setOsmRelationship()
        {
            List<OsmRelationship<String, String>> relationships = new List<OsmRelationship<String, String>>();
            OsmRelationship<String, String> r1 = new OsmRelationship<String, String>();
            r1.FilteredTree = "461FD37218F2E2BCBE4C5486629A2FC6"; //Notepad;
            r1.BrailleTree = "braille123_1";
            OsmRelationship<String, String> r2 = new OsmRelationship<String, String>();
            r2.FilteredTree = "gui123_2";
            r2.BrailleTree = "braille123_2";
            OsmRelationship<String, String> r3 = new OsmRelationship<String, String>();
            r3.FilteredTree = "6941463181BDAA498DBC02B4164EF1AA";
            r3.BrailleTree = "braille123_3";

            relationships.Add(r1);
            // relationships.Add(r2);
            relationships.Add(r3);
            return relationships;
        }

    }
}
