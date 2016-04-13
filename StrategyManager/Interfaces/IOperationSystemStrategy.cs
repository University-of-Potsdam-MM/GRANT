using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyManager.Interfaces
{
    public interface IOperationSystemStrategy
    {
        bool deliverCursorPosition();
        IntPtr getHWND();
        IntPtr getProcessHwndFromHwnd(int processId);
    }
}
