using System;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Represents the credentials required for user login.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Email address of the user attempting to log in.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password of the user attempting to log in.
        /// </summary>
        public string Password { get; set; }
    }
}
