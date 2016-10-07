using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager;
using GRANTManager.Interfaces;
using OSMElement;

namespace TemplatesUi
{
    abstract class ATemplateUi
    {
        protected StrategyManager strategyMgr { get; private set; }
        protected GeneratedGrantTrees grantTrees { get; private set; }
        public ATemplateUi()
        {

        }
            
        public ATemplateUi(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
        }

        public virtual void createUiElementFromTemplate(Object filteredSubtree, TempletUiObject templateObject, String brailleNodeId = null) //noch sollte eigenltich das OSM-Element reichen, aber bei Komplexeren Elementen wird wahrscheinlich ein Teilbaum benötigt
        {
            if (templateObject.Screens != null)
            {
                List<String> screenList = templateObject.Screens;
                foreach (String screen in screenList)
                {
                    templateObject.Screens = new List<string>();
                    templateObject.Screens.Add(screen);
                    Object brailleNode = createSpecialUiElement(filteredSubtree, templateObject);
                    addIdAndRelationship(brailleNode, filteredSubtree, templateObject);
                }
            }
        }

        private void addIdAndRelationship(Object brailleSubtree, Object filteredSubtree, TempletUiObject templateObject)
        { 
            if (brailleSubtree == null || !strategyMgr.getSpecifiedTree().HasChild(brailleSubtree)) { return; }
            OSMElement.OSMElement brailleNode = strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Child(brailleSubtree) );
            brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
            
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { return; }
            String parentId = idGenerated;
            GeneralProperties prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;
            if (templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
            {
                List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated, ref relationship);
                strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
            }

            if (strategyMgr.getSpecifiedTree().HasChild(brailleSubtree))
            {
                brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
                idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);
                /*if (idGenerated == null) { return; }
                prop = brailleNode.properties;
                prop.IdGenerated = idGenerated;
                brailleNode.properties = prop;
                if (templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
                {
                    List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                    OsmTreeRelationship.addOsmConnection(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
                    strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
                }*/
                while (strategyMgr.getSpecifiedTree().HasNext(brailleSubtree))
                {
                    brailleSubtree = strategyMgr.getSpecifiedTree().Next(brailleSubtree);
                    idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);
                    if (idGenerated == null) { return; }
                   /* prop = brailleNode.properties;
                    prop.IdGenerated = idGenerated;
                    brailleNode.properties = prop;
                    if (templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
                    {
                        List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                        OsmTreeRelationship.addOsmConnection(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
                        strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
                    }*/
                }
            }
        }

        protected abstract Object createSpecialUiElement(Object filteredSubtree, TempletUiObject templateObject, String brailleNodeId = null);


        
    }
}
