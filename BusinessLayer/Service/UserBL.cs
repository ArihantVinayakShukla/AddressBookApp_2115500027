using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.DTO;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRL;
        private readonly IMapper _mapper;

        public UserBL(IUserRL userRL, IMapper mapper)
        {
            _userRL = userRL;
            _mapper = mapper;
        }

        public UserDTO Register(UserDTO userDto)
        {
            return _mapper.Map<UserDTO>(_userRL.Register(userDto));
        }

        public async Task<string> Login(LoginDTO loginDto)
        {
            return await _userRL.Login(loginDto);  
        }

        public int GetUserIdByEmail(string email)
        {
            return _userRL.GetUserIdByEmail(email);
        }
        public string GetUserRoleByEmail(string email)
        {
            return _userRL.GetUserRoleByEmail(email);
        }
        public bool ForgotPassword(string email)
        {
            return _userRL.ForgotPassword(email);
        }

        public bool ResetPassword(string token, string newPassword, string confirmPassword)
        {
            return _userRL.ResetPassword(token, newPassword, confirmPassword);  
        }

        public UserDTO GetUserProfile(string email)
        {
            return _userRL.GetUserProfile(email);
        }

    }
}
