using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Normalizers
{
    public interface INormalizer
    {
        public IList<string> DeleteStopWords(IList<string> tokens);
        public IList<string> ToLowerCase(IList<string> tokens);
        public IList<string> StemTokens(IList<string> tokens);
        public IList<string> Normalize(IList<string> tokens);
        
        public Task<IList<string>> DeleteStopWordsAsync(IList<string> tokens);
        public Task<IList<string>>ToLowerCaseAsync(IList<string> tokens);
        public Task<IList<string>> StemTokensAsync(IList<string> tokens);
        public Task<IList<string>> NormalizeAsync(IList<string> tokens);
    }
}