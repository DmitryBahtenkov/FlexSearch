using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Analyzer.Filters;
using Core.Helper;


namespace Core.Analyzer
{
    public class Normalizer
    {
        private readonly IList<IFilter> _filters;

        public Normalizer()
        {
            _filters = FilterConstructor.GetFilters().GetAwaiter().GetResult();
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