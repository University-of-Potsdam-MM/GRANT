using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElements;
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
                    TemplateUiObject copyTemplate = templateObject.DeepCopy();
                    copyTemplate.Screens = new List<string>();
                    copyTemplate.Screens.Add(screen);
                    Object brailleNode = createSpecialUiElement(filteredSubtree, copyTemplate);
                    addSubtreeInBrailleTree(brailleNode);
                }
            }
        }

        private void addSubtreeInBrailleTree(object brailleNode)
        {
            OSMElements.OSMElement osm = strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child( brailleNode));
            foreach (Object typeOfView in strategyMgr.getSpecifiedTree().DirectChildrenNodes(grantTrees.brailleTree))
            {
                if (strategyMgr.getSpecifiedTree().GetData(typeOfView).brailleRepresentation.typeOfView.Equals(osm.brailleRepresentation.typeOfView))
                {
                    foreach(Object screenSubtree in strategyMgr.getSpecifiedTree().DirectChildrenNodes(typeOfView))
                    {
                        if (strategyMgr.getSpecifiedTree().GetData(screenSubtree).brailleRepresentation.screenName.Equals(osm.brailleRepresentation.screenName))
                        {
                            if(strategyMgr.getSpecifiedTree().Contains(grantTrees.brailleTree, osm))
                            {
                                //strategyMgr.getSpecifiedTree().Remove(grantTrees.brailleTree, osm);
                                treeOperation.updateNodes.RemoveNodeAndConnection(osm);
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
            OSMElements.OSMElement createdParentNode = createParentBrailleNode(filteredSubtree, templateObject);
            Object brailleSubtree = strategyMgr.getSpecifiedTree().NewTree();
            Object  brailleSubtreeParent = strategyMgr.getSpecifiedTree().AddChild(brailleSubtree, createdParentNode);
            int index = 0;
            foreach (OSMElements.OSMElement child in templateObject.groupElements)
            {
                TemplateUiObject childTemplate = new TemplateUiObject();
                childTemplate.osm = child;
                childTemplate.Screens = templateObject.Screens;                
                OSMElements.OSMElement childOsm = createChildBrailleNode(filteredSubtree, childTemplate, templateObject.viewName+"_"+index);
                if (!childOsm.Equals(new OSMElements.OSMElement()))
                {
                    strategyMgr.getSpecifiedTree().AddChild(brailleSubtreeParent, childOsm);
                    index++;
                }
                
            }

            return brailleSubtree;
        }

        private OSMElements.OSMElement createParentBrailleNode(Object filteredSubtree, TemplateUiObject templateObject)
        {
            OSMElements.OSMElement brailleNode = new OSMElements.OSMElement();
            brailleNode.brailleRepresentation = templateObject.osm.brailleRepresentation;
            brailleNode.properties = templateObject.osm.properties;

            brailleNode.properties.isEnabledFiltered = false;
            brailleNode.properties.controlTypeFiltered = "GroupElement";
            brailleNode.properties.isContentElementFiltered = false; //-> es ist Elternteil einer Gruppe
            brailleNode.brailleRepresentation.isVisible = true;
            if (templateObject.Screens == null)
            {
                Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElements.OSMElement();
            }
            brailleNode.brailleRepresentation.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            brailleNode.brailleRepresentation.viewName = "_" + templateObject.viewName + "_groupElementsStatic_Count_" + templateObject.groupElements.Count();
            brailleNode.brailleRepresentation.templateFullName = templateObject.groupImplementedClassTypeFullName;
            brailleNode.brailleRepresentation.templateNamspace = templateObject.groupImplementedClassTypeDllName;

            if (templateObject.osm.brailleRepresentation.boarder != null)
            {
                brailleNode.brailleRepresentation.boarder = templateObject.osm.brailleRepresentation.boarder;
            }
            if (templateObject.osm.brailleRepresentation.padding != null)
            {
                brailleNode.brailleRepresentation.padding = templateObject.osm.brailleRepresentation.padding;
            }
            if (templateObject.osm.brailleRepresentation.margin != null)
            {
                brailleNode.brailleRepresentation.margin = templateObject.osm.brailleRepresentation.margin;
            }
            
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return new OSMElements.OSMElement();
            }
            brailleNode.properties.IdGenerated = idGenerated;
            
            if (filteredSubtree != null && strategyMgr.getSpecifiedTree().GetData(filteredSubtree) != null)
            {
                treeOperation.osmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated);
            }
            treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);

            return brailleNode;
        }

        private OSMElements.OSMElement createChildBrailleNode(Object filteredSubtree, TemplateUiObject templateObject, String viewName)
        {
            //TODO: falls eine Beziehung im Baum erstellt werden soll muss diese hier? noch gesetzt werden => geht nicht ID ist noch nicht vorhanden
            OSMElements.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            if (templateObject.osm.properties.controlTypeFiltered.Equals("DropDownMenuItem"))
            {
                OSMElements.UiElements.DropDownMenuItem dropDownMenu = new OSMElements.UiElements.DropDownMenuItem();
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
                OSMElements.UiElements.TabItem tabView = new OSMElements.UiElements.TabItem();
                //tabView.orientation = templateObject.orientation;
                //braille.uiElementSpecialContent = tabView;
                braille.uiElementSpecialContent = templateObject.osm.brailleRepresentation.uiElementSpecialContent;
            }
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElements.OSMElement(); }
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
