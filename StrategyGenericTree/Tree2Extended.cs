using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRANTManager.Interfaces;
using GRANTManager;
using Common;
using System.IO;

namespace StrategyGenericTree
{
    public class Tree2Extended <T> : ITreeStrategy<T>
    {
        public Object NewTree()
        {
            return (Object)( NodeTree<T>.NewTree());
        }

        public void Add(Object treeOld, Object treeNewPart)
        {
            ((INode<T>)treeOld).Add((ITree<T>)treeNewPart);
            //((NodeTree<T>)treeOld).Add((ITree<T>)treeNewPart);
        }

        public Object Add(Object treeOld, T o)
        {
            return (((INode<T>)treeOld).Add(o));
        }

        public void AddChild(Object treeOld, Object treeChild)
        {
            Type treeOldType = treeOld.GetType();
            if (treeOldType.BaseType.Name.Equals(typeof(NodeTree<T>).Name) || treeOldType.Name.Equals(typeof(NodeTree<T>).Name))
            {
                ((NodeTree<T>)treeOld).AddChild((ITree<T>)treeChild);
            }
            else
            {
                if (treeOldType.BaseType.Name.Equals(typeof(ITree<T>).Name) || treeOldType.Name.Equals(typeof(ITree<T>).Name))
                {
                    ((ITree<T>)treeOld).AddChild((ITree<T>)treeChild);
                }
                else
                {
                    if (treeOldType.BaseType.Name.Equals(typeof(INode<T>).Name) || treeOldType.Name.Equals(typeof(INode<T>).Name))
                    {
                        ((INode<T>)treeOld).AddChild((ITree<T>)treeChild);
                    }
                }
            }
        }

        
        public Object AddChild(Object treeOld, T o)
        {
            Type treeOldType = treeOld.GetType();
            if (treeOldType.BaseType.Name.Equals(typeof(NodeTree<T>).Name) || treeOldType.Name.Equals(typeof(NodeTree<T>).Name))
            {
                return ((NodeTree<T>)treeOld).AddChild(o);
            }
            else
            {
                if (treeOldType.BaseType.Name.Equals(typeof(ITree<T>).Name) || treeOldType.Name.Equals(typeof(ITree<T>).Name))
                {
                    return ((ITree<T>)treeOld).AddChild(o);
                }
                else
                {
                    if (treeOldType.BaseType.Name.Equals(typeof(INode<T>).Name) || treeOldType.Name.Equals(typeof(INode<T>).Name))
                    {
                        return ((INode<T>)treeOld).AddChild(o);
                    }
                }
            }
            return null;
        }

        
        public int BranchCount(Object node)
        {
             return ((INode<T>)node).BranchCount; 
        }

        public int BranchIndex(Object node)
        {
             return ((INode<T>)node).BranchIndex; 
        }



        public Object Child(Object node)
        {
            return ((INode<T>)node).Child; 
        }

        public bool Contains(Object treeOld, Object item)
        {
            return ((ITree<T>)treeOld).Contains((INode<T>)item);
        }

        public bool Contains(Object treeOld, T item)
        {
            return ((ITree<T>)treeOld).Contains(item);
        }

        public Object Copy(Object treeOld)
        {
            return ((ITree<T>)treeOld).Copy();
        }

        public Object Copy(Object treeOld,T o)
        {
            return ((ITree<T>)treeOld).Copy(o);
        }

        public int Count(Object tree)
        {
            return((ITree<T>)tree).Count; 
        }

        public Object Cut(Object treeOld)
        {
            return (Object)((INode<T>)treeOld).Cut();
        }

        public Object Cut(Object treeOld, T o)
        {
            return ((ITree<T>)treeOld).Cut(o);
        }

        public T GetData(Object node)
        {
            return ((INode<T>)node).Data;
        }
        public void SetData(Object node, T o)
        {
            ((INode<T>)node).Data = o;
        }

        public IEqualityComparer<T> DataComparer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Type DataType(Object tree)
        {
            return ((ITree<T>)tree).DataType; 
        }

        public Object DeepCopy(Object treeOld)
        {
            return ((ITree<T>)treeOld).DeepCopy();
        }

        public Object DeepCopy(Object treeOld, T o)
        {
            return ((ITree<T>)treeOld).DeepCopy(o);
        }

        public int Depth(Object node)
        {
           return ((INode<T>)node).Depth;
        }

        public int DirectChildCount(Object node)
        {
            return ((INode<T>)node).DirectChildCount; 
        }

        public void Dispose(Object treeOld)
        {
           ((ITree<T>)treeOld).Dispose();
        }

        public Object First(Object node)
        {
            return ((INode<T>)node).First;
        }

        public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public bool HasChild(Object node)
        {
            return ((INode<T>)node).HasChild; 
        }

        public bool HasNext(Object node)
        {
            return ((INode<T>)node).HasNext;
        }

        public bool HasParent(Object node)
        {
            return ((INode<T>)node).HasParent; 
        }

        public bool HasPrevious(Object node)
        {
           return ((INode<T>)node).HasPrevious;
        }

        public void InsertChild(Object treeOld, Object treeChild)
        {
            ((INode<T>)treeOld).InsertChild((ITree<T>)treeChild);
        }

        public Object InsertChild(Object treeOld, T o)
        {
            return ((INode<T>)treeOld).InsertChild(o);
        }

        public void InsertNext(Object treeOld, Object treeNext)
        {
            ((INode<T>)treeOld).InsertNext((ITree<T>)treeNext);
        }

        public Object InsertNext(Object treeOld, T o)
        {
            return ((INode<T>)treeOld).InsertNext(o);
        }

        public void InsertPrevious(Object treeOld, Object tree)
        {
            ((INode<T>)treeOld).InsertPrevious((ITree<T>)tree);
        }

        public Object InsertPrevious(Object treeOld, T o)
        {
            return ((INode<T>)treeOld).InsertPrevious(o);
        }

        public bool IsFirst(Object node)
        {
            return ((INode<T>)node).IsFirst; 
        }

        public bool IsLast(Object node)
        {
            return ((INode<T>)node).IsLast; 
        }

        public bool IsRoot(Object node)
        {
            return ((INode<T>)node).IsRoot; 
        }

        public bool IsTop(Object node)
        {
            return ((INode<T>)node).IsTop; 
        }

        public bool IsTree(Object node)
        {
             return ((INode<T>)node).IsTree; 
        }

        public Object Last(Object node)
        {
           return ((INode<T>)node).Last; 
        }

        public Object LastChild(Object node)
        {
            return ((INode<T>)node).LastChild; 
        }

  /*      public void MoveToChild()
        {
            ((INode<T>)this).MoveToChild();
        }

        public void MoveToFirst()
        {
            ((INode<T>)this).MoveToFirst();
        }

        public void MoveToLast()
        {
            ((INode<T>)this).MoveToLast();
        }

        public void MoveToNext()
        {
            ((INode<T>)this).MoveToNext();
        }

        public void MoveToParent()
        {
            ((INode<T>)this).MoveToParent();
        }

        public void MoveToPrevious()
        {
            ((INode<T>)this).MoveToPrevious();
        }
        */
        public Object Next(Object node)
        {
            return ((INode<T>)node).Next; 
        }

        public Object Parent(Object node)
        {
            return ((INode<T>)node).Parent; 
        }

        public Object Previous(Object node)
        {
            return ((INode<T>)node).Previous; 
        }

        public void Remove(Object treeOld)
        {
            ((INode<T>)treeOld).Remove();
        }

        public bool Remove(Object treeOld, T o)
        {
            return ((INode<T>)treeOld).Remove(o);
        }

        public Object Root(Object node)
        {
            return ((INode<T>)node).Root; 
        }
        
        public Object Top(Object node)
        {
            return  ((INode<T>)node).Top; 
        }

        public string ToStringRecursive(Object treeOld)
        {
            Type treeOldType = treeOld.GetType();
           if (treeOldType.BaseType.Name.Equals(typeof(NodeTree<T>).Name) || treeOldType.Name.Equals(typeof(NodeTree<T>).Name))
            {                
                return ((NodeTree<T>)treeOld).ToStringRecursive();                
            }
            else
            {
                if (treeOldType.BaseType.Name.Equals(typeof(ITree<T>).Name) || treeOldType.Name.Equals(typeof(ITree<T>).Name))
                {
                    return ((ITree<T>)treeOld).ToStringRecursive();
                }
                else
                {
                    if (treeOldType.BaseType.Name.Equals(typeof(INode<T>).Name) || treeOldType.Name.Equals(typeof(INode<T>).Name))
                    {
                        return ((INode<T>)treeOld).ToStringRecursive();
                    }
                }
            }
            return null;
        }

        public Object Tree(Object node)
        {
             return ((INode<T>)node).Tree; 
        }

        public void XmlSerialize(Object treeOld, Stream stream)
        {
            ((ITree<T>)treeOld).XmlSerialize(stream);
        }

        public Object XmlDeserialize(Stream stream)
        {
            return NodeTree<T>.XmlDeserialize(stream);

        }

        public string ToString(Object treeOld)
        {
            return ((ITree<T>)treeOld).ToString();
        }


        public void Clear(Object tree)
        {
            Type treeType = tree.GetType();
            if (treeType.BaseType.Name.Equals(typeof(NodeTree<T>).Name) || treeType.Name.Equals(typeof(NodeTree<T>).Name))
            {
                ((NodeTree<T>)tree).Clear();
            }
            else
            {
                if (treeType.BaseType.Name.Equals(typeof(ITree<T>).Name) || treeType.Name.Equals(typeof(ITree<T>).Name))
                {
                    ((ITree<T>)tree).Clear();
                }
            }
        }

        public IEnumerable<object> AllNodes(object treeObject)
        {
            if (treeObject == null || !(treeObject.GetType().Equals(typeof(NodeTree<T>)) || treeObject.GetType().BaseType.Equals(typeof(NodeTree<T>)))) { return null; }
            return (IEnumerable<object>)((NodeTree<T>)treeObject).All.Nodes;
        }

        public IEnumerable<object> AllChildrenNodes(object treeObject)
        {
            if (treeObject == null || !(treeObject.GetType().Equals(typeof(NodeTree<T>)) || treeObject.GetType().BaseType.Equals(typeof(NodeTree<T>)))) { return null; }
            return (IEnumerable<object>)((NodeTree<T>)treeObject).AllChildren.Nodes;
        }

        public IEnumerable<object> DirectChildrenNodes(object treeObject)
        {
            if (treeObject == null || !(treeObject.GetType().Equals(typeof(NodeTree<T>)) || treeObject.GetType().BaseType.Equals(typeof(NodeTree<T>)))) { return null; }
            return (IEnumerable<object>)((NodeTree<T>)treeObject).DirectChildren.Nodes;
        }


        public bool Equals(object node1, object node2)
        {
            IEqualityComparer<T> comparer = ((NodeTree<T>)node1).DataComparer;
            bool result = comparer.Equals(((NodeTree<T>)node1).Data, ((NodeTree<T>)node2).Data);
            return result;
        }
    }
}
