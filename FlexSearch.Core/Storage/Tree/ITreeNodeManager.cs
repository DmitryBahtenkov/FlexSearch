using System;
using System.Collections.Generic;

namespace Core.Storage.Tree
{
    public interface ITreeNodeManager<TK, TV>
    {
        /// <summary>
        /// Minimum number of entries per node. Maximum number of entries
        /// must be equal to MinEntriesCountPerNode*2
        /// </summary>
        public ushort MinEntriesPerNode { get; }

        /// <summary>
        /// Get the comparer that used to compare keys
        /// </summary>
        public IComparer<TK> KeyComparer { get; }

        /// <summary>
        /// Get the comparer that used to compare entries,
        /// This must use KeyComparer declared above
        /// </summary>
        public IComparer<Tuple<TK, TV>> EntryComparer { get; }

        /// <summary>
        /// Get the root node. Root node must be cached, because it is always get called
        /// </summary>
        public TreeNode<TK, TV> RootNode { get; }

        /// <summary>
        /// Creates a new node that carries given entires, and keep references to given children nodes
        /// </summary>
        /// <param name="entries">Entries.</param>
        /// <param name="childrenIds">Children identifiers.</param>
        public TreeNode<TK, TV> Create(IEnumerable<Tuple<TK, TV>> entries, IEnumerable<uint> childrenIds);

        /// <summary>
        /// Find a node by its numeric ID
        /// </summary>
        public TreeNode<TK, TV> Find(uint id);

        /// <summary>
        /// Called by the tree to split a current root node to a new root node.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="leftNodeId">Left node identifier.</param>
        /// <param name="rightNodeId">Right node identifier.</param>
        public TreeNode<TK, TV> CreateNewRoot(TK key, TV value, uint leftNodeId, uint rightNodeId);

        /// <summary>
        /// Make given node tobe root straigh away
        /// </summary>
        public void MakeRoot(TreeNode<TK, TV> node);

        /// <summary>
        /// Mark a given node as dirty tobe saved later
        /// </summary>
        /// <param name="node">Node.</param>
        public void MarkAsChanged(TreeNode<TK, TV> node);

        /// <summary>
        /// Delete specified node straight away
        /// </summary>
        public void Delete(TreeNode<TK, TV> node);

        /// <summary>
        /// Write all dirty nodes to disk
        /// </summary>
        void SaveChanges();
    }
}