using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Rest.Interfaces
{
    public interface IRest
    {
        Task<T> GetAsync<T>(string url);
        Task<T> GetStreamAsync<T>(string url);
        Task<HttpResponseMessage> PostAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PostStreamAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PutAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> PutStreamAsync<T>(string url, T putContent);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<HttpResponseMessage> PatchAsync<T>(string url, T pathContent);
        Task<HttpResponseMessage>  PatchStreamAsync<T>(string url, T pathContent);
    }
}