using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrategyManager;
using OSMElement;
using StrategyManager.Interfaces;
using System.Windows.Forms;
using GRANTApplication;

namespace GRANTExample
{
    public class ExampleBrailleDis
    {
        StrategyMgr strategyMgr;
        public ExampleBrailleDis(StrategyMgr mgr)
        {
            strategyMgr = mgr;
        }
        public void UiBrailleDis(String fromGuiElement)
        {
            try
            {
                setDauGui(fromGuiElement);
                if (strategyMgr.getSpecifiedBrailleDisplay() == null)
                {
                    Settings settings = new Settings();
                    strategyMgr.setSpecifiedBrailleDisplay(settings.getPossibleBrailleDisplays()[0].className); // muss dynamisch ermittelt werden

                    strategyMgr.getSpecifiedBrailleDisplay().setStrategyMgr(strategyMgr);
                    strategyMgr.getSpecifiedBrailleDisplay().initializedSimulator();
                    strategyMgr.getSpecifiedBrailleDisplay().initializedBrailleDisplay();
                    strategyMgr.getSpecifiedBrailleDisplay().generatedBrailleUi();
                }

                if (strategyMgr.getOsmRelationship() == null)
                {                    
                    List<OsmRelationship<String, String>> relationship = ExampleTree.setOsmRelationship();
                    strategyMgr.setOsmRelationship(relationship);
                    
                }
                else
                {
                    if (strategyMgr.getFilteredTree() == null)
                    {
                        Console.WriteLine("Die Anwendung wurde noch nicht gefiltert - bitte 'F5' drücken");
                        return;
                    }
                   String  brailleId =  "braille123_3";
                    OsmRelationship<String, String> osmRelationships = strategyMgr.getOsmRelationship().Find(r => r.BrailleTree.Equals(brailleId) || r.FilteredTree.Equals(brailleId)); //TODO: was machen wir hier, wenn wir mehrere Paare bekommen? (FindFirst?)

                    strategyMgr.getSpecifiedFilter().updateNodeOfFilteredTree(osmRelationships.FilteredTree);
                    ITreeStrategy<OSMElement.OSMElement> relatedBrailleTreeObject = strategyMgr.getSpecifiedTreeOperations().getAssociatedNode(osmRelationships.BrailleTree, strategyMgr.getBrailleTree());
                    if (relatedBrailleTreeObject != null)
                    {
                        strategyMgr.getSpecifiedTreeOperations().updateNodeOfBrailleUi(relatedBrailleTreeObject.Data);
                        strategyMgr.getSpecifiedBrailleDisplay().updateViewContent(relatedBrailleTreeObject.Data);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: '{0}'", ex);
            }
            
        }

        #region Beispielobjekte
        private void setDauGui(String fromGuiElement3)
        {
            #region Element 1
            OSMElement.OSMElement osm1 = new OSMElement.OSMElement();
            BrailleRepresentation e1 = new BrailleRepresentation();
            e1.screenName = "screen1";
            Content c1 = new Content();
            //c1.text = "Hallo";
            e1.viewName = "v1";
            c1.fromGuiElement = "nameFiltered";
            c1.showScrollbar = true;
            e1.content = c1;
            Position p1 = new Position();
            p1.height = 8;
            p1.width = 23;
            p1.left = 0;
            p1.top = 0;
            e1.position = p1;
            GeneralProperties proper1 = new GeneralProperties();
            proper1.IdGenerated = "braille123_1";
            osm1.brailleRepresentation = e1;
            osm1.properties = proper1;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm1);
            #endregion

            #region Element 2
            OSMElement.OSMElement osm2 = new OSMElement.OSMElement();
            BrailleRepresentation e2 = new BrailleRepresentation();
            e2.screenName = "screen1";
            Content c2 = new Content();
            c2.text = "Hallo 2";
            e2.viewName = "v2";
            c2.showScrollbar = true;
            e2.content = c2;
            Position p2 = new Position();
            p2.height = 12;
            p2.width = 29;
            p2.left = 0;
            p2.top = 16;

            Padding padding = new Padding(1, 1, 1, 1);
            p2.padding = padding;
            Padding margin = new Padding(1, 1, 1, 1);
            p2.margin = margin;
            Padding boarder = new Padding(1, 1, 2, 1);
            p2.boarder = boarder;
            e2.position = p2;
            GeneralProperties proper2 = new GeneralProperties();
            proper2.IdGenerated = "braille123_2";
            osm2.brailleRepresentation = e2;
            osm2.properties = proper2;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm2);
            #endregion

            #region Element 3
            OSMElement.OSMElement osm3 = new OSMElement.OSMElement();
            BrailleRepresentation e3 = new BrailleRepresentation();
            e3.screenName = "screen1";
            Content c3 = new Content();
            //   c3.text = "Start Text";
            //c3.fromGuiElement = "nameFiltered";
            c3.fromGuiElement = fromGuiElement3.Equals("") ? "nameFiltered" : fromGuiElement3;
            e3.viewName = "v3";
            e3.content = c3;
            Position p3 = new Position();
            p3.height = 20;
            p3.width = 30;
            p3.left = 0;
            p3.top = 30;
            e3.position = p3;
            GeneralProperties proper3 = new GeneralProperties();
            proper3.IdGenerated = "braille123_3";
            osm3.brailleRepresentation = e3;
            osm3.properties = proper3;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm3);
            #endregion

            #region Element 4
            OSMElement.OSMElement osm4 = new OSMElement.OSMElement();
            BrailleRepresentation e4 = new BrailleRepresentation();
            e4.screenName = "screen1";
            Content c4 = new Content();
            c4.matrix = new bool[,] { 
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true},
                {false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false},
                {false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false},
            };
            e4.viewName = "v4";
            e4.content = c4;
            Position p4 = new Position();
            p4.height = 10;
            p4.width = 20;
            p4.left = 40;
            p4.top = 50;
            e4.position = p4;
            GeneralProperties proper4 = new GeneralProperties();
            proper4.IdGenerated = "braille123_4";
            osm4.brailleRepresentation = e4;
            osm4.properties = proper4;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm4);
            #endregion

            #region Element 5
            OSMElement.OSMElement osm5 = new OSMElement.OSMElement();
            BrailleRepresentation e5 = new BrailleRepresentation();
            e5.screenName = "screen1";
            Content c5 = new Content();
            //   c3.text = "Start Text";
            c5.otherContent = "Hallo - Button";
            e5.viewName = "v5";
            e5.content = c5;
            Position p5 = new Position();
            p5.height = 20;
            p5.width = 40;
            p5.left = 30;
            p5.top = 30;
            e5.position = p5;
            GeneralProperties proper5 = new GeneralProperties();
            proper5.IdGenerated = "braille123_5";
            osm5.brailleRepresentation = e5;
            osm5.properties = proper5;
            strategyMgr.getSpecifiedTreeOperations().addNodeInBrailleTree(osm5);
            #endregion
            Console.WriteLine();
        }



        #endregion
    }
}
