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
    public abstract class ATemplateUi
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        public ATemplateUi()
        {

        }
            
        public ATemplateUi(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        public virtual void createUiElementFromTemplate(ref ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId = null) //noch sollte eigenltich das OSM-Element reichen, aber bei Komplexeren Elementen wird wahrscheinlich ein Teilbaum benötigt
        {
            if (templateObject.Screens != null)
            {
                List<String> screenList = templateObject.Screens;
                foreach (String screen in screenList)
                {
                    templateObject.Screens = new List<string>();
                    templateObject.Screens.Add(screen);
                    OSMElement.OSMElement brailleNode = createSpecialUiElement(filteredSubtree, templateObject);
                    addIdAndRelationship(brailleNode, ref filteredSubtree, templateObject);
                }
            }
        }

        private void addIdAndRelationship(OSMElement.OSMElement brailleNode, ref ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject)
        {
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { return; }
            GeneralProperties prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;
            if (templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
            {
                List<OsmRelationship<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeRelationship.addOsmRelationship(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
                strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
            }
        }

        protected abstract OSMElement.OSMElement createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, GenaralUI.TempletUiObject templateObject, String brailleNodeId = null);


        
    }
}
