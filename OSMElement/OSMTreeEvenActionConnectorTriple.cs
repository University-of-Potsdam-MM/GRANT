using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{
    [Serializable]
    public class OSMTreeEvenActionConnectorTriple
    {
        public OSMTreeEvenActionConnectorTriple() { }
        public OSMTreeEvenActionConnectorTriple(String TreeId, String EventId, List<String> ActionId)
        {
            this.TreeId = TreeId;
            this.EventId = EventId;
            this.ActionIds = ActionId;
        }

        /// <summary>
        /// id of a node in the tree (filtered tree or grand tree)
        /// </summary>
        public String TreeId { get; set; }
        public String EventId { get; set; }
        public List<String> ActionIds { get; set; }

        public override string ToString()
        {
            return String.Format("OSMTreEvenActionConnector -- TreeId: {0}, EventId: {1}, ActionIds: {2}", TreeId, EventId, String.Join(", ", ActionIds) );
        }
    }
}
