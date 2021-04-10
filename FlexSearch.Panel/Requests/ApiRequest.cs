using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FlexSearch.Panel.Models.CoreModels;
using Newtonsoft.Json;

namespace FlexSearch.Panel
{
    public class ApiRequest
    {
        private readonly HttpClient _httpClient;
        private  Dictionary<string, string> _headers;



        public ApiRequest(bool ignoreSsl = true, Dictionary<string, string> headers = null)
        {
            if (ignoreSsl)
            {
                var clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
                _httpClient = new HttpClient(clientHandler);
            }
            else
            {
                _httpClient = new HttpClient();
            }

            SetHeaders(headers);
        }
        
        public async Task<ResponseModel<string>> SendGetAndParseString(string uri)
        {
            
            var response = await _httpClient.GetAsync(uri);
            return response.IsSuccessStatusCode
                ? new ResponseModel<string>(content: await response.Content.ReadAsStringAsync())
                : new ResponseModel<string>($"Request return not OK status: {response.StatusCode}");
        }
        
        public async Task<ResponseModel<DocumentModel>> SendGetAndParseObject<DocumentModel>(string uri) where DocumentModel : class
        {
            var response = await _httpClient.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                return new ResponseModel<DocumentModel>($"Request return not OK status code: {response.StatusCode}");
            }

            var str = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(str)) 
                return new ResponseModel<DocumentModel>((DocumentModel)null);
            
            var obj = JsonConvert.DeserializeObject<DocumentModel>(str);
            return new ResponseModel<DocumentModel>(obj);
        }
        
        public async Task<ResponseModel<string>> SendPostAndParseString<T>(string uri, T content)
        {
            var response = await _httpClient.PostAsync(
                uri, 
                new StringContent(
                    JsonConvert.SerializeObject(content), 
                    Encoding.Default, 
                    "application/json")
                );
            
            return response.IsSuccessStatusCode
                ? new ResponseModel<string>(content: await response.Content.ReadAsStringAsync())
                : new ResponseModel<string>($"Request return not OK status: {response.StatusCode}");
        }
        
        public async Task<ResponseModel<TResult>> SendGetWithJsonAndParseObject<TItem, TResult>(string uri, TItem item)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Content =  new StringContent(JsonConvert.SerializeObject(item), Encoding.Default, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return new ResponseModel<TResult>($"Request return not OK status code: {response.StatusCode}");
            }
            
            var result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            return new ResponseModel<TResult>(result);
        }
        
        public async Task<ResponseModel<string>> SendPut<T>(string uri, T item)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content =  new StringContent(JsonConvert.SerializeObject(item), Encoding.Default, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            return !response.IsSuccessStatusCode 
                ? new ResponseModel<string>(errorMessage: $"Request return not OK status code: {response.StatusCode}") 
                : new ResponseModel<string>(content:"Ok");
        }

        public async Task<ResponseModel<string>> SendDelete(string uri)
        {
            var response = await _httpClient.DeleteAsync(uri);
            return !response.IsSuccessStatusCode 
                ? new ResponseModel<string>(errorMessage: $"Request return not OK status code: {response.StatusCode}") 
                : new ResponseModel<string>(content:"Ok");
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            if(headers is null)
                return;
            
            _headers = headers;
            foreach (var (key, value) in _headers)
            {
                if (!_httpClient.DefaultRequestHeaders.Contains(key))
                {
                    _httpClient.DefaultRequestHeaders.Add(key, value);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Remove(key);
                    _httpClient.DefaultRequestHeaders.Add(key, value);        
                }
            }
        }
    }
}