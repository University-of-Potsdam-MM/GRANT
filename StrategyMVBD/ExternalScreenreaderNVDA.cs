using GRANTManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using OSMElements;

namespace StrategyMVBD
{
    /// <summary>
    /// use NVDA (see: https://www.nvaccess.org/) over MVBD (see: http://download.metec-ag.de/MVBD/ )
    /// </summary>
    public class ExternalScreenreaderNVDA : IExternalScreenreader
    {
        private StrategyManager strategyMgr;
        private GeneratedGrantTrees grantTrees;
        private MvbdConnectionTCPIP mvbdTcpIpConnection;
        internal String lastContent { get; set; }

        public ExternalScreenreaderNVDA(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            mvbdTcpIpConnection = new MvbdConnectionTCPIP(strategyMgr, this);
            lastContent = null;
        }

        public void setGeneratedGrantTrees(GeneratedGrantTrees grantTrees)
        {
            this.grantTrees = grantTrees;
        }

        public OSMElement getContentAsOSM()
        {
            //throw new NotImplementedException();
            /*
             * TCP/IP connection to MVBD is necessary
             * 
             * 
             */

             // Das hier ist erstmal nur zum Testen
            OSMElement osm = new OSMElement();
            osm.properties.valueFiltered = lastContent;
            return osm;
            
        }

        public void getListOfScreenreaderCommands()
        {
            throw new NotImplementedException();
        }

        public void sendScreenreaderCommand()
        {
            throw new NotImplementedException();
        }


    }
}
