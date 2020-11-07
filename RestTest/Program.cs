using Master.Berest.Facade;
using RestSharp;
using RestTest.Helper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RestTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Teste().Wait();
        }

        public static async Task Teste()
        {
            var rest = new RestLib();

            //var a = await rest.GetAsync<Rootobject>("https://www.receitaws.com.br/v1/cnpj/27865757000102");

            //var getStream = await rest.GetStreamAsync<object>("http://localhost:3000/get");
            //var postStream = await rest.PostStreamAsync("http://localhost:3000/post", new { Teste = "TEste" });
            //var putStream = await rest.PutStreamAsync("http://localhost:3000/put", new { Teste = "TEste" });
            //var patchStream = await rest.PatchStreamAsync("http://localhost:3000/patch", new { Teste = "TEste" });

            //var post = await rest.PostAsync("http://localhost:3000/post", new { Teste = "TEste" });
            //var put = await rest.PutAsync("http://localhost:3000/put", new { Teste = "TEste" });
            //var patch = await rest.PatchAsync("http://localhost:3000/patch", new { Teste = "TEste" });

            //var delete = await rest.DeleteAsync("http://localhost:3000/delete");

            //await rest.PutAsync("teste", obj);

            int warm = 15;
            int maxLoop = 1;

            //warm request
            await BenchmarkHelper.BenchAsync(GetBasic, warm, nameof(Program.GetBasic), null, CancellationToken.None);

            try
            {
                await BenchmarkHelper.BenchAsync(GetBasicRestSharp, maxLoop, nameof(Program.GetBasicRestSharp), null, CancellationToken.None);
                await BenchmarkHelper.BenchAsync(GetBasicStream, maxLoop, nameof(Program.GetBasicStream), null, CancellationToken.None);
                await BenchmarkHelper.BenchAsync(GetBasic, maxLoop, nameof(Program.GetBasic), null, CancellationToken.None);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            
        }

        public static async Task GetBasicRestSharp()
        {
            var client = new RestClient("https://api.twitter.com/1.1");
            var request = new RestRequest("https://restcountries.eu/rest/v2/", DataFormat.Json);

            var response = await client.GetAsync<object>(request);
        }


        public static async Task GetBasic()
        {
            var rest = new RestLib(new Dictionary<string, string>
            {
                {"Country","BR"},
                {"City","SP"},
            });

            await rest.GetAsync<object>("https://restcountries.eu/rest/v2/");
        }

        public static async Task GetBasicStream()
        {
            var rest = new RestLib();
            await rest.GetStreamAsync<object>("https://restcountries.eu/rest/v2/all");
        }
    }
}
