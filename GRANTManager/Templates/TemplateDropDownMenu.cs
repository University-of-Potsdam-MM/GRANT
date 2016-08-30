using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GRANTManager.Templates
{
    public class TemplateDropDownMenu : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        int? boxStartX;
        int? boxStartY;
        int deviceWidth;
        public TemplateDropDownMenu(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
            : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
        }

        public override void createUiElementFromTemplate(ref ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId)
        {
            //Bei der MenuBar wird für alle Kind-Elemente ein DropDownMenu angezeigt
            if (!(filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Contains("Item"))) { return; }

            // OSMElement.OSMElement brailleNode = createSpecialUiElement(filteredSubtree, templateObject);
            if (filteredSubtree.HasChild)
            {
                ITreeStrategy<OSMElement.OSMElement> filteredSubtreeCopy = filteredSubtree.Copy();
                //ersten Knoten
                // createSpecialUiElement(filteredSubtreeCopy, templateObject);
                // filteredSubtreeCopy = filteredSubtreeCopy.Child;
                if (filteredSubtreeCopy.HasChild)
                {
                    filteredSubtreeCopy = filteredSubtreeCopy.Child;
                    iteratedChildreen(filteredSubtreeCopy, templateObject, brailleNodeId);
                }
               /* if (filteredSubtree.HasParent)
                {
                    filteredSubtreeCopy = filteredSubtree.Copy();
                    filteredSubtree = filteredSubtree.Parent;
                    filteredSubtree.Remove(filteredSubtreeCopy.Child.Data);
                }*/
                //   Debug.WriteLine("");
            }
        }

        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId)
        {
            if ((filteredSubtree.Data.properties.Equals(new GeneralProperties()) || !filteredSubtree.Data.properties.controlTypeFiltered.Contains("Item"))) { return new OSMElement.OSMElement(); }

            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = templateObject.renderer;
            //      prop.valueFiltered = filteredSubtree.properties.valueFiltered;

            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "GroupChild" + filteredSubtree.Data.properties.IdGenerated;
            if (templateObject.boarder != null)
            {
                braille.boarder = templateObject.boarder;
            }
            if (templateObject.padding != null)
            {
                braille.padding = templateObject.padding;
            }
            if (templateObject.margin != null)
            {
                braille.margin = templateObject.margin;
            }

            OSMElement.UiElements.DropDownMenu dropDownMenu = new OSMElement.UiElements.DropDownMenu();
            if (filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
            if (filteredSubtree.HasNext && filteredSubtree.Next.Data.properties.controlTypeFiltered.Contains("Item"))
            {
                dropDownMenu.hasNext = true;
            }
            int lengthBox = Convert.ToInt32( templateObject.rect.Width);
            int heightBox = Convert.ToInt32( templateObject.rect.Height);
            //int line = filteredSubtree.BranchIndex / (deviceWidth / lengthBox); // beim nutzen von mehreren Zeilen, wird dadurch boxStartX korrigiert <------- 0
            if (boxStartY == null)
            { 
                boxStartY = Convert.ToInt32(templateObject.rect.Y); 
            }
            if (boxStartX == null)
            {
                boxStartX = Convert.ToInt32(templateObject.rect.X);
            }
            else
            {
                boxStartX = boxStartX + lengthBox;
            } 

          // System.Windows.Rect rect = new System.Windows.Rect(lengthBox * Convert.ToDouble(boxStartX), Convert.ToDouble(boxStartY), lengthBox, heightBox);
           System.Windows.Rect rect = new System.Windows.Rect( Convert.ToDouble(boxStartX), Convert.ToDouble(boxStartY), lengthBox, heightBox);

           if (filteredSubtree.HasPrevious && filteredSubtree.Previous.Data.properties.controlTypeFiltered.Contains("Item"))
            {
                dropDownMenu.hasPrevious = true;
            }
           if (filteredSubtree.HasParent && filteredSubtree.Parent.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.isChild = true; }
            dropDownMenu.isOpen = false;
            dropDownMenu.isVertical = false;
            braille.uiElementSpecialContent = dropDownMenu;
            prop.boundingRectangleFiltered = rect;

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;
            if (!filteredSubtree.HasParent) { return new OSMElement.OSMElement(); }
           // OsmRelationship<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.FilteredTree.Equals(filteredSubtree.Parent.Data.properties.IdGenerated));
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode, brailleNodeId);
//            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            
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
