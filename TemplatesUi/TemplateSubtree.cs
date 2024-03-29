﻿using GRANTManager.Interfaces;
using OSMElements;
using OSMElements.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GRANTManager;
using System.Windows;
using GRANTManager.TreeOperations;

namespace TemplatesUi
{
    class TemplateSubtree : ATemplateUi
    {
        private int? boxStartX;
        private int? boxStartY;
        private int deviceWidth;
        private int deviceHeight;
        public TemplateSubtree(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation) : base(strategyMgr, grantTrees, treeOperation)
        {
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            deviceHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
        }

        public override void createUiElementFromTemplate(Object filteredSubtree, TemplateUiObject templateObject,  String brailleNodeId)
        {
            if (!strategyMgr.getSpecifiedTree().HasChild(filteredSubtree)) { return; }
            if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtree))
            {
                Object filteredSubtreeCopy = strategyMgr.getSpecifiedTree().Copy(filteredSubtree);
                if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtreeCopy))
                {
                    filteredSubtreeCopy = strategyMgr.getSpecifiedTree().Child(filteredSubtreeCopy);
                    iteratedChildreen(filteredSubtreeCopy, templateObject, brailleNodeId);
                }
            }
        }

        protected override Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId)
        {
            if (strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.Equals(new GeneralProperties())) { return strategyMgr.getSpecifiedTree().NewTree(); }
            OSMElements.OSMElement brailleNode = templateObject.osm.DeepCopy();
            brailleNode.properties.resetIdGenerated = null; // zurücksetzen, da das die Id vom Elternknoten wäre
                                                       // prop.controlTypeFiltered = templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer; // den Renderer der Kindelemente setzen
            brailleNode.properties.isEnabledFiltered = false;
            brailleNode.brailleRepresentation.isVisible = true;
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewTree(); }
            brailleNode.brailleRepresentation.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            brailleNode.brailleRepresentation.viewName = "GroupChild" + strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated;
            brailleNode.brailleRepresentation.isGroupChild = true;
           // if(templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer != null) { 
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("DropDownMenuItem"))
            {
                OSMElements.UiElements.DropDownMenuItem dropDownMenu = new OSMElements.UiElements.DropDownMenuItem();
                if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(filteredSubtree)).properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
                if (strategyMgr.getSpecifiedTree().HasNext(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Next(filteredSubtree)).properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasNext = true;
                }
                if (strategyMgr.getSpecifiedTree().HasPrevious(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Previous(filteredSubtree)).properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasPrevious = true;
                }
                if (strategyMgr.getSpecifiedTree().HasParent(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(filteredSubtree)).properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.isChild = true; }
                dropDownMenu.isOpen = false;
                dropDownMenu.isVertical = false;
                brailleNode.brailleRepresentation.uiElementSpecialContent = dropDownMenu;
            }
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("ListItem"))
            {
                OSMElements.UiElements.ListMenuItem litItem = new ListMenuItem();
                if (strategyMgr.getSpecifiedTree().HasNext(filteredSubtree) ) { litItem.hasNext = true; }
                //TODO: multi
                if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtree))
                {
                    if (strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( filteredSubtree)).properties.controlTypeFiltered.Equals("CheckBox"))
                    {
                        litItem.isMultipleSelection = true;
                    }
                }
                brailleNode.brailleRepresentation.uiElementSpecialContent = litItem;
            }
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("TabItem"))
            {
                OSMElements.UiElements.TabItem tabView = new OSMElements.UiElements.TabItem();
                tabView.orientation = templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion; //templateObject.orientation;
                brailleNode.brailleRepresentation.uiElementSpecialContent = tabView;
                //                brailleNode.brailleRepresentation.uiElementSpecialContent = templateObject.osm.brailleRepresentation.uiElementSpecialContent;
            }

            OSMElements.OSMElement brailleGroupNode =  treeOperation.searchNodes.getBrailleTreeOsmElementById(brailleNodeId);
             bool groupViewWithScrollbars = false;
             if (!brailleGroupNode.Equals(new OSMElements.OSMElement()) && brailleGroupNode !=null && brailleGroupNode.brailleRepresentation != null)
             {
                 groupViewWithScrollbars = brailleGroupNode.brailleRepresentation.isScrollbarShow;
             }
             Rect rect;
             if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("TabItem"))
             {
                 Rect rectTmp = templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle;
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElements.UiElements.Orientation.Left) || templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElements.UiElements.Orientation.Right))
                 {
                     rectTmp.Y = (rectTmp.Height + 1) * strategyMgr.getSpecifiedTree().BranchIndex(filteredSubtree) + rectTmp.Y + 1;
                 }
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElements.UiElements.Orientation.Top) || templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElements.UiElements.Orientation.Bottom))
                 {
                     rectTmp.X = (rectTmp.Width + 1) * strategyMgr.getSpecifiedTree().BranchIndex( filteredSubtree) + rectTmp.X + 1;
                 }
                 boxStartX = Convert.ToInt32(rectTmp.X);
                 boxStartY = Convert.ToInt32(rectTmp.Y);
             }
             else
             {
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElements.UiElements.Orientation.Vertical))
                 {
                     int column = 0;
                     int subBoxModel = (brailleGroupNode == null || brailleGroupNode.brailleRepresentation == null)? 0 : brailleGroupNode.brailleRepresentation.boarder.Top + brailleGroupNode.brailleRepresentation.boarder.Bottom + brailleGroupNode.brailleRepresentation.margin.Top + brailleGroupNode.brailleRepresentation.margin.Bottom + brailleGroupNode.brailleRepresentation.padding.Top + brailleGroupNode.brailleRepresentation.padding.Bottom;
                     int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Height) - (groupViewWithScrollbars == true ? 3 : 0) - subBoxModel;
                     int elementsProColumn = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height);
                     if (brailleNode.brailleRepresentation.groupelementsOfSameType.isLinebreak == true)
                     {
                         column = strategyMgr.getSpecifiedTree().BranchIndex(filteredSubtree) / elementsProColumn;
                     }
                     if (boxStartY == null)
                     {
                         boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y);
                     }
                     if (boxStartX == null)
                     {
                         boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X);
                     }

                     boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y) + ((strategyMgr.getSpecifiedTree().BranchIndex(filteredSubtree) - (column * elementsProColumn)) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height));
                     boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X) + (column * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width));

                 }
                 else
                 { //horizontal
                     int line = 0;
                     int subBoxModel = (brailleGroupNode == null || brailleGroupNode.brailleRepresentation == null) ? 0 : brailleGroupNode.brailleRepresentation.boarder.Left + brailleGroupNode.brailleRepresentation.boarder.Right + brailleGroupNode.brailleRepresentation.margin.Left + brailleGroupNode.brailleRepresentation.margin.Right + brailleGroupNode.brailleRepresentation.padding.Left + brailleGroupNode.brailleRepresentation.padding.Right;
                     int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Width) - (groupViewWithScrollbars == true ? 3 : 0) - subBoxModel;
                     int elementsProLine = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width);
                     if (brailleNode.brailleRepresentation.groupelementsOfSameType.isLinebreak == true)
                     {
                         line = strategyMgr.getSpecifiedTree().BranchIndex(filteredSubtree) / elementsProLine;
                     }
                     if (boxStartY == null)
                     {
                         boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y);
                     }
                     if (boxStartX == null)
                     {
                         boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X);
                     }
                     else
                     {
                         boxStartX = boxStartX + Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width);
                     }
                     if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.isLinebreak == true)
                     {
                         boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y) + line * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height);
                         if (line > 0)
                         {
                             boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X) + ((strategyMgr.getSpecifiedTree().BranchIndex(filteredSubtree) - (elementsProLine * line))) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width);
                         }
                     }
                 }
               }
             rect = new System.Windows.Rect(Convert.ToDouble(boxStartX), Convert.ToDouble(boxStartY), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height));
            brailleNode.properties.boundingRectangleFiltered = rect;
            
            if (!strategyMgr.getSpecifiedTree().HasParent(filteredSubtree) ) { return strategyMgr.getSpecifiedTree().NewTree(); }
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode, brailleNodeId);
            
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return strategyMgr.getSpecifiedTree().NewTree();
            }
            brailleNode.properties.IdGenerated = idGenerated;

            treeOperation.osmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated);
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            Object tree = strategyMgr.getSpecifiedTree().NewTree();
            strategyMgr.getSpecifiedTree().AddChild(tree, brailleNode);
            return tree;
        }

        /// <summary>
        /// Iterriert über die Kinder eines Knotens und erstellt für jedes Kind ein Ui-Element
        /// </summary>
        /// <param name="parentNode">gibt den Elternknoten an</param>
        /// <param name="templateObject">gibt das zu nutzende Template an</param>
        private void iteratedChildreen(Object parentNode, TemplateUiObject templateObject, String brailleNodeId)
        {            
            if (strategyMgr.getSpecifiedTree().HasChild(parentNode))
            {
                parentNode = strategyMgr.getSpecifiedTree().Child(parentNode);
                createSpecialUiElement(parentNode, templateObject, brailleNodeId);
            }
            else
            {
                return;
            }
            while (strategyMgr.getSpecifiedTree().HasNext(parentNode))
            {
                parentNode =strategyMgr.getSpecifiedTree().Next( parentNode);
                createSpecialUiElement(parentNode, templateObject, brailleNodeId);
            }           
            return;       
        }

    }
}
