using System;

namespace Core.Storage.Blocks.Interfaces
{
    public interface IBlock : IDisposable
    {
        public uint Id { get; }
        
        public long GetHeader(int field);
        
        public void SetHeader(int field, long value);
        
        public void Read(byte[] dst, int dstOffset, int srcOffset, int count);
        
        public void Write(byte[] src, int srcOffset, int dstOffset, int count);
    }
}