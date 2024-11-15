using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyUtilityBot.Controllers
{
    internal class KeyboardController
    {
        private readonly ITelegramBotClient telegramClient;

       public KeyboardController(ITelegramBotClient telegramClient)
        {
            this.telegramClient = telegramClient;   
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery?.Data == null)
                return;

            string option = callbackQuery.Data switch
            {
                "sum" => "Сумма чисел",
                "result" => "Количество символов",
                _ => String.Empty
            };

            if (string.IsNullOrEmpty(option))
                return;

            await telegramClient.SendMessage(callbackQuery.From.Id,
                $"<b>Вы выбрали действие - {option}.</b>{Environment.NewLine}Теперь введите данные для выполнения выбранного действия.",
                cancellationToken: cancellationToken, parseMode: ParseMode.Html);

        }


    }
}
