using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OSMElement;
using GRANTManager.Interfaces;

namespace GRANTManager
{
    public class FilterstrategiesOfTree
    {
        /// <summary>
        /// Returns the main filter strategy for filtering nodes
        /// </summary>
        /// <param name="filteredTree">the filtered tree object</param>
        /// <param name="filterstrategies">list of filterstrategies</param>
        /// <param name="treeStrategy">tree strategy (<see cref="StrategyManager.specifiedTree"/>) </param>
        /// <returns></returns>
        public static FilterstrategyOfNode<String, String, String> getMainFilterstrategyOfTree(Object filteredTree, List<FilterstrategyOfNode<String, String, String>> filterstrategies, ITreeStrategy<OSMElement.OSMElement> treeStrategy)
        {
            if (filteredTree == null || !treeStrategy.HasChild(filteredTree))
            {
                Debug.WriteLine("Can't find a filter strategy in this tree!");
                return null;
            }
            return getFilterstrategyOfNode(treeStrategy.GetData(treeStrategy.Child(filteredTree)).properties.IdGenerated, filterstrategies);
        }

        /// <summary>
        /// Adds a filter strategy for a given node.
        /// </summary>
        /// <param name="idGeneratedOfNode">generated id of the filtered node</param>
        /// <param name="filterstrategyType">type of the new filter strategy for this node</param>
        /// <param name="filterstrategies">list of all existing filter strategies for this tree</param>
        /// <returns><c>true</c>  if the filter strategy was added; otherwise <c>false</c></returns>
        public static bool addFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            //TODO: prüfen, ob der Standardfilter genutzt wird
            if (idGeneratedOfNode == null || idGeneratedOfNode.Equals("")) { Debug.WriteLine("No Id specified - strategy couldn't be set!"); return false; }
            FilterstrategyOfNode<String, String, String> filterstrategyNew = new FilterstrategyOfNode<string, string, string>();
            filterstrategyNew.IdGenerated = idGeneratedOfNode;
            filterstrategyNew.FilterstrategyFullName = filterstrategyType.FullName;
            filterstrategyNew.FilterstrategyDll = filterstrategyType.Namespace;
            if (!filterstrategies.Contains(filterstrategyNew))
            {
                //Checkes whether an other strategy exists for this node and deletes it
                 FilterstrategyOfNode<String, String, String> filterstrategyOld = getFilterstrategyOfNode(idGeneratedOfNode, filterstrategies);
                 if (filterstrategyOld != null)
                 {
                     filterstrategies.Remove(filterstrategyOld);
                 }
                 filterstrategies.Add(filterstrategyNew);
                 return true;
            }
            Debug.WriteLine("This strategy already exists.");
            return false;
        }

        /// <summary>
        /// Deletes for this node the filter strategy
        /// </summary>
        /// <param name="idGeneratedOfNode">generated id of the filtered node</param>
        /// <param name="filterstrategyType">type of the new filter strategy for this node</param>
        /// <param name="filterstrategies">list of all existing filter strategies for this tree</param>
        /// <returns><c>true</c> if the strategy was deleted; otherwise <c>false</c></returns>
        public static bool removeFilterstrategyOfNode(String idGeneratedOfNode, Type filterstrategyType, ref List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            FilterstrategyOfNode<String, String, String> filterstrategyOld = getFilterstrategyOfNode(idGeneratedOfNode, filterstrategies);
            if (filterstrategyOld != null)
            {
                filterstrategies.Remove(filterstrategyOld);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gives the filter strategy of a node
        /// </summary>
        /// <param name="idGeneratedOfNode">generated id of the filtered node</param>
        /// <param name="filterstrategies">list of all existing filter strategies for this tree</param>
        /// <returns>a <c>FilterstrategyOfNode</c> object with the strategy for this node</returns>
        public static FilterstrategyOfNode<String, String, String> getFilterstrategyOfNode(String idGeneratedOfNode, List<FilterstrategyOfNode<String, String, String>> filterstrategies)
        {
            if (filterstrategies == null || filterstrategies.Equals(new List<FilterstrategyOfNode<String, String, String>>()) || filterstrategies.Count == 0 || idGeneratedOfNode.Equals("")) { return null; }

            FilterstrategyOfNode<String, String, String> filterstrategyFound = filterstrategies.Find(r => r.IdGenerated.Equals(idGeneratedOfNode));
            if (!(filterstrategyFound == null || filterstrategyFound.Equals(new FilterstrategyOfNode<String, String, String>())))
            {
                return filterstrategyFound;
            }
            return null;
        }

    }
}
