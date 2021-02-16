using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Analyzer.Filters
{
    public class LowerCaseFilter : IFilter
    {
        public Task<IList<string>> Execute(IList<string> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = tokens[i].ToLower();
            }

            return Task.FromResult(tokens);
        }
    }
}