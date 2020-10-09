using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Analyzer.Commands;
using Analyzer.Filters;
using Iveonik.Stemmers;

namespace Analyzer.Normalizers
{
    public class Normalizer
    {
        private readonly IList<IFilter> _filters;

        public Normalizer(Languages language)
        {
            _filters = new List<IFilter>
            {
                new LowerCaseFilter(),
                new StopWordsFilter(language),
                new StemmerFilter()
            };
        }

        public async Task<IList<string>> Normalize(IList<string> tokens)
        {
            foreach (var filter in _filters)
            {
                tokens = await filter.Execute(tokens);
            }

            return tokens;
        }
    }
}