using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace APIprot.Clients
{
    public class TelegramClient
    {
        private readonly Client _exerciseClient;
        private readonly CommandHandler _commandHandler;
        private readonly UserParameterHandler _userParameterHandler;
        private readonly SBDHandler _sbdHandler;
        private readonly ExerciseHandler _exerciseHandler;

        public TelegramClient(Client client, HttpClient httpClient)
        {
            _exerciseClient = client;
            _commandHandler = new CommandHandler();
            _userParameterHandler = new UserParameterHandler(client);
            _sbdHandler = new SBDHandler(httpClient);
            _exerciseHandler = new ExerciseHandler(client);
        }

        public async Task Start(string token)
        {
            var botClient = new TelegramBotClient(token);
            botClient.StartReceiving(Update, Error);
            await Task.Delay(Timeout.Infinite);
        }

        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Message != null)
            {
                await _exerciseHandler.Handle(client, update);
            }
        }

        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Error: {exception.Message}");
            await Task.CompletedTask;
        }
    }
}




