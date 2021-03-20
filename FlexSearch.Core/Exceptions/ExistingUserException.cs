using System;

namespace Core.Exceptions
{
    public class ExistingUserException : Exception
    {
        public ExistingUserException() : base("User already exist") { }
    }
}