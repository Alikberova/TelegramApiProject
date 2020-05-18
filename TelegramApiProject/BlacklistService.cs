using System.IO;

namespace TelegramApiProject
{
    public class BlacklistService
    {
        private readonly PathService _pathService;

        public BlacklistService(PathService pathService)
        {
            _pathService = pathService;
        }

        public bool BlacklistContainsId(int userId)
        {
            string path = _pathService.UsersPath("blacklist.txt");

            _pathService.CreateFileIfNotExists(path);

            return FileContainsString(path, userId.ToString());
        }

        public void WriteToBlacklistFile(int userId)
        {
            string path = _pathService.UsersPath("blacklist.txt");

            if (!FileContainsString(path, userId.ToString()))
            {
                using var writer = new StreamWriter(path, true);
                writer.WriteLine(userId.ToString());
                writer.Close();
            }
        }

        private bool FileContainsString(string file, string value)
        {
            using StreamReader reader = new StreamReader(file);
            string textInFile = reader.ReadToEnd();
            reader.Close();

            string idString = value;

            if (textInFile.Contains(idString))
            {
                return true;
            }

            return false;
        }
    }
}
