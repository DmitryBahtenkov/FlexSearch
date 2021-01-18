using System;
using System.IO;
using Core.Models;

namespace Core.Storage.BinaryStorage
{
    public sealed class DocumentDatabase
    {
        private readonly Stream _dbFileStream;
        private readonly Stream _indexFileStream;
        private readonly Tree<Guid, uint> _index;

        private readonly RecordStorage _records;
        private readonly DocumentSerializer _documentSerializer;
        private bool _disposed = false;

        public DocumentDatabase(string pathToDb)
        {
            if (pathToDb == null)
                throw new ArgumentNullException(nameof(pathToDb));

            _dbFileStream = new FileStream(pathToDb + "/a.as", FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.None, 4096);
            _indexFileStream = new FileStream(pathToDb + "/a.pidx", FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.None, 4096);

            // Construct the RecordStorage that use to store main cow data
            _records = new RecordStorage(new BlockStorage(_dbFileStream, 4096));

            // Construct the primary and secondary indexes 
            _index = new Tree<Guid, uint>(
                new TreeDiskNodeManager<Guid, uint>(
                    new GuidSerializer(),
                    new TreeUIntSerializer(),
                    new RecordStorage(new BlockStorage(_indexFileStream, 4096))
                )
            );
            _documentSerializer = new DocumentSerializer();
        }

        public void Update(DocumentModel model)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(model));
            }

            var entry = _index.Get(model.Id);
            _records.Update(entry.Item2, _documentSerializer.Serialize(model));
        }

        public void Delete(Guid id)
        {
            var entry = _index.Get(id);
            _index.Delete(id);
            _records.Delete(entry.Item2);
        }

        /// <summary>
        /// Insert a new cow entry into our cow database
        /// </summary>
        public void Insert(DocumentModel model)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("CowDatabase");
            }

            // Serialize the cow and insert it
            var recordId = _records.Create(_documentSerializer.Serialize(model));

            // Primary index
            _index.Insert(model.Id, recordId);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DocumentModel Find(Guid id)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("CowDatabase");
            }

            // Look in the primary index for this cow
            var entry = _index.Get(id);
            if (entry == null)
            {
                return null;
            }

            return _documentSerializer.Deserialize(_records.Find(entry.Item2));
        }

        private bool _isDisposed;

        private void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed) return;
            _dbFileStream.Dispose();
            _indexFileStream.Dispose();
            _isDisposed = true;
        }

        ~DocumentDatabase()
        {
            Dispose(false);
        }

        #endregion
    }
}