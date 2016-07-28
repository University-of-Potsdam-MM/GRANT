using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace GRANTManager.Interfaces
{
    public interface IOperationSystemStrategy
    {
        bool deliverCursorPosition();
        IntPtr getHWND();
        IntPtr getProcessHwndFromHwnd(int processId);
        //void paintRect(Rectangle rect, Graphics desktop);
        void paintRect(Rectangle rect);
        IntPtr deliverDesktopHWND();
        void getCursorPoint(out int x, out int y);
        Rectangle getRect(OSMElement.OSMElement osmElement);
    }
}
