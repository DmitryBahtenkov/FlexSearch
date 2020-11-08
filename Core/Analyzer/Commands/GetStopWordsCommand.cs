using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.Analyzer.Commands
{
    public class GetStopWordsCommand
    {
        private static readonly IList<string> StopWords;

        static GetStopWordsCommand()
        {
            StopWords = new List<string>();
        }

        private static async Task ParseWords()
        {
            var path = "Resources/stopwords.txt";


            using var sr = new StreamReader(path);
            string line;
            while ((line = await sr.ReadLineAsync()) != null)
            {
                StopWords.Add(line);
            }
        }

        public static async Task<IList<string>> GetStopWords()
        {
            if (StopWords.Count == 0)
                await ParseWords();
            return StopWords;
        }
    }
}