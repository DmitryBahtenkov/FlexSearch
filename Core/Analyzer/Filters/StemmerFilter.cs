using System.Collections.Generic;
using System.Threading.Tasks;
using Iveonik.Stemmers;

namespace Core.Analyzer.Filters
{
    public class StemmerFilter : IFilter
    {
        public Task<IList<string>> Execute(IList<string> tokens)
        {
            var stemmer = new EnglishStemmer();
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = stemmer.Stem(tokens[i]);
            }

            return Task.FromResult(tokens);
        }
    }
}