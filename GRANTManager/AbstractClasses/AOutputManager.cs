﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using OSMElement;

namespace GRANTManager.AbstractClasses
{
    public abstract class AOutputManager : IDisposable
    {
        protected StrategyManager strategyMgr{get; private set;}

        public AOutputManager(StrategyManager strategyMgr) { this.strategyMgr = strategyMgr; }

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
            if (!(this.GetType().FullName.Equals(device.deviceClassTypeFullName) || this.GetType().Namespace.Equals(device.deviceClassTypeNamespace)))
            {
                strategyMgr.setSpecifiedDisplayStrategy(device.deviceClassTypeFullName + ", " + device.deviceClassTypeNamespace);
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
                        using (AOutputManager ads = (AOutputManager)Activator.CreateInstance(type, strategyMgr))
                        {
                            List<Device> devices = ads.getPosibleDevices();
                            allDevices.AddRange(devices);
                        }                        
                    }
                    catch (InvalidCastException ic)
                    {
                        throw new InvalidCastException("Fehler bei GRANTManager.AbstractClasses.getAllPosibleDevices(): " + ic.Message);
                    }
                    catch (ArgumentNullException e)
                    {
                        throw new ArgumentNullException("Fehler bei GRANTManager.AbstractClasses.getAllPosibleDevices(): " + e.Message);
                    }
                }
            }

            return allDevices;
            
        }

        /// <summary>
        /// Ermittelt von einem Device-String das zugehörige Decice-Objekt
        /// </summary>
        /// <param name="deviceString">gibt den String des Devices an</param>
        /// <returns>ein Device-Objekt</returns>
        public Device getDeviceByName(String deviceString)
        {
            List<Device> devices = strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();
            foreach (Device d in devices)
            {
                if (d.ToString().Equals(deviceString))
                {
                    return d;
                }
            }
            return new Device();
        }


        public abstract void Dispose();
    }

        

}
