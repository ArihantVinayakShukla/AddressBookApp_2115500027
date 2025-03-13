using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace AddressBookAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDto)
        {
            var result = _userBL.Register(userDto);
            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var token = _userBL.Login(loginDto);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

    }
}
