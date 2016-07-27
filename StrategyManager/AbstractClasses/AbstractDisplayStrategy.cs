using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace StrategyManager.AbstractClasses
{
    public abstract class AbstractDisplayStrategy : IDisposable
    {
        private StrategyMgr strategyMgr;

        public AbstractDisplayStrategy(StrategyMgr strategyMgr) { this.strategyMgr = strategyMgr; }

        private List<Device> allDevices;

        /// <summary>
        /// Gibt das gewählte ausgabegerät zurück
        /// </summary>
        /// <returns></returns>
        public abstract Device getActiveDevice();

        /// <summary>
        /// Legt die Stiftplatte für die Ausgabe fest (abhängig von diesem wird der Adapter im BrailleIO gewählt)
        /// ändert ggf. auch die ausgewählte DisplayStrategy
        /// </summary>
        /// <param name="device">gibt das ausgewählte Ausgabegerät an</param>
        public void setActiveDevice(Device device)
        {
            //prüfen, ob es nötig ist
           // Console.WriteLine("this = {0}\n neu = {1}", this.GetType().AssemblyQualifiedName.ToString(), device.deviceClassType.AssemblyQualifiedName);
            if (!this.GetType().AssemblyQualifiedName.Equals(device.deviceClassType.AssemblyQualifiedName))
            {
                strategyMgr.setSpecifiedDisplayStrategy(device.deviceClassType.AssemblyQualifiedName);
            }            
            //TODO: bei MVDB Gerät setzen
        }

        /// <summary>
        /// Gibt alle möglichen Ausgabegeräte für den gewählten "Adapter"
        /// </summary>
        /// <returns></returns>
        public abstract List<Device> getPosibleDevices();

        /// <summary>
        /// Gibt von allen implementierten Adaptern die möglichen Ausgabegeräte
        /// </summary>
        /// <returns>Eine Liste mit allen möglichen Ausgabegeräten</returns>
        public List<Device> getAllPosibleDevices()
        {            
            // müssen nur abgefragt werden, wenn das nicht schon mal pasiert ist
            if (allDevices == null || allDevices.Equals(new List<Device>()))
            {
                allDevices = new List<Device>();
                Settings settings = new Settings();
                // alle Implementierungen der abstrakten Klasse erhalten
                List<Strategy> allDisplayStrategys = settings.getPosibleDisplayStrategies();
                foreach (Strategy st in allDisplayStrategys)
                {
                    try
                    {
                        Type type = Type.GetType(st.className);
                        if (type == null) { break; }
                        //beendet ggf. gleich wieder die TCPIP-Verbimdung (Dispose() wird aufgerufen)
                        using (AbstractDisplayStrategy ads = (AbstractDisplayStrategy)Activator.CreateInstance(type, strategyMgr))
                        {
                            List<Device> devices = ads.getPosibleDevices();
                            allDevices.AddRange(devices);
                        }                        
                    }
                    catch (InvalidCastException ic)
                    {
                        throw new InvalidCastException("Fehler bei StrategyManager.AbstractClasses.getAllPosibleDevices(): " + ic.Message);
                    }
                    catch (ArgumentNullException e)
                    {
                        throw new ArgumentNullException("Fehler bei StrategyManager.AbstractClasses.getAllPosibleDevices(): " + e.Message);
                    }
                }
            }

            return allDevices;
            
        }


        public abstract void Dispose();
    }

        

}
