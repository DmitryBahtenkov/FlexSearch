using System;
using Core.Storage.BinaryStorage;
using Core.Storage.Helpers;

namespace Core.Storage.Serializers
{
    public class StringSerializer : ISerializer<string>
    {
        public byte[] Serialize(string value)
        {
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(value);

            var data = new byte [4 + stringBytes.Length];

            BufferHelper.WriteBuffer(stringBytes.Length, data, 0);
            
            Buffer.BlockCopy(stringBytes, 0, data, 4, stringBytes.Length);
            return data;
        }

        public string Deserialize(byte[] buffer, int offset, int length)
        {
            var stringLength = BufferHelper.ReadBufferInt32 (buffer, offset);
            if (stringLength < 0 || stringLength > 16 * 1024) 
            {
                throw new Exception ("Invalid string length: " + stringLength);
            }
            
            var stringValue = System.Text.Encoding.UTF8.GetString (buffer, offset +4, stringLength);
            return stringValue;
        }
        
        
        public bool IsFixedSize => false;
        public int Length { get; }
    }
}