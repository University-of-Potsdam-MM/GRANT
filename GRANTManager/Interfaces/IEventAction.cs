using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    public interface IEventAction
    {
        void setGrandTrees(GeneratedGrantTrees trees);
        void refreshBrailleView(String viewId);
        void refreshBrailleScreen(String screenId);
        void changeBrailleScreen(String screenName);
    }
}
