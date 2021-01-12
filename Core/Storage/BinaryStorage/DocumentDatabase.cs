using System;
using System.IO;
using Core.Models;

namespace Core.Storage.BinaryStorage
{
    public class DocumentDatabase
    {
        private readonly Stream _dbFileStream;
        private readonly Stream _indexFileStream;
        private readonly Tree<Guid, uint> _index;
        
        private readonly RecordStorage _records;
        private readonly DocumentSerializer _documentSerializer;
        private bool _disposed = false;
        
        public DocumentDatabase (string pathToDb)	
        {
            if (pathToDb == null)
                throw new ArgumentNullException (nameof(pathToDb));

            // As soon as CowDatabase is constructed, open the steam to talk to the underlying files
            _dbFileStream = new FileStream (pathToDb, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096);
            _indexFileStream = new FileStream (pathToDb + ".pidx", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 4096);

            // Construct the RecordStorage that use to store main cow data
            _records = new RecordStorage(new BlockStorage(_dbFileStream, 4096, 48));

            // Construct the primary and secondary indexes 
            _index = new Tree<Guid, uint> (
                new TreeDiskNodeManager<Guid, uint>(
                    new GuidSerializer(),
                    new TreeUIntSerializer(),
                    new RecordStorage(new BlockStorage(_indexFileStream, 4096))
                )
            );
        }
        
        public void Update (DocumentModel model)
        {
            if (_disposed) {
                throw new ObjectDisposedException("CowDatabase");
            }

            throw new NotImplementedException ();
        }

        /// <summary>
        /// Insert a new cow entry into our cow database
        /// </summary>
        public void Insert (DocumentModel model)
        {
            if (_disposed) {
                throw new ObjectDisposedException ("CowDatabase");
            }

            // Serialize the cow and insert it
            var recordId = this._records.Create (_documentSerializer.Serialize(model));

            // Primary index
           _index.Insert (model.Id, recordId);
        }
        
        #region Dispose
        public void Dispose ()
        {
            Dispose (true);
            GC.SuppressFinalize (this);
        }

        bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                _dbFileStream.Dispose ();
                _indexFileStream.Dispose();
                this.disposed = true;
            }
        }

        ~DocumentDatabase() 
        {
            Dispose (false);
        }
        #endregion
    }
}