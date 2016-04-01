using System;
using Tree;

namespace Basics.Interfaces
{
    public interface IFilterStrategy
    {
        ITree<GeneralProperties> filtering(IntPtr hwnd);
    }
}
