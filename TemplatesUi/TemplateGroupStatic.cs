using GRANTManager.Interfaces;
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
        public TemplateGroupStatic(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees) { }

        protected override ITreeStrategy<OSMElement.OSMElement> createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, string brailleNodeId = null)
        {
            OSMElement.OSMElement createdParentNode = createParentBrailleNode(filteredSubtree, templateObject);
            ITreeStrategy<OSMElement.OSMElement> brailleSubtree = strategyMgr.getSpecifiedTree().NewNodeTree();
            ITreeStrategy<OSMElement.OSMElement>  brailleSubtreeParent = brailleSubtree.AddChild(createdParentNode);
            int index = 0;
            foreach (OSMElement.OSMElement child in templateObject.groupElementsStatic)
            {
                TempletUiObject childTemplate = new TempletUiObject();
                childTemplate.osm = child;
                childTemplate.Screens = templateObject.Screens;
                OSMElement.OSMElement childOsm = createChildBrailleNode(filteredSubtree, childTemplate, templateObject.name+"_"+index);
                brailleSubtreeParent.AddChild(childOsm);
                index++;
            }

            return brailleSubtree;



            throw new NotImplementedException();
        }

        private OSMElement.OSMElement createParentBrailleNode(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject)
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

            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            if (idGenerated == null)
            {
                Debug.WriteLine("Es konnte keine Id erstellt werden."); return new OSMElement.OSMElement();
            }
            prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;

            List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
            OsmTreeConnector.addOsmConnection(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
            strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);

            return brailleNode;
        }

        private OSMElement.OSMElement createChildBrailleNode(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, String viewName)
        {
            //TODO: falls eine Beziehung im Baum erstellt werden soll muss diese hier? noch gesetzt werden => geht nicht ID ist noch nicht vorhanden
            OSMElement.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            if (templateObject.osm.properties.controlTypeFiltered.Equals("DropDownMenu"))
            {
                OSMElement.UiElements.DropDownMenuItem dropDownMenu = new OSMElement.UiElements.DropDownMenuItem();
                if (filteredSubtree.HasChild && filteredSubtree.Child.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
                if (filteredSubtree.HasNext && filteredSubtree.Next.Data.properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasNext = true;
                }
                if (filteredSubtree.HasPrevious && filteredSubtree.Previous.Data.properties.controlTypeFiltered.Contains("Item"))
                {
                    dropDownMenu.hasPrevious = true;
                }
                if (filteredSubtree.HasParent && filteredSubtree.Parent.Data.properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.isChild = true; }
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

            return brailleNode;
        }
    }
}
