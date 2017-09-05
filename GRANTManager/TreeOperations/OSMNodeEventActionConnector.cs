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
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByActrion(String idAction)
        {
            if (idAction == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => !p.ActionIds.All(p2 =>  p2.Except(idAction).Any()));
        }

        /// <summary>
        /// Finds all connections for a given event id
        /// </summary>
        /// <param name="idEvent">the id of the event</param>
        /// <returns>All connections with the given id of the event OR <c>null</c></returns>
        public List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByEvent(String idEvent)
        {
            if (idEvent == null || grantTrees.osmTreeEventActionConnection == null) { return null; }
            return grantTrees.osmTreeEventActionConnection.FindAll(p => p.EventId.Equals(idEvent));
        }

        private Boolean exisitsConnection(String idNode, String idEvent, List<String> idsAction) 
        {
            if(grantTrees.osmTreeEventActionConnection == null) { return false; }
            Boolean result =  grantTrees.osmTreeEventActionConnection.Exists(p => p.TreeId.Equals(idNode) && p.EventId.Equals(idEvent) && p.ActionIds.All(p2 => idsAction.Contains(p2))); // p.ActionIds.All(p2 => idsAction.Contains(p2)) =>  all ids are appeared in both list
            return result;
        }
    }
}
