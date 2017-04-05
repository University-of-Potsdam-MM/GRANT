using GRANTManager;
using GRANTManager.TreeOperations;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TemplateTextview
{
    public class Textview
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        private int shiftingPerDepth = 2;

        public Textview(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        //Achtung: gibt noch Probleme, wenn die View zwei mal nacheinander erstellt wird
        public void createTextviewOfSubtree(Object subtree, int startYPosition = 0)
        {

             foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
             {
                 createBrailleGroupFromFilteredNode(node, ref startYPosition);
             }
        }

        private OSMElement.OSMElement addSeparatorElementInBrailleTree(String separator, ref int startX, int startY)
        {
            TextviewObject tvo = grantTrees.TextviewObject;

            OSMElement.OSMElement osmBraille = new OSMElement.OSMElement();
            GeneralProperties propBraille = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            braille.isVisible = true;
            propBraille.controlTypeFiltered = "Text";
            braille.isGroupChild = true;
            braille.typeOfView = tvo.viewCategory;
            braille.screenName = tvo.screenName;
            braille.viewName = "separator";
            propBraille.valueFiltered = separator;
            propBraille.boundingRectangleFiltered = new Rect(startX, startY, separator.Length *3, 5);//TODO: richtig machen

            osmBraille.properties = propBraille;
            osmBraille.brailleRepresentation = braille;
            propBraille.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmBraille);
            osmBraille.properties = propBraille;
            return osmBraille;
        }


        private OSMElement.OSMElement addElementInBrailleTree(object filteredNode, TextviewElement tve, int width, ref int startXPosition, int startYPosition)
        {
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(filteredNode);
            int depth = strategyMgr.getSpecifiedTree().Depth(filteredNode) * shiftingPerDepth;
            TextviewObject tvo = grantTrees.TextviewObject;

            OSMElement.OSMElement osmBraille = new OSMElement.OSMElement();
            GeneralProperties propBraille = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            braille.isVisible = true;
            propBraille.controlTypeFiltered = "Text";
            braille.isGroupChild = true;
            braille.typeOfView = tvo.viewCategory;
            braille.screenName = tvo.screenName;
            braille.displayedGuiElementType = tve.property;
            braille.viewName = osmFiltered.properties.IdGenerated + "_" + tve.order;

            propBraille.boundingRectangleFiltered = new Rect(startXPosition, startYPosition, width, 5);//TODO: richtig machen

            osmBraille.properties = propBraille;
            osmBraille.brailleRepresentation = braille;
            propBraille.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmBraille);
            osmBraille.properties = propBraille;
            List<OsmConnector<String, String>> relationship = grantTrees.osmRelationship;
            OsmTreeConnector.addOsmConnection(osmFiltered.properties.IdGenerated, propBraille.IdGenerated, ref relationship);
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref osmBraille);
            return osmBraille;

        }

        private void createBrailleGroupFromFilteredNode(object filteredNode, ref int startYPosition)
        {
            //Erst einen eigenen Teilbaum mit allen Kindern erzeugen und diesen anschließend dem Braille-Baum hinzufügen
            OSMElement.OSMElement osmGroup = new OSMElement.OSMElement();
            GeneralProperties propGroup = new GeneralProperties();
            BrailleRepresentation brailleGroup = new BrailleRepresentation();
            brailleGroup.isVisible = true;
            propGroup.controlTypeFiltered = "GroupElement";
            
            brailleGroup.typeOfView = grantTrees.TextviewObject.viewCategory;
            brailleGroup.screenName = grantTrees.TextviewObject.screenName;
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(filteredNode);
            brailleGroup.viewName = osmFiltered.properties.IdGenerated;
            int depth = strategyMgr.getSpecifiedTree().Depth(filteredNode) * shiftingPerDepth;
            int startXPosition = depth;
            propGroup.boundingRectangleFiltered = new Rect(startXPosition, startYPosition, 120 - startXPosition, 5);//TODO: richtig machen

            osmGroup.brailleRepresentation = brailleGroup;
            osmGroup.properties = propGroup;
            propGroup.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmGroup);
            String brailleParentId = null;
            if (treeOperation.searchNodes.getBrailleTreeOsmElementById(propGroup.IdGenerated).Equals( new OSMElement.OSMElement()))
            {
                object parent = strategyMgr.getSpecifiedTree().Parent(filteredNode);
                
                if(parent != null)
                {
                    List<string> ids = treeOperation.searchNodes.getConnectedBrailleTreenodeIds(strategyMgr.getSpecifiedTree().GetData(parent).properties.IdGenerated);
                    if (ids != null && ids.Count > 0)
                    {//die richtige Id raussuchen (screen & viewCategory)
                     //parent (Groupelement)
                        foreach (String id in ids)
                        {
                            List<Object> nodeList = treeOperation.searchNodes.getAssociatedNodeList(id, grantTrees.brailleTree);
                            if (nodeList != null && nodeList.Count > 0)
                            {
                                foreach (object o in nodeList)
                                {
                                    Object parentGroupelement = strategyMgr.getSpecifiedTree().Parent(o);
                                    OSMElement.OSMElement osmTmp = strategyMgr.getSpecifiedTree().GetData(parentGroupelement);
                                    if (osmTmp.brailleRepresentation.screenName.Equals(grantTrees.TextviewObject.screenName) && osmTmp.brailleRepresentation.typeOfView.Equals(grantTrees.TextviewObject.viewCategory))
                                    {
                                        brailleParentId = strategyMgr.getSpecifiedTree().GetData(parentGroupelement).properties.IdGenerated;
                                        break;
                                    }
                                }
                            }
                            if (brailleParentId != null)
                            {
                                break;
                            }
                        }

                    }
                }
                propGroup.IdGenerated = treeOperation.updateNodes.addNodeInBrailleTree(osmGroup, brailleParentId);//hier wird ggf. nur das Gruppen-(Start-)Element hinzugefügt
                osmGroup.properties = propGroup;
            }
            Object brailleSubtree = strategyMgr.getSpecifiedTree().NewTree();
            Object brailleSubtreeParent = strategyMgr.getSpecifiedTree().AddChild(brailleSubtree, osmGroup);
            List<TextviewElement> order;
            SpecialOrder so = treeOperation.templateOperations.textViewspecialOrderContainsControltype(osmFiltered.properties.controlTypeFiltered);
            if (so != null)
            {
                order = so.order;
            }
            else { order = grantTrees.TextviewObject.orders.defaultOrder; }
            foreach (TextviewElement tve in order)
            {
                #region Zeichen für Beginn der aufzählung
                if (grantTrees.TextviewObject.itemEnumerate != null && !grantTrees.TextviewObject.itemEnumerate.Equals("") && tve.order == 0)
                {
                    strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addSeparatorElementInBrailleTree(grantTrees.TextviewObject.itemEnumerate, ref startXPosition, startYPosition));
                    startXPosition += grantTrees.TextviewObject.itemEnumerate.Length * 3 + 2;
                }
                #endregion
                #region Separator
                if (tve.separator != null && !tve.separator.Equals("") && tve.order > 0)
                {
                    strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addSeparatorElementInBrailleTree(tve.separator, ref startXPosition, startYPosition));
                    startXPosition += tve.separator.Length * 3 + 2;
                }
                #endregion
                object objectText = OSMElement.Helper.getGeneralPropertieElement(tve.property, osmFiltered.properties);
                String text = (objectText != null ? objectText.ToString() : null);
                text = treeOperation.updateNodes.useAcronymForText(text);
                int width = (text.Length * 3) < tve.minWidth ? tve.minWidth : (text.Length * 3);
                strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addElementInBrailleTree(filteredNode, tve, width, ref startXPosition, startYPosition));
                startXPosition += width +2;
                
            }
            startYPosition += 5;
            addSubtreeInBrailleTree(strategyMgr.getSpecifiedTree().Root(brailleSubtreeParent), brailleParentId);
        }

        private void addSubtreeInBrailleTree(object brailleNode, String parentId)
        {
            OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(brailleNode));

           if (strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osm))
            {
                strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osm);
            }
            if (parentId != null)
            {
                foreach (object childOfNode in strategyMgr.getSpecifiedTree().AllNodes(grantTrees.brailleTree))
                {
                    string o = strategyMgr.getSpecifiedTree().GetData(childOfNode).properties.IdGenerated;

                    if (strategyMgr.getSpecifiedTree().GetData(childOfNode).properties.IdGenerated != null && strategyMgr.getSpecifiedTree().GetData(childOfNode).properties.IdGenerated.Equals(parentId))
                    {
                        strategyMgr.getSpecifiedTree().AddChild(childOfNode, brailleNode);
                        return;
                    }
                }
            }
            //else
            foreach (Object viewCategory in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(viewCategory).brailleRepresentation.typeOfView.Equals(osm.brailleRepresentation.typeOfView))
                {
                    foreach (Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(viewCategory))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName.Equals(osm.brailleRepresentation.screenName))
                        {
                            strategyMgr.getSpecifiedTree().AddChild(screenSubtree, brailleNode);
                            // es müssen von diesem Screen-Zweig alle Kinder durchsucht werden um die view mit der parentId zu finden

                            return;
                        }
                    }
                }
            }
        }
        
    }
}
