using System.Collections.Generic;
using System.Threading.Tasks;
using FlexSearch.Panel.Helpers;
using FlexSearch.Panel.Models.ViewModels;
using FlexSearch.Panel.Services.Contract;

namespace FlexSearch.Panel.Services.Impl
{
    public class AuthService : IAuthService
    {
        private ApiRequest _apiRequest;

        public async Task<bool> SendAuthData(AuthViewModel authViewModel)
        {
            _apiRequest = new ApiRequest(true, new Dictionary<string, string>
            {
                {"User", authViewModel.Login},
                {"Password", authViewModel.Password}
            });
            if (!(await _apiRequest.SendGetAndParseObject<string[]>($"{authViewModel.Host}")).IsSuccess)
            {
                return false;
            }
            UserHelper.CurrentUser = authViewModel;
            return true;

        }
    }
}