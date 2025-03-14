using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using RepositoryLayer.Helper;

namespace AddressBookAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly RabbitMQProducer _rabbitMQProducer;

        public UserController(IUserBL userBL, RabbitMQProducer rabbitMQProducer)
        {
            _userBL = userBL;
            _rabbitMQProducer = rabbitMQProducer;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDto)
        {
            var result = _userBL.Register(userDto);
            if (result == null)
            {
                return BadRequest("User registration failed.");
            }

            string message = $"User registered: {result.Email}";
            _rabbitMQProducer.PublishMessage("user.registration", message);

            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var token = _userBL.Login(loginDto);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            var result = _userBL.ForgotPassword(forgotPasswordDTO.Email);
            if(result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Password reset instructions sent to your email"
                });
            }
            else
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to process password reset request"
                });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromQuery] string token, [FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if(string.IsNullOrEmpty(token))
            {
                return BadRequest(new { Success = false, Message = "Token is required" });
            }

            if(resetPasswordDTO == null)
            {
                return BadRequest(new { Success = false, Message = "Password Data is required" });
            }

            var result = _userBL.ResetPassword(token, resetPasswordDTO.NewPassword, resetPasswordDTO.ConfirmPassword);

            if(result)
            {
                return Ok(new
                {
                    Success = true,
                    Message = "Password reset successful. Please login with your new password."
                });
            }
            else
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Failed to reset password. The token may be invalid or expired, or passwords don't match."
                });
            }
        }
    }
}
