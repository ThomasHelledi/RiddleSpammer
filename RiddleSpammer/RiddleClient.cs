using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RiddleSpammer
{
    public class RiddleClient : IDisposable
    {
        protected readonly int RiddleId;
        protected readonly int OptionId;

        private readonly HttpMessageHandler httpMessageHandler;
        private readonly HttpClient httpClient;
        private bool active;
        private Thread thread;

        public int Sent { get; private set; }

        public RiddleClient(int riddleId, int optionId)
        {
            RiddleId = riddleId;
            OptionId = optionId;

            httpMessageHandler = new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            httpClient = new HttpClient(httpMessageHandler);
            httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");

            active = true;
        }

        public async Task Start()
        {
            thread = new Thread(() =>
            {
                while (active)
                {
                    try
                    {
                        Tick().Wait();
                    }
                    catch(Exception ex)
                    {
                        var intialColor = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ForegroundColor = intialColor;
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        const string URL = "https://www.riddle.com/embed/stats/enqueue";
        protected async Task Tick()
        {
            StringContent content;
            HttpResponseMessage result;

            result = await httpClient.GetAsync($"https://www.riddle.com/a/{RiddleId}?");

            result.EnsureSuccessStatusCode();

            content = new StringContent(@"{""riddleId"":" + RiddleId + @",""data"":""" + RiddleId + @".start"",""event"":""start""}", Encoding.UTF8, "application/json");
            result = await httpClient.PostAsync(URL, content);

            result.EnsureSuccessStatusCode();

            content = new StringContent(@"{""riddleId"":" + RiddleId + @",""data"":""" + RiddleId + @".finish"",""event"":""finish""}", Encoding.UTF8, "application/json");
            result = await httpClient.PostAsync(URL, content);

            result.EnsureSuccessStatusCode();

            content = new StringContent(@"{""riddleId"":" + RiddleId + @",""data"":""" + RiddleId + @"." + OptionId + @""",""event"":""answer""}", Encoding.UTF8, "application/json");
            result = await httpClient.PostAsync(URL, content);

            result.EnsureSuccessStatusCode();

            Sent++;
        }

        public void Dispose()
        {
            this.active = true;
        }
    }
}
