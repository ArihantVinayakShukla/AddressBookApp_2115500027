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

        public string Login(LoginDTO loginDto)
        {
            return _userRL.Login(loginDto);
        }

        public int GetUserIdByEmail(string email)
        {
            return _userRL.GetUserIdByEmail(email);
        }
    }
}
