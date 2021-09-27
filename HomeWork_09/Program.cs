using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Newtonsoft.Json;

namespace HomeWork_09
{
    class Program
    {
        static TelegramBotClient bot; // Работа с ботом
        static List<string> list_file = new List<string>(); // Список ранее загруженных файлов
        static TelegramOptions telegramoptions = new TelegramOptions(); // Функционал бота

        /// <summary>
        /// Точка входа в программу
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Подключиться к боту
            string token = File.ReadAllText("");
            bot = new TelegramBotClient(token);

            // Записать в лист список ранее загруженных файлов
            TelegramOptions telegram_options = new TelegramOptions();
            list_file = telegram_options.DeSerialization();

            // Бот получает сообщение
            bot.OnMessage += Message;
            bot.StartReceiving();
            Console.ReadKey();
        }

        /// <summary>
        /// Обработчик события OnMessage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Message(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document) // Если отправили документ
            #region
            {
                Console.WriteLine($"User: {e.Message.Chat.FirstName}  |  FileName: {e.Message.Document.FileName}  |  Type: {e.Message.Type}");

                //Сохранить файл на диск
                try
                {
                    telegramoptions.Upload(e.Message.Document.FileId, e.Message.Document.FileName, bot);
                    list_file.Add(e.Message.Document.FileName);
                    telegramoptions.Serialization(list_file);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Загрузка прошла успешно!");
                }
                catch
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Ошибка загрузки...");
                }
            }
            #endregion

            else if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo) // Если отправили фото
            #region
            {
                Console.WriteLine($"User: {e.Message.Chat.FirstName}  |  Type: {e.Message.Type}");

                //Сохранить фото на диск
                try
                {
                    telegramoptions.Upload(e.Message.Photo[3].FileId, $"photo{list_file.Count + 1}", bot);
                    list_file.Add($"photo{list_file.Count + 1}");
                    telegramoptions.Serialization(list_file);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Загрузка прошла успешно!");
                }
                catch
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Ошибка загрузки...");
                }
            }
            #endregion

            else if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Audio) // Если отправили аудио
            #region
            {
                Console.WriteLine($"User: {e.Message.Chat.FirstName}  |  FileName: {e.Message.Audio.FileName}  |  Type: {e.Message.Type}");

                // Сохранить аудио на диск
                try
                {
                    telegramoptions.Upload(e.Message.Audio.FileId, e.Message.Audio.FileName, bot);
                    list_file.Add(e.Message.Audio.FileName);
                    telegramoptions.Serialization(list_file);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Загрузка прошла успешно!");
                }
                catch
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Ошибка загрузки...");
                }
            }
            #endregion

            else if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text) // Если отправили текст
            #region
            {
                Console.WriteLine($"User: {e.Message.Chat.FirstName}  |  Message: {e.Message.Text}  |  Type: {e.Message.Type}");

                if (e.Message.Text.Contains("/file")) // Отправить файл пользователю
                #region
                {
                    if (telegramoptions.GettFileNumber(e.Message.Text) == -1)
                        bot.SendTextMessageAsync(e.Message.Chat.Id, "К сожалению такого файла не найдено!");

                    else
                    {
                        try
                        {
                            if (list_file[telegramoptions.GettFileNumber(e.Message.Text)].Contains("."))
                            telegramoptions.Download(e.Message.Chat.Id.ToString(), 
                                list_file[telegramoptions.GettFileNumber(e.Message.Text)], bot);

                            // Когда сохраняется фото, то в его имени нет символа "."
                            else
                                telegramoptions.DownloadPhoto(e.Message.Chat.Id.ToString(), 
                                    list_file[telegramoptions.GettFileNumber(e.Message.Text)], bot);
                        }
                        catch
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id, "Ошибка скачивания...");
                        }
                    }
                }
                #endregion

                else
                {
                    switch (e.Message.Text)
                    {
                        case "/start":
                            #region
                            bot.SendTextMessageAsync(e.Message.Chat.Id, telegramoptions.Start($"{e.Message.Chat.LastName} " +
                                $"{e.Message.Chat.FirstName}"));
                            #endregion
                            break;

                        case "/download_files":
                            #region
                            if (list_file.Count == 0)
                                bot.SendTextMessageAsync(e.Message.Chat.Id, "Список загруженных файлов пуст");

                            else
                            {
                                    bot.SendTextMessageAsync(e.Message.Chat.Id, telegramoptions.ShowListFilesForDownload(list_file));
                            }
                            #endregion
                            break;

                        case "/list_files":
                            #region
                            if (list_file.Count == 0)
                                bot.SendTextMessageAsync(e.Message.Chat.Id, "Список загруженных файлов пуст");

                            else
                                bot.SendTextMessageAsync(e.Message.Chat.Id, telegramoptions.ShowListFiles(list_file));
                            #endregion
                            break;

                        default:
                            bot.SendTextMessageAsync(e.Message.Chat.Id, "К сожалению эта команда мне не знакома...");
                            break;
                    }
                }
            }
            #endregion

            else
                bot.SendTextMessageAsync(e.Message.Chat.Id, "К сожалению этот тип файла мне не знаком...");
        }
    }
}
