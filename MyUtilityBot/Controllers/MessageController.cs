using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace MyUtilityBot.Controllers
{
    internal class MessageController
    {

        private readonly ITelegramBotClient telegramClient;

        public MessageController( ITelegramBotClient telegramClient)
        {
            this.telegramClient = telegramClient;
        }

        public async Task Handle(Message message, CancellationToken cancellationToken)
        {
            switch (message.Text)
            {
                case "/start":

                    // Объект, представляющий кнопки
                    var buttons = new List<InlineKeyboardButton[]>();
                    buttons.Add(new[]
                    {
                        InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"sum"),
                        InlineKeyboardButton.WithCallbackData($" Количество символов" , $"result")
                    });

                    
                    await telegramClient.SendMessage(message.Chat.Id, 
                        $"{Environment.NewLine}Подсчет количества символов и сумма чисед{Environment.NewLine}", cancellationToken: cancellationToken, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

                    break;
                default:
                    await telegramClient.SendMessage(message.Chat.Id, "Отправьте сообщение", cancellationToken: cancellationToken);
                    break;
            }
        }
    }
}
