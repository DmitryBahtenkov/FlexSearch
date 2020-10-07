using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Normalizers
{
    public class Normalizer : INormalizer
    {
        public IEnumerable<string> DeleteStopWords(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> ToLowerCase(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> StemTokens(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<string> Normalize(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<string>> DeleteStopWordsAsync(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<string>> ToLowerCaseAsync(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<string>> StemTokensAsync(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<string>> NormalizeAsync(IEnumerable<string> tokens)
        {
            throw new System.NotImplementedException();
        }
    }
}