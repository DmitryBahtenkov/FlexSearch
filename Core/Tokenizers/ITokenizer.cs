using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core
{
    public interface ITokenizer
    {
        public string DeletePunctuation(string text);
        public IList<string> SplitString(string text);
        public IList<string> Tokenize(string text);

        public Task<string> DeletePunctuationAsync(string text);
        public Task<List<string>> SplitStringAsync(string text);
        public Task<IList<string>> TokenizeAsync(string text);
    }
}