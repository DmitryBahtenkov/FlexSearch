using System;

namespace Core.Storage
{
    public class BaseCommand
    {
        protected AppDomain AppDomain;

        public BaseCommand()
        {
            AppDomain = AppDomain.CurrentDomain;
        }
    }
}