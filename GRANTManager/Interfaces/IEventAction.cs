using GRANTManager.TreeOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    public interface IEventAction
    {
        //Übergabe aller OSM-Bäume und -Verbindungen: OSM-Original, OSM-Braille - Verbindung OSMTreeConnection; OSM-Event OSM-Action und Verbindung zwischen diesen und OSM-Original/Braille: OSMNodeEventActionConnector
        void setGrantTrees(GeneratedGrantTrees trees);
        void setTreeOperation(TreeOperation treeOperation);

        #region filter
        /// <summary>
        /// Filtered an OSM node (and depending on the <para>treescope</para> his sibilings, parents children etc. ) and updates the filtered tree object
        /// </summary>
        /// <param name="generatedId">the generated id of a node in the filtered tree</param>
        /// <param name="treescope">the scope for filtering</param>
        /// <param name="eventAction"></param>
        void filterOSM(String generatedId, TreeScopeEnum treescope, String eventAction);
        #endregion

        #region Output/Braille
        /// <summary>
        /// Refreshs all Braille OSM elements depending on the given id of a node in the filtered tree
        /// </summary>
        /// <param name="idGeneratedOffilteredNode">a node id of the filtered tree; the connected (Braille) nodes will be refreshed</param>
        /// <param name="treescopeOfFilteredNode">the tree scope for updating based on the filtered tree</param>
        /// <param name="onlyActiveScreen"><c>true</c> if only the Braille nodes on the active scree shold be updated, otherwise all connected Braille nodes will be updated</param>
        void refreshBrailleOSM(String idGeneratedOffilteredNode, TreeScopeEnum treescopeOfFilteredNode, Boolean onlyActiveScreen = true);

        void refreshBrailleView(String viewId);
        void refreshBrailleScreen(String screenId);
        void changeBrailleScreen(String screenName);
        void changeBrailleScreenById(string screenId);
        #endregion
    }
}
