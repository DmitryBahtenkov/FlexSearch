using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Analyzer.Filters
{
    public interface IFilter
    {
        Task<IList<string>> Execute(IList<string> tokens);
    }
}