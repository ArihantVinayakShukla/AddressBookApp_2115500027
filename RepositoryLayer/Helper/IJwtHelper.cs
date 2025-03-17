using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Helper
{
    public interface IJwtHelper
    {
        string GenerateToken(string email, string role);
    }
}
