using APIprot.Clients;
using APIprot;
using System.Net.Http;

class Program
{
    static readonly Client _client = new Client();
    static readonly HttpClient _httpClient = new HttpClient();

    static async Task Main(string[] args)
    {
        _httpClient.BaseAddress = new Uri(Constant.Address);
        var telegramClient = new TelegramClient(_client, _httpClient);
        await telegramClient.Start(Constant.Token);
    }
}









