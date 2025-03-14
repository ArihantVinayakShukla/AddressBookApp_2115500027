using System;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Represents the data required for initiating a forgot password request.
    /// </summary>
    public class ForgotPasswordDTO
    {
        /// <summary>
        /// Email address of the user requesting a password reset.
        /// </summary>
        public string Email { get; set; }
    }
}
