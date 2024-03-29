﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElements;
using GRANTManager.TreeOperations;

namespace TemplatesUi
{
    class TemplateNode : ATemplateUi
    {
        public TemplateNode(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation) : base(strategyMgr, grantTrees, treeOperation) { }

        protected override Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null)
        {
            OSMElements.OSMElement brailleNode = templateObject.osm;
            GeneralProperties prop = templateObject.osm.properties;
            BrailleRepresentation braille = templateObject.osm.brailleRepresentation;

            prop.isEnabledFiltered = false;
            braille.isVisible = true;
            #region DropDownMenuItem
            if (templateObject.osm.properties.controlTypeFiltered.Equals("DropDownMenuItem"))
            {
                OSMElements.UiElements.DropDownMenuItem dropDownMenu = new OSMElements.UiElements.DropDownMenuItem();
                if (strategyMgr.getSpecifiedTree().HasChild(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Child(filteredSubtree)).properties.controlTypeFiltered.Contains("Item")) { dropDownMenu.hasChild = true; }
                if (strategyMgr.getSpecifiedTree().HasNext(filteredSubtree) && strategyMgr.getSpecifiedTree().GetData(strategyMgr.getSpecifiedTree().Next(filteredSubtree)).properties.controlTypeFiltered.Contains("Item"))
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
            #endregion
            #region TabItem
            if (templateObject.osm.properties.controlTypeFiltered.Equals("TabItem"))
            {
                OSMElements.UiElements.TabItem tabView = new OSMElements.UiElements.TabItem();
                tabView.orientation = templateObject.orientation;
                braille.uiElementSpecialContent = tabView;
                //braille.uiElementSpecialContent = templateObject.osm.brailleRepresentation.uiElementSpecialContent;
            }
            #endregion
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return strategyMgr.getSpecifiedTree().NewTree(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            if (!templateObject.allElementsOfType || !treeOperation.searchNodes.existViewInScreen(braille.screenName, templateObject.viewName, templateObject.osm.brailleRepresentation.typeOfView))
            {
                braille.viewName = templateObject.viewName;
            }
            else
            {
                int i = 0;
                String viewName = templateObject.viewName + "_"+i;
                
                while (treeOperation.searchNodes.existViewInScreen(braille.screenName, viewName, templateObject.osm.brailleRepresentation.typeOfView))
                {
                    i++;
                    viewName += i;                    
                }
                braille.viewName = viewName;
            }
            
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;
            Object tree = strategyMgr.getSpecifiedTree().NewTree();
            strategyMgr.getSpecifiedTree().AddChild(tree, brailleNode);
            return tree;
        }
    }
}
