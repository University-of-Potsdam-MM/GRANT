using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElements;
using OSMElements.UiElements;
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
            OSMElements.OSMElement brailleNode = new OSMElements.OSMElement();
            brailleNode.properties = templateObject.osm.properties;
            brailleNode.brailleRepresentation = templateObject.osm.brailleRepresentation;

            
            brailleNode.properties.controlTypeFiltered = "GroupElement";
            brailleNode.properties.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            brailleNode.brailleRepresentation.isVisible = true;
            if (templateObject.Screens == null) {
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewTree();
            }
            brailleNode.brailleRepresentation.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            //braille.viewName = templateObject.name+"_"+ strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated;

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

            List<Object>  existingNodesWithProperties = treeOperation.searchNodes.getNodesByProperties(grantTrees.brailleTree, brailleNode);
            if(existingNodesWithProperties != null && !existingNodesWithProperties.Equals(new List<Object>()))
            {               
                String idOsmFilteredSubtree = strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated;
                foreach (Object o in existingNodesWithProperties)
                {
                    OSMElements.OSMElement tmpOsmBraille = strategyMgr.getSpecifiedTree().GetData(o);
                    String connectedIdFilteredTree = treeOperation.searchNodes.getConnectedFilteredTreenodeId(tmpOsmBraille.properties.IdGenerated);
                    if (connectedIdFilteredTree != null && connectedIdFilteredTree.Equals(idOsmFilteredSubtree))
                    {
                        Debug.WriteLine("The node is already exist.");
                        return strategyMgr.getSpecifiedTree().NewTree(); ;
                    }
                }
            }
            brailleNode.properties.isEnabledFiltered = false;
            if (!treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, templateObject.viewName, templateObject.osm.brailleRepresentation.typeOfView)) //!templateObject.allElementsOfType ||
            {
                brailleNode.brailleRepresentation.viewName = templateObject.viewName;
            }
            else
            {
                int i = 0;
                String viewName = templateObject.viewName + "_" + i;

                while (treeOperation.searchNodes.existViewInScreen(brailleNode.brailleRepresentation.screenName, viewName, templateObject.osm.brailleRepresentation.typeOfView))
                {
                    i++;
                    viewName += i;
                }
                brailleNode.brailleRepresentation.viewName = viewName;
            }
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return strategyMgr.getSpecifiedTree().NewTree();
            }
            brailleNode.properties.IdGenerated = idGenerated;

            treeOperation.osmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated);
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            Object tree = strategyMgr.getSpecifiedTree().NewTree();
            //strategyMgr.getSpecifiedTree().AddChild(tree, brailleNode);
            return tree;
        }

    }
}
