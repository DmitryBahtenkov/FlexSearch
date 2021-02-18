using System;
using System.Collections.Generic;
using Core.Storage.Helpers;
using Newtonsoft.Json;

namespace Core.Storage.Serializers
{
    public class DictSerializer : ISerializer<Dictionary<string, Guid>>
    {
        public byte[] Serialize(Dictionary<string, Guid> value)
        {
            var serializeStr = JsonConvert.SerializeObject(value);
            var stringBytes = System.Text.Encoding.UTF8.GetBytes(serializeStr);

            var data = new byte [
                4 +                    // First 4 bytes indicate length of the string
                stringBytes.Length   // another X bytes of actual string content
            ];

            BufferHelper.WriteBuffer(stringBytes.Length, data, 0);
            Buffer.BlockCopy(stringBytes, 0, data, 4, stringBytes.Length);
            return data;
        }

        public Dictionary<string, Guid> Deserialize(byte[] buffer, int offset, int length)
        {
            var stringLength = BufferHelper.ReadBufferInt32 (buffer, offset);
            if (stringLength < 0 || stringLength > 16 * 1024) {
                throw new Exception ("Invalid string length: " + stringLength);
            }
            var stringValue = System.Text.Encoding.UTF8.GetString (buffer, offset +4, stringLength);
            return JsonConvert.DeserializeObject<Dictionary<string, Guid>>(stringValue);
        }
        
        
        public bool IsFixedSize => false;
        public int Length { get; }
    }
}