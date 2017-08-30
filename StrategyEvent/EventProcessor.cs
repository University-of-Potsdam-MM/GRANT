using GRANTManager;
using GRANTManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Prism.Events;
using OSMElement;
using GRANTManager.TreeOperations;

namespace StrategyEvent
{
    /// <summary>
    /// Verarbeitung der Events:
    /// 1. Parsen des PRISM
    /// 2. EventManagerDaten abfragen
    /// 3. Actions ausführen
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        //folgende Variablen müssen gesetzt werden
        private StrategyManager strategyManager;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperations;

        public EventProcessor(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
            eventTest();
        }
        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }
        public void setTreeOperations(TreeOperation treeOperation) { this.treeOperations = treeOperation; }

        //todo init der prismeventaggreagtorclass in grantapplication einbauen
        public IEventAggregator prismEventAggregatorClass;// = new EventAggregator();

        public void eventTest()
        {
            //erhalt des prismaggregator über interface
            prismEventAggregatorClass = strategyManager.getSpecifiedEventStrategy().getSpecifiedEventManagerClass();

            //Anmeldung für zu verarbeitenden Events
            prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Subscribe(EventValueParsing); 
        }

        //Aufschlüsselung der Informationen aus dem Event-String in ein OSMEvent-Format für die weitere Verarbeitung
        //liefert den string, die eventid
        public void EventValueParsing(string osm)
        {
            Debug.WriteLine("winevent verarbeitet in eventprocessor" + osm);
            //osm = "werhers";

            //Aufruf eienr Methode aus EventAction über startegymanager
            //strategyManager.getSpecifiedEventAction().filterOSM();

            //strategyManager.getSpecifiedEventManager().deliverActionListForEvent();


            //strategyManager.getSpecifiedEventManager()
            //id des events ermittelt

            Debug.WriteLine("winevent verarbeitet in mainwindowxaml_" + osm);
            string pattern = "_";
            string[] substrings = System.Text.RegularExpressions.Regex.Split(osm, pattern);
            //NodeBox.Text = ("osm" + osm + " " + substrings[0]);

            IntPtr test;
            test = (IntPtr)Convert.ToInt32(substrings[3]);
            //string applicationName = strategyMgr.getSpecifiedOperationSystem().getProcessNameOfApplication((int)test);
            Debug.WriteLine("osmpat"+ test.ToString());

            //id nur aus bereits gefiltertem osm-baum erhaltbar mit der methode getidfilterednodebyhwnd
            //dazu muss hier  auch der baum hier  in treeoperation abfragbar sein, siehe InitializeFilterComponent in mainwindowxamls.cs von grantexample

            //String foundId = treeOperation.searchNodes.getIdFilteredNodeByHwnd(osmData.properties.hWndFiltered);
            String foundId = treeOperations.searchNodes.getIdFilteredNodeByHwnd(test);

            Debug.WriteLine("osmpat2" + test.ToString() + " " + foundId);

            EventTypes t = EventTypes.Keyboard;

            Debug.WriteLine("Keyhandler" + t.ToString("G") + " " + Enum.GetName(typeof(EventTypes), EventTypes.Maus));



        }

        public void PrismStringHandler(string prismString)
        {
            string eventType = PrismStringSplitter(prismString, 0);

            switch (eventType)
            {
                //case ((Enum.GetName(typeof(EventTypes), EventTypes.Keyboard).ToString();)):
                case ("Keyboard"):
                    //Keyhandler
                    /// <param name="eventType"></param>
                    /// <param name="mouseKeyEventType"></param>
                    /// <param name="mouseKeyEventValue"></param>
                    /// <param name="HWNDString"></param>
                    /// <param name="dateTimeNow"></param>
                    Debug.WriteLine("Keyhandler");
                    KeyHandler(prismString);
                    //Abfrage der festegeleten Actions anhand der eventid aus eventmanager, vorher eventid aus prismstring ermitteln
                    //actionliste weitergeben an ausführende methode
                    //actions ausführen aus eventaction
                    break;
                case ("UIA"):
                    Debug.WriteLine("UIA Handler");
                    break;
                default:
                    Debug.WriteLine("No Handler");
                    break;
            }
        }

        public void KeyHandler(string prismString)
        {
            //getActions for the event - actiongetter
            //doactions - actionhandler
        }

        public void ActionHandler(string actionString)
        {
            string actionType = PrismStringSplitter(actionString, 0);

            switch (actionType)
            {
                case ("Keyboard"):

                    //einstellung in keymanager nachsehen

                    //enstprechende action aufrufen!

                    //Keyhandler
                    /// <param name="eventType"></param>
                    /// <param name="mouseKeyEventType"></param>
                    /// <param name="mouseKeyEventValue"></param>
                    /// <param name="HWNDString"></param>
                    /// <param name="dateTimeNow"></param>
                    /// 

                    EventTypes t = EventTypes.Keyboard;

                    Debug.WriteLine("Keyhandler" + t.ToString("G") + " " + Enum.GetName(typeof(EventTypes), EventTypes.Maus));
                    KeyHandler(actionString);

                    //hwnd id bei eventvalueparsing methode
                    //String foundId = treeOperations.searchNodes.getIdFilteredNodeByHwnd(test);

                    break;
                case ("UIA"):
                    Debug.WriteLine("UIA Handler");
                    break;
                default:
                    Debug.WriteLine("No Handler");
                    break;
            }
        }


        /// <summary>
        /// liefert den Wert an der Stelle X im primsString, Trennung des Pattern durch "_"
        /// </summary>
        /// <param name="prismString"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public string PrismStringSplitter(string prismString, int x)
        {
            string pattern = "_";
            string[] subStrings = System.Text.RegularExpressions.Regex.Split(prismString, pattern);

            return subStrings[x];
        }

        public void GetEventManagerData()
        {
            OSMEvent osmEvent1 = new OSMEvent();
            osmEvent1.Name = "Event1";
            osmEvent1.Priority = 1;
            osmEvent1.Type = EventTypes.Keyboard;
            osmEvent1.Id = treeOperations.generatedIds.generatedIdOsmEvent(osmEvent1);

            // alle Knoten zu einer Event ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Event = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByEvent(osmEvent1.Id);
            Debug.WriteLine("\nAlle Verbindungen, die zu dem Event mit der Id {0} gehören:\n{1}", osmEvent1.Id, String.Join(", ", listOfConnections_Event));
            //// alle Knoten zu einer Knoten ID
            //List<OSMTreeEvenActionConnectorTriple> listOfConnections_Node = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByTree(nodeIdFilteredTree);
            //Debug.WriteLine("Alle Verbindungen, die zu dem Knoten mit der Id {0} gehören:\n{1}", nodeIdFilteredTree, String.Join(", ", listOfConnections_Node));
            //// alle Knoten zu einer Actions ID
            //List<OSMTreeEvenActionConnectorTriple> listOfConnections_Action = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByActrion(osmAction3.Id);
            //Debug.WriteLine("Alle Verbindungen, die zu der Action mit der Id {0} gehören:\n{1}\n", nodeIdFilteredTree, String.Join(", ", listOfConnections_Action));

        }



    }
}
