using Telegram.Bot;
using Telegram.Bot.Types;
using APIprot.Clients;
namespace APIprot.Clients
{
    public class UserParameterHandler
    {
        private readonly Client _client;

        public UserParameterHandler(Client client)
        {
            _client = client;
        }
        public async Task Handle(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message;
            if (message?.Text == null) return;
            var text = message.Text.ToLower();
            string botMessage = "";
            if (text.StartsWith("/set"))
            {
                string[] parts = text.Split(' ');
                if (parts.Length != 3 || !double.TryParse(parts[1], out double height) || !double.TryParse(parts[2], out double weight))
                {
                    botMessage = "Правильний формат:\n/set [зріст] [вага]\nНаприклад: /set 180 75";
                }
                else
                {
                    await _client.SetUserParameters(message.Chat.Id, height, weight);
                    botMessage = "Ваші параметри збережено.";
                }
            }
            else if (text == "/bmi")
            {
                var parameters = await _client.GetUserParameters(message.Chat.Id);
                if (parameters == null)
                {
                    botMessage = "Параметри не знайдено. Введіть їх через команду /set.";
                }
                else
                {
                    double height = parameters.Value.height;
                    double weight = parameters.Value.weight;
                    double bmi = weight / ((height / 100) * (height / 100));
                    string category = bmi < 18.5 ? "Надмірна худорлявість" :
                                     bmi < 25 ? "Норма" :
                                     bmi < 30 ? "Надмірна вага" : "Ожиріння";
                    botMessage = $"Індекс маси тіла (BMI): {bmi:F2} — {category}";
                }
            }
            else if (text == "/deleteparameters")
            {
                await _client.DeleteUserParameters(message.Chat.Id);
                botMessage = "Ваші параметри були видалені.";
            }
            if (!string.IsNullOrEmpty(botMessage))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, botMessage);
            }
        }
    }
}


