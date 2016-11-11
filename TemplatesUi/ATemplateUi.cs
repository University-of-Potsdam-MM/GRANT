using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElement;

namespace TemplatesUi
{
    abstract class ATemplateUi
    {
        protected StrategyManager strategyMgr { get; private set; }
        protected GeneratedGrantTrees grantTrees { get; private set; }
        protected TreeOperation treeOperation { get; private set; }
        public ATemplateUi()
        {

        }

        public ATemplateUi(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }

        public virtual void createUiElementFromTemplate(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null) //noch sollte eigenltich das OSM-Element reichen, aber bei Komplexeren Elementen wird wahrscheinlich ein Teilbaum benötigt
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

        /// <summary>
        /// Erstellt ausgehend von einem Template einen Knoten im Braille-Baum sowie dessen Beziehung
        /// </summary>
        /// <param name="screenshotObject">gibt die Eigenschaften für den Screenshot-Knoten an</param>
        /// <param name="nodes">gibt eine Liste aller Knoten des gefilterten Baums an von denen der Screenshot erstellt werden soll</param>
        public void createUiScreenshotFromTemplate(TemplateScreenshotObject screenshotObject, List<Object> nodes)
        {
            //TODO: verschiedene Screens beachten
            //TODO: Baumbeziehung setzen
            foreach (Object node in nodes)
            {
                createUiElementFromTemplate(node, CastScreenshotObject(screenshotObject));
            }
        }

        private TemplateUiObject CastScreenshotObject(TemplateScreenshotObject screenshotObject)
        {
            TemplateUiObject castedSO = new TemplateUiObject();
            castedSO.name = screenshotObject.name;
            castedSO.Screens = screenshotObject.Screens;
            castedSO.osm = screenshotObject.osm;

            return castedSO;
        }

        private void addIdAndRelationship(Object brailleSubtree, Object filteredSubtree, TemplateUiObject templateObject)
        { 
            if (brailleSubtree == null || !strategyMgr.getSpecifiedTree().HasChild(brailleSubtree)) { return; }
            OSMElement.OSMElement brailleNode = strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Child(brailleSubtree) );
            brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
            
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { return; }
            String parentId = idGenerated;
            GeneralProperties prop = brailleNode.properties;
            prop.IdGenerated = idGenerated;
            brailleNode.properties = prop;
            if(filteredSubtree != null) //(templateObject.osm.brailleRepresentation.fromGuiElement != null && !templateObject.osm.brailleRepresentation.fromGuiElement.Trim().Equals(""))
            {
                List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated, ref relationship);
                treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            }

            if (strategyMgr.getSpecifiedTree().HasChild(brailleSubtree))
            {
                brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
                idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);
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
                    idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);
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

        protected abstract Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null);


        
    }
}
