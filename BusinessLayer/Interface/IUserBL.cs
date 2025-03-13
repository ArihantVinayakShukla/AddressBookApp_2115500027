using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        UserDTO Register(UserDTO userDto);
        string Login(LoginDTO loginDTO);
        int GetUserIdByEmail(string email);

    }
}
