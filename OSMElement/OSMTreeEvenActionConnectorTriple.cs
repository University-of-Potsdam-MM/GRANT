using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{
    [Serializable]
    public class OSMTreeEvenActionConnectorTriple
    {
        public OSMTreeEvenActionConnectorTriple() { }
        public OSMTreeEvenActionConnectorTriple(String Tree, String Event, String Action)
        {
            this.Tree = Tree;
            this.Event = Event;
            this.Action = Action;
        }

        /// <summary>
        /// id of a node in the tree (filtered tree or grand tree)
        /// </summary>
        public String Tree { get; set; }
        public String Event { get; set; }
        public String Action { get; set; }

        public override string ToString()
        {
            return String.Format("OSMTreEvenActionConnector -- Tree: {0}, Event: {1}, Action: {2}", Tree, Event, Action);
        }
    }
}
