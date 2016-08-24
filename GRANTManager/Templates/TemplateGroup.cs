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
        int deviceHeight;
        int deviceWidth;
        public TemplateGroup(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
            : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            deviceHeight = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().height;
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
        }



        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = templateObject.renderer;
            prop.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            prop.boundingRectangleFiltered = new System.Windows.Rect(0, 0, templateObject.width, templateObject.height); //TODO. x, y

            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            braille.screenName = "mainScreen"; //?
            braille.viewName = filteredSubtree.Data.properties.IdGenerated;
            braille.templateFullName = templateObject.groupImplementedClassTypeFullName;
            braille.templateNamspace = templateObject.groupImplementedClassTypeDllName;            
 
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
