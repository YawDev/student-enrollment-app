using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentEnrollment.Core;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Services
{
    public class ApiService : IApiService
    {

        private readonly IHttpClientService _httpClient;

        public ApiService(IHttpClientService httpClient)
        {
            _httpClient=httpClient;
        }

        public HttpResponseMessage DeleteResponse(string ApiUrl)
        {
            return _httpClient.Delete(ApiUrl).Result;
        }

        public string GetApiResultMessage(HttpResponseMessage response)
        {
            return _httpClient.GetResult(response);
        }

        public  T GetDeserializedObject<T>(HttpResponseMessage response)
        {
            return  _httpClient.GetDeserializedObject<T>(response);
        }

        public  HttpResponseMessage  GetResponse(string url)
        {
            return  _httpClient.Get(url);
        }

       
        public  HttpResponseMessage PostObjectResponse(string url, object Dto)
        {
            return  _httpClient.Post<object>(url, Dto);
        }

         public  HttpResponseMessage PostResponse(string url)
        {
            return  _httpClient.Post(url);
        }

        public async Task<HttpResponseMessage> PutResponse<T>(string ApiUrl, T Object)
        {
            return await _httpClient.Put<T>(ApiUrl, Object);
        }
    }
    
}
