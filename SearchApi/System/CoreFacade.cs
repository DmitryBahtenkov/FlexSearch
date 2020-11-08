using System;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;

namespace SearchApi.System
{
    public class CoreFacade
    {
        private readonly CreateIndexCommand _createIndexCommand;
        private readonly AddObjectToIndexCommand _addObjectToIndexCommand;
        private readonly IndexingOperations _indexingOperations;
        private readonly GetDocumentsCommand _getDocumentsCommand;

        public CoreFacade()
        {
            _createIndexCommand = new CreateIndexCommand();
            _addObjectToIndexCommand = new AddObjectToIndexCommand();
            _indexingOperations = new IndexingOperations();
            _getDocumentsCommand = new GetDocumentsCommand();
        }

        public async Task<bool> CreateAll(IndexModel indexModel, object obj)
        {
            var raw = obj.ToString();
            try
            {
                await _createIndexCommand.CreateIndex(indexModel);
                await _addObjectToIndexCommand.Add(indexModel, raw);
                await _indexingOperations.SetIndexes(indexModel);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}