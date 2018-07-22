namespace Backend.DataObjects
{
    public class JwtConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string SigningToken { get; set; }
    }
}
