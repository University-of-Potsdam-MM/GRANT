using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;

namespace GRANTManager.Templates
{
    public class TemplateTitleBar : ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        int deviceWidth;
        public TemplateTitleBar(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees) : base(strategyMgr, grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            deviceWidth = strategyMgr.getSpecifiedDisplayStrategy().getActiveDevice().width;
        }
        protected override OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            OSMElement.OSMElement brailleNode = new OSMElement.OSMElement();
            GeneralProperties prop = new GeneralProperties();
            BrailleRepresentation braille = new BrailleRepresentation();

            prop.isEnabledFiltered = false;
            prop.boundingRectangleFiltered = new System.Windows.Rect(0, 0, templateObject.width, templateObject.height); //TODO
            prop.controlTypeFiltered = templateObject.renderer;

            braille.boarder = new System.Windows.Forms.Padding(0, 0, 0, 1);
            braille.fromGuiElement = templateObject.textFromUIElement;
            braille.isVisible = true;
            braille.padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            braille.screenName = "mainScreen"; //?
            braille.viewName = "TitleBar";

            brailleNode.properties = prop;
            brailleNode.brailleRepresentation = braille;

            return brailleNode;
        }
    }
}
