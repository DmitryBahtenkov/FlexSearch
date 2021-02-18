namespace Core.Storage.Blocks.Interfaces
{
    public interface IBlockStorage
    {
        public int BlockContentSize { get; }
        
        public int BlockHeaderSize { get; }
        public int DiskSectorSize { get; }
        
        public int BlockSize { get; }
        
        public IBlock Find(uint blockId);
        
        public IBlock CreateNew();
    }
}