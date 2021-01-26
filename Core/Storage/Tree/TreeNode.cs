using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core.Storage.Tree
{
    public class TreeNode<TK, TV>
    {
	    private uint _parentId;
	    private readonly ITreeNodeManager<TK, TV> _nodeManager;
	    private readonly List<uint> _childrenIds;
	    private readonly List<Tuple<TK, TV>> _entries;
        
        public TK MaxKey => _entries[^1].Item1;
        public TK MinKey => _entries[0].Item1;

        public bool IsEmpty => _entries.Count == 0;
        public bool IsLeaf => _childrenIds.Count == 0;
        public bool IsOverflow => _entries.Count > _nodeManager.MinEntriesPerNode * 2;

        public int EntriesCount => _entries.Count;
        public int ChildrenNodeCount => _childrenIds.Count;
        
        public uint ParentId
        {
            get => _parentId;
            private set
            {
                _parentId = value;
                _nodeManager.MarkAsChanged(this);
            }
        }

        public uint[] ChildrenIds => _childrenIds.ToArray();
        public Tuple<TK, TV>[] Entries => _entries.ToArray();
        public uint Id { get; }

        public TreeNode(ITreeNodeManager<TK, TV> nodeManager, uint id, uint parentId,
            IEnumerable<Tuple<TK, TV>> entries = null, IEnumerable<uint> childrenIds = null)
        {
	        Id = id;
            _parentId = parentId;

            // Setting up readonly attributes
            _nodeManager = nodeManager ?? throw new ArgumentNullException(nameof(nodeManager));
            _childrenIds = new List<uint>();
            _entries = new List<Tuple<TK, TV>>(_nodeManager.MinEntriesPerNode*2);

            // Loading up data
            if (entries is not null)
            {
                _entries.AddRange(entries);
            }

            if (childrenIds is not null) 
            {
                _childrenIds.AddRange(childrenIds);
            }
        }
        
        public void Remove(int removeAt)
        {
            // Validate argument
            if (false == removeAt >= 0 && removeAt < _entries.Count) 
            {
                throw new ArgumentOutOfRangeException ();
            }
            
            // If this is a node leave, flagged entry will be removed
            if (IsLeaf) 
            {
                // Step 1: Remove X from the current node. 
                // Being a leaf node there are no subtrees to worry about.
                _entries.RemoveAt (removeAt);
                _nodeManager.MarkAsChanged (this);

                // If the removal does not cause underflow then we are done here
                if (EntriesCount >= _nodeManager.MinEntriesPerNode || _parentId == 0) 
                {
                    return;
                }
                // Otherwise, rebalance this node
                Rebalance();
            }
            // If the value to be deleted does not occur in a leaf,
            // we replace it with the largest value in its left subtree 
            // and then proceed to delete that value from the node that 
            // originally contained it
            else 
            {
                // Grab the largest entry on the left subtree
                var leftSubTree = _nodeManager.Find(_childrenIds[removeAt]);
                leftSubTree.FindLargest (out var largestNode, out var largestIndex);
                var replacementEntry = largestNode.GetEntry(largestIndex);

                // Replace it
                _entries[removeAt] = replacementEntry;
                _nodeManager.MarkAsChanged(this);

                // Remove it from the node we took it from
                largestNode.Remove(largestIndex);
            }
        }
        
        /// <summary>
        /// Get this node's index in its parent
        /// </summary>
        public int IndexInParent ()
        {
            var parent = _nodeManager.Find(_parentId);
            if (parent == null) 
            {
                throw new Exception("IndexInParent fails to find parent node of " + Id);
            }
            var childrenIds = parent.ChildrenIds;
            for (var i = 0; i < childrenIds.Length; i++)
            {
                if (childrenIds[i] == Id)
                {
                    return i;
                }
            }

            throw new Exception("Failed to find index of node " + Id + " in its parent");
        }
        
        /// <summary>
        /// Find the largest entry on this subtree and output it to specified parameters
        /// </summary>
        private void FindLargest(out TreeNode<TK, TV> node, out int index)
        {
            // If this node is leave then we reached
            // the bottom of the tree, return this node's max value
            if (IsLeaf) 
            {
                node = this;
                index = _entries.Count -1;
            }
            // Otherwise, keep drilling down to the right
            else 
            {
                var rightMostNode = _nodeManager.Find(_childrenIds[^1]);
                rightMostNode.FindLargest (out node, out index);
            }
        }
        
        /// <summary>
        /// Find the smallest entry on this subtree and output it to specified parameters
        /// </summary>
        public void FindSmallest(out TreeNode<TK, TV> node, out int index)
        {
            // If this node is leave then we reached
            // the bottom of the tree, return this node's max value
            if (IsLeaf) 
            {
                node = this;
                index = 0;
            }
            // Otherwise, keep drilling down to the right
            else 
            {
                var leftMostNode = _nodeManager.Find(_childrenIds[0]);
                leftMostNode.FindSmallest (out node, out index);
            }
        }
        
        public void InsertAsLeaf(TK key, TV value, int insertPosition)
        {
            Debug.Assert(IsLeaf, "Call this method on leaf node only");

            _entries.Insert(insertPosition, new Tuple<TK, TV>(key, value));
            _nodeManager.MarkAsChanged(this);
        }

        private void InsertAsParent(TK key, TV value, uint leftReference, uint rightReference, out int insertPosition)
        {
            Debug.Assert(false == IsLeaf, "Call this method on non-leaf node only");

            // Find insert position
            insertPosition = BinarySearchEntriesForKey(key);
            insertPosition = insertPosition >= 0 ? insertPosition : ~insertPosition;

            // Insert entry first
            _entries.Insert (insertPosition, new Tuple<TK, TV>(key, value));

            // Then insert and update child references
            _childrenIds.Insert (insertPosition, leftReference);
            _childrenIds[insertPosition+1] = rightReference;

            // This node has been changed as we modified entries and children references
            _nodeManager.MarkAsChanged (this);
        }
        
        /// <summary>
		/// Split this node in half
		/// </summary>
		public void Split(out TreeNode<TK, TV> outLeftNode, out TreeNode<TK, TV> outRightNode)
		{
			Debug.Assert(IsOverflow, "Calling Split when node is not overflow");

			var halfCount = _nodeManager.MinEntriesPerNode;
			var middleEntry = _entries[halfCount];

			// Create new node that holds all values
			// that larger than the middle one
			var rightEntries = new Tuple<TK, TV>[halfCount];
			var rightChildren = (uint[])null;
			_entries.CopyTo(halfCount+1, rightEntries, 0, rightEntries.Length);
			if (false == IsLeaf) 
			{
				rightChildren = new uint[halfCount + 1];
				_childrenIds.CopyTo (halfCount+1, rightChildren, 0, rightChildren.Length);
			}
			var newRightNode = _nodeManager.Create(rightEntries, rightChildren);

			// As we moved half of the children node to the new parent,
			// the ParentId property of these nodes also need tobe updated
			if (rightChildren != null) 
			{
				foreach (var childId in rightChildren) 
				{
					_nodeManager.Find(childId).ParentId = newRightNode.Id;
				}
			}

			// Remove all values that larger than the middle 
			// one from current node
			_entries.RemoveRange(0, halfCount);

			if (false == IsLeaf) 
			{
				_childrenIds.RemoveRange(0, halfCount + 1);
			}

			// alright now we have 2 nodes,
			// insert the middle element to parent node.
			var parent = _parentId == 0 ? null : _nodeManager.Find(_parentId);

			// If there is no parent,
			// then the middle element becomes the new root node
			if (parent == null) 
			{
				parent = _nodeManager.CreateNewRoot (middleEntry.Item1, middleEntry.Item2,Id, newRightNode.Id);
				ParentId = parent.Id;
				newRightNode.ParentId = parent.Id;
			}
			// Otherwise, elevate the middle element
			// to the parent node
			else 
			{
				int insertPosition;
				parent.InsertAsParent (middleEntry.Item1, middleEntry.Item2, Id, newRightNode.Id, out insertPosition);
				
				newRightNode.ParentId = parent.Id;

				// If parent is overflow, split and update reference
				if (parent.IsOverflow) 
				{
					TreeNode<TK, TV> left, right;
					parent.Split (out left, out right);
				}
			}

			// Output the node that 
			outLeftNode = this;
			outRightNode = newRightNode;

			// Mark this node as changed
			_nodeManager.MarkAsChanged(this);
		}
        
        /// <summary>
        /// Perform a binary search on entries
        /// </summary>
        public int BinarySearchEntriesForKey(TK key)
        {
	        return _entries.BinarySearch(new Tuple<TK, TV>(key, default), _nodeManager.EntryComparer);
        }

        /// <summary>
        /// Perform binary search on entries, but if there are multiple occurences,
        /// return either last or first occurence based on firstOccurrence param
        /// </summary>
        /// <param name="key"></param>
        /// <param name="firstOccurence">If set to <c>true</c> first occurence.</param>
        public int BinarySearchEntriesForKey (TK key, bool firstOccurence)
        {
	        if (firstOccurence) 
	        {
		        return _entries.BinarySearch(new Tuple<TK, TV>(key, default), _nodeManager.EntryComparer);
	        }

	        _entries.Reverse();
	        return  _entries.BinarySearch(new Tuple<TK, TV>(key, default), _nodeManager.EntryComparer);
        }

        /// <summary>
        /// Get a children node by its internal position to this node
        /// </summary>
        public TreeNode<TK, TV> GetChildNode(int atIndex)
        {
	        return _nodeManager.Find(_childrenIds[atIndex]);
        }

        /// <summary>
        /// Get a Key-Value entry inside this node
        /// </summary>
        public Tuple<TK, TV> GetEntry(int atIndex)
        {
	        return _entries[atIndex];
        }

        /// <summary>
        /// Check if there is an entry at given index
        /// </summary>
        public bool EntryExists(int atIndex)
        {
	        return atIndex < _entries.Count;
        }

        public override string ToString()
        {
	        if (IsLeaf) 
	        {
		        var numbers = (from tuple in _entries select tuple.Item1.ToString()).ToArray ();
		        return $"[Node: Id={Id}, ParentId={ParentId}, Entries={string.Join(",", numbers)}]";	
	        } 
	        else 
	        {
		        var numbers = (from tuple in _entries select tuple.Item1.ToString()).ToArray ();
		        var ids = (from id in _childrenIds select id.ToString()).ToArray ();
		        return
			        $"[Node: Id={Id}, ParentId={ParentId}, Entries={string.Join(",", numbers)}, Children={String.Join(",", ids)}]";
	        }
        }

	    /// <summary>
		/// Rebalance this node after an element has been removed causing it to underflow
		/// </summary>
		private void Rebalance()
		{
			// If the deficient node's right sibling exists and has more 
			// than the minimum number of elements, then rotate left
			var indexInParent = IndexInParent ();
			var parent = _nodeManager.Find (_parentId);
			var rightSibling = indexInParent + 1 < parent.ChildrenNodeCount ? parent.GetChildNode (indexInParent+1) : null;
			if (rightSibling != null && rightSibling.EntriesCount > _nodeManager.MinEntriesPerNode)
			{
				// Copy the separator from the parent to the end of the deficient node 
				// (the separator moves down; the deficient node now has the minimum number of elements)
				_entries.Add (parent.GetEntry (indexInParent));

				// Replace the separator in the parent with the first element of the right sibling 
				// (right sibling loses one node but still has at least the minimum number of elements)
				parent._entries[indexInParent] = rightSibling._entries[0];
				rightSibling._entries.RemoveAt(0);

				// Move the first child reference from right sibling to me.
				if (false == rightSibling.IsLeaf) 
				{
					// First, update parentId of the child that will be moved
					var n = _nodeManager.Find (rightSibling._childrenIds[0]);
					n._parentId = Id;
					_nodeManager.MarkAsChanged (n);	
					// Then move it
					_childrenIds.Add (rightSibling._childrenIds[0]);
					rightSibling._childrenIds.RemoveAt (0);
				}

				// The tree is now balanced
				_nodeManager.MarkAsChanged (this);
				_nodeManager.MarkAsChanged (parent);
				_nodeManager.MarkAsChanged (rightSibling);
				return;
			}

			// Otherwise, if the deficient node's left sibling exists and has more
			// than the minimum number of elements, then rotate right
			var leftSibling = indexInParent -1 >= 0 ? parent.GetChildNode (indexInParent -1) : null;
			if (leftSibling != null && leftSibling.EntriesCount > _nodeManager.MinEntriesPerNode)
			{
				// Copy the separator from the parent to the start of the deficient node 
				// (the separator moves down; deficient node now has the minimum number of elements)
				_entries.Insert (0, parent.GetEntry(indexInParent -1));

				// Replace the separator in the parent with the last element 
				// of the left sibling (left sibling loses one node but still has 
				// at least the minimum number of elements)
				parent._entries[indexInParent -1] = leftSibling._entries[leftSibling._entries.Count -1];
				leftSibling._entries.RemoveAt (leftSibling._entries.Count -1);

				// Move the last child reference from the left sibing to me.
				// First, update parentId of the child that will be moved.
				if (false == IsLeaf) {
					var n = _nodeManager.Find (leftSibling._childrenIds[leftSibling._childrenIds.Count - 1]);
					n._parentId = Id;
					_nodeManager.MarkAsChanged (n);
					// Then move it
					_childrenIds.Insert (0, leftSibling._childrenIds[leftSibling._childrenIds.Count - 1]);
					leftSibling._childrenIds.RemoveAt (leftSibling._childrenIds.Count - 1);
				}

				// The tree is now balanced;
				_nodeManager.MarkAsChanged (this);
				_nodeManager.MarkAsChanged (parent);
				_nodeManager.MarkAsChanged (leftSibling);
				return;
			}

			// Otherwise, if both immediate siblings have only the minimum number of elements, 
			// then merge with a sibling sandwiching their separator taken off from their parent
			var leftChild = rightSibling != null ? this : leftSibling;
			var rightChild = rightSibling != null ? rightSibling : this;
			var seperatorParentIndex = rightSibling != null ? indexInParent : indexInParent-1;

			// Step 1:
			// Copy the separator to the end of the left node (the left node may be the
			// deficient node or it may be the sibling with the minimum number of elements)
			leftChild._entries.Add (parent.GetEntry (seperatorParentIndex));

			// Move all elements from the right node to the left 
			// node (the left node now has the maximum number of elements, and the right node – empty)
			leftChild._entries.AddRange(rightChild._entries);
			leftChild._childrenIds.AddRange (rightChild._childrenIds);
			// Update parent id of the children that has been moved from rightChild to leftChild
			foreach (var id in rightChild._childrenIds)
			{
				var n = _nodeManager.Find (id);
				n._parentId = leftChild.Id;
				_nodeManager.MarkAsChanged (n);;
			}

			// Remove the separator from the parent along with its
			// empty right child (the parent loses an element)
			parent._entries.RemoveAt(seperatorParentIndex);
			parent._childrenIds.RemoveAt(seperatorParentIndex + 1);
			_nodeManager.Delete(rightChild);

			// If the parent is the root and now has no elements, 
			// then free it and make the merged node the new root (tree becomes shallower)
			if (parent._parentId == 0 && parent.EntriesCount == 0) 
			{
				leftChild._parentId = 0;
				_nodeManager.MarkAsChanged(leftChild); // Changed left one
				_nodeManager.MakeRoot(leftChild);      
				_nodeManager.Delete(parent);           // Deleted parent
			}
			// Otherwise, if the parent has fewer than 
			// the required number of elements, then rebalance the parent
			else if (parent._parentId != 0 && parent.EntriesCount < _nodeManager.MinEntriesPerNode) 
			{
				_nodeManager.MarkAsChanged(leftChild);  // Changed left one
				_nodeManager.MarkAsChanged(parent);     // Changed parent
				parent.Rebalance();
			} 
			else 
			{
				_nodeManager.MarkAsChanged(leftChild);  // Changed left one
				_nodeManager.MarkAsChanged(parent);     // Changed parent
			}
		}
    }
}