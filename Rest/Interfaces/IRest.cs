using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Master.Berest.Interfaces
{
    public interface IRest
    {
        Task<T> GetAsync<T>(string url);
        Task<T> GetStreamAsync<T>(string url);
        Task<HttpResponseMessage> PostAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PutAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PatchAsync<T>(string url, T pathContent);
        Task<HttpResponseMessage> PostStreamAsync<T>(string url, T putContent);
        Task<T> PostResponseReadStreamAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PutStreamAsync<T>(string url, T putContent);
        Task<T> PutResponseReadStreamAsync<T>(string url, T putContent);
        Task<HttpResponseMessage>  PatchStreamAsync<T>(string url, T pathContent);
        Task<T> PatchResponseReadStreamAsync<T>(string url, T pathContent);
        Task<HttpResponseMessage> DeleteAsync(string url);
    }
}