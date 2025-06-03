using Telegram.Bot;
using Telegram.Bot.Types;
using VshghCoachBot.BotMessages;

namespace APIprot.Clients
{
    public class CommandHandler
    {
        public async Task Handle(ITelegramBotClient client, Update update)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                var botMessage = "";
                switch (message.Text.ToLower())
                {
                    case "/start":
                        botMessage = CommandsResponse.start;
                        break;
                    case "/help":
                        botMessage = CommandsResponse.help;
                        break;
                    case "/exercises":
                        botMessage = CommandsResponse.exercises;
                        break;
                }
                if (!string.IsNullOrEmpty(botMessage))
                {
                    await client.SendTextMessageAsync(message.Chat.Id, botMessage);
                }
            }
        }
    }
}
