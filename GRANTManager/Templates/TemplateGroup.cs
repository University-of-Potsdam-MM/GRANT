using GRANTManager.Interfaces;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GRANTManager.Templates
{
    public class TemplateGroup : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        public TemplateGroup(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
            : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }



        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId = null)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = templateObject.renderer;
            prop.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            prop.boundingRectangleFiltered = templateObject.rect;

            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            if (templateObject.Screens == null) { 
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); 
            }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = filteredSubtree.Data.properties.IdGenerated;
            braille.templateFullName = templateObject.groupImplementedClassTypeFullName;
            braille.templateNamspace = templateObject.groupImplementedClassTypeDllName;
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
 
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
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

    }
}
