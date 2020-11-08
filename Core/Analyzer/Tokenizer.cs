using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace Core.Analyzer
{
    public class Tokenizer 
    {
        private readonly char[] _punctuations = new[] {',', '.', '-', ';', ':', '?', '!'};

        public Task<string> DeletePunctuation(string text)
        {
            return Task.FromResult(
                _punctuations.Aggregate(
                    text, 
                    (current, punctuation) =>
                        current.Replace(punctuation.ToString(), "")
                        )
                );
        }

        public Task<string> ToLowerCase(string text)
        {
            return Task.FromResult(text.ToLower());
        }

        public Task<string> DeleteStopWords(string text, string lang)
        {
            return Task.FromResult(text);
        }

        public Task<List<string>> SplitString(string text)
        {
            var result = text.Split(' ');
            return Task.FromResult(result.ToList());
        }


        public async Task<IList<string>> Tokenize(string text, string lang = "en")
        {
            var normText = await DeletePunctuation(text);
            var lowerText = await ToLowerCase(normText);
            var withoutStopWordsText = await DeleteStopWords(lowerText, lang);
            var result = await SplitString(withoutStopWordsText);

            return result;
        }
    }
}