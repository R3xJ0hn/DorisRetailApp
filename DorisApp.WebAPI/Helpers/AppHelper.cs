using DorisApp.Data.Library.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace DorisApp.WebAPI.Helpers
{
    public static class AppHelper
    {
        public static string GetFullName(UserModel user)
        {
            var firstName = char.ToUpper(user.FirstName[0]) + user.FirstName[1..];
            var lastName = char.ToUpper(user.LastName[0]) + user.LastName[1..];
            return $"{CapitalizeFirstWords(firstName)} {CapitalizeFirstWords(lastName)}";
        }

        public static string CapitalizeFirstWords(string value)
        {
            if (value == null)
                throw new Exception(nameof(value));
            if (value.Length == 0)
                return value;

            value = value.ToLower();
            StringBuilder result = new(value);
            result[0] = char.ToUpper(result[0]);

            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
            }

            return result.ToString();
        }

        public static int CountPages(int totalItems, int itemsPerPage)
        {
            int numPages = totalItems / itemsPerPage;

            if (totalItems % itemsPerPage != 0)
            {
                numPages++;
            }
            return numPages;
        }

        public static string GetFirstWord(string str)
        {
            return Regex.Match(str ?? "Anonymous", @"^([\w\-]+)").Value;
        }

        public static bool MoveTempToDestPath(string rootFolder, string? soredFileName, string type)
        {
            var uploadsFolder = Path.Combine(rootFolder, "uploads");
            var destinationFolder = Path.Combine(uploadsFolder, type);
            var tempFolder = Path.Combine(uploadsFolder, "temp");

            if (soredFileName == null) return false;

            var targetTempFile = Path.Combine(tempFolder, soredFileName);
            if (!File.Exists(targetTempFile)) return false;


            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var path = Path.Combine(destinationFolder, soredFileName);

            File.Move(targetTempFile, path);
            DeleteFile(targetTempFile);

            return true;
        }

        public static bool DeleteFile(string path)
        {
            if (!File.Exists(path)) return false;
            File.Delete(path);
            return true;
        }


    }
}
