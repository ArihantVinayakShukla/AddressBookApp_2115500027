using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        UserDTO Register(UserDTO userDto);
        string Login(LoginDTO loginDto);
        int GetUserIdByEmail(string email);
    }
}
