﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElement;

namespace GRANTManager.Templates
{
    public class TemplateStatusBar : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;

        public TemplateStatusBar(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        protected override OSMElement.OSMElement createSpecialUiElement(Interfaces.ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId = null)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.boundingRectangleFiltered = templateObject.rect;
            prop.controlTypeFiltered = templateObject.renderer;
            prop.valueFiltered = "Statusleiste";

            braille.boarder = new System.Windows.Forms.Padding(0, 1, 0, 0);
            //braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            braille.padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "statusBar";

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            return brailleNode;
        }
    }
}
