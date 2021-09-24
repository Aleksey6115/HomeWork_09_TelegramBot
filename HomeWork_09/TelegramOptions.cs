using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Telegram.Bot;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HomeWork_09
{
    class TelegramOptions
    {
        /// <summary>
        /// Метод загружает файл на ПК
        /// </summary>
        /// <param name="fileId">id файла</param>
        /// <param name="path">Путь для созранения</param>
        /// <param name="bot">бот</param>
        public async void Upload(string fileId, string path, TelegramBotClient bot)
        {
            var file = await bot.GetFileAsync(fileId);
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await bot.DownloadFileAsync(file.FilePath, fs);
            }
        }

        /// <summary>
        /// Метод отправляет файл пользователю
        /// </summary>
        /// <param name="chatid">id чата</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="bot">бот</param>
        public async void Download(string chatid, string path, TelegramBotClient bot)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                await bot.SendDocumentAsync(chatid, new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs, path));
            }
        }

        /// <summary>
        /// Метод отправляет фото пользователю
        /// </summary>
        /// <param name="chatid">id чата</param>
        /// <param name="path">Путь к файлу</param>
        /// <param name="bot">бот</param>
        public async void DownloadPhoto(string chatid, string path, TelegramBotClient bot)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                await bot.SendPhotoAsync(chatid, new Telegram.Bot.Types.InputFiles.InputOnlineFile(fs, path));
            }
        }

        /// <summary>
        /// Метод показывает перечень команд
        /// </summary>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public string Start(string user_name)
        {
            string result = $"Привет, {user_name}!\n\n" +
                $"- Если Вы хотите загрузить файл, то просто отправьте его" +
                $"\n- Если Вы хотите скачать файл выберите /download_files" +
                $"\n- Для просмотра списка загруженных файлов выберите /list_files";
            return result;
        }

        /// <summary>
        /// Дессериализация файла со списком ранее загруженных файлов
        /// </summary>
        /// <param name="file_base"></param>
        /// <returns></returns>
        public List<string> DeSerialization()
        {
            List<string> file_base = new List<string>();

            try // Если бот открывается первый раз и такого файла нет
            {
                string json = File.ReadAllText("base.json"); // Считать список загруженных ранее файлов
                file_base = JsonConvert.DeserializeObject<List<string>>(json);
                return file_base;
            }
            catch
            {
                return file_base;
            }
        }

        /// <summary>
        /// Сериализация файла со списком загруженных файлов
        /// </summary>
        /// <param name="list_files">Список загруженных файлов</param>
        public void Serialization(List<string> list_files)
        {
            string json = JsonConvert.SerializeObject(list_files);
            File.WriteAllText("base.json", json);
        }

        /// <summary>
        /// Метод показывает список ранее загруженных файлов
        /// </summary>
        /// <param name="list_files"></param>
        /// <returns></returns>
        public string ShowListFiles(List<string> list_files)
        {
            string result = $"Список ранее загруженных файлов:\n";

            for (int i = 0; i<list_files.Count; i++)
                result += $"\n{i + 1}. {list_files[i]}";
           
            return result;
        }

        /// <summary>
        /// Метод показывает список файлов для скачивания
        /// </summary>
        /// <param name="list_files">Список ранее загруженных файлов</param>
        /// <returns></returns>
        public string ShowListFilesForDownload(List<string> list_files)
        {
            string result = $"Для скачивание нажмите на файл:\n";

            for (int i = 0; i < list_files.Count; i++)
                result += $"\n/file{i + 1}-{list_files[i]}";

            return result;
        }

        /// <summary>
        /// Метод находит номер файла из строки, в противном случае возвращает -1
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int GettFileNumber (string message)
        {
            bool input; // Проверка ввода
            int file_number; // Номер файла

            input = int.TryParse(message.Remove(0, 5), out file_number);

            if (input) return file_number - 1;
            else return -1;
        }
    }
}
