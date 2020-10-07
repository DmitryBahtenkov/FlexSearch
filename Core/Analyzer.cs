using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Normalizers;

namespace Core
{
    public class Analyzer
    {
        private readonly ITokenizer _tokenizer;
        private readonly INormalizer _normalizer;

        public Analyzer(ITokenizer tokenizer, INormalizer normalizer)
        {
            _tokenizer = tokenizer;
            _normalizer = normalizer;
        }

        public IEnumerable<string> Anal(string text)
        {
            var tokens = _tokenizer.Tokenize(text);
            var normalTokens = _normalizer.Normalize(tokens);
            return normalTokens;
        }
        
        public async Task<IEnumerable<string>> AnalAsync(string text)
        {
            var tokens = await _tokenizer.TokenizeAsync(text);
            var normalTokens = await _normalizer.NormalizeAsync(tokens);
            return normalTokens;
        }
    }
}