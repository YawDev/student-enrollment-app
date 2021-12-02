using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudentEnrollment.Services;

namespace StudentEnrollment.App.Services
{
    public class HttpClientService : IHttpClientService
    {
        private  HttpClient _client;

        public HttpClientService(HttpClient client)
        {
            _client = client;
        }
        
        

        public HttpResponseMessage Get(string ApiUrl)
        {
            try
            {
    
                var CompleteUrl = _client.BaseAddress+ApiUrl;
                var response =  _client.GetAsync(CompleteUrl).Result;
                return response;
            }
            catch(Exception ex)
            {
                throw ex;
            }
      
        }

        public HttpResponseMessage Post<T>(string ApiUrl, T Object)
        {
            var CompleteUrl = _client.BaseAddress+ApiUrl;
            
            var response = _client.PostAsJsonAsync<Object>(CompleteUrl, Object);
            response.Wait();
            return response.Result;
        }

        public string GetResult(HttpResponseMessage response)
        {
             var resultMessage = DeserializeResponse(response);
             return resultMessage;
        }

        public Task<HttpResponseMessage> Put<T>(string ApiUrl, T Object)
        {
            var CompleteUrl = _client.BaseAddress+ApiUrl;
            
            var response = _client.PutAsJsonAsync<Object>(CompleteUrl, Object);
            response.Wait();
            return response;
        }

        public Task<HttpResponseMessage> Delete(string ApiUrl)
        {
            var CompleteUrl = _client.BaseAddress+ApiUrl;
            
            var response = _client.DeleteAsync(CompleteUrl);
            response.Wait();
            return response;
        }

        

        public HttpClientHandler ClientHandler()
        {
            var handler = new HttpClientHandler() 
            { 
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                UseProxy=false,
                Proxy = null
            };
            return handler;
            
        }

        public  T GetDeserializedObject<T>(HttpResponseMessage response)
        {
            var responseContent =  response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(responseContent.Result.ToString());
            var DeserializeObject = JsonConvert.DeserializeObject<T>(jObject.SelectToken("result").ToString());
            return DeserializeObject;
        }

        private string DeserializeResponse(HttpResponseMessage response)
        {
            try
            {
                JArray jsonResult = new JArray();

                var responseJsonString = response.Content.ReadAsStringAsync();

                var jobj = JObject.Parse(responseJsonString.Result.ToString());

                var result = JsonConvert.DeserializeObject<Envelope>(jobj.ToString());

                if (result.ErrorMessage == null)
                    jsonResult = (JArray)JsonConvert.DeserializeObject(result.Result?.ToString());
                else
                    return result.ErrorMessage;
                
                return jsonResult.ToString();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public HttpResponseMessage Post(string url)
        {
            var CompleteUrl = _client.BaseAddress+url;
            
            var response = _client.PostAsync(CompleteUrl, null);
            response.Wait();
            return response.Result;
        }

        private class Envelope
        {
            public object Result { get; set; }
            public string ErrorMessage { get; set; }
            public DateTime TimeGenerated { get; set; }
        }
    }
}