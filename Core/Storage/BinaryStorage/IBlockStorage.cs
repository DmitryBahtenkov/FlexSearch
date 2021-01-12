namespace Core.Storage.BinaryStorage
{
    public interface IBlockStorage
    {
        /// <summary>
        /// Number of bytes of custom data per block that this storage can handle.
        /// </summary>
        public int BlockContentSize { get; }

        /// <summary>
        /// Total number of bytes in header
        /// </summary>
        public int BlockHeaderSize { get; }

        /// <summary>
        /// Total block size, equal to content size + header size, should be a multiple of 128B
        /// </summary>
        public int BlockSize { get; }

        /// <summary>
        /// Find a block by its id
        /// </summary>
        public IBlock Find(uint blockId);

        /// <summary>
        /// Allocate new block, extend the length of underlying storage
        /// </summary>
        public IBlock CreateNew();
    }
}