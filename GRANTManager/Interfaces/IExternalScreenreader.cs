using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRANTManager.Interfaces
{
    /// <summary>
    /// Interface to use external screenreader like NVDA
    /// </summary>
    public interface IExternalScreenreader
    {
        void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees);
        
        OSMElements.OSMElement getContentAsOSM();
        void sendScreenreaderCommand();
        void getListOfScreenreaderCommands(); //TODO return-Wert
    }
}
