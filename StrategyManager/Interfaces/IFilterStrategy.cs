using System;
using StrategyGenericTree;

namespace StrategyManager.Interfaces
{
    public interface IFilterStrategy
    {
        ITree<GeneralProperties> filtering(IntPtr hwnd);
        int deliverElementID(IntPtr hwnd);
        ITree<GeneralProperties> getParentsOfElement(INode<GeneralProperties> node, IntPtr hwnd, IOperationSystemStrategy operationSystemStrategy); //Achtung: evtl. an einer anderen Stelle das gewählte Betriebssystem übergeben
        void setSpecifiedOperationSystem(IOperationSystemStrategy operationSystem);
        IOperationSystemStrategy getSpecifiedOperationSystem();
        //void setSpecifiedTree(ITreeStrategy<GeneralProperties> tree);
        //ITreeStrategy<GeneralProperties> getSpecifiedTree();

    }
}
