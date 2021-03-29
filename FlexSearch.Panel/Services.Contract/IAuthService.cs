using System.Threading.Tasks;
using FlexSearch.Panel.Models.ViewModels;

namespace FlexSearch.Panel.Services.Contract
{
    public interface IAuthService
    {
        public Task<bool> SendAuthData(AuthViewModel authViewModel);
    }
}