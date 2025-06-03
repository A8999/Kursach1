using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace APIprot.Clients
{
    public class SBDHandler
    {
        private readonly HttpClient _client;

        public SBDHandler(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("http://localhost:5100");
        }

        public async Task Handle(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message;
            if (message?.Text == null) return;

            var text = message.Text.ToLower();
            string botMessage = "";

            if (text.StartsWith("/sbdset"))
            {
                string[] parts = text.Split(' ');
                if (parts.Length != 4 || !double.TryParse(parts[1], out double squat) ||
                    !double.TryParse(parts[2], out double bench) ||
                    !double.TryParse(parts[3], out double deadlift))
                {
                    botMessage = "Формат команди:\n/sbdset [присідання] [жим] [тяга]";
                }
                else
                {
                    var userParamsResp = await _client.GetAsync($"/api/userparameters/{message.Chat.Id}");
                    if (!userParamsResp.IsSuccessStatusCode)
                    {
                        botMessage = "Спочатку задайте свої параметри через /set [зріст] [вага].";
                    }
                    else
                    {
                        var payload = new { userId = message.Chat.Id, squat, bench, deadlift };
                        var json = JsonConvert.SerializeObject(payload);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await _client.PostAsync("/api/sbd", content);
                        if (response.IsSuccessStatusCode)
                        {
                            botMessage = "Ваші SBD-результати збережено.";
                        }
                        else
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            if (string.IsNullOrWhiteSpace(error))
                                error = $"Код статусу: {(int)response.StatusCode} {response.ReasonPhrase}";
                            botMessage = $"Помилка при збереженні SBD. Деталі: {error}";
                        }
                    }
                }
            }
            else if (text == "/sbd")
            {
                var response = await _client.GetAsync($"/api/sbd/{message.Chat.Id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(json);
                    double total = result.squat + result.bench + result.deadlift;
                    botMessage = $"Ваші SBD результати:\nПрисідання: {result.squat} кг\nЖим: {result.bench} кг\nТяга: {result.deadlift} кг\nЗагалом: {total} кг";
                }
                else
                {
                    botMessage = "Дані SBD не знайдено.";
                }
            }
            else if (text == "/deletesbd")
            {
                var response = await _client.DeleteAsync($"/api/sbd/{message.Chat.Id}");
                if (response.IsSuccessStatusCode)
                {
                    botMessage = "Ваші SBD-результати видалено.";
                }
                else
                {
                    botMessage = "Помилка при видаленні SBD.";
                }
            }

            if (!string.IsNullOrEmpty(botMessage))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, botMessage);
            }
        }
    }
}
