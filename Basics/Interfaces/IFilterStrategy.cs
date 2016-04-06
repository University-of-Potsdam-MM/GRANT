using System;
using Tree;

namespace Basics.Interfaces
{
    public interface IFilterStrategy
    {
        ITree<GeneralProperties> filtering(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);
        ITree<GeneralProperties> getParentsOfElement(INode<GeneralProperties> node, IntPtr hwnd);
    }
}
