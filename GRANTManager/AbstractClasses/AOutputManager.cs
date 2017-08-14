using System;
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
        /// Returns the choosen (braille) device
        /// </summary>
        /// <returns>the choosen device</returns>
        public abstract Device getActiveDevice();

        /// <summary>
        /// Sets the (braille) device -> depending on this an adapter is selectet in BrailleIO
        /// if necessary the <see cref="StrategyManager.specifiedDisplayStrategy"/> is changed
        /// </summary>
        /// <param name="device">the choosen device</param>
        public void setActiveDevice(Device device)
        {
            if (!this.GetType().FullName.Equals(device.deviceClassTypeFullName) || !this.GetType().Namespace.Equals(device.deviceClassTypeNamespace))
            {
                if (device.deviceClassTypeFullName != null && device.deviceClassTypeNamespace != null)
                {
                    strategyMgr.setSpecifiedDisplayStrategy(device.deviceClassTypeFullName + ", " + device.deviceClassTypeNamespace);
                    strategyMgr.getSpecifiedDisplayStrategy().getAllPosibleDevices();

                }
            }
            if (device.deviceClassTypeNamespace != null && device.deviceClassTypeNamespace.Equals("StrategyMVBD"))
            {
                strategyMgr.getSpecifiedDisplayStrategy().setDevice(device);
            }
        }

        protected abstract void setDevice(Device device);

        /// <summary>
        /// Returns all possible devices for the choosen Adapter
        /// </summary>
        /// <returns>a list of possible devices for the choosen Adapter</returns>
        public abstract List<Device> getPosibleDevices();

        /// <summary>
        /// Returns ALL possible devices (for all implemented Adapters)
        /// </summary>
        /// <returns>a list of ALL possible devices</returns>
        public List<Device> getAllPosibleDevices()
        {            
            // the list of devices are only requested if no list exist
            if (allDevices == null || allDevices.Equals(new List<Device>()))
            {
                allDevices = new List<Device>();
                Settings settings = new Settings();
                // gets all implementations of this abstract class
                List<Strategy> allDisplayStrategys = settings.getPosibleDisplayStrategies();
                foreach (Strategy st in allDisplayStrategys)
                {
                    try
                    {
                        Type type = Type.GetType(st.className);
                        if (type == null) { break; }
                        using (AOutputManager ads = (AOutputManager)Activator.CreateInstance(type, strategyMgr))
                        {
                            List<Device> devices = ads.getPosibleDevices();
                            allDevices.AddRange(devices);
                        }                        
                    }
                    catch (InvalidCastException ic)
                    {
                        throw new InvalidCastException("Exception in GRANTManager.AbstractClasses.getAllPosibleDevices(): " + ic.Message);
                    }
                    catch (ArgumentNullException e)
                    {
                        throw new ArgumentNullException("Exception in GRANTManager.AbstractClasses.getAllPosibleDevices(): " + e.Message);
                    }
                }
            }
            return allDevices;           
        }

        /// <summary>
        /// Seeks the device object to a device name
        /// </summary>
        /// <param name="deviceString">the name of a device</param>
        /// <returns>a device object</returns>
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

        /// <summary>
        /// Determines whether a display strategy is available
        /// </summary>
        /// <param name="displayStrategy">the display strategy object</param>
        /// <returns><c>true</c> if the strategy available, otherwise <c>false</c></returns>
        public bool isDisplayStrategyAvailable(AOutputManager displayStrategy)
        {
            if (displayStrategy.GetType().Namespace != null && displayStrategy.GetType().Namespace.Equals("StrategyMVBD"))
            {
                return displayStrategy.isDisplayStrategyAvailable();
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines whether a display strategy is available
        /// </summary>
        /// <returns><c>true</c> if the strategy available, otherwise <c>false</c></returns>
        protected virtual bool isDisplayStrategyAvailable() { return true; }
    }
}
