using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GRANTManager.Templates
{
    public class TemplateMenuBar : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        int deviceHeight;
        int deviceWidth;
        public TemplateMenuBar(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
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
                iteratedTree(ref filteredSubtreeCopy, templateObject);
                if (filteredSubtree.HasParent) 
                {
                    filteredSubtreeCopy = filteredSubtree.Copy();
                    filteredSubtree = filteredSubtree.Parent;
                    filteredSubtree.Remove(filteredSubtreeCopy.Child.Data);
                }
             //   Debug.WriteLine("");
            }
            


        }

        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            if ((filteredSubtree.Data.properties.Equals(new GeneralProperties()) || !filteredSubtree.Data.properties.controlTypeFiltered.Equals("MenuItem")) ) { return new OSMElement.OSMElement(); }
                
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            System.Windows.Rect rect = new System.Windows.Rect(0, 7, (5 * 3) + 5, 10); //TODO
            prop.controlTypeFiltered = templateObject.renderer;
            //      prop.valueFiltered = filteredSubtree.properties.valueFiltered;

            braille.fromGuiElement = templateObject.TextFromUIElement;
            braille.isVisible = true;
            braille.screenName = "mainScreen"; //?
            braille.viewName = "MenuBar"+filteredSubtree.Data.properties.IdGenerated;

            DropDownMenu dropDownMenu = new DropDownMenu();
            if (filteredSubtree.HasChild) { dropDownMenu.hasChild = true; }
            if (filteredSubtree.HasNext && filteredSubtree.Next.Data.properties.controlTypeFiltered.Equals("MenuItem")) { dropDownMenu.hasNext = true; }
            if (filteredSubtree.HasPrevious && filteredSubtree.Previous.Data.properties.controlTypeFiltered.Equals("MenuItem"))
            {
                dropDownMenu.hasPrevious = true;
                //rect.X = (5 * 3) + 4;
                rect.X = filteredSubtree.BranchIndex * ((5 * 3) + 5);
            }
            if (filteredSubtree.HasParent && filteredSubtree.Parent.Data.properties.controlTypeFiltered.Equals("MenuItem")) { dropDownMenu.isChild = true; }
            dropDownMenu.isOpen = false;
            dropDownMenu.isVertical = true;
            braille.uiElementSpecialContent = dropDownMenu;
            prop.boundingRectangleFiltered = rect;

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { Debug.WriteLine("Es konnte keine Id erstellt werden."); return new OSMElement.OSMElement(); }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmRelationship<String, String>> relationship = grantTrees.getOsmRelationship();
            OsmTreeRelationship.addOsmRelationship(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
            strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);

            return brailleNode;
        }

        private void iteratedTree(ref ITreeStrategy<OSMElement.OSMElement> tree, GenaralUI.TempletUiObject templateObject)
        {
            ITreeStrategy<OSMElement.OSMElement> node1;
            //Falls die Baumelemente Kinder des jeweiligen Elements sind
            while ((tree.HasChild || tree.HasNext) && !(tree.Count == 1 && tree.Depth == -1) ) //&& !(tree.Count == 1 && tree.Depth == -1) 
            {
               if (tree.HasChild)
                {
                    node1 = tree.Child;
                    createSpecialUiElement(node1, templateObject);
                    iteratedTree(ref node1, templateObject);
                }
                else
                {
                    node1 = tree.Next;
                    if (tree.HasNext)
                    {
                        createSpecialUiElement(tree, templateObject);
                    }
                    iteratedTree(ref node1, templateObject);
                }
            }
            if (tree.Count == 1 && tree.Depth == -1)
            {
                if (!tree.Data.brailleRepresentation.Equals(new BrailleRepresentation()))
                {
                    createSpecialUiElement(tree, templateObject);
                }
            }
            if (!tree.HasChild)
            {

                if (tree.HasParent)
                {
                    node1 = tree;
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


    }
}
