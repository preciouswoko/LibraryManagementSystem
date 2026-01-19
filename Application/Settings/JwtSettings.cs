using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Settings
{
    //public class JwtSettings
    //{
    //    public string SecretKey { get; set; } = string.Empty;
    //    public string Issuer { get; set; } = string.Empty;
    //    public string Audience { get; set; } = string.Empty;
    //    public int ExpirationMinutes { get; set; } = 60;
    //}

    /// <summary>
    /// Configuration settings for JWT authentication.
    /// Bound from appsettings.json "JwtSettings" section.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Secret key for signing JWT tokens.
        /// Must be at least 32 characters for HS256 algorithm.
        /// IMPORTANT: Change this in production and store securely (Azure Key Vault, AWS Secrets Manager, etc.)
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer - identifies who created the token.
        /// Should match your API domain/name.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience - identifies who can use the token.
        /// Should match your client application(s).
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Token expiration time in minutes.
        /// Default: 60 minutes (1 hour)
        /// Production: Consider shorter expiration (15-30 minutes) with refresh tokens
        /// </summary>
        public int ExpirationMinutes { get; set; } = 60;

        /// <summary>
        /// Validates that all required settings are configured.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(SecretKey))
                throw new InvalidOperationException("JWT SecretKey is not configured.");

            if (SecretKey.Length < 32)
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long.");

            if (string.IsNullOrWhiteSpace(Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured.");

            if (string.IsNullOrWhiteSpace(Audience))
                throw new InvalidOperationException("JWT Audience is not configured.");

            if (ExpirationMinutes <= 0)
                throw new InvalidOperationException("JWT ExpirationMinutes must be greater than 0.");
        }
    }
}
