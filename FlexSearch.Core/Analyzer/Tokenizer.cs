using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Analyzer
{
    public class Tokenizer 
    {
        private readonly char[] _punctuations = new[] {',', '.', '-', ';', ':', '?', '!', '\"', '\'', '\t', '\n'};

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

        public Task<List<string>> SplitString(string text)
        {
            var result = text.Split(' ');
            return Task.FromResult(result.ToList());
        }


        public async Task<IList<string>> Tokenize(string text)
        {
            var normText = await DeletePunctuation(text);
            var result = await SplitString(normText);

            return result;
        }
    }
}