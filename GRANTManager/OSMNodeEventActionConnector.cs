using OSMElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager
{
    public class OSMNodeEventActionConnector
    {
        /// <summary>
        /// Adds a connection between a node of a Tree (filtered tree or Braille tree) <--> Event <--> Action
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idAction">the id of the action</param>
        /// <param name="osmConnection">(previous) connections</param>
        public static void addOsmNodeEventActionConnection(String idNode, String idEvent, String idAction, ref List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            //TODO: evtl. noch prüfen, ob die Ids existieren
            if(idNode != null && idEvent != null && idAction != null)
            {
                if(!exisitsConnection(idNode, idEvent, idAction, osmConnection))
                {
                    osmConnection.Add(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idAction));
                }
            }
        }

        /// <summary>
        /// Sets a connection (a node of a Tree (filtered tree or Braille tree) <--> Event <--> Action) and delets the old ones 
        /// </summary>
        /// <param name="idNode">the id of the tree</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idAction">the id of the action</param>
        /// <param name="osmConnection">(previous) connections</param>
        public static void setOsmNodeEventActionConnection(String idNode, String idEvent, String idAction, ref List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            //TODO: evtl. noch prüfen, ob die Ids existieren
            if (idNode != null && idEvent != null && idAction != null)
            {
                osmConnection.Clear();
                osmConnection.Add(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idAction));
            }
        }
        /// <summary>
        /// Removes a connection
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="idAction">the id of the action</param>
        /// <param name="osmConnection">(previous) connections</param>
        public static void removeOsmNodeEventActionConnection(String idNode, String idEvent, String idAction, ref List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            if (idNode != null && idEvent != null && idAction != null)
            {
                if (exisitsConnection(idNode, idEvent, idAction, osmConnection))
                {
                    osmConnection.Remove(new OSMTreeEvenActionConnectorTriple(idNode, idEvent, idAction));
                }
            }
        }

        /// <summary>
        /// Finds all connections for a given tree id
        /// </summary>
        /// <param name="idNode">the id of the node in the tree (filtered tree or Braille tree)</param>
        /// <param name="osmConnection">connections</param>
        /// <returns>All connections with the given id of the tree OR <c>null</c></returns>
        public static List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByTree(String idNode, List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            if(idNode == null || osmConnection == null) { return null; }
            return osmConnection.FindAll(p => p.Tree.Equals(idNode) );
        }

        /// <summary>
        /// Finds all connections for a given action id
        /// </summary>
        /// <param name="idAction">the id of the action</param>
        /// <param name="osmConnection">connections</param>
        /// <returns>All connections with the given id of the action OR <c>null</c></returns>
        public static List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByActrion(String idAction, List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            if (idAction == null || osmConnection == null) { return null; }
            return osmConnection.FindAll(p => p.Action.Equals(idAction));
        }

        /// <summary>
        /// Finds all connections for a given event id
        /// </summary>
        /// <param name="idEvent">the id of the event</param>
        /// <param name="osmConnection">connections</param>
        /// <returns>All connections with the given id of the event OR <c>null</c></returns>
        public static List<OSMTreeEvenActionConnectorTriple> getAllOSMNodeEventActionConnectionsByEvent(String idEvent, List<OSMTreeEvenActionConnectorTriple> osmConnection)
        {
            if (idEvent == null || osmConnection == null) { return null; }
            return osmConnection.FindAll(p => p.Event.Equals(idEvent));
        }

        private static Boolean exisitsConnection(String idNode, String idEvent, String idAction, List<OSMTreeEvenActionConnectorTriple> osmConnection) 
        {
            if(osmConnection == null) { return false; }
            return osmConnection.Exists(p => p.Tree.Equals(idNode) && p.Event.Equals(idEvent) && p.Action.Equals(idAction));
        }
    }
}
