using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace Dumbify_CSharp
{
	class Program
	{
		static async Task Main(string[] args)
		{
			/*Si estamos depurando en visual studio, tenemos que cambiar la ruta relativa en PC
			* para que funcione igual que en el contenedor de Docker*/
			if (Environment.GetEnvironmentVariable("PLATFORM_PC") != null)
			{
				Console.WriteLine("Estamos en PC");
				Directory.SetCurrentDirectory("./../../..");
			}
			else
			{
				Console.WriteLine("Estamos en Docker");
			}

			string token = "";
			try
			{
				token = System.IO.File.ReadAllText("assets/token.txt");
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("No se ha encontrado el archivo token.txt en la raíz del proyecto.");
				Environment.Exit(-1);
			}
			var botClient = new TelegramBotClient(token);
			var me = await botClient.GetMeAsync();
			Console.WriteLine("Hello World! I am user " + me.Id + " and my name is " + me.FirstName);

			using var cts = new CancellationTokenSource();

			botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync), cts.Token);

			Console.WriteLine($"Start listening for @{me.Username}");
            while (true)
            {
				Console.WriteLine("Listening...");
            }

			cts.Cancel();
		}

		static string DumbifyText(in string text)
		{
			string newText = "";
			var rand = new Random();
			for (int i = 0; i < text.Length; i++)
			{
				int res = rand.Next(2);
				if (res == 0) newText = newText + Char.ToUpper(text[i]);
				else newText = newText + Char.ToLower(text[i]);
			}
			return newText;
		}

		static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			var handler = update.Type switch
			{
				// UpdateType.Unknown:
				// UpdateType.ChannelPost:
				// UpdateType.EditedChannelPost:
				// UpdateType.ShippingQuery:
				// UpdateType.PreCheckoutQuery:
				// UpdateType.Poll:
				UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
				UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
				//UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
				UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
				//UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
				//_ => UnknownUpdateHandlerAsync(botClient, update)
			};

			try
			{
				await handler;
			}
			catch (Exception exception)
			{
				await HandleErrorAsync(botClient, exception, cancellationToken);
			}            
		}

		static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery query)
		{
			Console.WriteLine("Inline query: " + query.Query + " recieved from " + query.From.Username);
			InlineQueryResultBase[] results = {
				// displayed result
				new InlineQueryResultArticle(
					id: "0",
					title: "dumbify your message!",
					inputMessageContent: new InputTextMessageContent(
						DumbifyText(query.Query)
					)
				)
			};

			await botClient.AnswerInlineQueryAsync(
				inlineQueryId: query.Id,
				results: results,
				isPersonal: true,
				cacheTime: 0);
		}

		static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
		{
			Console.WriteLine("Message: " + message.Text + " recieved from " + message.From.Username);
			var chatId = message.Chat.Id;

			await botClient.SendTextMessageAsync(chatId: chatId, text: DumbifyText(message.Text));
		}

		static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
		{
			var errorMessage = exception switch
			{
				ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
				_ => exception.ToString()
			};
			Console.WriteLine(errorMessage);
			return Task.CompletedTask;
		}
	}
}
