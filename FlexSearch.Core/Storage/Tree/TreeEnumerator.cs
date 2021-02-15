using System;
using System.Collections;
using System.Collections.Generic;
using Core.Storage.BinaryStorage;

namespace Core.Storage.Tree
{
    public class TreeEnumerator<TK, TV> : IEnumerator<Tuple<TK, TV>>
    {
        private readonly ITreeNodeManager<TK, TV> _nodeManager;
        private readonly TreeTraverseDirection _direction;

        private bool _doneIterating;

        public TreeNode<TK, TV> CurrentNode { get; private set; }

        public int CurrentEntry { get; private set; }

        public Tuple<TK, TV> Current { get; private set; }

        object IEnumerator.Current => Current; 

        
        public TreeEnumerator (ITreeNodeManager<TK, TV> nodeManager, TreeNode<TK, TV> node, int fromIndex, TreeTraverseDirection direction)
        {
            _nodeManager = nodeManager;
            CurrentNode = node;
            CurrentEntry = fromIndex;
            _direction = direction;
        }
        public bool MoveNext()
        {
            if (_doneIterating) 
            {
                return false;
            }

            return _direction switch
            {
                TreeTraverseDirection.Ascending => MoveForward(),
                TreeTraverseDirection.Descending => MoveBackward(),
                _ => throw new ArgumentException(nameof(_direction))
            };
        }

        private bool MoveForward()
        {
            // Leaf node, either move right or up
            if(CurrentNode.IsLeaf)
            {
                // First, move right
                CurrentEntry++;

                while(true)
                {
                    // If currentEntry is valid
                    // then we are done here.
                    if (CurrentEntry < CurrentNode.EntriesCount) 
                    {
                        Current = CurrentNode.GetEntry(CurrentEntry);
                        return true;
                    }
                    // If can't move right then move up

                    if (CurrentNode.ParentId != 0)
                    {
                        CurrentEntry = CurrentNode.IndexInParent ();
                        CurrentNode = _nodeManager.Find (CurrentNode.ParentId);

                        // Validate move up result
                        if (CurrentEntry < 0 || CurrentNode == null) 
                        {
                            throw new Exception("Something gone wrong with the BTree");
                        }
                    }
                    // If can't move up when we are done iterating
                    else 
                    {
                        Current = null;
                        _doneIterating = true;
                        return false;
                    }
                }
            }
            // Parent node, always move right down

            CurrentEntry++; // Increase currentEntry, this make firstCall to nodeManager.Find 
            // to return the right node, but does not affect subsequence calls

            do 
            {
                CurrentNode = CurrentNode.GetChildNode(CurrentEntry);
                CurrentEntry = 0;
            } while (false == CurrentNode.IsLeaf);

            Current = CurrentNode.GetEntry (CurrentEntry);
            return true;
        }

        private bool MoveBackward ()
        {
            // Leaf node, either move right or up
            if (CurrentNode.IsLeaf)
            {
                // First, move left
                CurrentEntry--;

                while (true)
                {
                    // If currentEntry is valid
                    // then we are done here.
                    if (CurrentEntry >= 0) 
                    {
                        Current = CurrentNode.GetEntry (CurrentEntry);
                        return true;
                    }
                    // If can't move left then move up

                    if (CurrentNode.ParentId != 0)
                    {
                        CurrentEntry = CurrentNode.IndexInParent() -1;
                        CurrentNode = _nodeManager.Find(CurrentNode.ParentId);

                        // Validate move result
                        if (CurrentNode == null) 
                        {
                            throw new Exception("Something gone wrong with the BTree");
                        }
                    }
                    // If can't move up when we are done here
                    else 
                    {
                        _doneIterating = true;
                        Current = null;
                        return false;
                    }
                }
            }
            // Parent node, always move left down

            do 
            {
                CurrentNode = CurrentNode.GetChildNode(CurrentEntry);
                CurrentEntry = CurrentNode.EntriesCount;

                // Validate move result
                if (CurrentEntry < 0 || CurrentNode == null) 
                {
                    throw new Exception("Something gone wrong with the BTree");
                }
            } while (false == CurrentNode.IsLeaf);

            CurrentEntry -= 1;
            Current = CurrentNode.GetEntry(CurrentEntry);
            return true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose() { }
    }
}