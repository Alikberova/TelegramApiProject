using System;
using System.IO;

namespace TelegramApiProject
{
    public class PathService
    {
        public string SessionPath()
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Telegram Desktop"
            );

            CreateDirIfNotExists(filePath);

            return Path.Combine(filePath, "session");
        }

        public string UsersPath(string file)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
                "users");

            CreateDirIfNotExists(filePath);

            return Path.Combine(filePath, file);
        }

        public void CreateFileIfNotExists(string filepath)
        {
            if (!File.Exists(filepath))
            {
                using (File.Create(filepath));
            }
        }

        public void CreateDirIfNotExists(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }
    }
}
