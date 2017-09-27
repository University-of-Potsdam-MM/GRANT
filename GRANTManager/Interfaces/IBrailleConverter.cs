using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    public interface IBrailleConverter
    {
        /// <summary>
        /// transformed (computer) Braille dots to String
        /// </summary>
        /// <param name="dots"></param>
        /// <returns></returns>
        String getStringFromDots(List<List<int>> dots);
    }
}
