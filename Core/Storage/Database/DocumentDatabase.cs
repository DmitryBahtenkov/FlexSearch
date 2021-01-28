using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage.BinaryStorage;
using Core.Storage.Serializers;
using Core.Storage.Tree;

namespace Core.Storage.Database
{
    public sealed class DocumentDatabase : IDisposable
    {
        private readonly Stream _dbFileStream;
        private readonly Stream _indexFileStream;

        private readonly Tree<Guid, uint> _index;
        private readonly Tree<string, Dictionary<string, Guid>> _secondaryIndex;
        private readonly IndexingOperations _indexingOperations;

        private readonly RecordStorage _records;
        private readonly DocumentSerializer _documentSerializer;
        private bool _disposed = false;

        public DocumentDatabase(IndexModel indexModel) 
        {
            var path = $"{AppDomain.CurrentDomain.BaseDirectory}data/{indexModel.DatabaseName}";
            FileOperations.CheckOrCreateDirectory(path);
            path += $"/{indexModel.IndexName}";
            _dbFileStream = new FileStream(path + ".col", FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.None, 4096);
            _indexFileStream = new FileStream(path + ".pidx", FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.None, 4096);
            Stream secondaryFileStream = new FileStream(path + ".sidx", FileMode.OpenOrCreate, FileAccess.ReadWrite,
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
            
            _secondaryIndex = new Tree<string, Dictionary<string, Guid>>(
                new TreeDiskNodeManager<string, Dictionary<string, Guid>>(
                    new StringSerializer(),
                    new DictSerializer(),
                    new RecordStorage(
                        new BlockStorage(secondaryFileStream, 4096)
                        )
                    ),
                true);
            
            _documentSerializer = new DocumentSerializer();
            _indexingOperations = new IndexingOperations();
        }
        
        
        public async Task Update(DocumentModel model)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(model));
            }

            var entry = await _index.Get(model.Id);
            var oldModel = _documentSerializer.Deserialize(_records.Find(entry.Item2));
            _records.Update(entry.Item2, _documentSerializer.Serialize(model));

            var oldDict = _indexingOperations.CreateIndexes(oldModel.Value, oldModel).Result;
            foreach (var (k, v) in oldDict)
            {
                await _secondaryIndex.Delete(k, v);
            }
            
            var dict = await _indexingOperations.CreateIndexes(model.Value, model);
            foreach (var (k, v) in dict)
            {
               await _secondaryIndex.Insert(k, v);
            }
        }

        public async Task Delete(DocumentModel model)
        {
            var entry = await _index.Get(model.Id);
            var dict = await _indexingOperations.CreateIndexes(model.Value, model);
            
            //var keys = model.Value.To;
            await _index.Delete(model.Id);
            _records.Delete(entry.Item2);

            foreach (var (k, v) in dict)
            {
                await _secondaryIndex.Delete(k, v);
            }
        }

        /// <summary>
        /// Insert a new cow entry into our cow database
        /// </summary>
        public async Task Insert(DocumentModel model)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(DocumentDatabase));
            }

            // Serialize the cow and insert it
            var recordId = _records.Create(_documentSerializer.Serialize(model));

            // Primary index
            await _index.Insert(model.Id, recordId);

            var dict = await _indexingOperations.CreateIndexes(model.Value, model);
            foreach (var (k, v) in dict)
            {
                await _secondaryIndex.Insert(k, v);
            }
            
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<DocumentModel> Find(Guid id)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("CowDatabase");
            }

            // Look in the primary index for this cow
            var entry = await _index.Get(id);
            if (entry == null)
            {
                return null;
            }

            return _documentSerializer.Deserialize(_records.Find(entry.Item2));
        }

        public async Task<List<Dictionary<string, Guid>>> GetIndexes(string key)
        {
            return (await _secondaryIndex
                .LargerThanOrEqualTo(key))
                .Where(x => x.Item1.Contains(key))
                .Select(x => x.Item2)
                .ToList();
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