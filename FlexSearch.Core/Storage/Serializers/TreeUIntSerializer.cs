using System;
using Core.Storage.BinaryStorage;
using Core.Storage.Helpers;

namespace Core.Storage.Serializers
{
    public class TreeUIntSerializer : ISerializer<uint>
    {
        public byte[] Serialize(uint value)
        {
            return LittleEndianByteOrder.GetBytes(value);
        }

        public uint Deserialize(byte[] buffer, int offset, int length)
        {
            if (length != 4) 
            {
                throw new ArgumentException("Invalid length: " + length);
            }
			
            return BufferHelper.ReadBufferUInt32(buffer, offset);
        }

        public bool IsFixedSize => true;

        public int Length => 4;
    }
}