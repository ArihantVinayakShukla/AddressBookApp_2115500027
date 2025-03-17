using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interface
{
    public interface IEmailService
    {
        bool SendPasswordResetEmail(string email, string token, string baseUrl);
        Task SendWelcomeEmailAsync(string email);
    }

}
