using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Storage;

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
                
        }

        public async Task<List<UserModel>> GetUsers()
        {
            
        }

        public async Task<UserModel> GetUser(string userName)
        {
            
        }

        public async Task UpdateUser(string userName, UserModel userModel)
        {
            
        }

        public async Task DeleteUser(string userName)
        {
            
        }
    }
}