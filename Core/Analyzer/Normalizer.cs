using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer.Commands;
using Core.Analyzer.Filters;
using Core.Enums;

namespace Core.Analyzer
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