using System;
using StrategyManager;
using OSMElement;

namespace StrategyManager.Interfaces
{
    public interface IFilterStrategy
    {
        ITreeStrategy<OSMElement.OSMElement> filtering(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);
        ITreeStrategy<OSMElement.OSMElement> getParentsOfElement(ITreeStrategy<OSMElement.OSMElement> node, IntPtr hwnd);

        void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem);
        IOperationSystemStrategy getSpecifiedOperationSystem();
        void setSpecifiedTree(ITreeStrategy<OSMElement.OSMElement> tree);
        ITreeStrategy<OSMElement.OSMElement> getSpecifiedTree();

    }
}
