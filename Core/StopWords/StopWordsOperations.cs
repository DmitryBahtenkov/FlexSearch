using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Core.StopWords
{
    public class StopWordsOperations
    {
        //Ебаный костыль
        private static List<string> _stopWords => GetWords().Result;

        private static async Task<List<string>> GetWords()
        {
            if (_stopWords is { }) return _stopWords;
            var result = new List<string>(9000);
            var path = "Resources/stopwords.txt";
            using (var stream = new StreamReader(path))
            {
                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }
    }
    
}