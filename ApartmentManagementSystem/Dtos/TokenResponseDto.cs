namespace ApartmentManagementSystem.Dtos
{
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActive { get;set; }
    }
}
