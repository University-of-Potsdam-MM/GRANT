using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GRANTManager;
using System.Windows;

namespace TemplatesUi
{
    class TemplateSubtree : ATemplateUi
    {
        int? boxStartX;
        int? boxStartY;
        int deviceWidth;
        int deviceHeight;
        public TemplateSubtree(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
        {
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            deviceHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
        }

        public override void createUiElementFromTemplate(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, String brailleNodeId)
        {
            if (!filteredSubtree.HasChild ) { return; }
            if (filteredSubtree.HasChild)
            {
                ITreeStrategy<OSMElement.OSMElement> filteredSubtreeCopy = filteredSubtree.Copy();
                if (filteredSubtreeCopy.HasChild)
                {
                    filteredSubtreeCopy = filteredSubtreeCopy.Child;
                    iteratedChildreen(filteredSubtreeCopy, templateObject, brailleNodeId);
                }
            }
        }

        protected override ITreeStrategy<OSMElement.OSMElement> createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, String brailleNodeId)
        {
            if (filteredSubtree.Data.properties.Equals(new GeneralProperties())) { return strategyMgr.getSpecifiedTree().NewNodeTree(); }
            OSMElement.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;
            prop.IdGenerated = null; // zurücksetzen, da das die Id vom Elternknoten wäre
           // prop.controlTypeFiltered = templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer; // den Renderer der Kindelemente setzen
            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewNodeTree(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "GroupChild" + filteredSubtree.Data.properties.IdGenerated;
            braille.isGroupChild = true;
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("DropDownMenu"))
            {
                OSMElement.UiElements.DropDownMenuItem dropDownMenu = new OSMElement.UiElements.DropDownMenuItem();
                if (filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
                if (filteredSubtree.HasNext && filteredSubtree.Next.Data.properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasNext = true;
                }
                if (filteredSubtree.HasPrevious && filteredSubtree.Previous.Data.properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasPrevious = true;
                }
                if (filteredSubtree.HasParent && filteredSubtree.Parent.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.isChild = true; }
                dropDownMenu.isOpen = false;
                dropDownMenu.isVertical = false;
                braille.uiElementSpecialContent = dropDownMenu;
            }
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("ListItem"))
            {
                OSMElement.UiElements.ListMenuItem litItem = new ListMenuItem();
                if (filteredSubtree.HasNext) { litItem.hasNext = true; }
                //TODO: multi
                if (filteredSubtree.HasChild)
                {
                    if (filteredSubtree.Child.Data.properties.controlTypeFiltered.Equals("CheckBox"))
                    {
                        litItem.isMultipleSelection = true;
                    }
                }
                braille.uiElementSpecialContent = litItem;
            }
            if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("TabItem"))
            {
                OSMElement.UiElements.TabItem tabView = new OSMElement.UiElements.TabItem();
                tabView.orientation = templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion; //templateObject.orientation;
                braille.uiElementSpecialContent = tabView;
//                braille.uiElementSpecialContent = templateObject.osm.brailleRepresentation.uiElementSpecialContent;
            }
            
             OSMElement.OSMElement brailleGroupNode =  strategyMgr.getSpecifiedTreeOperations().getBrailleTreeOsmElementById(brailleNodeId);
             bool groupViewWithScrollbars = false;
             if (!brailleGroupNode.Equals(new OSMElement.OSMElement()))
             {
                 groupViewWithScrollbars = brailleGroupNode.brailleRepresentation.showScrollbar;
             }
             Rect rect;
             if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.renderer.Equals("TabItem"))
             {
                 Rect rectTmp = templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle;
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElement.UiElements.Orientation.Left) || templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElement.UiElements.Orientation.Right))
                 {
                     rectTmp.Y = (rectTmp.Height + 1) * filteredSubtree.BranchIndex + rectTmp.Y + 1;
                 }
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElement.UiElements.Orientation.Top) || templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElement.UiElements.Orientation.Bottom))
                 {
                     rectTmp.X = (rectTmp.Width + 1) * filteredSubtree.BranchIndex + rectTmp.X + 1;
                 }
                 boxStartX = Convert.ToInt32(rectTmp.X);
                 boxStartY = Convert.ToInt32(rectTmp.Y);
             }
             else
             {
                 if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.orienataion.Equals(OSMElement.UiElements.Orientation.Vertical))
                 {
                     int column = 0;
                     int subBoxModel = brailleGroupNode.brailleRepresentation.boarder.Top + brailleGroupNode.brailleRepresentation.boarder.Bottom + brailleGroupNode.brailleRepresentation.margin.Top + brailleGroupNode.brailleRepresentation.margin.Bottom + brailleGroupNode.brailleRepresentation.padding.Top + brailleGroupNode.brailleRepresentation.padding.Bottom;
                     int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Height) - (groupViewWithScrollbars == true ? 3 : 0) - subBoxModel;
                     int elementsProColumn = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height);
                     if (braille.groupelementsOfSameType.linebreak == true)
                     {
                         column = filteredSubtree.BranchIndex / elementsProColumn;
                     }
                     if (boxStartY == null)
                     {
                         boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y);
                     }
                     if (boxStartX == null)
                     {
                         boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X);
                     }

                     boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y) + ((filteredSubtree.BranchIndex - (column * elementsProColumn)) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height));
                     boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X) + (column * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width));

                 }
                 else
                 { //horizontal
                     int line = 0;
                     int subBoxModel = brailleGroupNode.brailleRepresentation.boarder.Left + brailleGroupNode.brailleRepresentation.boarder.Right + brailleGroupNode.brailleRepresentation.margin.Left + brailleGroupNode.brailleRepresentation.margin.Right + brailleGroupNode.brailleRepresentation.padding.Left + brailleGroupNode.brailleRepresentation.padding.Right;
                     int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Width) - (groupViewWithScrollbars == true ? 3 : 0) - subBoxModel;
                     int elementsProLine = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width);
                     if (braille.groupelementsOfSameType.linebreak == true)
                     {
                         line = filteredSubtree.BranchIndex / elementsProLine;
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
                     if (templateObject.osm.brailleRepresentation.groupelementsOfSameType.linebreak == true)
                     {
                         boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Y) + line * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height);
                         if (line > 0)
                         {
                             boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.X) + ((filteredSubtree.BranchIndex - (elementsProLine * line))) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width);
                         }
                     }
                 }
               }
             rect = new System.Windows.Rect(Convert.ToDouble(boxStartX), Convert.ToDouble(boxStartY), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Width), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelementsOfSameType.childBoundingRectangle.Height));
             //Debug.WriteLine("Rect: " + rect.ToString());
            prop.boundingRectangleFiltered = rect;

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;
            if (!filteredSubtree.HasParent) { return strategyMgr.getSpecifiedTree().NewNodeTree(); }
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode, brailleNodeId);
            
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return strategyMgr.getSpecifiedTree().NewNodeTree();
            }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmRelationship<String, String>> relationship = grantTrees.getOsmRelationship();
            OsmTreeRelationship.addOsmRelationship(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
            strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
            ITreeStrategy<OSMElement.OSMElement> tree = strategyMgr.getSpecifiedTree().NewNodeTree();
            tree.AddChild(brailleNode);
            return tree;
        }

        /// <summary>
        /// Iterriert über die Kinder eines Knotens und erstellt für jedes Kind ein Ui-Element
        /// </summary>
        /// <param name="parentNode">gibt den Elternknoten an</param>
        /// <param name="templateObject">gibt das zu nutzende Template an</param>
        private void iteratedChildreen(ITreeStrategy<OSMElement.OSMElement> parentNode, TempletUiObject templateObject, String brailleNodeId)
        {            
            if (parentNode.HasChild)
            {
                parentNode = parentNode.Child;
                createSpecialUiElement(parentNode, templateObject, brailleNodeId);
            }
            else
            {
                return;
            }
            while (parentNode.HasNext)
            {
                parentNode = parentNode.Next;
                createSpecialUiElement(parentNode, templateObject, brailleNodeId);
            }           
            return;       
        }

    }
}
