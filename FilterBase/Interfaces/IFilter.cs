using System;
using Tree;

namespace FilterBase.Interfaces
{
    public interface IFilter
    {
        ITree<GeneralProperties> filtering(IntPtr hwnd);
    }
}
