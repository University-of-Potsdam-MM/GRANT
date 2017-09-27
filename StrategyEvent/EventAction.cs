using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;
using GRANTManager.TreeOperations;

namespace StrategyEvent
{
    public class EventAction : IEventAction
    {
        private StrategyManager strategyManager;
        private GeneratedGrantTrees grantTrees;
        private TreeOperation treeOperations;

        public EventAction(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
        }
        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperations = treeOperation; }
       
        #region filter
        /// <summary>
        /// Filtered an OSM node (and depending on the <para>treescope</para> his sibilings, parents children etc. ) and updates the filtered tree object
        /// </summary>
        /// <param name="generatedId">the generated id of a node in the filtered tree</param>
        /// <param name="treescope">the scope for filtering</param>
        /// <param name="eventAction"></param>
        public void filterOSM(string generatedId, TreeScopeEnum treescope, String eventAction)
        {
            if(generatedId == null) { return; }
            switch (treescope)
            {                
                case TreeScopeEnum.Ancestors:
                    throw new NotImplementedException();
                    break;
                case TreeScopeEnum.Sibling:
                    throw new NotImplementedException();
                    break;
                default: //Application, Subtree, Children, Descendants, Element
                    treeOperations.updateNodes.filteredTree(generatedId, treescope);
                    break;
            }
        }
        #endregion

        #region Output/Braille
        /// <summary>
        /// Refreshs all Braille OSM elements depending on the given id of a node in the filtered tree; it wouldn't be filtered 
        /// </summary>
        /// <param name="idGeneratedOfFilteredNode">a node id of the filtered tree; the connected (Braille) nodes will be refreshed</param>
        /// <param name="treescopeOfFilteredNode">the tree scope for updating based on the filtered tree</param>
        /// <param name="onlyActiveScreen"><c>true</c> if only the Braille nodes on the active scree shold be updated, otherwise all connected Braille nodes will be updated</param>
        public void refreshBrailleOSM(string idGeneratedOfFilteredNode, TreeScopeEnum treescopeOfFilteredNode, bool onlyActiveScreen = true)
        {
            treeOperations.updateNodes.refreshBrailleOSM(idGeneratedOfFilteredNode, treescopeOfFilteredNode, onlyActiveScreen);
        }

        public void refreshBrailleView(string viewId)
        {
            throw new NotImplementedException();
        }

        public void refreshBrailleScreen(string screenId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// change the visible screen
        /// </summary>
        /// <param name="screenName">name of the new visible screen</param>
        public void changeBrailleScreen(string screenName)
        {
            if (screenName == null || screenName.Equals("")) { return; }
            if (strategyManager.getSpecifiedBrailleDisplay() == null) { return; }
            if (treeOperations.searchNodes.getPosibleScreenNames().Contains(screenName))
            {
                strategyManager.getSpecifiedBrailleDisplay().setVisibleScreen(screenName);
            }
        }

        /// <summary>
        /// change the visible screen
        /// </summary>
        /// <param name="screenId">id of the new visible screen OR a node id of the new visible screen branch</param>
        public void changeBrailleScreenById(string screenId)
        {
            String screenName = treeOperations.searchNodes.getScreenIdToScreenName(screenId);
            if(screenName == null) { return; }
            changeBrailleScreen(screenName);
        }

        #endregion
    }
}
