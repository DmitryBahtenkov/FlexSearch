using System;
using System.IO;
using Core.Storage.Helpers;
using Core.Storage.Serializers;

namespace Core.Storage.Tree
{
    public class TreeDiskNodeSerializer<TK, TV>
    {
        private readonly ISerializer<TK> _keySerializer;
        private readonly ISerializer<TV> _valueSerializer;
        private readonly ITreeNodeManager<TK, TV> _nodeManager;

        public TreeDiskNodeSerializer(ITreeNodeManager<TK, TV> nodeManager, ISerializer<TK> keySerializer,
            ISerializer<TV> valueSerializer)
        {
            _nodeManager = nodeManager ?? throw new ArgumentNullException(nameof(nodeManager));
            _keySerializer = keySerializer ?? throw new ArgumentNullException(nameof(keySerializer));
            _valueSerializer = valueSerializer ?? throw new ArgumentNullException(nameof(valueSerializer));
        }

        public byte[] Serialize(TreeNode<TK, TV> node)
        {
            if (_keySerializer.IsFixedSize && _valueSerializer.IsFixedSize)
            {
                return FixedLengthSerialize(node);
            }

            return _valueSerializer.IsFixedSize ? 
                VariableKeyLengthSerialize(node) : 
                VariableLengthSerialize(node);
        }

        public TreeNode<TK, TV> Deserialize(uint assignId, byte[] record)
        {
            if (_keySerializer.IsFixedSize && _valueSerializer.IsFixedSize)
            {
                return FixedLengthDeserialize(assignId, record);
            }

            return _valueSerializer.IsFixedSize ? 
                VariableKeyLengthDeserialize(assignId, record) : 
                VariableLengthDeserialize(assignId, record);
        }

        private byte[] FixedLengthSerialize(TreeNode<TK, TV> node)
        {
            var entrySize = _keySerializer.Length + _valueSerializer.Length;
            var size = 16
                       + node.Entries.Length * entrySize
                       + node.ChildrenIds.Length * 4;
            if (size >= 1024 * 64)
            {
                throw new Exception("Serialized node size too large: " + size);
            }

            var buffer = new byte[size];

            // First 4 bytes of the buffer is parent id of this node
            BufferHelper.WriteBuffer(node.ParentId, buffer, 0);

            // Followed by 4 bytes of how many entries
            BufferHelper.WriteBuffer((uint) node.EntriesCount, buffer, 4);

            // Followed by 4 bytes of how manu child references
            BufferHelper.WriteBuffer((uint) node.ChildrenNodeCount, buffer, 8);

            // Start writing entries && child refs
            for (var i = 0; i < node.EntriesCount; i++)
            {
                // Write entry
                var entry = node.GetEntry(i);
                Buffer.BlockCopy(_keySerializer.Serialize(entry.Item1), 0, buffer, 12 + i * entrySize,
                    _keySerializer.Length);
                Buffer.BlockCopy(_valueSerializer.Serialize(entry.Item2), 0, buffer,
                    12 + i * entrySize + _keySerializer.Length, _valueSerializer.Length);
            }

            // Start writing child references
            var childrenIds = node.ChildrenIds;
            for (var i = 0; i < node.ChildrenNodeCount; i++)
            {
                // Write child refs
                BufferHelper.WriteBuffer(childrenIds[i], buffer, 12 + entrySize * node.EntriesCount + i * 4);
            }

            // Return generated buffer
            return buffer;
        }

        private TreeNode<TK, TV> FixedLengthDeserialize(uint assignId, byte[] buffer)
        {
            var entrySize = _keySerializer.Length + _valueSerializer.Length;

            // First 4 bytes uint32 is parent id 
            var parentId = BufferHelper.ReadBufferUInt32(buffer, 0);

            // Followed by 4 bytes uint32 of by how many entries this node has
            var entriesCount = BufferHelper.ReadBufferUInt32(buffer, 4);

            // Followed by 4 bytes uint32 of how many child reference this node has
            var childrenCount = BufferHelper.ReadBufferUInt32(buffer, 8);

            // Deserialize entries
            var entries = new Tuple<TK, TV>[entriesCount];
            for (var i = 0; i < entriesCount; i++)
            {
                var key = _keySerializer.Deserialize(buffer
                    , 12 + i * entrySize
                    , _keySerializer.Length);
                var value = _valueSerializer.Deserialize(buffer
                    , 12 + i * entrySize + _keySerializer.Length
                    , _valueSerializer.Length);
                entries[i] = new Tuple<TK, TV>(key, value);
            }

            // Decode child refs..
            var children = new uint[childrenCount];
            for (var i = 0; i < childrenCount; i++)
            {
                // Decode child refs..
                children[i] = BufferHelper.ReadBufferUInt32(buffer, (int) (12 + entrySize * entriesCount + i * 4));
            }

            // Reconstuct the node
            return new TreeNode<TK, TV>(_nodeManager, assignId, parentId, entries, children);
        }

        private TreeNode<TK, TV> VariableKeyLengthDeserialize(uint assignId, byte[] buffer)
        {
            // First 4 bytes uint32 is parent id 
            var parentId = BufferHelper.ReadBufferUInt32(buffer, 0);

            // Followed by 4 bytes uint32 of by how many entries this node has
            var entriesCount = BufferHelper.ReadBufferUInt32(buffer, 4);

            // Followed by 4 bytes uint32 of how many child reference this node has
            var childrenCount = BufferHelper.ReadBufferUInt32(buffer, 8);

            // Deserialize entries
            var entries = new Tuple<TK, TV>[entriesCount];
            var p = 12;
            for (var i = 0; i < entriesCount; i++)
            {
                var keyLength = BufferHelper.ReadBufferInt32(buffer, p);
                var key = _keySerializer.Deserialize(buffer, p + 4, keyLength);
                var value = _valueSerializer.Deserialize(buffer, p + 4 + keyLength, _valueSerializer.Length);

                entries[i] = new Tuple<TK, TV>(key, value);

                p += 4 + keyLength + _valueSerializer.Length;
            }

            // Decode child refs..
            var children = new uint[childrenCount];
            for (var i = 0; i < childrenCount; i++)
            {
                // Decode child refs..
                children[i] = BufferHelper.ReadBufferUInt32(buffer, p + i * 4);
            }

            // Reconstuct the node
            return new TreeNode<TK, TV>(_nodeManager, assignId, parentId, entries, children);
        }

        private byte[] VariableKeyLengthSerialize(TreeNode<TK, TV> node)
        {
            using (var m = new MemoryStream())
            {
                // 4 bytes uint parent id
                m.Write(LittleEndianByteOrder.GetBytes(node.ParentId), 0, 4);
                // Followed by 4 bytes of how many entries
                m.Write(LittleEndianByteOrder.GetBytes((uint) node.EntriesCount), 0, 4);
                // Followed by 4 bytes of how manu child references
                m.Write(LittleEndianByteOrder.GetBytes((uint) node.ChildrenNodeCount), 0, 4);

                // Write entries..
                for (var i = 0; i < node.EntriesCount; i++)
                {
                    // Write entry
                    var entry = node.GetEntry(i);
                    var key = _keySerializer.Serialize(entry.Item1);
                    var value = _valueSerializer.Serialize(entry.Item2);

                    m.Write(LittleEndianByteOrder.GetBytes(key.Length), 0, 4);
                    m.Write(key, 0, key.Length);
                    m.Write(value, 0, value.Length);
                }

                // Write child refs..
                var childrenIds = node.ChildrenIds;
                for (var i = 0; i < node.ChildrenNodeCount; i++)
                {
                    m.Write(LittleEndianByteOrder.GetBytes(childrenIds[i]), 0, 4);
                }

                return m.ToArray();
            }
        }
        
        private byte[] VariableLengthSerialize(TreeNode<TK, TV> node)
        {
            using (var m = new MemoryStream())
            {
                // 4 bytes uint parent id
                m.Write(LittleEndianByteOrder.GetBytes(node.ParentId), 0, 4);
                // Followed by 4 bytes of how many entries
                m.Write(LittleEndianByteOrder.GetBytes((uint) node.EntriesCount), 0, 4);
                // Followed by 4 bytes of how manu child references
                m.Write(LittleEndianByteOrder.GetBytes((uint) node.ChildrenNodeCount), 0, 4);

                // Write entries..
                for (var i = 0; i < node.EntriesCount; i++)
                {
                    // Write entry
                    var entry = node.GetEntry(i);
                    var key = _keySerializer.Serialize(entry.Item1);
                    var value = _valueSerializer.Serialize(entry.Item2);

                    m.Write(LittleEndianByteOrder.GetBytes(key.Length), 0, 4);
                    m.Write(key, 0, key.Length);
                    m.Write(LittleEndianByteOrder.GetBytes(value.Length), 0, 4);
                    m.Write(value, 0, value.Length);
                }

                // Write child refs..
                var childrenIds = node.ChildrenIds;
                for (var i = 0; i < node.ChildrenNodeCount; i++)
                {
                    m.Write(LittleEndianByteOrder.GetBytes(childrenIds[i]), 0, 4);
                }

                return m.ToArray();
            }
        }
        
        private TreeNode<TK, TV> VariableLengthDeserialize(uint assignId, byte[] buffer)
        {
            // First 4 bytes uint32 is parent id 
            var parentId = BufferHelper.ReadBufferUInt32(buffer, 0);

            // Followed by 4 bytes uint32 of by how many entries this node has
            var entriesCount = BufferHelper.ReadBufferUInt32(buffer, 4);

            // Followed by 4 bytes uint32 of how many child reference this node has
            var childrenCount = BufferHelper.ReadBufferUInt32(buffer, 8);

            // Deserialize entries
            var entries = new Tuple<TK, TV>[entriesCount];
            var p = 12;
            for (var i = 0; i < entriesCount; i++)
            {
                var keyLength = BufferHelper.ReadBufferInt32(buffer, p);
                var key = _keySerializer.Deserialize(buffer, p + 4, keyLength);
                var valLength = BufferHelper.ReadBufferInt32(buffer, p+4+keyLength);
                var value = _valueSerializer.Deserialize(buffer, p + 8 + keyLength, valLength);

                entries[i] = new Tuple<TK, TV>(key, value);

                p += 8 + keyLength + valLength;
            }

            // Decode child refs..
            var children = new uint[childrenCount];
            for (var i = 0; i < childrenCount; i++)
            {
                // Decode child refs..
                children[i] = BufferHelper.ReadBufferUInt32(buffer, p + i * 4);
            }

            // Reconstuct the node
            return new TreeNode<TK, TV>(_nodeManager, assignId, parentId, entries, children);
        }
    }
}