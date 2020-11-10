using System.Threading.Tasks;
using Core.Models;
using Core.Storage;

namespace SearchApi.Services
{
    public class ObjectCreatorFacade
    {
        private readonly CreateOperations _createOperations;
        private readonly IndexingOperations _indexingOperations;

        public ObjectCreatorFacade()
        {
            _createOperations = new CreateOperations();
            _indexingOperations = new IndexingOperations();
        }

        public async Task CreateIndexAndAddObject(IndexModel indexModel, object obj)
        {
            await _createOperations.CreateIndexAndAddObject(indexModel, obj);
            await _indexingOperations.SetIndexes(indexModel);
        }
    }
}