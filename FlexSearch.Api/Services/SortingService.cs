using System;
using System.Diagnostics.CodeAnalysis;
using Core.Analyzer.Commands;
using Core.Models;
using SearchApi.Mappings;

namespace SearchApi.Services
{
    public class SortingService
    {
        public static object GetKeyType(string key, DocumentModel document)
        {
            var token = JsonCommand.GetValueForKey(document.Value, key);
            if (token is not null)
            {
                if (double.TryParse(token.ToString(), out _))
                {
                    return Convert.ChangeType(token.ToString(), typeof(double));
                }

                if (DateTime.TryParse(token.ToString(), out _))
                {
                    return Convert.ChangeType(token.ToString(), typeof(DateTime));
                }
                return Convert.ChangeType(token.ToString(), typeof(string));
            }

            return null;
        }
        
    }
}