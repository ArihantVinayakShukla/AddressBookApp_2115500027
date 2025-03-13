using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.DTO;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Hashing;
using RepositoryLayer.Helper;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        private readonly AddressBookContext _context;
        private readonly JwtHelper _jwtHelper;


        public UserRL(AddressBookContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public UserDTO Register(UserDTO userDto)
        {
            var passwordHasher = new Password_Hash();
            string hashedPassword = passwordHasher.HashPassword(userDto.Password);

            var user = new UserEntry
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                Role = "User"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        public string Login(LoginDTO loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);

            var passwordHasher = new Password_Hash();
            if (user == null || !passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            return _jwtHelper.GenerateToken(user.Email, user.Role);
        }

        public int GetUserIdByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user?.Id ?? 0;
        }




    }
}
