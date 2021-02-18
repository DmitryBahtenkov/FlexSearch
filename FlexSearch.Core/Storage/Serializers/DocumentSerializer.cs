using System;
using Core.Models;
using Core.Storage.Helpers;
using Newtonsoft.Json.Linq;

namespace Core.Storage.Serializers
{
    public class DocumentSerializer
    {
        public byte[] Serialize(DocumentModel model)
        {
            var value = model.Value.ToString();
            var valueBytes = System.Text.Encoding.UTF8.GetBytes(value);
            var data = new byte[16 + 4 + valueBytes.Length];
            
            //Первая часть блока - Id 
            Buffer.BlockCopy(model.Id.ToByteArray(), 0, data, 0, 16);
            
            //Вторая часть блока - длина json-объекта
            Buffer.BlockCopy(LittleEndianByteOrder.GetBytes(valueBytes.Length), 0, data, 16, 4);
            
            //Третья часть блока - json-объект
            Buffer.BlockCopy(valueBytes, 0, data, 16 + 4, valueBytes.Length);

            return data;
        }

        public DocumentModel Deserialize(byte[] data)
        {
            if (data is null)
                return null;
            var model = new DocumentModel {Id = BufferHelper.ReadBufferGuid(data, 0)};


            var valueLength = BufferHelper.ReadBufferInt32(data, 16);
            if (valueLength < 0 || valueLength > 16*1024) 
            {
                throw new Exception ("Invalid string length: " + valueLength);
            }
            var strValue = System.Text.Encoding.UTF8.GetString (data, 16 + 4, valueLength);
            
            model.Value = JObject.Parse(strValue);

            return model;
        }
    }
}