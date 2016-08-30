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
    public class TemplateText : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        public TemplateText(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
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
            if (templateObject.boarder != null)
            {
                braille.boarder = templateObject.boarder;
            }
            if (templateObject.padding != null)
            {
                braille.padding = templateObject.padding;
            }
            if (templateObject.margin != null)
            {
                braille.margin = templateObject.margin;
            }
            if (templateObject.textFromUIElement != null && !templateObject.textFromUIElement.Equals(""))
            {
                braille.fromGuiElement = templateObject.textFromUIElement;
            }
            braille.isVisible = true;
            
            if (templateObject.Screens == null) { Debug.WriteLine("Achtung, hier wurde kein Screen angegeben!"); return new OSMElement.OSMElement(); }
            braille.screenName = templateObject.Screens[0]; // hier wird immer nur ein Screen-Name übergeben
            braille.viewName = templateObject.name;
            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            return brailleNode;
        }
    }
}
