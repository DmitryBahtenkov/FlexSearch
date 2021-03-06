using System;

namespace Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("Пользователь не найден") { }
    }
}