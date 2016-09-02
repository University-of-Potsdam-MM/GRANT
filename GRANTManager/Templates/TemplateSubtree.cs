using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GRANTManager.Templates
{
    public class TemplateSubtree : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        int? boxStartX;
        int? boxStartY;
        int deviceWidth;
        int deviceHeight;
        public TemplateSubtree(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
            : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
            deviceHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
        }

        public override void createUiElementFromTemplate(ref ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId)
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

        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId)
        {
            if (filteredSubtree.Data.properties.Equals(new GeneralProperties())) { return new OSMElement.OSMElement(); }
            OSMElement.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;
            prop.IdGenerated = null; // zurücksetzen, da das die Id vom Elternknoten wäre
            prop.controlTypeFiltered = templateObject.osm.brailleRepresentation.groupelements.renderer; // den Renderer der Kindelemente setzen
            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            braille.padding = templateObject.osm.brailleRepresentation.groupelements.padding;
            braille.margin = templateObject.osm.brailleRepresentation.groupelements.padding;
            braille.boarder = templateObject.osm.brailleRepresentation.groupelements.boarder;
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "GroupChild" + filteredSubtree.Data.properties.IdGenerated;

            if (prop.controlTypeFiltered.Equals("DropDownMenu"))
            {
                OSMElement.UiElements.DropDownMenu dropDownMenu = new OSMElement.UiElements.DropDownMenu();
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

            if (templateObject.osm.brailleRepresentation.groupelements.vertical)
            {
                int column = 0;
                int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Height);
                int elementsProColumn = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Height); //(max - Convert.ToInt32(templateObject.rect.Y)) / Convert.ToInt32(templateObject.rect.Height);
                if (braille.groupelements.linebreak == true)
                {
                    column = filteredSubtree.BranchIndex / elementsProColumn;
                }
                if (boxStartY == null)
                {
                    boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Y);
                }
                if (boxStartX == null)
                {
                    boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.X);
                }

                boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Y) + ((filteredSubtree.BranchIndex - (column * elementsProColumn)) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Height));
                boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.X) + (column * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Width));
                
            }
            else
            { //horizontal
                int line = 0;
                int max = Convert.ToInt32(templateObject.osm.properties.boundingRectangleFiltered.Width);
                int elementsProLine = max / Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Width);
                if (braille.groupelements.linebreak == true)
                {
                    line = filteredSubtree.BranchIndex / elementsProLine;
                }
                if (boxStartY == null)
                {
                    boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Y);
                }
                if (boxStartX == null)
                {
                    boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.X);
                }
                else
                {
                    boxStartX = boxStartX + Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Width);
                }
                if (templateObject.osm.brailleRepresentation.groupelements.linebreak == true)
                {
                    boxStartY = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Y) + line * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Height);
                    if (line > 0)
                    {
                        boxStartX = Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.X) + ((filteredSubtree.BranchIndex - (elementsProLine * line))) * Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Width);
                    }
                }
            }
            System.Windows.Rect rect = new System.Windows.Rect(Convert.ToDouble(boxStartX), Convert.ToDouble(boxStartY), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Width), Convert.ToInt32(templateObject.osm.brailleRepresentation.groupelements.childBoundingRectangle.Height));


            prop.boundingRectangleFiltered = rect;

            brailleNode.properties = prop;
            braille.zIntex = 60; //TODO richtig bestimmen
            brailleNode.brailleRepresentation = braille;
            if (!filteredSubtree.HasParent) { return new OSMElement.OSMElement(); }
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode, brailleNodeId);
            
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return new OSMElement.OSMElement();
            }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmRelationship<String, String>> relationship = grantTrees.getOsmRelationship();
            OsmTreeRelationship.addOsmRelationship(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
            strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
            return brailleNode;
        }

        /// <summary>
        /// Iterriert über die Kinder eines Knotens und erstellt für jedes Kind ein Ui-Element
        /// </summary>
        /// <param name="parentNode">gibt den Elternknoten an</param>
        /// <param name="templateObject">gibt das zu nutzende Template an</param>
        private void iteratedChildreen(ITreeStrategy<OSMElement.OSMElement> parentNode, GenaralUI.TempletUiObject templateObject, String brailleNodeId)
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
