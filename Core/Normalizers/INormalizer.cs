using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Normalizers
{
    public interface INormalizer
    {
        public IEnumerable<string> DeleteStopWords(IEnumerable<string> tokens);
        public IEnumerable<string> ToLowerCase(IEnumerable<string> tokens);
        public IEnumerable<string> StemTokens(IEnumerable<string> tokens);
        public IEnumerable<string> Normalize(IEnumerable<string> tokens);
        
        public Task<IEnumerable<string>> DeleteStopWordsAsync(IEnumerable<string> tokens);
        public Task<IEnumerable<string>>ToLowerCaseAsync(IEnumerable<string> tokens);
        public Task<IEnumerable<string>> StemTokensAsync(IEnumerable<string> tokens);
        public Task<IEnumerable<string>> NormalizeAsync(IEnumerable<string> tokens);
    }
}