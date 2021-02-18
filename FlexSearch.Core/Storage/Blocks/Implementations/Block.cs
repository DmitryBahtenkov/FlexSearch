using System;
using System.IO;
using Core.Storage.Blocks.Interfaces;
using Core.Storage.Helpers;

namespace Core.Storage.Blocks.Implementations
{
    public class Block : IBlock
    {
        private readonly byte[] _firstSector;
        private readonly long?[] _cachedHeaderValue = new long?[5];
        private readonly Stream _stream;
        private readonly IBlockStorage _storage;

        private bool _isFirstSectorDirty;
        private bool _isDisposed;
        
        public event EventHandler Disposed;
        public uint Id { get; }

        public Block(IBlockStorage storage, uint id, byte[] firstSector, Stream stream)
        {
            if(firstSector == null)
                throw new ArgumentNullException(nameof(firstSector));

            if (firstSector.Length != storage.DiskSectorSize)
                throw new ArgumentException("firstSector length must be " + storage.DiskSectorSize);

            _storage = storage;
            Id = id;
            _stream = stream ?? throw new ArgumentNullException (nameof(stream));
            _firstSector = firstSector;
        }
        
        public long GetHeader(int field)
        {
            if (_isDisposed) 
            {
                throw new ObjectDisposedException("Block");
            }

            // Validate field number
            if (field < 0) 
            {
                throw new IndexOutOfRangeException();
            }
            if (field >= _storage.BlockHeaderSize/8) 
            {
                throw new ArgumentException("Invalid field: " + field);
            }

            // Check from cache, if it is there then return it
            if (field < _cachedHeaderValue.Length)
            {
                _cachedHeaderValue[field] ??= BufferHelper.ReadBufferInt64(_firstSector, field * 8);
                return (long)_cachedHeaderValue[field];
            }

            return BufferHelper.ReadBufferInt64(_firstSector, field * 8);
        }
        
        public void SetHeader(int field, long value)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Block");
            }

            if (field < 0)
            {
                throw new IndexOutOfRangeException();
            }

            // Update cache if this field is cached
            if (field < _cachedHeaderValue.Length) 
            {
                _cachedHeaderValue[field] = value;
            }

            // Write in cached buffer
            BufferHelper.WriteBuffer (value, _firstSector, field * 8);
            _isFirstSectorDirty = true;
        }
        
        public void Read(byte[] dst, int dstOffset, int srcOffset, int count)
        {
            if (_isDisposed) 
            {
                throw new ObjectDisposedException("Block");
            }

            // Validate argument
            if (false == (count >= 0 && count + srcOffset <= _storage.BlockContentSize)) 
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Requested count is outside of src bounds: Count=" + count);
            }

            if (false == count + dstOffset <= dst.Length) 
            {
                throw new ArgumentOutOfRangeException ("Requested count is outside of dest bounds: Count=" + count);
            }
            
            var dataCopied = 0;
            var copyFromFirstSector = _storage.BlockHeaderSize + srcOffset < _storage.DiskSectorSize;
            if (copyFromFirstSector)
            {
                var tobeCopied = Math.Min(_storage.DiskSectorSize -_storage.BlockHeaderSize - srcOffset, count);

                Buffer.BlockCopy (_firstSector, _storage.BlockHeaderSize + srcOffset, dst, dstOffset, tobeCopied);

                dataCopied += tobeCopied;
            }
            
            if (dataCopied < count) 
            {
                if (copyFromFirstSector) 
                {
                    _stream.Position = Id * _storage.BlockSize + _storage.DiskSectorSize;
                } 
                else 
                {
                    _stream.Position = Id * _storage.BlockSize + _storage.BlockHeaderSize + srcOffset;
                }
            }
            
            while (dataCopied < count)
            {
                var bytesToRead = Math.Min(_storage.DiskSectorSize, count - dataCopied);
                var thisRead = _stream.Read(dst, dstOffset + dataCopied, bytesToRead);
                if (thisRead == 0) 
                {
                    throw new EndOfStreamException();
                }
                dataCopied += thisRead;
            }
        }
        
        public void Write(byte[] src, int srcOffset, int dstOffset, int count)
        {
            if (_isDisposed) 
            {
                throw new ObjectDisposedException("Block");
            }

            // Validate argument
            if (false == (dstOffset >= 0 && dstOffset + count <= _storage.BlockContentSize)) 
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count argument is outside of dest bounds: Count=" + count);
            }

            if (false == (srcOffset >= 0 && srcOffset + count <= src.Length)) 
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count argument is outside of src bounds: Count=" + count);
            }

            // Write bytes that belong to the firstSector
            if(_storage.BlockHeaderSize + dstOffset < _storage.DiskSectorSize) 
            {
                var thisWrite = Math.Min (count, _storage.DiskSectorSize -_storage.BlockHeaderSize -dstOffset);
                Buffer.BlockCopy (src, srcOffset, _firstSector, _storage.BlockHeaderSize + dstOffset, thisWrite);
                _isFirstSectorDirty = true;
            }

            // Write bytes that do not belong to the firstSector
            if(_storage.BlockHeaderSize + dstOffset + count > _storage.DiskSectorSize)
            {
                // Move underlying stream to correct position ready for writting
                _stream.Position = Id * _storage.BlockSize 
                                       + Math.Max (_storage.DiskSectorSize, _storage.BlockHeaderSize + dstOffset);

                // Exclude bytes that have been written to the first sector
                var d = _storage.DiskSectorSize - (_storage.BlockHeaderSize + dstOffset);
                if (d > 0) {
                    dstOffset += d;
                    srcOffset += d;
                    count -= d;
                }
                
                var written = 0;
                while (written < count)
                {
                    var bytesToWrite = Math.Min(4096, count - written);
                    _stream.Write(src, srcOffset + written, bytesToWrite);
                    _stream.Flush();
                    written += bytesToWrite;
                }
            }
        }
        
        public override string ToString ()
        {
            return $"[Block: Id={Id}, ContentLength={GetHeader(2)}, Prev={GetHeader(3)}, Next={GetHeader(0)}]";
        }
        
        protected virtual void OnDisposed (EventArgs e)
        {
            Disposed?.Invoke(this, e);
        }

        //
        // Dispose
        //
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed) return;
            _isDisposed = true;

            if (_isFirstSectorDirty)
            {
                _stream.Position = Id * _storage.BlockSize;
                _stream.Write(_firstSector, 0, 4096);
                _stream.Flush();
                _isFirstSectorDirty = false;
            }

            OnDisposed(EventArgs.Empty);
        }

        ~Block()
        {
            Dispose(false);
        }
    }
}