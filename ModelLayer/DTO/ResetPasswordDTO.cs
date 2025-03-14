using System;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Represents the data required to reset a user's password.
    /// </summary>
    public class ResetPasswordDTO
    {
        /// <summary>
        /// The new password to be set.
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmation of the new password to ensure accuracy.
        /// </summary>
        public string ConfirmPassword { get; set; }
    }
}
