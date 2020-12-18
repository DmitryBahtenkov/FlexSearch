using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Analyzer.Filters
{
    public class PunctuationFilter : IFilter
    {
        private readonly string[] _punctuations =  {",", ".", "-", ";", ":", "?", "!", "\"", "'\'", "\t", "\n"};
        public async Task<IList<string>> Execute(IList<string> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                tokens[i] = await DeletePunctuation(tokens[i]);
                if(tokens[i] == "")
                    tokens.RemoveAt(i);
            }

            return tokens;
        }
        
        private Task<string> DeletePunctuation(string text)
        {
            return Task.FromResult(
                _punctuations.Aggregate(
                    text, 
                    (current, punctuation) =>
                        current.Replace(punctuation.ToString(), "")
                )
            );
        }
    }
}