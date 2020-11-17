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
            var rest = new RestLib(new Dictionary<string, string>
            {
                {"Country","BR"},
                {"City","SP"},
            });


            //var getStream = await rest.GetStreamAsync<object>("http://localhost:3000/get");
            var postStream = await rest.PostStreamAsync("https://tryer.free.beeceptor.com/", new
            {
                items = "$mockData",
                count = "$count",
                anyKey = "anyValue"
            });


            var post = await rest.PostAsync("https://tryer.free.beeceptor.com/", new
            {
                items = "$mockData",
                count = "$count",
                anyKey = "anyValue"
            });

            var putStream = await rest.PutStreamAsync("https://tryer.free.beeceptor.com/", new { Teste = "TEste" });
            var patchStream = await rest.PatchStreamAsync("https://tryer.free.beeceptor.com/", new { Teste = "TEste" });

            //var post = await rest.PostAsync("https://tryer.free.beeceptor.com/", new { Teste = "TEste" });
            //var put = await rest.PutAsync("https://tryer.free.beeceptor.com/", new { Teste = "TEste" });
            //var patch = await rest.PatchAsync("https://tryer.free.beeceptor.com/", new { Teste = "TEste" });
            //var delete = await rest.DeleteAsync("https://tryer.free.beeceptor.com/");

            int warm = 15;
            int maxLoop = 1;

            //warm request
            await BenchmarkHelper.BenchAsync(GetBasic, warm, nameof(Program.GetBasic), null, CancellationToken.None);

            try
            {
                //This is called by RestSharp Lib to compare performance with our lib...
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
            var client = new RestClient("https://tryer.free.beeceptor.com/");
            var request = new RestRequest("https://tryer.free.beeceptor.com/", DataFormat.Json);

            var response = await client.GetAsync<object>(request);
        }


        public static async Task GetBasic()
        {
            var rest = new RestLib(new Dictionary<string, string>
            {
                {"Country","BR"},
                {"City","SP"},
            });

            await rest.GetAsync<object>("https://tryer.free.beeceptor.com/");
        }

        public static async Task GetBasicStream()
        {
            var rest = new RestLib();
            await rest.GetStreamAsync<object>("https://tryer.free.beeceptor.com/");
        }
    }
}
