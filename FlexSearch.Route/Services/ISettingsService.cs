using System.Threading.Tasks;
using FlexSearch.Router.Models;

namespace FlexSearch.Router.Services
{
    public interface ISettingsService
    {
        public Task<Settings> GetSettings();
        public Task<bool> SetSettings();
    }
}