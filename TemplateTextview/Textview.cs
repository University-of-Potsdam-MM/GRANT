﻿using GRANTManager;
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


        public void createTextviewOfSubtree(Object subtree, int startYPosition = 0)
        {

             foreach (Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
             {
                 createBrailleGroupFromFilteredNode(node, ref startYPosition);
             }
           // createTextviewOfSubtreeIteration(subtree, ref startYPosition);
        }

   /*     private void createTextviewOfSubtreeIteration(Object subtree, ref int startYPosition)
        {
            foreach (Object node in strategyMgr.getSpecifiedTree().DirectChildrenNodes(subtree))
            {
                //wenn für diesen Knoten ein spezielles Verhalten definiert wurde, dann wird dieses angewendet andernfalls werden die Kindknoten betrachtet
                OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(node);
                SpecialGroup specialGroup = existSpecialGroup(osmFiltered.properties.controlTypeFiltered);
                if (specialGroup != null)
                {
                    addSpecialGroup(node, ref startYPosition, specialGroup);
                }
                else
                {
                    createBrailleGroupFromFilteredNode(node, ref startYPosition);
                    createTextviewOfSubtreeIteration(node, ref startYPosition);
                }
            }
        }*/

       /* private void addSpecialGroup(Object subtree, ref int startYPosition, SpecialGroup specialGroup)
        {
            //Erst einen eigenen Teilbaum mit allen Kindern erzeugen und diesen anschließend dem Braille-Baum hinzufügen
            OSMElement.OSMElement osmGroup = new OSMElement.OSMElement();
            GeneralProperties propGroup = new GeneralProperties();
            BrailleRepresentation brailleGroup = new BrailleRepresentation();
            brailleGroup.isVisible = true;
            propGroup.controlTypeFiltered = "GroupElement";

            brailleGroup.screenCategory = grantTrees.TextviewObject.viewCategory;
            brailleGroup.screenName = grantTrees.TextviewObject.screenName;
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(subtree);
            brailleGroup.viewName = osmFiltered.properties.IdGenerated;
            int depth = strategyMgr.getSpecifiedTree().Depth(subtree) * shiftingPerDepth;
            int startXPosition = depth;
            propGroup.boundingRectangleFiltered = new Rect(startXPosition, startYPosition, 120 - startXPosition, 5);//TODO: richtig machen

            osmGroup.brailleRepresentation = brailleGroup;
            osmGroup.properties = propGroup;
            propGroup.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmGroup);

            if (treeOperation.searchNodes.getBrailleTreeOsmElementById(propGroup.IdGenerated).Equals(new OSMElement.OSMElement()))
            {
                propGroup.IdGenerated = treeOperation.updateNodes.addNodeInBrailleTree(osmGroup);//hier wird ggf. nur das Gruppen-(Start-)Element hinzugefügt
                osmGroup.properties = propGroup;
            }
            Object brailleSubtree = strategyMgr.getSpecifiedTree().NewTree();
            Object brailleSubtreeParent = strategyMgr.getSpecifiedTree().AddChild(brailleSubtree, osmGroup);

            #region Zeichen für Beginn der aufzählung
            if (grantTrees.TextviewObject.itemEnumerate != null && !grantTrees.TextviewObject.itemEnumerate.Equals(""))
            {
                strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addSeparatorElementInBrailleTree(grantTrees.TextviewObject.itemEnumerate, ref startXPosition, startYPosition));
                startXPosition += grantTrees.TextviewObject.itemEnumerate.Length * 3 + 2;
            }
            #endregion
            #region TextviewElement -> sollte auch mit ausgelesen werden
            TextviewElement tvGroup = new TextviewElement();
            tvGroup.minWidth = 3;
            tvGroup.order = 0;
            tvGroup.property = "controlTypeFiltered";
            object objectText = OSMElement.Helper.getGeneralPropertieElement(tvGroup.property, osmFiltered.properties);
            String text = (objectText != null ? objectText.ToString() : null);
            text = treeOperation.updateNodes.useAcronymForText(text);
            int width = (text.Length * 3) < tvGroup.minWidth ? tvGroup.minWidth : (text.Length * 3);
            #endregion
            strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addElementInBrailleTree(subtree, tvGroup, width, ref startXPosition, startYPosition));
            startXPosition += width + 2;

            int orderid = 1;
            foreach(Object child in strategyMgr.getSpecifiedTree().AllChildrenNodes(subtree))
            {
                #region Separator
                if (specialGroup.separator != null && !specialGroup.separator.Equals(""))
                {
                    strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addSeparatorElementInBrailleTree(specialGroup.separator, ref startXPosition, startYPosition));
                    startXPosition += specialGroup.separator.Length * 3 + 2;
                }
                #endregion

                OSMElement.OSMElement osmChildFiltered = strategyMgr.getSpecifiedTree().GetData(child);
                #region TextviewElement -> sollte auch mit ausgelesen werden
                TextviewElement tvChild = new TextviewElement();
                tvChild.minWidth = 3;
                tvChild.order = orderid;
                tvChild.property = "nameFiltered";
                objectText = OSMElement.Helper.getGeneralPropertieElement(tvChild.property, osmChildFiltered.properties);
                text = (objectText != null ? objectText.ToString() : null);
                text = treeOperation.updateNodes.useAcronymForText(text);
                width = (text.Length * 3) < tvChild.minWidth ? tvChild.minWidth : (text.Length * 3);
                #endregion

                strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, addElementInBrailleTree(child, tvChild, width, ref startXPosition, startYPosition));
                startXPosition += width + 2;
                orderid++;
            }
            startYPosition += 5;
            // Debug.WriteLine("=>\n" + strategyMgr.getSpecifiedTree().ToStringRecursive(strategyMgr.getSpecifiedTree().Root(brailleSubtreeParent)));
            addSubtreeInBrailleTree(strategyMgr.getSpecifiedTree().Root(brailleSubtreeParent));
        }
*/
     /*   private SpecialGroup existSpecialGroup(String controltype)
        {
            foreach(SpecialGroup group in grantTrees.TextviewObject.specialGroups)
            {
                if (group.controltype.Equals(controltype))
                {
                    return group;
                }
            }
            return null;
        }

    */
        private OSMElement.OSMElement addSeparatorElementInBrailleTree(String separator, ref int startX, int startY)
        {
            TextviewObject tvo = grantTrees.TextviewObject;

            OSMElement.OSMElement osmBraille = new OSMElement.OSMElement();
            GeneralProperties propBraille = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            braille.isVisible = true;
            propBraille.controlTypeFiltered = "Text";
            braille.isGroupChild = true;
            braille.screenCategory = tvo.viewCategory;
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
            braille.screenCategory = tvo.viewCategory;
            braille.screenName = tvo.screenName;
            braille.fromGuiElement = tve.property;
            braille.viewName = osmFiltered.properties.IdGenerated + "_" + tve.order;

            propBraille.boundingRectangleFiltered = new Rect(startXPosition, startYPosition, width, 5);//TODO: richtig machen

            osmBraille.properties = propBraille;
            osmBraille.brailleRepresentation = braille;
            propBraille.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmBraille);
            osmBraille.properties = propBraille;
            List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
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
            
            brailleGroup.screenCategory = grantTrees.TextviewObject.viewCategory;
            brailleGroup.screenName = grantTrees.TextviewObject.screenName;
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(filteredNode);
            brailleGroup.viewName = osmFiltered.properties.IdGenerated;
            int depth = strategyMgr.getSpecifiedTree().Depth(filteredNode) * shiftingPerDepth;
            int startXPosition = depth;
            propGroup.boundingRectangleFiltered = new Rect(startXPosition, startYPosition, 120 - startXPosition, 5);//TODO: richtig machen

            osmGroup.brailleRepresentation = brailleGroup;
            osmGroup.properties = propGroup;
            propGroup.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(osmGroup);

            if (treeOperation.searchNodes.getBrailleTreeOsmElementById(propGroup.IdGenerated).Equals( new OSMElement.OSMElement()))
            {
                propGroup.IdGenerated = treeOperation.updateNodes.addNodeInBrailleTree(osmGroup);//hier wird ggf. nur das Gruppen-(Start-)Element hinzugefügt
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
           // Debug.WriteLine("=>\n" + strategyMgr.getSpecifiedTree().ToStringRecursive(strategyMgr.getSpecifiedTree().Root(brailleSubtreeParent)));
            addSubtreeInBrailleTree(strategyMgr.getSpecifiedTree().Root(brailleSubtreeParent));
        }

        private void addSubtreeInBrailleTree(object brailleNode)
        {
            OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(brailleNode));
            foreach (Object viewCategory in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.getBrailleTree()))
            {
                if (strategyMgr.getSpecifiedTree().GetData(viewCategory).brailleRepresentation.screenCategory.Equals(osm.brailleRepresentation.screenCategory))
                {
                    foreach (Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(viewCategory))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName.Equals(osm.brailleRepresentation.screenName))
                        {
                            if (strategyMgr.getSpecifiedTree().Contains(grantTrees.getBrailleTree(), osm))
                            {
                                strategyMgr.getSpecifiedTree().Remove(grantTrees.getBrailleTree(), osm);
                            }
                            strategyMgr.getSpecifiedTree().AddChild(screenSubtree, brailleNode);
                            return;
                        }
                    }
                }
            }

        }
    }
}
