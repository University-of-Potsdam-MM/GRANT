using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSMElement;

namespace StrategyManager.Interfaces
{
    public interface IBrailleDisplayStrategy
    {
        void initializedSimulator();
        void generatedBrailleUi(XMLDevice xmlObject, ITreeStrategy<OSMElement.OSMElement> tree, ITreeStrategy<OSMElement.OSMElement> treeStrategy);
    }
}
