namespace UserList.API.Util.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpiryInMinutes { get; set; }
    }

}
