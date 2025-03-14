using System.Security.Claims;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userBL">The business logic layer for user operations.</param>
        /// <param name="rabbitMQProducer">The RabbitMQ producer for event-driven messaging.</param>
        public UserController(IUserBL userBL, RabbitMQProducer rabbitMQProducer)
        {
            _userBL = userBL;
            _rabbitMQProducer = rabbitMQProducer;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDto">User registration details</param>
        /// <returns>returns the registered user details</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserDTO userDto)
        {
            var result = _userBL.Register(userDto);
            if (result == null)
            {
                return BadRequest("User registration failed.");
            }

            string message = $"User registered: {result.Email}:{result.Role}";
            _rabbitMQProducer.PublishMessage("user.registration", message);

            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>Returns a JWT token if authentication is successful</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDto)
        {
            var token = _userBL.Login(loginDto);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

        /// <summary>
        /// Initiates a password reset request by sending reset instructions to the user's email.
        /// </summary>
        /// <param name="forgotPasswordDTO">User's email address for password reset</param>
        /// <returns>Returns success or failure response</returns>
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

        /// <summary>
        /// Resets the user's password using a reset token
        /// </summary>
        /// <param name="token">The password reset token</param>
        /// <param name="resetPasswordDTO">New password and confirmation password</param>
        /// <returns>Returns success or failure response</returns>
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


        /// <summary>
        /// Get the logged-in user's profile
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            try
            {
                string userEmail = User.FindFirstValue(ClaimTypes.Email);
                Console.WriteLine($"[Email]: {userEmail}");

                if (string.IsNullOrEmpty(userEmail))
                    return Unauthorized("Invalid user credentials.");

                var userProfile = _userBL.GetUserProfile(userEmail);
                if (userProfile == null)
                    return NotFound("User not found.");

                return Ok(userProfile);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
