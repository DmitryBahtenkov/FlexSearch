using System;
using Core.Storage.BinaryStorage;
using Core.Storage.Helpers;

namespace Core.Storage.Serializers
{
    public class GuidSerializer : ISerializer<Guid>
    {
        public byte[] Serialize(Guid value)
        {
            return value.ToByteArray();
        }

        public Guid Deserialize(byte[] buffer, int offset, int length)
        {
            if (length != 16) 
            {
                throw new ArgumentException("incorrect length");
            }

            return BufferHelper.ReadBufferGuid(buffer, offset);
        }

        public bool IsFixedSize => true;
        public int Length => 16;
    }
}