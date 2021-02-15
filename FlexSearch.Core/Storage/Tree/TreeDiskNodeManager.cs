using System;
using System.Collections.Generic;
using Core.Storage.BinaryStorage;
using Core.Storage.Blocks.Interfaces;
using Core.Storage.Helpers;
using Core.Storage.Serializers;

namespace Core.Storage.Tree
{
    public class TreeDiskNodeManager<K, V> : ITreeNodeManager<K, V>
    {
        private readonly IRecordStorage _recordStorage;
        private readonly Dictionary<uint, TreeNode<K, V>> _dirtyNodes = new Dictionary<uint, TreeNode<K, V>> ();
        private readonly Dictionary<uint, WeakReference<TreeNode<K, V>>> _nodeWeakRefs = new Dictionary<uint, WeakReference<TreeNode<K, V>>>();
        private readonly Queue<TreeNode<K, V>> _nodeStrongRefs = new Queue<TreeNode<K, V>> ();
        private readonly int maxStrongNodeRefs = 200;
        private readonly TreeDiskNodeSerializer<K, V> _serializer;

        private int _cleanupCounter;

        public ushort MinEntriesPerNode { get; } = 36;

        public IComparer<Tuple<K, V>> EntryComparer { get; }
        public IComparer<K> KeyComparer { get; }
        public TreeNode<K, V> RootNode { get; private set; }

        public TreeDiskNodeManager (ISerializer<K> keySerializer, ISerializer<V> valueSerializer, IRecordStorage nodeStorage) 
            : this (keySerializer, valueSerializer, nodeStorage, Comparer<K>.Default)
        {
        }

        private TreeDiskNodeManager (ISerializer<K> keySerializer, ISerializer<V> valueSerializer, IRecordStorage recordStorage, IComparer<K> keyComparer)
        {
            _recordStorage = recordStorage ?? throw new ArgumentNullException (nameof(recordStorage));
            _serializer = new TreeDiskNodeSerializer<K, V> (this, keySerializer, valueSerializer);
            KeyComparer = keyComparer;
            EntryComparer = Comparer<Tuple<K, V>>.Create ((a, b) => KeyComparer.Compare (a.Item1, b.Item1));

            // The first record of nodeStorage stores id of root node,
            // if this record do not exist at the time this index instanitate,
            // then attempt to create it
            var firstBlockData = recordStorage.Find(1u);
            if (firstBlockData != null)
            {
                RootNode = Find(BufferHelper.ReadBufferUInt32(firstBlockData, 0));
            } 
            else 
            {
                RootNode = CreateFirstRoot();
            }
        }
        
        public TreeNode<K, V> Create(IEnumerable<Tuple<K, V>> entries, IEnumerable<uint> childrenIds)
        {
            // Create new record
            TreeNode<K, V> node = null;

            _recordStorage.Create (nodeId => 
            {
                // Instantiate a new node
                node = new TreeNode<K, V>(this, nodeId, 0, entries, childrenIds);

                // Always keep reference to any node that we created
                OnNodeInitialized(node);

                // Return its deserialized value
                return _serializer.Serialize (node);
            });

            if (node == null) 
            {
                throw new Exception("dataGenerator never called by nodeStorage");
            }

            return node;
        }
        
        public TreeNode<K, V> Find(uint id)
        {
            // Check if the node is being held in memory, 
            // if it does then return it
            if (_nodeWeakRefs.ContainsKey(id))
            {
                if (_nodeWeakRefs[id].TryGetTarget(out var node)) 
                {
                    return node;
                }

                // node deallocated, remove weak reference
                _nodeWeakRefs.Remove (id);
            }

            // Not is not in memory, go get it
            var data = _recordStorage.Find(id);
            if (data == null) 
            {
                return  null;
            }
            var dNode = _serializer.Deserialize(id, data);

            // Always keep reference to node we created
            OnNodeInitialized(dNode);
            return dNode;
        }
        
        public TreeNode<K, V> CreateNewRoot(K key, V value, uint leftNodeId, uint rightNodeId)
        {
            // Create new node as normal
            var node = Create(new Tuple<K, V>[] 
            {
                new(key, value)
            }, new[]
            {
                leftNodeId,
                rightNodeId
            });

            // Make it the root node
            RootNode = node;
            _recordStorage.Update (1u, LittleEndianByteOrder.GetBytes(node.Id));
            
            return RootNode;
        }
        
        public void MakeRoot (TreeNode<K, V> node)
        {
            RootNode = node;
            _recordStorage.Update(1u, LittleEndianByteOrder.GetBytes(node.Id));
        }

        public void Delete (TreeNode<K, V> node)
        {
            if (node == RootNode) 
            {
                RootNode = null;
            }
			
            _recordStorage.Delete(node.Id);

            if (_dirtyNodes.ContainsKey(node.Id)) 
            {
                _dirtyNodes.Remove(node.Id);
            }
        }
        
        public void MarkAsChanged (TreeNode<K, V> node)
        {
            if (false == _dirtyNodes.ContainsKey(node.Id)) 
            {
                _dirtyNodes.Add (node.Id, node);
            }
        }

        public void SaveChanges ()
        {
            foreach (var kv in _dirtyNodes)  
            {
                _recordStorage.Update(kv.Value.Id, _serializer.Serialize (kv.Value));
            }

            _dirtyNodes.Clear();
        }
        
        private TreeNode<K, V> CreateFirstRoot()
        {
            // Write down the id of first node into the first block
            _recordStorage.Create (LittleEndianByteOrder.GetBytes((uint)2));

            // Return a new node, this node should has id of 2
            return Create (null, null);
        }

        private void OnNodeInitialized(TreeNode<K, V> node)
        {
            // Keep a weak reference to it
            _nodeWeakRefs.Add (node.Id, new WeakReference<TreeNode<K, V>>(node));

            // Keep a strong reference to prevent weak refs from being dellocated
            _nodeStrongRefs.Enqueue (node);

            // Clean up strong refs if we been holding too many of them
            if (_nodeStrongRefs.Count >= maxStrongNodeRefs) 
            {
                while (_nodeStrongRefs.Count >= (maxStrongNodeRefs/2f)) 
                {
                    _nodeStrongRefs.Dequeue ();
                }
            }

            // Clean up weak refs
            if (_cleanupCounter++ >= 1000)
            {
                _cleanupCounter = 0;
                var tobeDeleted = new List<uint>();
                foreach (var kv in _nodeWeakRefs)
                {
                    TreeNode<K, V> target;
                    if (false == kv.Value.TryGetTarget(out target)) 
                    {
                        tobeDeleted.Add(kv.Key);
                    }
                }

                foreach (var key in tobeDeleted)
                {
                    _nodeWeakRefs.Remove(key);
                }
            }
        }
    }
}