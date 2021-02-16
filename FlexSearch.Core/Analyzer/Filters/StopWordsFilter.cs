using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer.Commands;


namespace Core.Analyzer.Filters
{
    public class StopWordsFilter : IFilter
    {
        private readonly IList<string> _stopWords;
        
        public StopWordsFilter()
        {
            _stopWords = GetStopWordsCommand.GetStopWords().Result;
        }
        
        public Task<IList<string>> Execute(IList<string> tokens)
        {
            var result = tokens.ToList();
            for (var i = 0; i < tokens.Count; i++)
            {
                if (_stopWords.Any(x => x == tokens[i]))
                    result.Remove(tokens[i]);
            }

            return Task.FromResult((IList<string>)result);
        }
    }
}