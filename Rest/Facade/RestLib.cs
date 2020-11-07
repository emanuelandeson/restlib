using Newtonsoft.Json;
using Rest.Interfaces;
using Rest.Utils.MemoryOptmization;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rest.Facade
{
    public class RestLib : IRest
    {
        private static HttpClient Client = new HttpClient();

        public IDictionary<string, string> Header { get; private set; }

        public RestLib()
        { }

        public RestLib(IDictionary<string, string> header)
        {
            Header = header;
            AddHeder();
        }

        public async Task<T> GetAsync<T>(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await Client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
        }

        protected async Task<HttpResponseMessage> PatchPostPut<T>(string url, T content, HttpMethod method)
        {
            using (var request = new HttpRequestMessage(method, url))
            {
                var json = JsonConvert.SerializeObject(content);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();
                        return response;
                    }
                }
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T postContent)
        {
            return await this.PatchPostPutStream(url, postContent, HttpMethod.Post);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string url, T putContent)
        {
            return await this.PatchPostPutStream(url, putContent, HttpMethod.Put);
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string url, T pathContent)
        {
            return await this.PatchPostPutStream(url, pathContent, HttpMethod.Patch);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                      .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return response;
                }
            }
        }

        public async Task<T> GetStreamAsync<T>(string url)
        {
            var cancelToken = new CancellationTokenSource();
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                response.EnsureSuccessStatusCode();
                return StreamHandler.DeserializeJsonFromStream<T>(stream);
            }
        }

        private async Task<HttpResponseMessage> PatchPostPutStream<T>(string url, T content, HttpMethod method)
        {
            using (var request = new HttpRequestMessage(method, url))
            using (var httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;

                using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return response;
                }
            }
        }

        public async Task<HttpResponseMessage> PostStreamAsync<T>(string url, T content)
        {
            return await this.PatchPostPutStream(url, content, HttpMethod.Post);
        }

        public async Task<HttpResponseMessage> PatchStreamAsync<T>(string url, T pathContent)
        {
            return await this.PatchPostPutStream(url, pathContent, HttpMethod.Patch);
        }

        public async Task<HttpResponseMessage> PutStreamAsync<T>(string url, T putContent)
        {
            return await this.PatchPostPutStream(url, putContent, HttpMethod.Put);
        }

        private HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                StreamHandler.SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
            }

            return httpContent;
        }

        private void AddHeder()
        {
            Client.DefaultRequestHeaders.Clear();

            foreach (var prop in Header)
            {
                if (Header == null)
                    continue;

                var name = prop.Key;
                var value = prop.Value;
                Client.DefaultRequestHeaders.Add(name, value);
            }
        }
    }
}