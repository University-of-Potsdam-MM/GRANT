using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using GRANTManager;
using GRANTManager.Interfaces;

using Prism.Events;
using OSMElements;
using GRANTManager.TreeOperations;

namespace StrategyEvent
{
    public class EventManager : IEventManager
    {
        private StrategyManager strategyManager;

        //todo nutzugn von prism hier notwendig?
        public IEventAggregator prismEventAggregatorClass;// = new EventAggregator();

        public EventManager(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;

            //todo nutzugn von prism hier notwendig?
            //erhalt des prismaggregator über interface
            prismEventAggregatorClass = strategyManager.getSpecifiedEventStrategy().getSpecifiedEventManagerClass();
            ////Anmeldung für zu verarbeitenden Events
            //prismEventAggregatorClass.GetEvent<StrategyEvent_PRISM.updateOSMEvent>().Subscribe(EventValueParsing);

            ////Festlegung der ersten EventManagerdaten für Test
            //EventExample();
        }

        //erhalt der subscriptions auf ein event
        //https://msdn.microsoft.com/en-us/library/microsoft.practices.prism.pubsubevents.eventbase.subscriptions(v=pandp.50).aspx
        //https://stackoverflow.com/questions/45410678/eventaggregator-get-list-of-subscribers

        //folgende Variablen müssen gesetzt werden
        //private StrategyManager strategyManager;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperations;

        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }
        public void setTreeOperations(TreeOperation treeOperation) { this.treeOperations = treeOperation; }


        public void deliverActionListForEvent(string eventID)
        {

        }

        //für eventmanager!
        public void EventExample()
        {
            #region create example Event
            OSMEvent osmEvent1 = new OSMEvent();
            osmEvent1.Name = "Event1";
            osmEvent1.Priority = 1;
            osmEvent1.Type = EventTypes.Keyboard;
            osmEvent1.Id = treeOperations.generatedIds.generatedIdOsmEvent(osmEvent1);

            OSMEvent osmEvent2 = new OSMEvent();
            osmEvent2.Name = "Event2";
            osmEvent2.Priority = 1;
            osmEvent2.Type = EventTypes.BrailleDisplay;
            osmEvent2.Id = treeOperations.generatedIds.generatedIdOsmEvent(osmEvent2);

            grantTrees.osmEvents = new List<OSMEvent>();
            grantTrees.osmEvents.Add(osmEvent1);
            grantTrees.osmEvents.Add(osmEvent2);
            #endregion

            #region create example actions
            OSMAction osmAction1 = new OSMAction();
            osmAction1.Name = "filterOSM";
            osmAction1.Priority = 1;
            osmAction1.Type = EventTypes.Application;
            osmAction1.Id = treeOperations.generatedIds.generatedIdOsmAction(osmAction1);

            OSMAction osmAction2 = new OSMAction();
            osmAction2.Name = "refreshBrailleOSM";
            osmAction2.Priority = 1;
            osmAction2.Type = EventTypes.BrailleDisplay;
            osmAction2.Id = treeOperations.generatedIds.generatedIdOsmAction(osmAction2);

            OSMAction osmAction3 = new OSMAction();
            osmAction3.Name = "changeBrailleScreen";
            osmAction3.Priority = 1;
            osmAction3.Type = EventTypes.BrailleDisplay;
            osmAction3.Id = treeOperations.generatedIds.generatedIdOsmAction(osmAction3);

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
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Event = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByEventId(osmEvent1.Id);
            Debug.WriteLine("\nAlle Verbindungen, die zu dem Event mit der Id {0} gehören:\n{1}", osmEvent1.Id, String.Join(", ", listOfConnections_Event));

            //OSMAction test 
            List<string> testActions2GivenEvent = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByEventId(osmEvent1.Id)[0].ActionIds;

            //wichtige abfrage der action, als objekt und davon des namens über die methdoe find https://stackoverflow.com/questions/9854917/how-can-i-find-a-specific-element-in-a-listt
            //für die weitere verarbeitung in getaction()
            testActionName = grantTrees.osmActions.Find(x => x.Id == testActions2GivenEvent[0]).Name; //testActions2GivenEvent[0].

            

            // alle Knoten zu einer Knoten ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Node = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByTree(nodeIdFilteredTree);
            Debug.WriteLine("Alle Verbindungen, die zu dem Knoten mit der Id {0} gehören:\n{1}", nodeIdFilteredTree, String.Join(", ", listOfConnections_Node));
            // alle Knoten zu einer Actions ID
            List<OSMTreeEvenActionConnectorTriple> listOfConnections_Action = treeOperations.oSMNodeEventActionConnector.getAllOSMNodeEventActionConnectionsByActionId(osmAction3.Id);
            Debug.WriteLine("Alle Verbindungen, die zu der Action mit der Id {0} gehören:\n{1}\n", nodeIdFilteredTree, String.Join(", ", listOfConnections_Action));
        }

        public string testActionName;

        //methode in manager rein!
        public void getaction()
        {
            if (testActionName == "filterOSM")
            {
                //hwnd id bei eventvalueparsing methode
                //übergabe der id des hwnd, siehe             
                //String foundId = treeOperations.searchNodes.getIdFilteredNodeByHwnd(test);

                //aufruf der methde für die filterung des knotens mit der id und in welchem umfang, letzte übergebene variable kann evtl raus
                strategyManager.getSpecifiedEventAction().filterOSM("asds", TreeScopeEnum.Application,"nix");
                //EventAction
            }
        }
    }
}
