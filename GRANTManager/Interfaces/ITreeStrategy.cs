using System;
using System.IO;
using System.Collections.Generic;
using OSMElement;

namespace GRANTManager.Interfaces
{
    public interface ITreeStrategy<T>
    {
        object NewTree();

        void Add(object treeOld, object treeNewPart); //ITree
        object Add(object NodeOld, T o); //INode
        void AddChild(object treeOld, object treeChild); //ITree
        object AddChild(object treeOld, T o); //INode
        IEnumerable<object> AllNodes(object treeObject); //Common.IEnumerableCollectionPair<T> All { get; }
        System.Collections.Generic.IEnumerable<Object> AllChildrenNodes(object treeObject);//  Common.IEnumerableCollectionPair<T> AllChildren { get; }
        int BranchCount(object node);
        int BranchIndex(object node);
        /*bool CanMoveToChild { get; }
        bool CanMoveToFirst { get; }
        bool CanMoveToLast { get; }
        bool CanMoveToNext { get; }
        bool CanMoveToParent { get; }
        bool CanMoveToPrevious { get; }*/
        object Child(object node); //INode
         void Clear(object tree);
        bool Contains(object treeOld, object item); //INOde
        bool Contains(object treeOld, T item);
        object Copy(object treeOld); //ITree
        object Copy(object treeOld, T o); //ITree
        int Count(object tree);
        object Cut(object treeOld); //ITree
        object Cut(object treeOld, T o); //ITree
        T GetData(object node);
        void SetData(object node, T o);
        System.Collections.Generic.IEqualityComparer<T> DataComparer { get; set; }
        Type DataType(object tree);
        object DeepCopy(object treeOld); //ITree
        object DeepCopy(object treeOld, T o); //ITree
        int Depth(object node);
        int DirectChildCount(object node);
        System.Collections.Generic.IEnumerable<Object> DirectChildrenNodes(object treeObject);// Common.IEnumerableCollectionPair<T> DirectChildren { get; }
        // Common.IEnumerableCollectionPair<T> DirectChildrenInReverse { get; }
        void Dispose(object treeOld);
        object First(object node); //INode
        void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
        bool HasChild(object node);
        bool HasNext(object node);
        bool HasParent(object node);
        bool HasPrevious(object node);
        void InsertChild(object treeOld, object treeChild); //ITree
        object InsertChild(object treeOld, T o); //INode
        void InsertNext(object treeOld, object treeNext); //ITree
        object InsertNext(object treeOld, T o); //INOde
        void InsertPrevious(object treeOld, object tree); //ITree
        object InsertPrevious(object treeOld, T o); //Inode
        bool IsFirst(object node);
        bool IsLast(object node);
        bool IsRoot(object node);
        bool IsTop(object node);
        bool IsTree(object node);
        object Last(object node); //INode
        object LastChild(object node); //Inode
        /*     void MoveToChild(Object treeOld);
             void MoveToFirst(Object treeOld);
             void MoveToLast(Object treeOld);
             void MoveToNext(Object treeOld);
             void MoveToParent(Object treeOld);
             void MoveToPrevious(Object treeOld);*/
        object Next(object node); //INode
        //  Common.IEnumerableCollection<Common.INode<T>> Nodes { get; }
        object Parent(object node); //INode
        object Previous(object node); //INode
        void Remove(object treeOld);
        bool Remove(object treeOld, T o);
        object Root(object node); //INode
        //   Object this[T item] { get; } //INode
        object Top(object node); //INOde
        string ToString(object treeOld);
        string ToStringRecursive(object treeOld);
        object Tree(object node); //ITree
        //Common.IEnumerableCollection<T> Values { get; }
        void XmlSerialize(object treeOld, Stream stream);
        object XmlDeserialize(Stream stream);
        bool Equals(object node1, object node2);
    }
}
