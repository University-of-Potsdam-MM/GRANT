using OSMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.TreeOperations
{
    public class OSMNodeEventActionConnector
    {
        private GeneratedGrantTrees grantTrees;

        public OSMNodeEventActionConnector(GeneratedGrantTrees grantTrees)
        {
            this.grantTrees = grantTrees;
        }

        /// <summary>
        /// Adds a connection between a node of a Tree (filtered tree or Braille tree) <--> Event <--> Action
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idsAction">the ids of the actions</param>
        /// <param name="osmConnection">(previous) connections</param>
        public void addOsmNodeEventActionConnection(String idNode, String idEvent, List<String> idsAction)
        {
            //TODO: evtl. noch prüfen, ob die Ids existieren
            
            if((idNode != null && !idNode.Equals("")) && (idEvent != null  && !idEvent.Equals("")) && idsAction != null ) 
            {
                if(!exisitsConnection(idNode, idEvent, idsAction))
                {
                    grantTrees.osmTreeEventActionConnection.Add(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idsAction));
                }
            }
        }

        /// <summary>
        /// Sets a connection (a node of a Tree (filtered tree or Braille tree) <--> Event <--> Actions) and delets the old ones 
        /// </summary>
        /// <param name="idNode">the id of the tree</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idsAction">the ids of the actions</param>
        public void setOsmNodeEventActionConnection(String idNode, String idEvent, List<String> idsAction)
        {
            //TODO: evtl. noch prüfen, ob die Ids existieren
            if (idNode != null && idEvent != null && idsAction != null)
            {
                grantTrees.osmTreeEventActionConnection.Clear();
                grantTrees.osmTreeEventActionConnection.Add(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idsAction));
            }
        }
        /// <summary>
        /// Removes a connection
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idsAction">the ids of the actions</param>
        public void removeOsmNodeEventActionConnection(String idNode, String idEvent, List<String> idsAction)
        {
            if (idNode != null && idEvent != null && idsAction != null)
            {
                if (exisitsConnection(idNode, idEvent, idsAction))
                {
                    grantTrees.osmTreeEventActionConnection.Remove(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idsAction));
                }
            }
        }

        /// <summary>
        /// Finds all connections for a given tree id
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <returns>All connections with the given id of the tree OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByTree(String idNode)
        {
            if(idNode == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => p.TreeId.Equals(idNode) );
        }

        /// <summary>
        /// Finds all connections for a given action id
        /// </summary>
        /// <param name="idAction">the id of the action</param>
        /// <returns>All connections with the given id of the action OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByActionId(String idAction)
        {
            if (idAction == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => !p.ActionIds.All(p2 =>  p2.Except(idAction).Any()));
        }

        /// <summary>
        /// Finds all connections for a given action name
        /// </summary>
        /// <param name="nameAction">the name of the action</param>
        /// <returns>All connections with the given id of the action OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByActionName(String nameAction)
        {
            if (nameAction == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            String idAction = actionName2Id(nameAction);
            if(idAction == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => !p.ActionIds.All(p2 => p2.Except(idAction).Any()));
        }

        private String actionName2Id(String nameAction)
        {
            if (grantTrees.osmActions == null || nameAction == null) { return null; }
            if (grantTrees.osmActions.Exists(p => p.Name.Equals(nameAction)))
            {
                return grantTrees.osmActions.Find(p => p.Name.Equals(nameAction)).Id;
            }
            return null;
        }


        /// <summary>
        /// Finds all connections for a given event id
        /// </summary>
        /// <param name="idEvent">the id of the event</param>
        /// <returns>All connections with the given id of the event OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByEventId(String idEvent)
        {
            if (idEvent == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => p.EventId.Equals(idEvent));
        }

        /// <summary>
        /// Finds all connections for a given event name
        /// </summary>
        /// <param name="nameEvent">the name of the event</param>
        /// <returns>All connections with the given id of the event OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByEventName(String nameEvent)
        {
            if (nameEvent == null || grantTrees.osmTreeEventActionConnection == null || grantTrees.osmEvents == null) { return null; }
            String eventId = eventName2Id(nameEvent);
            if(eventId == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => p.EventId.Equals(eventId));
        }

        private String eventName2Id(String nameEvent)
        {
            if(grantTrees.osmEvents == null || nameEvent == null) { return null; }
            if(grantTrees.osmEvents.Exists(p => p.Name.Equals(nameEvent)))
            {
                return grantTrees.osmEvents.Find(p => p.Name.Equals(nameEvent)).Id;
            }
            return null;
        }


        private Boolean exisitsConnection(String idNode, String idEvent, List<String> idsAction) 
        {
            if(grantTrees.osmTreeEventActionConnection == null) { return false; }
            Boolean result =  grantTrees.osmTreeEventActionConnection.Exists(p => p.TreeId.Equals(idNode) && p.EventId.Equals(idEvent) && p.ActionIds.All(p2 => idsAction.Contains(p2))); // p.ActionIds.All(p2 => idsAction.Contains(p2)) =>  all ids are appeared in both list
            return result;
        }
    }
}
