using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyUtilityBot.Controllers;
using Telegram.Bot.Types;

namespace MyUtilityBot
{
    internal class Program
    {
        public static async Task Main()
        {
            Console.OutputEncoding = Encoding.Unicode;

            // Объект, отвечающий за постоянный жизненный цикл приложения
            var host = new HostBuilder()
                .ConfigureServices((hostContext, services) => ConfigureServices(services)) // Задаем конфигурацию
                .UseConsoleLifetime() // Позволяет поддерживать приложение активным в консоли
                .Build(); // Собираем

            Console.WriteLine("Сервис запущен");
            // Запускаем сервис
            await host.RunAsync();
            Console.WriteLine("Сервис остановлен");


        }


      
            static void ConfigureServices(IServiceCollection services)
        {


            // Регистрируем объект TelegramBotClient c токеном подключения
            services.AddTransient<MessageController>();
            services.AddTransient<KeyboardController>();
            services.AddSingleton<ITelegramBotClient>(provider =>
            {
                var botClient = new TelegramBotClient("7844523269:AAEcwtmqpnwBtsL0VuJOqqf4PY1GPGoKZAU");
                return botClient;
            });

            // Регистрируем постоянно активный сервис бота
            services.AddHostedService<Bot>();
        }
    }
}
