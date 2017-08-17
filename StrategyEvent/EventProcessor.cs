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
    
    public class EventProcessor : IEventProcessor
    {
        private StrategyManager strategyManager;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperations;

        public EventProcessor(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;

            eventTest();
        }
        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }
        public void setTreeOperations(TreeOperation treeOperations) { this.treeOperations = treeOperations; }

        //todo inti der prismeventaggreagtorclass in grantapplication einbauen
        public IEventAggregator prismEventAggregatorClass;// = new EventAggregator();

        public void eventTest()
        {
            //erhalt des prismaggregator über interface
            prismEventAggregatorClass = strategyManager.getSpecifiedEventStrategy().getSpecifiedEventManagerClass();

            //prismEventAggregatorClass.GetEvent<GRANTManager.PRISMHandler_Class.updateOSMEvent_PRISMHandler_GrantManager>().Subscribe(generateOSMmwxaml); ///hier muss ein subscribe hin
            prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Subscribe(eventValueParsing); ///hier muss ein subscribe hin

            //Console.WriteLine("test winevent verarbeitet in mainwindowxaml_");


        }

        //Aufschlüsselung der Informationen aus dem Event-String in ein OSMEvent-Format für die weitere Verarbeitung
        //liefert den string, die eventid
        public void eventValueParsing(string todo)
        {
            Debug.WriteLine("winevent verarbeitet in eventprocessor" + todo);
            //osm = "werhers";

            //Aufruf eienr Methode aus EventAction über startegymanager
            //strategyManager.getSpecifiedEventAction().filterOSM();

            //strategyManager.getSpecifiedEventManager().deliverActionListForEvent();


            //strategyManager.getSpecifiedEventManager()
            //id des events ermittelt
        }

        public void eventManagerActionList()
        {

        }

        public void EventExample()
        {
            #region create example Event
            OSMEvent osmEvent1 = new OSMEvent();
            osmEvent1.Id = "e_abc"; //TODO: generate id
            osmEvent1.Name = "Event1";
            osmEvent1.Priority = 1;
            osmEvent1.Type = EventTypes.Keyboard;

            OSMEvent osmEvent2 = new OSMEvent();
            osmEvent2.Id = "e_def"; //TODO: generate id
            osmEvent2.Name = "Event2";
            osmEvent2.Priority = 1;
            osmEvent2.Type = EventTypes.BrailleDisplay;

            grantTrees.osmEvents = new List<OSMEvent>();
            grantTrees.osmEvents.Add(osmEvent1);
            grantTrees.osmEvents.Add(osmEvent2);
            #endregion

            #region create example actions
            OSMAction osmAction1 = new OSMAction();
            osmAction1.Id = "a_abc"; //TODO: generate id
            osmAction1.Name = "filterOSM";
            osmAction1.Priority = 1;
            osmAction1.Type = EventTypes.Application;

            OSMAction osmAction2 = new OSMAction();
            osmAction2.Id = "a_def"; //TODO: generate id
            osmAction2.Name = "refreshBrailleOSM";
            osmAction2.Priority = 1;
            osmAction2.Type = EventTypes.BrailleDisplay;

            OSMAction osmAction3 = new OSMAction();
            osmAction3.Id = "a_ghj"; //TODO: generate id
            osmAction3.Name = "changeBrailleScreen";
            osmAction3.Priority = 1;
            osmAction3.Type = EventTypes.BrailleDisplay;

            grantTrees.osmActions = new List<OSMAction>();
            grantTrees.osmActions.Add(osmAction1);
            grantTrees.osmActions.Add(osmAction2);
            grantTrees.osmActions.Add(osmAction3);
            #endregion
            // alle existierenden Verbindungen:
            List<OSMTreeEvenActionConnectorTriple> osmTreeEventActionConnection = grantTrees.osmTreeEventActionConnection;


            String nodeIdFilteredTree = "417F2ACC323396E993B4DC2AD2515D5E";
            String nodeIdBrailleTree = "692CD3C3D18675DC98C98130F6CDAD3E";
            treeOperations.oSMNodeEventActionConnector.addOsmNodeEventActionConnection(nodeIdFilteredTree, osmEvent1.Id, new List<string>() { osmAction1.Id, osmAction2.Id });
            treeOperations.oSMNodeEventActionConnector.addOsmNodeEventActionConnection(nodeIdBrailleTree, osmEvent2.Id, new List<string>() { osmAction3.Id });
            
            // alle Knoten zu einer Event ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Event = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByEvent(osmEvent1.Id);
            Debug.WriteLine("\nAlle Verbindungen, die zu dem Event mit der Id {0} gehören:\n{1}", osmEvent1.Id, String.Join(", ", listOfConnections_Event) );
            // alle Knoten zu einer Knoten ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Node = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByTree(nodeIdFilteredTree);
            Debug.WriteLine("Alle Verbindungen, die zu dem Knoten mit der Id {0} gehören:\n{1}", nodeIdFilteredTree, String.Join(", ", listOfConnections_Node));
            // alle Knoten zu einer Actions ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Action = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByActrion(osmAction1.Id);
            Debug.WriteLine("Alle Verbindungen, die zu der Action mit der Id {0} gehören:\n{1}\n", nodeIdFilteredTree, String.Join(", ", listOfConnections_Action) );

            //
        }


    }
}
