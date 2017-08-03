using System;
using System.IO;
using System.Collections.Generic;
using OSMElement;

namespace GRANTManager.Interfaces
{
    /// <summary>
    /// interface for the tree operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeStrategy<T>
    {
        /// <summary>
        /// Creat a new tree structure
        /// </summary>
        /// <returns>a object with a new (empty) tree</returns>
        object NewTree();

        /// <summary>
        /// Adds a subtree (or node) to a given tree
        /// </summary>
        /// <param name="treeOld">the 'old' tree to add a part</param>
        /// <param name="treeNewPart">the subtree (or node) to add</param>
        void Add(object treeOld, object treeNewPart); //ITree

        /// <summary>
        /// Adds node object to a tree
        /// </summary>
        /// <param name="treeOld">the 'old' tree to add a part</param>
        /// <param name="nodeOsm"><c>OSMElement.OSMElement</c> with the properties of the new node</param>
        /// <returns>tree object where the <para>nodeOsm</para> is added</returns>
        object Add(object treeOld, T nodeOsm); //INode

        /// <summary>
        /// Adds a subtree in a given tree
        /// </summary>
        /// <param name="treeOld">>the 'old' tree to add a part; the pointer shows the position to add the subtree</param>
        /// <param name="treeChild">the child subtree to add</param>
        void AddChild(object treeOld, object treeChild); //ITree

        /// <summary>
        ///  Adds a subtree in a given tree
        /// </summary>
        /// <param name="treeOld">>the 'old' tree to add a part; the pointer shows the position to add the subtree</param>
        /// <param name="nodeOsm"><c>OSMElement.OSMElement</c> with the properties of the new node</param>
        /// <returns>tree object where the <para>nodeOsm</para> is added</returns>
        object AddChild(object treeOld, T nodeOsm); //INode

        IEnumerable<object> AllNodes(object treeObject); //Common.IEnumerableCollectionPair<T> All { get; }
        System.Collections.Generic.IEnumerable<Object> AllChildrenNodes(object treeObject);//  Common.IEnumerableCollectionPair<T> AllChildren { get; }

        /// <summary>
        /// Gives the count of branches of a node
        /// </summary>
        /// <param name="node"> a node object</param>
        /// <returns>count of branches of the node</returns>
        int BranchCount(object node);

        /// <summary>
        /// gives the index of a branch
        /// </summary>
        /// <param name="node">a node object</param>
        /// <returns>index of branch of the node</returns>
        int BranchIndex(object node);

        /// <summary>
        /// Gives the child of a node
        /// </summary>
        /// <param name="node">a node object</param>
        /// <returns>object with the child of the given node</returns>
        object Child(object node); //INode
         void Clear(object tree);


        /// <summary>
        /// Determines whether an element is in the tree object
        /// </summary>
        /// <param name="treeOld">the tree object which maybe contains the item</param>
        /// <param name="item">The object to locate in the given <para>treeold</para></param>
        /// <returns><c>true</c> if item is found; otherwise, <c>false</c></returns>
        bool Contains(object treeOld, object item); //INOde

        /// <summary>
        /// Determines whether an element is in the tree object
        /// </summary>
        /// <param name="treeOld">the tree object which maybe contains the item</param>
        /// <param name="nodeOsm">a <c>OSMElement.OSMElement<c></param>
        /// <returns><c>true</c> if item is found; otherwise, <c>false</c></returns>
        bool Contains(object treeOld, T nodeOsm);


        /// <summary>
        /// Copied a tree object
        /// </summary>
        /// <param name="treeOld">tree to copy</param>
        /// <returns>a copy of the given tree</returns>
        object Copy(object treeOld); //ITree
        object Copy(object treeOld, T o); //ITree

        /// <summary>
        /// Counts the nodes the tree
        /// </summary>
        /// <param name="tree">a tree object</param>
        /// <returns>number of nodes</returns>
        int Count(object tree);
        object Cut(object treeOld); //ITree
        object Cut(object treeOld, T o); //ITree
        T GetData(object node);
        void SetData(object node, T o);
        System.Collections.Generic.IEqualityComparer<T> DataComparer { get; set; }
        Type DataType(object tree);
      //  object DeepCopy(object treeOld); //ITree
       // object DeepCopy(object treeOld, T o); //ITree
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

        bool moveSubtree(Object nodeToMove, Object parentNew);

        /// <summary>
        /// Replace and "old" subtree withe a new one
        /// </summary>
        /// <param name="subtreeOld">the old subtree</param>
        /// <param name="subtreeNew">the new subtree</param>
        void ReplaceSubtree(ref object subtreeOld, object subtreeNew);
    }
}
