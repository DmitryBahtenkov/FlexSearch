using System.Collections.Generic;
using System.Threading.Tasks;
using Iveonik.Stemmers;

namespace Core.Analyzer.Filters
{
    public class StemmerFilter : IFilter
    {
        private readonly List<IStemmer> _stemmers;

        public StemmerFilter()
        {
            _stemmers = new List<IStemmer>
            {
                new EnglishStemmer(),
                new FrenchStemmer(),
                new RussianStemmer()
            };
        }
        public Task<IList<string>> Execute(IList<string> tokens)
        {
            
             
            for (var i = 0; i < tokens.Count; i++)
            {
                foreach (var stemmer in _stemmers)
                {
                    tokens[i] = stemmer.Stem(tokens[i]);
                }
            }

            return Task.FromResult(tokens);
        }
    }
}