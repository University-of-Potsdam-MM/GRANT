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
        int deviceHeight;
        int deviceWidth;
        public TemplateDropDownMenu(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
            : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            deviceHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
        }

        public override void createUiElementFromTemplate(ref ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            //Bei der MenuBar wird für alle Kind-Elemente ein DropDownMenu angezeigt
            if (!(filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Equals("MenuItem"))) { return; }

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
                    iteratedChildreen(filteredSubtreeCopy, templateObject);
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

        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            if ((filteredSubtree.Data.properties.Equals(new GeneralProperties()) || !filteredSubtree.Data.properties.controlTypeFiltered.Equals("MenuItem"))) { return new OSMElement.OSMElement(); }

            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = templateObject.renderer;
            //      prop.valueFiltered = filteredSubtree.properties.valueFiltered;

            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            braille.screenName = "mainScreen"; //?
            braille.viewName = "GroupChild" + filteredSubtree.Data.properties.IdGenerated;

            OSMElement.UiElements.DropDownMenu dropDownMenu = new OSMElement.UiElements.DropDownMenu();
            if (filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Equals("MenuItem")) { dropDownMenu.hasChild = true; }
            if (filteredSubtree.HasNext && filteredSubtree.Next.Data.properties.controlTypeFiltered.Equals("MenuItem"))
            {
                dropDownMenu.hasNext = true;
            }
            int lengthBox = templateObject.width;
            int heightBox = templateObject.height;
            int boxStartY = 7;
            int boxStartX = filteredSubtree.BranchIndex;

           //Console.WriteLine(filteredSubtree.Data.properties.nameFiltered);
           int line = 0;// filteredSubtree.BranchIndex / (deviceWidth / lengthBox); // beim nutzen von mehreren Zeilen, wird dadurch boxStartX korrigiert
          //  boxStartY = boxStartY + (line * (heightBox + 1)); // würde dafür sorgen, dass ein neue Zeile genutzt wird, wenn die erste voll ist
            boxStartX = filteredSubtree.BranchIndex - ((deviceWidth / lengthBox) * line); // 

            System.Windows.Rect rect = new System.Windows.Rect(lengthBox * boxStartX, boxStartY, lengthBox, heightBox);
            if (filteredSubtree.HasPrevious && filteredSubtree.Previous.Data.properties.controlTypeFiltered.Equals("MenuItem"))
            {
                dropDownMenu.hasPrevious = true;
            }
            if (filteredSubtree.HasParent && filteredSubtree.Parent.Data.properties.controlTypeFiltered.Equals("MenuItem")) { dropDownMenu.isChild = true; }
            dropDownMenu.isOpen = false;
            dropDownMenu.isVertical = false;
            braille.uiElementSpecialContent = dropDownMenu;
            prop.boundingRectangleFiltered = rect;

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;
            if (!filteredSubtree.HasParent) { return new OSMElement.OSMElement(); }
            OsmRelationship<String, String> osmRelationships = grantTrees.getOsmRelationship().Find(r => r.FilteredTree.Equals(filteredSubtree.Parent.Data.properties.IdGenerated));
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode, osmRelationships != null ? osmRelationships.BrailleTree : null); //<<-- hier als KindElement der Gruppe hinzufügen
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
        private void iteratedChildreen(ITreeStrategy<OSMElement.OSMElement> parentNode, GenaralUI.TempletUiObject templateObject)
        {            
            if (parentNode.HasChild)
            {
                parentNode = parentNode.Child;
                createSpecialUiElement(parentNode, templateObject);
            }
            else
            {
                return;
            }
            while (parentNode.HasNext)
            {
                parentNode = parentNode.Next;
                createSpecialUiElement(parentNode, templateObject);
            }           
            return;
       
        }


    }
}
