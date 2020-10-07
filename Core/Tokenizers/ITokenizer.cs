using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public interface ITokenizer
    {
        public string DeletePunctuation(string text);
        public IEnumerable<string> SplitString(string text);
        public IEnumerable<string> Tokenize(string text);

        public Task<string> DeletePunctuationAsync(string text);
        public Task<IEnumerable<string>> SplitStringAsync(string text);
        public Task<IEnumerable<string>> TokenizeAsync(string text);
    }
}