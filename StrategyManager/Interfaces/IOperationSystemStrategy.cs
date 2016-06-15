using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace StrategyManager.Interfaces
{
    public interface IOperationSystemStrategy
    {
        bool deliverCursorPosition();
        IntPtr getHWND();
        IntPtr getProcessHwndFromHwnd(int processId);
        void paintMouseRect(OSMElement.OSMElement osmElement);
        IntPtr deliverDesktopHWND();
        void getCursorPoint(out int x, out int y);
    }
}
