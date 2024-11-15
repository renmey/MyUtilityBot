using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using MyUtilityBot.Controllers;

namespace MyUtilityBot
{
    internal class Bot : BackgroundService
    {
        private ITelegramBotClient telegramClient;
        private KeyboardController keyboardController;
        private MessageController messageController;
        private Dictionary<long, string> userActions = new(); // словарь для отслеживания выбора пользователя
        public Bot(ITelegramBotClient telegramClient, KeyboardController keyboardController, MessageController     messageController)
        {
            this.telegramClient = telegramClient;
            this.keyboardController = keyboardController;
            this.messageController = messageController;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.CallbackQuery)
            {

                // Запоминаем, какую кнопку нажал пользователь
                
                
                userActions[update.CallbackQuery.From.Id] = update.CallbackQuery.Data;
                await keyboardController.Handle(update.CallbackQuery, cancellationToken);
                return;
            }

            // Обрабатываем входящие сообщения из Telegram Bot API
            if (update.Type == UpdateType.Message)
            {
                long userId = update.Message!.From.Id;
                string messageText = update.Message.Text;

                if (messageText.StartsWith("/start"))
                {
                    // если пользователь запускает /start убирается запись о предыдущем действии
                    if (userActions.ContainsKey(userId))
                        userActions.Remove(userId);

                    await messageController.Handle(update.Message, cancellationToken);
                    return;
                }

                // Проверяем, выбрано ли действие пользователем
                if (userActions.TryGetValue(userId, out string? action))
                {
                    if (action == "sum")
                    {
                        if (IsStringOfNumbers(messageText))
                        {
                            int sum = CalculateSumOfNumbers(messageText);
                            await telegramClient.SendMessage(
                                userId,
                                $"Сумма чисел: {sum}",
                                cancellationToken: cancellationToken);
                        }
                        else
                        {
                            // Сообщение, если введены некорректные данные
                            await telegramClient.SendMessage(
                                userId,
                                "Ошибка: Для выбранного действия нужно ввести числа, разделенные пробелами.",
                                cancellationToken: cancellationToken);
                        }
                    }
                    else if (action == "result")
                    {
                        // Выполняем подсчет символов для текста
                        await telegramClient.SendMessage(
                            userId,
                            $"Длина сообщения: {messageText.Length} знаков",
                            cancellationToken: cancellationToken);
                    }
                }
                else
                {
                    // Если действие не выбрано, предлагаем выбрать через /start
                    await telegramClient.SendMessage(
                        userId,
                        "Пожалуйста, выберите действие с помощью команды /start.",
                        cancellationToken: cancellationToken);
                }
            }
            
        }



        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Задаем сообщение об ошибке в зависимости от того, какая именно ошибка произошла
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            // Выводим в консоль информацию об ошибке
            Console.WriteLine(errorMessage);

            // Задержка перед повторным подключением
            Console.WriteLine("Ожидаем 10 секунд перед повторным подключением.");
            Thread.Sleep(10000);

            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {

            telegramClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new ReceiverOptions() { AllowedUpdates = { } }, // Здесь выбираем, какие обновления хотим получать. В данном случае разрешены все
                cancellationToken: stoppingToken);

            Console.WriteLine("Бот запущен");
            return Task.CompletedTask;
        }

        // Метод для проверки, состоит ли строка только их чисел, разделенных запятфыми
        private bool IsStringOfNumbers(string input)
        {
            string[] parts = input.Split(' '); 
            foreach (string part in parts)
            {
                if (!int.TryParse(part, out _))
                {
                    return false;
                }
            }
            return true;
        }

        // Метод для вычисления суммы чисел в строке
        private int CalculateSumOfNumbers(string input)
        {
            int sum = 0;
            string[] parts = input.Split(' ');
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    sum += number;
                }
            }
            return sum;
        }

    }
}
