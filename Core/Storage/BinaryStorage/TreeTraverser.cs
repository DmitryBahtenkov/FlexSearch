using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Storage.BinaryStorage
{
    public class TreeTraverser<K, V> : IEnumerable<Tuple<K, V>>
    {
        private readonly TreeNode<K, V> _fromNode;
        private readonly int _fromIndex;
        private readonly TreeTraverseDirection _direction;
        private readonly ITreeNodeManager<K, V> _nodeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeTraverser{K,V}"/> class.
        /// </summary>
        /// <param name="nodeManager">Node manager.</param>
        /// <param name="fromNode">From node.</param>
        /// <param name="fromIndex">From index.</param>
        /// <param name="direction">Direction.</param>
        public TreeTraverser (ITreeNodeManager<K, V> nodeManager, TreeNode<K,V> fromNode, int fromIndex, TreeTraverseDirection direction)
        {
            if (fromNode is null)
                throw new ArgumentNullException ("fromNode");

            _direction = direction;
            _fromIndex = fromIndex;
            _fromNode = fromNode;
            _nodeManager = nodeManager;
        }

        IEnumerator<Tuple<K, V>> IEnumerable<Tuple<K, V>>.GetEnumerator()
        {
            return new TreeEnumerator<K, V> (_nodeManager, _fromNode, _fromIndex, _direction);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Use the generic version
            return ((IEnumerable<Tuple<K, V>>)this).GetEnumerator();
        }
    }
}