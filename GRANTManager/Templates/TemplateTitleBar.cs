using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;

namespace GRANTManager.Templates
{
    public class TemplateTitleBar : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        public TemplateTitleBar(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
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
            prop.boundingRectangleFiltered = templateObject.rect;
            prop.controlTypeFiltered = templateObject.renderer;

            braille.boarder = new System.Windows.Forms.Padding(0, 0, 0, 1);
            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            braille.padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = "TitleBar";
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            return brailleNode;
        }
    }
}
