using GRANTManager;
using GRANTManager.TreeOperations;
using OSMElement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace TemplateTextview
{
    public class Textview
    {
        StrategyManager strategyMgr;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperation;
        private int shiftingPerDepth = 2;

        public Textview(StrategyManager strategyMgr, GeneratedGrantTrees grantTrees, TreeOperation treeOperation)
        {
            this.strategyMgr = strategyMgr;
            this.grantTrees = grantTrees;
            this.treeOperation = treeOperation;
        }


        public void createTextviewOfSubtree(Object subtree, int startYPosition = 0)
        {
            
            foreach(Object node in strategyMgr.getSpecifiedTree().AllNodes(subtree))
            {
                addElementInBrailleTree(node, ref startYPosition);
            }
        }

        private void addElementInBrailleTree(object filteredNode, ref int startYPosition)
        {
            OSMElement.OSMElement osmFiltered = strategyMgr.getSpecifiedTree().GetData(filteredNode);
            int depth = strategyMgr.getSpecifiedTree().Depth(filteredNode) * shiftingPerDepth;
            startYPosition += 5;
            TextviewObject tvo = grantTrees.TextviewObject;

            int x = depth;
            foreach(TextviewElement tve in tvo.textviewElements)
            {
                OSMElement.OSMElement osmBraille = new OSMElement.OSMElement();
                GeneralProperties propBraille = new GeneralProperties();
                BrailleRepresentation braille = new BrailleRepresentation();

                braille.isVisible = true;
                propBraille.controlTypeFiltered = "Text";
                braille.screenCategory = tvo.viewCategory;
                braille.screenName = tvo.screenName;
                braille.fromGuiElement = tve.property;
                braille.viewName = osmFiltered.properties.IdGenerated + "_" + tve.order;

                #region ermitteln der Länge für die View
                object objectText = OSMElement.Helper.getGeneralPropertieElement(braille.fromGuiElement, osmFiltered.properties);
                String text = (objectText != null ? objectText.ToString() : null);
                text = treeOperation.updateNodes.useAcronymForText(text);
                int width = (text.Length *3) < tve.minWidth ? tve.minWidth : (text.Length *3);
                #endregion
                propBraille.boundingRectangleFiltered = new Rect(x, startYPosition, width, 5);//TODO: richtig machen
                x += width + 2;

                osmBraille.properties = propBraille;
                osmBraille.brailleRepresentation = braille;

                #region Id und Beziehung hinzufühgen
                String idGenerated = treeOperation.updateNodes.addNodeInBrailleTree(osmBraille);
                if (idGenerated == null) { return; }
                propBraille.IdGenerated = idGenerated;
                osmBraille.properties = propBraille;
                List<OsmConnector<String, String>> relationship = grantTrees.getOsmRelationship();
                OsmTreeConnector.addOsmConnection(osmFiltered.properties.IdGenerated, idGenerated, ref relationship);
                #endregion
                treeOperation.updateNodes.updateNodeOfBrailleUi(ref osmBraille);
            }
        }
        

    }
}
