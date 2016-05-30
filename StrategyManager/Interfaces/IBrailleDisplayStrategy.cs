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
        void generatedBrailleUi(ITreeStrategy<OSMElement.OSMElement> osm);
    }
}
