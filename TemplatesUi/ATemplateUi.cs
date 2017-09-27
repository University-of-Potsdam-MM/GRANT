using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;
using OSMElements;

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
                    if (!filteredSubtree.Equals(strategyMgr.getSpecifiedTree().NewTree()))
                    {
                        addIdAndRelationship(brailleNode, filteredSubtree, templateObject);
                    }
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
            List<String> viewCategories = Settings.getPossibleTypesOfViews();
            if(viewCategories == null || viewCategories.Count > 2) { throw new Exception("Die ViewCategorien wurden in den Settings nicht (ausreichend) angegeben!"); }
            //TODO: verschiedene Screens beachten
            //TODO: Baumbeziehung setzen
            foreach (Object node in nodes)
            {
                // viewCategories[1] = LayoutView
                createUiElementFromTemplate(node, CastScreenshotObject(screenshotObject));
            }
        }

        private TemplateUiObject CastScreenshotObject(TemplateScreenshotObject screenshotObject)
        {
            TemplateUiObject castedSO = new TemplateUiObject();
            castedSO.viewName = screenshotObject.viewName;
            castedSO.Screens = screenshotObject.Screens;
            castedSO.osm = screenshotObject.osm;

            return castedSO;
        }

        private void addIdAndRelationship(Object brailleSubtree, Object filteredSubtree, TemplateUiObject templateObject)
        { 
            if (brailleSubtree == null || !strategyMgr.getSpecifiedTree().HasChild(brailleSubtree)) { return; }
            OSMElements.OSMElement brailleNode = strategyMgr.getSpecifiedTree().GetData( strategyMgr.getSpecifiedTree().Child(brailleSubtree) );
            brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
            String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(brailleNode);
            if (idGenerated == null) { return; }
            String parentId = idGenerated;
            if(filteredSubtree != null && strategyMgr.getSpecifiedTree().Count(filteredSubtree) > 0) //(templateObject.osm.brailleRepresentation.displayedGuiElementType != null && !templateObject.osm.brailleRepresentation.displayedGuiElementType.Trim().Equals(""))
            {
                treeOperation.osmTreeConnector.addOsmConnection(strategyMgr.getSpecifiedTree().GetData(filteredSubtree).properties.IdGenerated, idGenerated);
                treeOperation.updateNodes.updateNodeOfBrailleUi(ref brailleNode);
            }

            if (strategyMgr.getSpecifiedTree().HasChild(brailleSubtree))
            {
                brailleSubtree = strategyMgr.getSpecifiedTree().Child(brailleSubtree);
                idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);

                while (strategyMgr.getSpecifiedTree().HasNext(brailleSubtree))
                {
                    brailleSubtree = strategyMgr.getSpecifiedTree().Next(brailleSubtree);
                    idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(strategyMgr.getSpecifiedTree().GetData(brailleSubtree), parentId);
                    if (idGenerated == null) { return; }

                }
            }
        }

        protected abstract Object createSpecialUiElement(Object filteredSubtree, TemplateUiObject templateObject, String brailleNodeId = null);


        
    }
}
