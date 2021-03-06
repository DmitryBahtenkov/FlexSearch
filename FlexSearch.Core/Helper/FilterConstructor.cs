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

            var type = typeof(IFilter);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .Where(t => config.Filters.Contains(t.Name.Replace("Filter", "")));

            var result = types.Select(t => Activator.CreateInstance(t) as IFilter).ToList();
            if(!result.Contains(new PunctuationFilter()))
                result.Add(new PunctuationFilter());
            if(!result.Contains(new StemmerFilter()))
                result.Add(new StemmerFilter());
            if(!result.Contains(new LowerCaseFilter()))
                result.Add(new LowerCaseFilter());
            
            return result;
        }
    }
}