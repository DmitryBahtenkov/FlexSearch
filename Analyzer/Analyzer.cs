using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Analyzer.Normalizers;
using Analyzer.Tokenizers;

namespace Analyzer
{
    public class Analyzer
    {
        private readonly Tokenizer _tokenizer;
        private readonly Normalizer _normalizer;

        public Analyzer(Tokenizer tokenizer, Normalizer normalizer)
        {
            _tokenizer = tokenizer;
            _normalizer = normalizer;
        }

        public async Task<IList<string>> Anal(string text)
        {
            if(string.IsNullOrEmpty(text))
                throw new ArgumentException();
            var tokens = await _tokenizer.Tokenize(text);
            var normalTokens = await _normalizer.Normalize(tokens);
            return normalTokens;
        }
        
    }
}