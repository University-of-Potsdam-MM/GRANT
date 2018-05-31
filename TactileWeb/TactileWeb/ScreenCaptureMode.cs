using System;



namespace TactileWeb
{

    /// <summary>How to capture and show</summary>
    public enum ScreenCaptureMode
    {

        /// <summary>0 = Pixelcopy with threshold</summary>
        Threshold       = 0,

        /// <summary>1 = Pixelcopy with edge detection</summary>
        EdgeDetection   = 1,

        /// <summary>2 = Show UIA elements with borders and braille text inside</summary>
        UIA             = 2,

    }

}