namespace ApartmentManagementSystem
{
    public class AppSettings
    {
        public static ConnectionString ConnectionStrings { get; set; }
        public static CloudinarySetting Cloudinary { get; set; }
        public static string SeedPwd { get; set; }
        public static JwtSettings JwtSettings { get; set; }
    }
    public class ConnectionString
    {
        public string DataApartment { get; set; }
        public string Identity { get; set; }
    }
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Secret { get; set; }
    }
    public class CloudinarySetting
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
