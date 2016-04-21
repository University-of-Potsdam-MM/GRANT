using System;
using StrategyManager;

namespace StrategyManager.Interfaces
{
    public interface IFilterStrategy
    {
        ITreeStrategy<GeneralProperties> filtering(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);
        ITreeStrategy<GeneralProperties> getParentsOfElement(ITreeStrategy<GeneralProperties> node, IntPtr hwnd);
        void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem);
        IOperationSystemStrategy getSpecifiedOperationSystem();
        void setSpecifiedTree(ITreeStrategy<GeneralProperties> tree);
        ITreeStrategy<GeneralProperties> getSpecifiedTree();

    }
}
