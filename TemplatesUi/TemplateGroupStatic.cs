using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;


namespace TemplatesUi
{
    class TemplateGroupStatic : ATemplateUi
    {
        public TemplateGroupStatic(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees,  TreeOperation treeOperation) : base(strategyMgr, grantTrees, treeOperation) { }

        public override void createUiElementFromTemplate(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null) //noch sollte eigenltich das OSM-Element reichen, aber bei Komplexeren Elementen wird wahrscheinlich ein Teilbaum benötigt
        {
            if (templateObject.Screens != null)
            {
                List<String> screenList = templateObject.Screens;
                foreach (String screen in screenList)
                {
                    templateObject.Screens = new List<string>();
                    templateObject.Screens.Add(screen);
                    Object brailleNode = createSpecialUiElement(filteredSubtree, templateObject);
                    addSubtreeInBrailleTree(brailleNode);
                }
            }
        }

        private void addSubtreeInBrailleTree(object brailleNode)
        {
            OSMElement.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( brailleNode));
            foreach (Object viewCategory in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(viewCategory).brailleRepresentation.typeOfView.Equals(osm.brailleRepresentation.typeOfView))
                {
                    foreach(Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(viewCategory))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName.Equals(osm.brailleRepresentation.screenName))
                        {
                            if(strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osm))
                            {
                                strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osm);
                            }
                            strategyMgr.getSpecifiedTree().AddChild(screenSubtree, brailleNode);
                            return;
                        }
                    }
                }
            }
          
        }

        protected override Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, string brailleNodeId = null)
        {
            OSMElement.OSMElement createdParentNode = createParentBrailleNode(filteredSubtree, templateObject);
            Object brailleSubtree = strategyMgr.getSpecifiedTree().NewTree();
            Object  brailleSubtreeParent = strategyMgr.getSpecifiedTree().AddChild(brailleSubtree, createdParentNode);
            int index = 0;
            foreach (OSMElement.OSMElement child in templateObject.groupElementsStatic)
            {
                TemplateUiObject childTemplate = new TemplateUiObject();
                childTemplate.osm = child;
                childTemplate.Screens = templateObject.Screens;                
                OSMElement.OSMElement childOsm = createChildBrailleNode(filteredSubtree, childTemplate, templateObject.name+"_"+index);
                if (!childOsm.Equals(new OSMElement.OSMElement()))
                {
                    strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, childOsm);
                    index++;
                }
                
            }

            return brailleSubtree;
        }

        private OSMElement.OSMElement createParentBrailleNode(Object filteredSubtree, TemplateUiObject templateObject)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            prop.controlTypeFiltered = "GroupElement";
            prop.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            braille.isVisible = true;
            if (templateObject.Screens == null)
            {
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement();
            }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "_" + templateObject.name + "_groupElementsStatic_Count_" + templateObject.groupElementsStatic.Count();
            braille.templateFullName = templateObject.groupImplementedClassTypeFullName;
            braille.templateNamspace = templateObject.groupImplementedClassTypeDllName;

            if (templateObject.osm.brailleRepresentation.boarder != null)
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
            }

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return new OSMElement.OSMElement();
            }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmConnector<String, String>> relationship = grantTrees.osmRelationship;
            if (filteredSubtree != null)
            {
                OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated, ref relationship);
            }
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);

            return brailleNode;
        }

        private OSMElement.OSMElement createChildBrailleNode(Object filteredSubtree, TemplateUiObject templateObject, String viewName)
        {
            //TODO: falls eine Beziehung im Baum erstellt werden soll muss diese hier? noch gesetzt werden => geht nicht ID ist noch nicht vorhanden
            OSMElement.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            if (templateObject.osm.properties.controlTypeFiltered.Equals("DropDownMenuItem"))
            {
                OSMElement.UiElements.DropDownMenuItem dropDownMenu = new OSMElement.UiElements.DropDownMenuItem();
                if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtree) &&  strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(filteredSubtree)).properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
                if (strategyMgr.getSpecifiedTree().HasNext(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Next(filteredSubtree)).properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasNext = true;
                }
                if (strategyMgr.getSpecifiedTree().HasPrevious(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Previous(filteredSubtree)).properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasPrevious = true;
                }
                if (strategyMgr.getSpecifiedTree().HasParent(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Parent(filteredSubtree)).properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.isChild = true; }
                dropDownMenu.isOpen = false;
                dropDownMenu.isVertical = false;
                braille.uiElementSpecialContent = dropDownMenu;
            }
            if (templateObject.osm.properties.controlTypeFiltered.Equals("TabItem"))
            {
                OSMElement.UiElements.TabItem tabView = new OSMElement.UiElements.TabItem();
                //tabView.orientation = templateObject.orientation;
                //braille.uiElementSpecialContent = tabView;
                braille.uiElementSpecialContent = templateObject.osm.brailleRepresentation.uiElementSpecialContent;
            }
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = viewName;
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;
            prop.IdGenerated = treeOperation.generatedIds.generatedIdBrailleNode(brailleNode);
            brailleNode.properties = prop;

            return brailleNode;
        }
    }
}
