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
        StrategyManager strategyManager;
        GeneratedGrantTrees grantTrees;
        TreeOperation treeOperations;

        public EventAction(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
        }
        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }
        public void setTreeOperation(TreeOperation treeOperation) { this.treeOperations = treeOperation; }

        public void refreshBrailleView(string viewId)
        {
            throw new NotImplementedException();
        }

        public void refreshBrailleScreen(string screenId)
        {
            throw new NotImplementedException();
        }

        public void changeBrailleScreen(string screenName)
        {
            throw new NotImplementedException();
        }

        public void changeWholeOSMElement()
        {
            throw new NotImplementedException();
        }
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
        public void refreshBrailleOSM(string idGeneratedOffilteredNode, TreeScopeEnum treescopeOfFilteredNode, bool onlyActiveScreen = true)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
