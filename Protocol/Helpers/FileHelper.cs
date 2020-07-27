using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Helpers
{
    public static class FileHelper
    {
        public static byte[] ReadFile(string path)
        {
            FileStream stream = File.OpenRead(path);
            byte[] file = new byte[stream.Length];

            stream.Read(file, 0, (int)stream.Length);
            stream.Close();

            return file;
        }

        public static void CreateFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
            stream.Close();
        }

        public static void SaveFile(string path, byte[] file)
        {
            FileStream stream = new FileStream(path, FileMode.Append, FileAccess.Write);
            stream.Write(file, 0, file.Length);
            stream.Close();
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static List<string> SearchFilesInLocation(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
