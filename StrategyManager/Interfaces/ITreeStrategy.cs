using System;
using System.IO;
using System.Collections.Generic;
using OSMElement;

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
        ITreeStrategy<T> XmlDeserialize(Stream stream);

        #region eigene Methoden
        /// <summary>
        /// Gibt die (einige) <code>GeneralProperties</code> des angegebenen Baumes aus
        /// </summary>
        /// <param name="tree">Gibt den auszugebenen Baum an</param>
        /// <param name="depth">Gibt die Tiefe der Ausgabe an; Wenn der gesamte Baum ausgegeben werden soll, so muss <value>-1</value> angegeben werden.</param>
        void printTreeElements(ITreeStrategy<T> tree, int depth);
        
        /// <summary>
        /// Sucht anhand der angegebenen <code>GeneralProperties</code> alle Knoten die diesen Eigenschaften entsprechen
        /// </summary>
        /// <param name="tree">Gibt den Baum an, in welchem gesucht werden soll</param>
        /// <param name="properties">gibt die zusuchenden Eigenschaften an</param>
        /// <param name="oper">gibt an wie die Eigenschaften verknüpft werden sollen</param>
        /// <returns>Eine Liste mit allen Knoten auf den die Eigenschaften zutreffen</returns>
        List<ITreeStrategy<T>> searchProperties(ITreeStrategy<T> tree, GeneralProperties properties, OperatorEnum oper); //TODO: properties sollten generisch sein
        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum alle zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder die zugehörigen Knoten ermittelt werden sollen</param>
        /// <param name="tree">gibt den Baum an, in welchem die zugehörigen Knoten ermittelt werden sollen </param>
        /// <returns>Gibt eine Liste mit den Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
        List<ITreeStrategy<T>> getAssociatedNodeList(String generatedId, ITreeStrategy<T> tree);
        /// <summary>
        /// Gibt zu der angegebenen generierten Id aus dem angegeben Baum einen zugehörigen Knoten an
        /// </summary>
        /// <param name="generatedId">Gibt die Id an zuder ein zugehöriger Knoten ermittelt werden soll</param>
        /// <param name="tree">gibt den Baum an, in welchem ein zugehöriger Knoten ermittelt werden soll</param>
        /// <returns>Gibt einen Knoten, bei denen die generierte Id übereinstimmt zurück</returns>
        ITreeStrategy<T> getAssociatedNode(String id, ITreeStrategy<T> tree);
        /// <summary>
        /// Ändert die Eigenschaften eines Knotens des gefilterten Baumes.
        /// </summary>
        /// <param name="properties">Gibt die neuen Eigenschaften an.</param>
        void changePropertiesOfFilteredNode(GeneralProperties properties);
        StrategyMgr getStrategyMgr();
        void setStrategyMgr(StrategyMgr strategyMgr);

        /// <summary>
        /// Ändert von einem angegeben Knoten im Braille-Baum die Eigenschaften 
        /// </summary>
        /// <param name="element">Gibt die neuen Eigenschften an</param>
        void changeBrailleRepresentation(OSMElement.OSMElement element);
        #endregion

    }
}
