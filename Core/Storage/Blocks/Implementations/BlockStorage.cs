using System;
using System.Collections.Generic;
using System.IO;
using Core.Storage.Blocks.Implementations;
using Core.Storage.Blocks.Interfaces;

namespace Core.Storage.BinaryStorage
{
    public sealed class BlockStorage : IBlockStorage
    {
        private readonly Stream _stream;
        private readonly Dictionary<uint, Block> _blocks = new();

        public int DiskSectorSize { get; }

        public int BlockSize { get; }

        public int BlockHeaderSize { get; }

        public int BlockContentSize { get; }

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

            DiskSectorSize = blockSize >= 4096 ? 4096 : 128;
            BlockSize = blockSize;
            BlockHeaderSize = blockHeaderSize;
            BlockContentSize = blockSize - blockHeaderSize;
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
            var blockPosition = blockId * BlockSize;
            if (blockPosition + BlockSize > _stream.Length)
            {
                return null;
            }

            // Read the first 4KB of the block to construct a block from it
            var firstSector = new byte[DiskSectorSize];
            _stream.Position = blockId * BlockSize;
            _stream.Read(firstSector, 0, DiskSectorSize);

            var block = new Block(this, blockId, firstSector, this._stream);
            OnBlockInitialized(block);
            return block;
        }
        
        public IBlock CreateNew ()
        {
            if (_stream.Length % BlockSize != 0) 
            {
                throw new DataMisalignedException("Unexpected length of the stream: " + _stream.Length);
            }

            // Calculate new block id
            var blockId = (uint)Math.Ceiling(_stream.Length / (double) BlockSize);

            // Extend length of underlying stream
            _stream.SetLength(blockId * BlockSize + BlockSize);
            _stream.Flush ();

            // Return desired block
            var block = new Block(this, blockId, new byte[DiskSectorSize], _stream);
            OnBlockInitialized(block);
            return block;
        }

        private void OnBlockInitialized(Block block)
        {
            // Keep reference to it
            _blocks[block.Id] = block;

            // When block disposed, remove it from memory
            block.Disposed += HandleBlockDisposed;
        }

        private void HandleBlockDisposed(object sender, EventArgs e)
        {
            // Stop listening to it
            var block = (Block)sender;
            block.Disposed -= HandleBlockDisposed;

            // Remove it from memory
            _blocks.Remove(block.Id);
        }
    }
}