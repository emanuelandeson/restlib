using Master.Berest.Interfaces;
using Master.Berest.Utils.Extesions;
using Master.Berest.Utils.MemoryOptmization;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Master.Berest.Facade
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

        private async Task<HttpResponseMessage> PatchPostPut<T>(string url, T content, HttpMethod method)
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
                        if (response.IsSuccessStatusCode)
                            return response;

                        var resultContent = await response.Content.ReadAsStringAsync();

                        throw new ApiException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = resultContent
                        };
                    }
                }
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<T>(content);

                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Content = content
                };
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
                    if (response.IsSuccessStatusCode)
                        return response;

                    var resultContent = await response.Content.ReadAsStringAsync();

                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = resultContent
                    };
                }
            }
        }

        public async Task<T> GetStreamAsync<T>(string url)
        {
            var cancelToken = new CancellationTokenSource();
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancelToken.Token))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return StreamHandler.DeserializeJsonFromStream<T>(stream);

                var content = await StreamHandler.StreamToStringAsync(stream);

                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Content = content
                };
            }
        }

        private async Task<HttpResponseMessage> PatchPostPutStream<T>(string url, T content, HttpMethod method)
        {
            using (var request = new HttpRequestMessage(method, url))
            using (var httpContent = CreateHttpStreamContent(content))
            {
                request.Content = httpContent;

                using (var response = await Client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                        return response;

                    var resultContent = await response.Content.ReadAsStringAsync();

                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = resultContent
                    };
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

        private HttpContent CreateHttpStreamContent(object content)
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