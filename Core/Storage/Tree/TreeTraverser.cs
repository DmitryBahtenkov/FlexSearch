using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Storage.Tree
{
    public class TreeTraverser<TK, TV> : IEnumerable<Tuple<TK, TV>>
    {
        private readonly TreeNode<TK, TV> _fromNode;
        private readonly int _fromIndex;
        private readonly TreeTraverseDirection _direction;
        private readonly ITreeNodeManager<TK, TV> _nodeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeTraverser{K,V}"/> class.
        /// </summary>
        /// <param name="nodeManager">Node manager.</param>
        /// <param name="fromNode">From node.</param>
        /// <param name="fromIndex">From index.</param>
        /// <param name="direction">Direction.</param>
        public TreeTraverser (ITreeNodeManager<TK, TV> nodeManager, TreeNode<TK,TV> fromNode, int fromIndex, TreeTraverseDirection direction)
        {
            _direction = direction;
            _fromIndex = fromIndex;
            _fromNode = fromNode ?? throw new ArgumentNullException (nameof(fromNode));
            _nodeManager = nodeManager;
        }

        IEnumerator<Tuple<TK, TV>> IEnumerable<Tuple<TK, TV>>.GetEnumerator()
        {
            return new TreeEnumerator<TK, TV>(_nodeManager, _fromNode, _fromIndex, _direction);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Use the generic version
            return ((IEnumerable<Tuple<TK, TV>>)this).GetEnumerator();
        }
    }
}