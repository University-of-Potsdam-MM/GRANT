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

        public virtual void createUiElementFromTemplate(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, String brailleNodeId = null) //noch sollte eigenltich das OSM-Element reichen, aber bei Komplexeren Elementen wird wahrscheinlich ein Teilbaum benötigt
        {
            if (templateObject.Screens != null)
            {
                List<String> screenList = templateObject.Screens;
                foreach (String screen in screenList)
                {
                    templateObject.Screens = new List<string>();
                    templateObject.Screens.Add(screen);
                    ITreeStrategy<OSMElement.OSMElement> brailleNode = createSpecialUiElement(filteredSubtree, templateObject);
                    addIdAndRelationship(brailleNode, filteredSubtree, templateObject);
                }
            }
        }

        private void addIdAndRelationship(ITreeStrategy<OSMElement.OSMElement> brailleSubtree, ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject)
        { 
            if (brailleSubtree == null || !brailleSubtree.HasChild) { return; }
            OSMElement.OSMElement brailleNode = brailleSubtree.Child.Data;
            brailleSubtree = brailleSubtree.Child;
            
            String idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { return; }
            String parentId = idGenerated;
            GeneralProperties prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;
            if (templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
            {
                List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeConnector.addOsmConnection(filteredSubtree.Data.properties.IdGenerated, idGenerated, ref relationship);
                strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(ref brailleNode);
            }

            if (brailleSubtree.HasChild)
            {
                brailleSubtree = brailleSubtree.Child;
                idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleSubtree.Data, parentId);
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
                while (brailleSubtree.HasNext)
                {
                    brailleSubtree = brailleSubtree.Next;
                    idGenerated = strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(brailleSubtree.Data, parentId);
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

        protected abstract ITreeStrategy<OSMElement.OSMElement> createSpecialUiElement(ITreeStrategy<OSMElement.OSMElement> filteredSubtree, TempletUiObject templateObject, String brailleNodeId = null);


        
    }
}
