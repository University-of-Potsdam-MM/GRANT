using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElements
{

    /// <summary>
    /// Specifies the relationship between "filteredTree" and "brailleTree" based on the generated id
    /// http://stackoverflow.com/questions/166089/what-is-c-sharp-analog-of-c-stdpair
    /// </summary>
    [Serializable]
    public class OsmTreeConnectorTuple
    {
        public OsmTreeConnectorTuple()
        {
        }

        /// <summary>
        /// the relationship between "filteredTree" and "brailleTree" based on the generated id
        /// </summary>
        /// <param name="filteredTree">The generated id of the node in the filtered tree.</param>
        /// <param name="brailleTree">The generated id of the node in the braille (output) tree</param>
        public OsmTreeConnectorTuple(String filteredTree, String brailleTree)
        {
            this.FilteredTreeId = filteredTree;
            this.BrailleTreeId = brailleTree;
        }

        /// <summary>
        /// id of a node in the filtered tree
        /// </summary>
        public String FilteredTreeId { get; set; }
        /// <summary>
        /// id of a node in the braille tree
        /// </summary>
        public String BrailleTreeId { get; set; }

        public override string ToString()
        {
            return String.Format("OsmTreeConnector -- FilteredTree: {0}, BrailleTree: {1}", FilteredTreeId, BrailleTreeId);
        }
    }

}
