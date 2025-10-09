using System.ComponentModel.DataAnnotations;

namespace ApartmentManagementSystem.DbContext.Entity
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string TokenHash { get; set; }
        public string TokenSalt { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Revoked { get; set; }

        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
