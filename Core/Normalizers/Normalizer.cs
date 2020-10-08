using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Commands;
using Iveonik.Stemmers;

namespace Core.Normalizers
{
    public class Normalizer : INormalizer
    {
        private readonly IList<string> _stopWords;
        public Normalizer()
        {
            _stopWords = GetStopWordsCommand.GetStopWords(Languages.English).Result;
        }

        public IList<string> DeleteStopWords(IList<string> tokens)
        {
            var result = tokens;
            for (var i = 0; i < tokens.Count; i++)
            {
                if (_stopWords.Any(x => x == tokens[i]))
                    result.Remove(tokens[i]);
            }

            return result;
        }

        public IList<string> ToLowerCase(IList<string> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = tokens[i].ToLower();
            }

            return tokens;
        }

        public IList<string> StemTokens(IList<string> tokens)
        {
            var stemmer = new EnglishStemmer();
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = stemmer.Stem(tokens[i]);
            }

            return tokens;
        }

        public IList<string> Normalize(IList<string> tokens)
        {
            var lowerTokens = ToLowerCase(tokens);
            var nonStopWords = DeleteStopWords(lowerTokens);
            var stemTokens = StemTokens(nonStopWords);

            return stemTokens;
        }

        public Task<IList<string>> DeleteStopWordsAsync(IList<string> tokens)
        {
            var result = tokens;
            for (var i = 0; i < tokens.Count; i++)
            {
                if (_stopWords.Any(x => x == tokens[i]))
                    result.Remove(tokens[i]);
            }

            return Task.FromResult(result);
        }

        public Task<IList<string>> ToLowerCaseAsync(IList<string> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = tokens[i].ToLower();
            }

            return Task.FromResult(tokens);
        }

        public Task<IList<string>> StemTokensAsync(IList<string> tokens)
        {
            var stemmer = new EnglishStemmer();
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = stemmer.Stem(tokens[i]);
            }

            return Task.FromResult(tokens);
        }

        public async Task<IList<string>> NormalizeAsync(IList<string> tokens)
        {
            var lowerTokens = await ToLowerCaseAsync(tokens);
            var nonStopWords = await DeleteStopWordsAsync(lowerTokens);
            var stemTokens = await StemTokensAsync(nonStopWords);

            return stemTokens;
        }
    }
}