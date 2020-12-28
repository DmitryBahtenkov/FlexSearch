using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;
using Newtonsoft.Json;

namespace Core.Users
{
    public class UserRepository
    {
        private string Path { get; set; }

        public UserRepository()
        {
            Path = $"{AppDomain.CurrentDomain.BaseDirectory}users/";
        }
        public async Task CreateUser(UserModel userModel)
        {
            var path = Path + userModel.UserName + ".json";
            if(!File.Exists(path))
                File.Create(path).Close();

            await FileOperations.WriteObjectToFile(path, userModel);
        }

        public async Task<List<UserModel>> GetUsers()
        {
            var docs = Directory.GetFiles(Path);
            var result = docs.Select(doc => 
                JsonConvert.DeserializeObject<UserModel>(File.ReadAllText(doc))).ToList();
            return result;
        }
        
        public async Task<object> GetUsersNoPassword()
        {
            var docs = Directory.GetFiles(Path);
            var result = docs.Select(doc => 
                JsonConvert.DeserializeObject<UserModel>(File.ReadAllText(doc))).ToList();
            return result.Select(x=>new
            {
                UserName = x.UserName,
                Database = x.Database
            }).ToList();
        }

        public async Task<UserModel> GetUser(string userName)
        {
            var path = Path + userName + ".json";
            return File.Exists(path)
                ? JsonConvert.DeserializeObject<UserModel>(await File.ReadAllTextAsync(path))
                : null;
        }

        public async Task UpdateUser(string userName, UserModel userModel)
        {
            var path = Path + userName + ".json";
            await FileOperations.DeleteFile(path);
            path = Path + userModel.UserName + ".json";
            if(!File.Exists(path))
                File.Create(path).Close();
            await FileOperations.WriteObjectToFile(path, userModel);
        }

        public async Task DeleteUser(string userName)
        {
            var path = Path + userName + ".json";
            await FileOperations.DeleteFile(path);
        }
    }
}