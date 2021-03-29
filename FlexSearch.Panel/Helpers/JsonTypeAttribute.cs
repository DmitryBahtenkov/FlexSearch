using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FlexSearch.Panel.Helpers
{
    public class JsonTypeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            try
            {
                return JsonConvert.DeserializeObject<JObject>(value?.ToString()!) is not null;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}