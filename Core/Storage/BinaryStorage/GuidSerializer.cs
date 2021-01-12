using System;

namespace Core.Storage.BinaryStorage
{
    public class GuidSerializer : ISerializer<Guid>
    {
        public byte[] Serialize (Guid value)
        {
            return value.ToByteArray ();
        }

        public Guid Deserialize (byte[] buffer, int offset, int length)
        {
            if (length != 16) {
                throw new ArgumentException ("length");
            }

            return BufferHelper.ReadBufferGuid (buffer, offset);
        }

        public bool IsFixedSize => true;
        public int Length => 16;
    }
}