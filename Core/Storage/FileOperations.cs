using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;

namespace Core.Storage
{
    public class FileOperations
    {
        public static async Task<T> ReadObjectFromFile<T>(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return JsonConvert.DeserializeObject<T>(await sr.ReadToEndAsync());
            }
        }
        public static async Task WriteObjectToFile(string path, object obj)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                var str = JsonConvert.SerializeObject(obj);
                var bytes = System.Text.Encoding.Default.GetBytes(str);
                await fileStream.WriteAsync(bytes);
            }
        }

        public static Task CheckOrCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public static Task RenameDirectory(string oldPath, string newName)
        {
            FileSystem.RenameDirectory(oldPath, newName);
            return Task.CompletedTask;
        }

        public static Task DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
            return Task.CompletedTask;
        }

        public static Task DeleteFile(string path)
        {
            File.Delete(path);
            return Task.CompletedTask;
        }
        
    }
}