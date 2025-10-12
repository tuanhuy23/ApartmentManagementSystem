using System.Security.Cryptography;
using System.Text;

namespace ApartmentManagementSystem.Common
{
    public static class TokenHelper
    {
         public static string GenerateSalt(int size = 32)
        {
            var bytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string HashToken(string token, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(token + salt);
                var hash = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }
    }
}