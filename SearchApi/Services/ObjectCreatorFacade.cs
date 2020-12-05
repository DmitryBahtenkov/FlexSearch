using System.Threading.Tasks;
using Core.Models;
using Core.Storage;

namespace SearchApi.Services
{
    public class ObjectCreatorFacade
    {
        private readonly CreateOperations _createOperations;
        private readonly IndexingOperations _indexingOperations;
        private readonly UpdateOperations _updateOperations;

        public ObjectCreatorFacade()
        {
            _createOperations = new CreateOperations();
            _indexingOperations = new IndexingOperations();
            _updateOperations = new UpdateOperations();
        }

        public async Task CreateIndexAndAddObject(IndexModel indexModel, object obj)
        {
            await _createOperations.CreateIndexAndAddObject(indexModel, obj);
            await _indexingOperations.SetIndexes(indexModel);
        }

        public async Task UpdateObjectAndIndexing(IndexModel indexModel, string id, object obj)
        {
            await _updateOperations.UpdateObject(indexModel, id, obj);
            await _indexingOperations.SetIndexes(indexModel);
        }
    }
}