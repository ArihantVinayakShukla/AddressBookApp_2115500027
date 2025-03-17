using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public interface IResetTokenHelper
    {
        string GeneratePasswordResetToken(int userId, string email);
        bool ValidatePasswordResetToken(string token, out string email);
    }

}
