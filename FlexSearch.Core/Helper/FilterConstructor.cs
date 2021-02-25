using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Analyzer.Filters;
using Core.Configuration;

namespace Core.Helper
{
    public static class FilterConstructor
    {
        public static async Task<List<IFilter>> GetFilters()
        {
            var config = await ConfigurationService.Get();
            
            var result = new List<IFilter>();
            
            var type = typeof(IFilter);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Where(t => config.Filters.Contains(t.Name.Replace("Filter", "")));

            foreach (var t in types)
            {
                var filter = Activator.CreateInstance(t) as IFilter;
                result.Add(filter);
            }

            return result;
        }
    }
}