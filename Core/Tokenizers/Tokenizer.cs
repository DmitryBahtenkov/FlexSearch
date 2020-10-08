using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Tokenizers
{
    public class Tokenizer : ITokenizer
    {
        private readonly char[] _punctuations = new[] {',', '.', '-', ';', ':', '?', '!'}; 
        public string DeletePunctuation(string text)
        {
            return 
                _punctuations.Aggregate(
                    text, 
                    (current, punctuation) => current
                        .Replace(punctuation.ToString(), ""));
        }

        public IList<string> SplitString(string text)
        {
            return text.Split(" ");
        }

        public IList<string> Tokenize(string text)
        {
            var normText = DeletePunctuation(text);
            var result = SplitString(normText);

            return result;
        }

        public  Task<string> DeletePunctuationAsync(string text)
        {
            return Task.FromResult(
                _punctuations.Aggregate(
                    text, 
                    (current, punctuation) =>
                        current.Replace(punctuation.ToString(), "")
                        )
                );
        }

        public Task<List<string>> SplitStringAsync(string text)
        {
            var result = text.Split(' ');
            return Task.FromResult(result.ToList());
        }


        public async Task<IList<string>> TokenizeAsync(string text)
        {
            var normText = await DeletePunctuationAsync(text);
            var result = await SplitStringAsync(normText);

            return result;
        }
    }
}