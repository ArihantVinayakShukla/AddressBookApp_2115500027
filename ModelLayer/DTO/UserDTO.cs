using System;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Represents the data required for user registration.
    /// </summary>
    public class UserDTO
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password chosen by the user for authentication.
        /// </summary>
        public string Password { get; set; }
    }
}
