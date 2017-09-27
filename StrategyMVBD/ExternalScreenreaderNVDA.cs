using GRANTManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager;
using OSMElements;
using System.Security.Cryptography;

namespace StrategyMVBD
{
    /// <summary>
    /// use NVDA (see: https://www.nvaccess.org/) over MVBD (see: http://download.metec-ag.de/MVBD/ )
    /// </summary>
    public class ExternalScreenreaderNVDA : IExternalScreenreader
    {
        private StrategyManager strategyMgr;
        private MvbdConnectionTCPIP mvbdTcpIpConnection;
        internal String lastContent { get; set; }

        public ExternalScreenreaderNVDA(StrategyManager strategyMgr)
        {
            this.strategyMgr = strategyMgr;
            mvbdTcpIpConnection = new MvbdConnectionTCPIP(strategyMgr, this);
            lastContent = null;
        }
                
        /// <summary>
        /// Returns an <c>OSMElement</c> with the last received content from NVDA
        /// </summary>
        /// <returns></returns>
        public OSMElement getScreenreaderContent()
        {
            //throw new NotImplementedException();
            /*
             * TCP/IP connection to MVBD is necessary
             * 
             * 
             */
            if (!strategyMgr.getSpecifiedOperationSystem().isApplicationRunning("NVDA")) { return null; }
            // Das hier ist erstmal nur zum Testen
            Settings settings = new Settings();
            List<Strategy> externalSRStrategies = settings.getPossibleExternalScreenreaders();
            if (externalSRStrategies.Exists(p => p.className.Equals(this.GetType().FullName + ", " + this.GetType().Namespace)))
            {
                OSMElement osm = new OSMElement();
                osm.properties.valueFiltered = lastContent;
                osm.properties.controlTypeFiltered = "external screenreader";
                osm.properties.grantFilterStrategy = externalSRStrategies.Find(p => p.className.Equals(this.GetType().FullName + ", " + this.GetType().Namespace)).userName;

                osm.properties.IdGenerated = generatedIdforOsmNode();
                return osm;
            }
            return null;
        }

        /// <summary>
        /// Generated the id for a NVDA node in the filtered tree;
        /// The Id is always the same.
        /// </summary>
        /// <returns></returns>
        private String generatedIdforOsmNode()
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(this.GetType().FullName));
            }
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }
            String tmpHash = String.Join(" : ", hash.Select(p => p.ToString()).ToArray());
            return sb.ToString();
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
