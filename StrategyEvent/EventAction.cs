using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using GRANTManager.Interfaces;

namespace StrategyEvent
{
    public class EventAction : IEventAction
    {
        StrategyManager strategyManager;
        GeneratedGrantTrees grantTrees;

        public EventAction(StrategyManager strategyMgr)
        {
            strategyManager = strategyMgr;
        }
        public void setGrantTrees(GeneratedGrantTrees trees) { grantTrees = trees; }

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
    }
}
