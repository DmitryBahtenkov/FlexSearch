using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Analyzer.Commands
{
    public class GetStopWordsCommand
    {
        private static IList<string> _stopWords;
        private static Languages _language; 

        static GetStopWordsCommand()
        {
            _stopWords = new List<string>();
        }

        private static async Task ParseWords(Languages language)
        {
            var path = "Resources/";
            switch (language)
            {
                case Languages.English:
                    path += "English.txt";
                    break;
                case Languages.Russian:
                    break;
                default:
                    path += "Russian.txt";
                    break;
            }

            _language = language;
            using var sr = new StreamReader(path);
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                _stopWords.Add(line);
            }
        }

        public static async Task<IList<string>> GetStopWords(Languages language)
        {
            if (_stopWords.Count == 0)
                await ParseWords(language);
            else if (language != _language)
                await ParseWords(language);
            return _stopWords;
        }
    }
}