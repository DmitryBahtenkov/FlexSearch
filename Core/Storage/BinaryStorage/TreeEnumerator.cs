using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Storage.BinaryStorage
{
    public class TreeEnumerator<K, V> : IEnumerator<Tuple<K, V>>
    {
        private readonly ITreeNodeManager<K, V> _nodeManager;
        private readonly TreeTraverseDirection _direction;

        private bool _doneIterating = false;
        private int _currentEntry = 0;
        
        private TreeNode<K, V> _currentNode;
        private Tuple<K, V> current;
        
        public TreeNode<K, V> CurrentNode => _currentNode;
        public int CurrentEntry => _currentEntry;
        public Tuple<K, V> Current => current;
        object IEnumerator.Current => Current; 

        
        public TreeEnumerator (ITreeNodeManager<K, V> nodeManager, TreeNode<K, V> node, int fromIndex, TreeTraverseDirection direction)
        {
            _nodeManager = nodeManager;
            _currentNode = node;
            _currentEntry = fromIndex;
            _direction = direction;
        }
        public bool MoveNext ()
        {
            if (_doneIterating) 
            {
                return false;
            }

            switch (_direction) 
            {
                case TreeTraverseDirection.Ascending:
                    return MoveForward();
                case TreeTraverseDirection.Descending:
                    return MoveBackward();
                default:
                    throw new ArgumentException(nameof(_direction));
            }
        }
        
        bool MoveForward ()
        {
            // Leaf node, either move right or up
            if (_currentNode.IsLeaf)
            {
                // First, move right
                _currentEntry++;

                while (true)
                {
                    // If currentEntry is valid
                    // then we are done here.
                    if (_currentEntry < _currentNode.EntriesCount) {
                        current = _currentNode.GetEntry (_currentEntry);
                        return true;
                    }
                    // If can't move right then move up
                    else if (_currentNode.ParentId != 0){
                        _currentEntry = _currentNode.IndexInParent ();
                        _currentNode = _nodeManager.Find (_currentNode.ParentId);

                        // Validate move up result
                        if ((_currentEntry < 0) || (_currentNode == null)) {
                            throw new Exception ("Something gone wrong with the BTree");
                        }
                    }
                    // If can't move up when we are done iterating
                    else {
                        current = null;
                        _doneIterating = true;
                        return false;
                    }
                }
            }
            // Parent node, always move right down
            else 
            {
                _currentEntry++; // Increase currentEntry, this make firstCall to nodeManager.Find 
                // to return the right node, but does not affect subsequence calls

                do 
                {
                    _currentNode = _currentNode.GetChildNode(_currentEntry);
                    _currentEntry = 0;
                } while (false == _currentNode.IsLeaf);

                current = _currentNode.GetEntry (_currentEntry);
                return true;
            }
        }
        bool MoveBackward ()
        {
            // Leaf node, either move right or up
            if (_currentNode.IsLeaf)
            {
                // First, move left
                _currentEntry--;

                while (true)
                {
                    // If currentEntry is valid
                    // then we are done here.
                    if (_currentEntry >= 0) {
                        current = _currentNode.GetEntry (_currentEntry);
                        return true;
                    }
                    // If can't move left then move up
                    else if (_currentNode.ParentId != 0){
                        _currentEntry = _currentNode.IndexInParent () -1;
                        _currentNode = _nodeManager.Find (_currentNode.ParentId);

                        // Validate move result
                        if (_currentNode == null) {
                            throw new Exception ("Something gone wrong with the BTree");
                        }
                    }
                    // If can't move up when we are done here
                    else {
                        _doneIterating = true;
                        current = null;
                        return false;
                    }
                }
            }
            // Parent node, always move left down
            else {
                do {
                    _currentNode = _currentNode.GetChildNode(_currentEntry);
                    _currentEntry = _currentNode.EntriesCount;

                    // Validate move result
                    if ((_currentEntry < 0) || (_currentNode == null)) {
                        throw new Exception ("Something gone wrong with the BTree");
                    }
                } while (false == _currentNode.IsLeaf);

                _currentEntry -= 1;
                current = _currentNode.GetEntry (_currentEntry);
                return true;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose () { }
    }
}