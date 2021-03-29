using System.Threading.Tasks;

namespace FlexSearch.Router.Services
{
    public interface IEmailService
    {
        public Task<bool> SendEmailNotification(string emailText);
    }
}