using System;
using System.Collections.Generic;
using System.IO;

namespace Core.Storage.BinaryStorage
{
    public class BlockStorage : IBlockStorage
    {
        private readonly Stream _stream;
        private readonly int _blockSize;
        private readonly int _blockHeaderSize;
        private readonly int _blockContentSize;
        private readonly int _unitOfWork;
        private readonly Dictionary<uint, Block> _blocks = new Dictionary<uint, Block>();

        public int DiskSectorSize => _unitOfWork;
        public int BlockSize => _blockSize;
        public int BlockHeaderSize => _blockHeaderSize;
        public int BlockContentSize => _blockContentSize;
        
        public BlockStorage (Stream storage, int blockSize = 40960, int blockHeaderSize = 48)
        {
            if (blockHeaderSize >= blockSize) 
            {
                throw new ArgumentException ("blockHeaderSize cannot be " +
                                             "larger than or equal " +
                                             "to " + "blockSize");
            }

            if (blockSize < 128) 
            {
                throw new ArgumentException ("blockSize too small");
            }

            _unitOfWork = ((blockSize >= 4096) ? 4096 : 128);
            _blockSize = blockSize;
            _blockHeaderSize = blockHeaderSize;
            _blockContentSize = blockSize - blockHeaderSize;
            _stream = storage ?? throw new ArgumentNullException ("storage");
        }
        
        public IBlock Find (uint blockId)
        {
            // Check from initialized blocks
            if (_blocks.ContainsKey(blockId))
            {
                return _blocks[blockId];
            }

            // First, move to that block.
            // If there is no such block return NULL
            var blockPosition = blockId * _blockSize;
            if ((blockPosition + _blockSize) > _stream.Length)
            {
                return null;
            }

            // Read the first 4KB of the block to construct a block from it
            var firstSector = new byte[DiskSectorSize];
            _stream.Position = blockId * _blockSize;
            _stream.Read (firstSector, 0, DiskSectorSize);

            var block = new Block (this, blockId, firstSector, this._stream);
            OnBlockInitialized(block);
            return block;
        }
        
        public IBlock CreateNew ()
        {
            if ((_stream.Length % _blockSize) != 0) {
                throw new DataMisalignedException("Unexpected length of the stream: " + _stream.Length);
            }

            // Calculate new block id
            var blockId = (uint)Math.Ceiling ((double) _stream.Length / (double) _blockSize);

            // Extend length of underlying stream
            _stream.SetLength ((long)((blockId * _blockSize) + _blockSize));
            _stream.Flush ();

            // Return desired block
            var block = new Block(this, blockId, new byte[DiskSectorSize], _stream);
            OnBlockInitialized (block);
            return block;
        }
        
        protected virtual void OnBlockInitialized(Block block)
        {
            // Keep reference to it
            _blocks[block.Id] = block;

            // When block disposed, remove it from memory
            block.Disposed += HandleBlockDisposed;
        }

        protected virtual void HandleBlockDisposed (object sender, EventArgs e)
        {
            // Stop listening to it
            var block = (Block)sender;
            block.Disposed -= HandleBlockDisposed;

            // Remove it from memory
            _blocks.Remove (block.Id);
        }
    }
}