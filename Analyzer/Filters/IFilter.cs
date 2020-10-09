using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Filters
{
    public interface IFilter
    {
        Task<IList<string>> Execute(IList<string> tokens);
    }
}