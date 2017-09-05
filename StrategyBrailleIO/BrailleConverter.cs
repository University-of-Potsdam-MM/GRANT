using GRANTManager.Interfaces;
using BrailleIO.Renderer.BrailleInterpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyBrailleIO
{
    public class BrailleConverter : IBrailleConverter
    {
        private IBrailleInterpreter brailleInterpreter;
        public BrailleConverter()
        {
            brailleInterpreter = new BrailleIO.Renderer.BrailleInterpreter.SimpleBrailleInterpreter();
        }
        public string getStringFromDots(List<List<int>> dots)
        {
            return brailleInterpreter.GetStringFormDots(dots);
        }
    }
}
