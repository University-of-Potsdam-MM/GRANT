using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSMElement
{

    /// <summary>
    /// Specifies the relationship between "filteredTree" and "brailleTree" based on the generated id
    /// http://stackoverflow.com/questions/166089/what-is-c-sharp-analog-of-c-stdpair
    /// </summary>
    /// <typeparam name="T">The generated id of the node in the filtered tree.</typeparam>
    /// <typeparam name="U">The generated id of the node in the braille (output) tree</typeparam>
    [Serializable]
    public class OsmConnector<T, U>
    {
        public OsmConnector()
        {
        }

        /// <summary>
        /// the relationship between "filteredTree" and "brailleTree" based on the generated id
        /// </summary>
        /// <param name="filteredTree">The generated id of the node in the filtered tree.</param>
        /// <param name="brailleTree">The generated id of the node in the braille (output) tree</param>
        public OsmConnector(T filteredTree, U brailleTree)
        {
            this.FilteredTree = filteredTree;
            this.BrailleTree = brailleTree;
        }

        /// <summary>
        /// id of a node in the filtered tree
        /// </summary>
        public T FilteredTree { get; set; }
        /// <summary>
        /// id of a node in the braille tree
        /// </summary>
        public U BrailleTree { get; set; }

        public override string ToString()
        {
            return String.Format("OsmConnector -- FilteredTree: {0}, BrailleTree: {1}", FilteredTree, BrailleTree);
        }
    }

}
