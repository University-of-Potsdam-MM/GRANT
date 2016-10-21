using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GRANTManager;
using Prism.Events;

namespace GRANTManager.Interfaces
{
    public interface IEventManagerStrategy
    {
        void setStrategyMgr(StrategyManager manager);
        //void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        //StrategyManager getStrategyMgr();

        IEventAggregator getSpecifiedEventManagerClass(); 

        //class updateOSMEven();


    }
}
