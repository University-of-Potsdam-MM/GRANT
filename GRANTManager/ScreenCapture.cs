using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

//////////////////////////////////////////////////////////////////////////
// http://www.developerfusion.com/code/4630/capture-a-screen-shot/
// Capture a Screen Shot
// By James Crowley, published on 13 Apr 2004 
//////////////////////////////////////////////////////////////////////////

namespace GRANTManager
{
    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public static class ScreenCapture
    {
        /// <summary>
        /// Saves an image to a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="img">The img to save.</param>
        /// <param name="format">The file format.</param>
        public static void SaveImageToFile(String filename, Image img, ImageFormat format = null)
        {
            if (img != null && !String.IsNullOrWhiteSpace(filename))
            {
                img.Save(filename, format != null ? format : ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Captures the screen at a specific position.
        /// </summary>
        /// <param name="capturingBounds">The capturing bounds.</param>
        /// <returns>the resulting image or NULL</returns>
        public static Image CaptureScreenPos(System.Drawing.Rectangle capturingBounds)
        {
            Bitmap image = null;

            if ((capturingBounds.Width > 0) && (capturingBounds.Height > 0))
            {
                image = new Bitmap(capturingBounds.Width, capturingBounds.Height, PixelFormat.Format32bppArgb);
                Graphics gr = Graphics.FromImage(image);
                gr.CopyFromScreen(capturingBounds.X, capturingBounds.Y, 0, 0, new System.Drawing.Size(capturingBounds.Width, capturingBounds.Height), CopyPixelOperation.SourceCopy);
                gr.Dispose();
                return image as Image;
            }
            return null;
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public static Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static Image CaptureWindowPartAtScreenpos(IntPtr handle, int height, int width, int nXSrc, int nYSrc)
        {
            // get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);

            int top = nYSrc - windowRect.top;
            int left = nXSrc - windowRect.left;

            int wWidth = windowRect.right - windowRect.left;
            int wHeight = windowRect.bottom - windowRect.top;

            return CaptureWindow(handle, height, width, left, top);
        }
        /// <summary>
        /// Captures the only the client button of a window.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        /// <returns>the image of the client button.</returns>
        public static Image CaptureWindowClientArea(IntPtr handle)
        {
            // get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);


            User32.RECT windowClRect = new User32.RECT();
            User32.GetClientRect(handle, ref windowClRect);

            int top = 0;
            int left = 0;

            System.Drawing.Point cPoint = new System.Drawing.Point();
            User32.ClientToScreen(handle, ref cPoint);
            top = cPoint.Y - windowRect.top;
            left = cPoint.X - windowRect.left;

            int wWidth = windowRect.right - windowRect.left;
            int wHeight = windowRect.bottom - windowRect.top;

            return CaptureWindow(handle, windowClRect.bottom, windowClRect.right, left, top);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static void CaptureWindowPartAtScreenpoToFile(IntPtr handle, string filename, int height, int width, int nXSrc, int nYSrc)
        {
            Image img = CaptureWindowPartAtScreenpos(handle, height, width, nXSrc, nYSrc);
            img.Save(filename, ImageFormat.Bmp);
        }

        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public static Image CaptureWindow(IntPtr handle)
        {
            // get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            return CaptureWindow(handle, height, width);
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="nXDest">The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="nYDest">The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="nXSrc">The x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <param name="nYSrc">The y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <returns></returns>
        public static Image CaptureWindow(IntPtr handle, int height, int width, int nXSrc = 0, int nYSrc = 0, int nXDest = 0, int nYDest = 0)
        {
            // get the hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, nXDest, nYDest, width, height, hdcSrc, nXSrc, nYSrc, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = null;
            try
            {
                if (hBitmap != null && hBitmap != IntPtr.Zero)
                {
                    img = Image.FromHbitmap(hBitmap);
                }
                else { }
            }
            catch { }
            finally
            {
                // free up the Bitmap object
                GDI32.DeleteObject(hBitmap);
            }
            return img;
        }

        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        public static void CaptureWindowToFile(IntPtr handle, string filename)
        {
            CaptureWindowToFile(handle, filename, ImageFormat.Bmp);
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle">The window handle</param>
        /// <param name="filename">the file name to save</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="nXSrc">The start X in the sorce.</param>
        /// <param name="nYSrc">The start Y in the source.</param>
        /// <param name="nXDest">The start X in the destination.</param>
        /// <param name="nYDest">The start Y in the destination.</param>
        public static void CaptureWindowToFile(IntPtr handle, string filename, int height, int width, int nXSrc = 0, int nYSrc = 0, int nXDest = 0, int nYDest = 0)
        {
            using (Image img = CaptureWindow(handle, height, width, nXSrc, nXSrc, nXDest, nYDest))
            {
                img.Save(filename, ImageFormat.Bmp);
            }
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle">The window handle</param>
        /// <param name="filename">the file name to save</param>
        /// <param name="format">the image format</param>
        public static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            using (Image img = CaptureWindow(handle))
            {
                img.Save(filename, format);
            }
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle">The window handle</param>
        /// <param name="filename">the file name to save</param>
        /// <param name="format">The target file format.</param>
        /// <param name="height">The height.</param>
        /// <param name="width">The width.</param>
        /// <param name="nXSrc">The start X in the sorce.</param>
        /// <param name="nYSrc">The start Y in the source.</param>
        /// <param name="nXDest">The start X in the destination.</param>
        /// <param name="nYDest">The start Y in the destination.</param>
        public static void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format, int height, int width, int nXSrc = 0, int nYSrc = 0, int nXDest = 0, int nYDest = 0)
        {
            using (Image img = CaptureWindow(handle, height, width, nXSrc, nXSrc, nXDest, nYDest))
            {
                img.Save(filename, format);
            }
        }

        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename">the file name to save</param>
        public static void CaptureScreenToFile(string filename)
        {
            CaptureScreenToFile(filename, ImageFormat.Bmp);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename">the file name to save</param>
        /// <param name="format">The target file format.</param>
        public static void CaptureScreenToFile(string filename, ImageFormat format)
        {
            using (Image img = CaptureScreen())
            {
                img.Save(filename, format);
            }
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private static class GDI32
        {
            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        public static class User32
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindow(IntPtr hWnd);

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern IntPtr GetClientRect(IntPtr hWnd, ref RECT rect);
            [DllImport("user32.dll")]
            public static extern bool ClientToScreen(IntPtr hWnd, ref System.Drawing.Point lpPoint);
        }
    }
}