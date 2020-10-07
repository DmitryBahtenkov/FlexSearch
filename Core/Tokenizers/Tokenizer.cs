using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public class Tokenizer : ITokenizer
    {
        public string DeletePunctuation(string text)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> SplitString(string text)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> Tokenize(string text)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> DeletePunctuationAsync(string text)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<string>> SplitStringAsync(string text)
        {
            throw new System.NotImplementedException();
        }


        public Task<IEnumerable<string>> TokenizeAsync(string text)
        {
            throw new System.NotImplementedException();
        }
    }
}