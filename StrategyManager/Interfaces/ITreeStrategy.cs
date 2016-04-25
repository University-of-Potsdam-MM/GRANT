using System;
using System.Collections.Generic;

namespace StrategyManager.Interfaces
{
    public interface ITreeStrategy<T>
    {
        ITreeStrategy<T> NewNodeTree();
        void Add(ITreeStrategy<T> tree);
        ITreeStrategy<T> Add(T o);
        void AddChild(ITreeStrategy<T> tree);
        ITreeStrategy<T> AddChild(T o);
    //    IEnumerableCollectionPair<T> All { get; }
    //    IEnumerableCollectionPair<T> AllChildren { get; }
        int BranchCount { get; }
        int BranchIndex { get; }
        bool CanMoveToChild { get; }
        bool CanMoveToFirst { get; }
        bool CanMoveToLast { get; }
        bool CanMoveToNext { get; }
        bool CanMoveToParent { get; }
        bool CanMoveToPrevious { get; }
        ITreeStrategy<T> Child { get; }
        void Clear();
        event EventHandler Cleared;
        event EventHandler Clearing;
        bool Contains(ITreeStrategy<T> item);
        bool Contains(T item);
    //    event EventHandler<NodeTreeNodeEventArgs<T>> Copied;
        ITreeStrategy<T> Copy();
        ITreeStrategy<T> Copy(T o);
    //    event EventHandler<NodeTreeNodeEventArgs<T>> Copying;
        int Count { get; }
        ITreeStrategy<T> Cut();
        ITreeStrategy<T> Cut(T o);
        event EventHandler CutDone;
        event EventHandler Cutting;
        T Data { get; set; }
        System.Collections.Generic.IEqualityComparer<T> DataComparer { get; set; }
        Type DataType { get; }
    //    event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopied;
        ITreeStrategy<T> DeepCopy();
        ITreeStrategy<T> DeepCopy(T o);
       // event EventHandler<NodeTreeNodeEventArgs<T>> DeepCopying;
        int Depth { get; }
        int DirectChildCount { get; }
    //    IEnumerableCollectionPair<T> DirectChildren { get; }
    //    IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }
        void Dispose();
        ITreeStrategy<T> First { get; }
        void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
        bool HasChild { get; }
        bool HasNext { get; }
        bool HasParent { get; }
        bool HasPrevious { get; }
        void InsertChild(ITreeStrategy<T> tree);
        ITreeStrategy<T> InsertChild(T o);
   //     event EventHandler<NodeTreeInsertEventArgs<T>> Inserted;
   //     event EventHandler<NodeTreeInsertEventArgs<T>> Inserting;
        void InsertNext(ITreeStrategy<T> tree);
        ITreeStrategy<T> InsertNext(T o);
        void InsertPrevious(ITreeStrategy<T> tree);
        ITreeStrategy<T> InsertPrevious(T o);
        bool IsFirst { get; }
        bool IsLast { get; }
        bool IsReadOnly { get; }
        bool IsRoot { get; }
        bool IsTop { get; }
        bool IsTree { get; }
        ITreeStrategy<T> Last { get; }
        ITreeStrategy<T> LastChild { get; }
        void MoveToChild();
        void MoveToFirst();
        void MoveToLast();
        void MoveToNext();
        void MoveToParent();
        void MoveToPrevious();
        ITreeStrategy<T> Next { get; }
   //     IEnumerableCollection<INodeTree<T>> Nodes { get; }
        ITreeStrategy<T> Parent { get; }
        ITreeStrategy<T> Previous { get; }
        void Remove();
        bool Remove(T o);
        ITreeStrategy<T> Root { get; }
    //    event EventHandler<NodeTreeDataEventArgs<T>> SetDone;
    //    event EventHandler<NodeTreeDataEventArgs<T>> Setting;
        ITreeStrategy<T> this[T item] { get; }
        ITreeStrategy<T> Top { get; }
        string ToString();
        string ToStringRecursive();
        ITreeStrategy<T> Tree { get; }
   //     event EventHandler<NodeTreeDataEventArgs<T>> Validate;
   //     IEnumerableCollection<T> Values { get; }
        void XmlSerialize(System.IO.Stream stream);

        #region eigene Methoden
        void printTreeElements(ITreeStrategy<GeneralProperties> tree, int depth);
        List<ITreeStrategy<GeneralProperties>> searchProperties(ITreeStrategy<GeneralProperties> tree, GeneralProperties properties, OperatorEnum oper);
        #endregion

    }
}
