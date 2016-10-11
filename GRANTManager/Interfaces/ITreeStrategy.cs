using System;
using System.IO;
using System.Collections.Generic;
using OSMElement;

namespace GRANTManager.Interfaces
{
    public interface ITreeStrategy<T>
    {
        Object NewTree();

        void Add(Object treeOld, Object treeNewPart); //ITree
        Object Add(Object NodeOld, T o); //INode
        void AddChild(Object treeOld, Object treeChild); //ITree
        Object AddChild(Object treeOld, T o); //INode
        IEnumerable<Object> AllNodes(Object treeObject); //Common.IEnumerableCollectionPair<T> All { get; }
        System.Collections.Generic.IEnumerable<Object> AllChildrenNodes(Object treeObject);//  Common.IEnumerableCollectionPair<T> AllChildren { get; }
        int BranchCount(Object node);
        int BranchIndex(Object node);
        /*bool CanMoveToChild { get; }
        bool CanMoveToFirst { get; }
        bool CanMoveToLast { get; }
        bool CanMoveToNext { get; }
        bool CanMoveToParent { get; }
        bool CanMoveToPrevious { get; }*/
        Object Child(Object node); //INode
         void Clear(Object tree);
        bool Contains(Object treeOld, Object item); //INOde
        bool Contains(Object treeOld, T item);
        Object Copy(Object treeOld); //ITree
        Object Copy(Object treeOld, T o); //ITree
        int Count(Object tree);
        Object Cut(Object treeOld); //ITree
        Object Cut(Object treeOld, T o); //ITree
        T GetData(Object node);
        void SetData(Object node, T o);
        System.Collections.Generic.IEqualityComparer<T> DataComparer { get; set; }
        Type DataType(Object tree);
        Object DeepCopy(Object treeOld); //ITree
        Object DeepCopy(Object treeOld, T o); //ITree
        int Depth(Object node);
        int DirectChildCount(Object node);
        System.Collections.Generic.IEnumerable<Object> DirectChildrenNodes(Object treeObject);// Common.IEnumerableCollectionPair<T> DirectChildren { get; }
        // Common.IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }
        void Dispose(Object treeOld);
        Object First(Object node); //INode
        void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
        bool HasChild(Object node);
        bool HasNext(Object node);
        bool HasParent(Object node);
        bool HasPrevious(Object node);
        void InsertChild(Object treeOld, Object treeChild); //ITree
        Object InsertChild(Object treeOld, T o); //INode
        void InsertNext(Object treeOld, Object treeNext); //ITree
        Object InsertNext(Object treeOld, T o); //INOde
        void InsertPrevious(Object treeOld, Object tree); //ITree
        Object InsertPrevious(Object treeOld, T o); //Inode
        bool IsFirst(Object node);
        bool IsLast(Object node);
        bool IsRoot(Object node);
        bool IsTop(Object node);
        bool IsTree(Object node);
        Object Last(Object node); //INode
        Object LastChild(Object node); //Inode
        /*     void MoveToChild(Object treeOld);
             void MoveToFirst(Object treeOld);
             void MoveToLast(Object treeOld);
             void MoveToNext(Object treeOld);
             void MoveToParent(Object treeOld);
             void MoveToPrevious(Object treeOld);*/
        Object Next(Object node); //INode
        //  Common.IEnumerableCollection<Common.INode<T>> Nodes { get; }
        Object Parent(Object node); //INode
        Object Previous(Object node); //INode
        void Remove(Object treeOld);
        bool Remove(Object treeOld, T o);
        Object Root(Object node); //INode
        //   Object this[T item] { get; } //INode
        Object Top(Object node); //INOde
        string ToString(Object treeOld);
        string ToStringRecursive(Object treeOld);
        Object Tree(Object node); //ITree
        //Common.IEnumerableCollection<T> Values { get; }
        void XmlSerialize(Object treeOld, Stream stream);
        Object XmlDeserialize(Stream stream);

    }
}
