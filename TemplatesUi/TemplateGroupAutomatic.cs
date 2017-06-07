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
            brailleNode.properties = templateObject.osm.properties;
            brailleNode.brailleRepresentation = templateObject.osm.brailleRepresentation;

            brailleNode.properties.isEnabledFiltered = false;
            brailleNode.properties.controlTypeFiltered = "GroupElement";
            brailleNode.properties.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            brailleNode.brailleRepresentation.isVisible = true;
            if (templateObject.Screens == null) {
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewTree();
            }
            brailleNode.brailleRepresentation.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            //braille.viewName = templateObject.name+"_"+ strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated;
            if ( !treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, templateObject.viewName, templateObject.osm.brailleRepresentation.typeOfView )) //!templateObject.allElementsOfType ||
            {
                brailleNode.brailleRepresentation.viewName = templateObject.viewName;
            }
            else
            {
                int i = 0;
                String viewName = templateObject.viewName + "_"+i;
                
                while (treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, viewName, templateObject.osm.brailleRepresentation.typeOfView))
                {
                    i++;
                    viewName += i;
                }
                brailleNode.brailleRepresentation.viewName = viewName;
            }
            brailleNode.brailleRepresentation.templateFullName = templateObject.groupImplementedClassTypeFullName;
            brailleNode.brailleRepresentation.templateNamspace = templateObject.groupImplementedClassTypeDllName;

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

            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return strategyMgr.getSpecifiedTree().NewTree();
            }
            brailleNode.properties.IdGenerated = idGenerated;

            List<OsmConnector<String, String>> relationship = grantTrees.osmRelationship;
            OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated, ref relationship);
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            Object tree = strategyMgr.getSpecifiedTree().NewTree();
            //strategyMgr.getSpecifiedTree().AddChild(tree, brailleNode);
            return tree;
        }

    }
}
