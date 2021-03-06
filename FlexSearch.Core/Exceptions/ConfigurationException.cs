using System;

namespace Core.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base (message) { }
    }
}