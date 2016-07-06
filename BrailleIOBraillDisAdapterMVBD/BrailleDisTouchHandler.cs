using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyperBraille.HBBrailleDis;
using System.Drawing;

namespace BrailleIOBraillDisAdapter
{
    internal static class BrailleDisTouchHandler
    {
        private const double maxTouchValue = 70;

        private static double max = maxTouchValue;

        /// <summary>
        /// Gets the touch matrix corresponding to a braille matrix.
        /// Interprets the given touched module structure and project it to a
        /// two dimensional array corresponding to the pin-matrix of the given 
        /// device.
        /// </summary>
        /// <param name="states">The states of touched sensor modules.</param>
        /// <param name="device">Informations about the used device.</param>
        /// <param name="normalized">if set to <c>true</c> then the returned values are normalized values between 0.0 and 1.0.</param>
        /// <returns>A two dimensional array projecting touched modules/sensors to pins.</returns>
        internal static double[,] getTouchMatrixCorrespondingToBrailleMatrix(BrailleDisModuleState[] states, DeviceTypeInformation device, bool normalized, double[,] correction)
        {
            double[,] m;
            if (device != null)
            {
                m = new double[device.NumberOfPinRows, device.NumberOfPinColumns];

                foreach (var module in states)
                {
                    var sensorPins = getSensorPins(module, device);
                    foreach (var pin in sensorPins) {
                        if (m.GetLength(0) > pin.Y && m.GetLength(1) > pin.X)
                        {
                            max = Math.Max(max, pin.Touch);

                            double val = normalized ? Math.Min(pin.Touch / maxTouchValue, 1) : pin.Touch;
                            if (correction != null && correction.GetLength(0) > pin.Y && correction.GetLength(1) > pin.X && correction[pin.Y, pin.X] > 0)
                                val = Math.Max(0, val - correction[pin.Y, pin.X]);
                            m[pin.Y, pin.X] = val;

                        }
                    }
                }
            }
            else
            {
                m = new double[0, 0];
            }
            return m;
        }
        /// <summary>
        /// Gets the touch matrix corresponding to a braille matrix.
        /// Interprets the given touched module structure and project it to a
        /// two dimensional array corresponding to the pin-matrix of the given 
        /// device.
        /// </summary>
        /// <param name="states">The states of touched sensor modules.</param>
        /// <param name="device">Informations about the used device.</param>
        /// <returns>A two dimensional array projecting touched modules/sensors to pins.</returns>
        internal static double[,] getTouchMatrixCorrespondingToBrailleMatrix(BrailleDisModuleState[] states, DeviceTypeInformation device, double[,] correction)
        {
            return getTouchMatrixCorrespondingToBrailleMatrix(states, device, false, correction);
        }

        private static List<Pin> getSensorPins(BrailleDisModuleState module, DeviceTypeInformation device)
        {
            var pl = new List<Pin>();
            if (device != null)
            {
                int r, c;
                r = module.SensorRow;
                c = module.ModuleColumn;
                var mr = pinRowsPerSensor(device);
                var mc = pinColumsPerSensor(device);
                double val = module.CurrentValue;
                int rCorrector = r % 2;

                for (int i = 0; i < mr; i++)
                {
                    for (int j = 0; j < mc; j++)
                    {
                        try
                        {
                            Pin p = new Pin(
                                (int)Math.Round((c * mc + j), MidpointRounding.AwayFromZero),
                                (int)Math.Round(r * mr + i - rCorrector, MidpointRounding.AwayFromZero), 
                                val);
                            pl.Add(p);
                        }
                        catch { }
                    }
                }
            }
            return pl;
        }

        /// <summary>
        /// Pin-rows per sensor.
        /// </summary>
        /// <param name="device">Informations about the used device.</param>
        /// <returns>Returns the count of pins in vertical direction for one touch-sensor, depending on the given device.</returns>
        private static double pinRowsPerSensor(DeviceTypeInformation device)
        {
            if (device == null) return 1;
            switch (device.DeviceType)
            {
                case "2H":
                    return 2.5;
                case "2":
                    return 2.5;
                default:
                    return 5;
            }
        }
        /// <summary>
        /// Pin-columns per sensor.
        /// </summary>
        /// <param name="device">Informations about the used device.</param>
        /// <returns>Returns the count of pins in horizontal direction for one touch-sensor, depending on the given device.</returns>
        private static double pinColumsPerSensor(DeviceTypeInformation device) { return 2; }
    }

    internal struct Pin
    {
        /// <summary>
        /// horizontal position of the pin
        /// </summary>
        public int X;
        /// <summary>
        /// vertical position of the pin
        /// </summary>
        public int Y;
        /// <summary>
        /// is the pin raised or lowered
        /// </summary>
        public bool Raised;
        /// <summary>
        /// touch sensor value
        /// </summary>
        public double Touch;

        public Pin(int x, int y, bool raised, double touchValue) { X = x; Y = y; Raised = raised; Touch = touchValue; }
        public Pin(int x, int y, bool raised) : this(x, y, raised, 0) { }
        public Pin(int x, int y, double touchValue) : this(x, y, false, touchValue) { }
        public Pin(int x, int y) : this(x, y, false, 0) { }
        public override string ToString() { return base.ToString() + " | [" + Y + "," + X + "] Raised:" + Raised + " TouchValue:" + Touch; }

    }
}
