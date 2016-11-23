using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElement;
using OSMElement.UiElements;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GRANTManager;

namespace TemplatesUi
{
    class TemplateGroupAutomatic : ATemplateUi
    {
        public TemplateGroupAutomatic(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation) : base(strategyMgr, grantTrees, treeOperation) { }

        protected override Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = "GroupElement";
            prop.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            braille.isVisible = true;
            if (templateObject.Screens == null) {
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewTree();
            }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            //braille.viewName = templateObject.name+"_"+ strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated;
            if ( !treeOperation.searchNodes.existViewInScreen(braille.screenName, templateObject.name, templateObject.osm.brailleRepresentation.screenCategory )) //!templateObject.allElementsOfType ||
            {
                braille.viewName = templateObject.name;
            }
            else
            {
                int i = 0;
                String viewName = templateObject.name + "_"+i;
                
                while (treeOperation.searchNodes.existViewInScreen(braille.screenName, viewName, templateObject.osm.brailleRepresentation.screenCategory))
                {
                    i++;
                    viewName += i;
                }
                braille.viewName = viewName;
            }
            braille.templateFullName = templateObject.groupImplementedClassTypeFullName;
            braille.templateNamspace = templateObject.groupImplementedClassTypeDllName;

            /*if (templateObject.osm.brailleRepresentation.boarder != null)
            {
                braille.boarder = templateObject.osm.brailleRepresentation.boarder;
            }
            if (templateObject.osm.brailleRepresentation.padding != null)
            {
                braille.padding = templateObject.osm.brailleRepresentation.padding;
            }
            if (templateObject.osm.brailleRepresentation.margin != null)
            {
                braille.margin = templateObject.osm.brailleRepresentation.margin;
            }*/
 
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode, templateObject.osm.brailleRepresentation.screenCategory);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return strategyMgr.getSpecifiedTree().NewTree();
            }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
            OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated, ref relationship);
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            Object tree = strategyMgr.getSpecifiedTree().NewTree();
            //strategyMgr.getSpecifiedTree().AddChild(tree, brailleNode);
            return tree;
        }

    }
}
