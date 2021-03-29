using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlexSearch.Panel.Exceptions;
using FlexSearch.Panel.Models.CoreModels;
using FlexSearch.Panel.Models.ViewModels;
using FlexSearch.Panel.Services.Contract;

namespace FlexSearch.Panel.Services.Impl
{
    public class IndexService : IIndexService
    {
        private string _url;
        private ApiRequest _apiRequest;

        public IndexService(ApiRequest apiRequest)
        {
            _apiRequest = apiRequest;
        }

        public void Initialize(AuthViewModel authViewModel)
        {
            _url = authViewModel.Host + "/index/";
            _apiRequest.SetHeaders(new Dictionary<string, string>()
            {
                {"User", authViewModel.Login},
                {"Password", authViewModel.Password}
            });
        }

        /// <summary>
        /// Создаёт объект. Если индекса ещё нет, создаст новый индекс
        /// </summary>
        /// <param name="idxName"></param>
        /// <param name="obj"></param>
        /// <param name="dbname"></param>
        /// <returns></returns>
        /// <exception cref="ApiException"></exception>
        public async Task<string> Create(string dbname, string idxName, object obj)
        {
            var uri = _url + $"{dbname}/{idxName}/";
            var response = await _apiRequest.SendPostAndParseString(uri, obj);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);

            return response.Content;
        }
        
        public async Task<List<DocumentModel>> GetAll(string dbname, string idxName)
        {
            var uri = _url + $"{dbname}/{idxName}/";
            var response = await _apiRequest.SendGetAndParseObject<List<DocumentModel>>(uri);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);
            return response.Content;
        }
        
        public async Task<DocumentModel> GetById(string dbname, string idxName, Guid id)
        {
            var uri = _url + $"{dbname}/{idxName}/{id}";
            var response = await _apiRequest.SendGetAndParseObject<DocumentModel>(uri);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);
            return response.Content;
        }

        public async Task Update(string dbname, string idxName, Guid id, object obj)
        {
            var uri = _url + $"{dbname}/{idxName}/{id}/";
            var response = await _apiRequest.SendPut(uri, obj);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);
        }

        public async Task DeleteItem(string dbname, string idxName, Guid id)
        {
            var uri = _url + $"{dbname}/{idxName}/{id}/";
            var response = await _apiRequest.SendDelete(uri);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);
        }

        public async Task<string[]> GetDbs()
        {
            var uri = _url.Replace("/index/", "");
            var response = await _apiRequest.SendGetAndParseObject<string[]>(uri);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);

            return response.Content;
        }

        public async Task<string[]> GetIndexes(string dbname)
        {
            var uri = _url + $"{dbname}";
            var response = await _apiRequest.SendGetAndParseObject<string[]>(uri);
            if(!response.IsSuccess)
                throw new ApiException(response.ErrorMessage);

            return response.Content;
        }
    }
}