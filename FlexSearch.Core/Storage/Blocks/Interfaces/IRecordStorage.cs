using System;

namespace Core.Storage.Blocks.Interfaces
{
    public interface IRecordStorage
    {
        public void Update(uint recordId, byte[] data);
        
        public byte[] Find(uint recordId);

        public uint Create(byte[] data);
        
        public uint Create(Func<uint, byte[]> dataGenerator);
        
        public void Delete(uint recordId);
    }
}