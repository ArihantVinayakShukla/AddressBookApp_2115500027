using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Helper
{
    public class ResetTokenHelper
    {
        private readonly IConfiguration _configuration;

        public ResetTokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GeneratePasswordResetToken(int userId, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:resetKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("ID", userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim("Purpose", "PasswordReset")
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidatePasswordResetToken(string token, out string email)
        {
            email = null;

            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:resetKey"]); // Use the same key as in GeneratePasswordResetToken

                if (key == null || key.Length == 0)
                {
                    return false;
                }

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Check if token is expired
                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    return false;
                }

                // Verify the token purpose
                var purposeClaim = principal.FindFirst("Purpose");
                if (purposeClaim == null || purposeClaim.Value != "PasswordReset")
                {
                    return false;
                }

                // Extract email from the token
                var emailClaim = principal.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    email = emailClaim.Value;
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
