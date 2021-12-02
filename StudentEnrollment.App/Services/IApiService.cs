using System.Net.Http;
using System.Threading.Tasks;

namespace StudentEnrollment.App.Services
{
    public interface IApiService
    {
        HttpResponseMessage GetResponse(string url);
       

        HttpResponseMessage PostObjectResponse(string url, object Dto);
        HttpResponseMessage PostResponse(string url);

        T GetDeserializedObject<T>(HttpResponseMessage response);

        string GetApiResultMessage(HttpResponseMessage response);

        Task<HttpResponseMessage> PutResponse<T>(string ApiUrl, T Object);

        HttpResponseMessage DeleteResponse(string ApiUrl);
        
    }
}