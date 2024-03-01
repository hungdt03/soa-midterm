namespace authentication_service.Dtos
{
    public class TokenResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireTime { get; set; }
        public object Data { get; set; }

        public TokenResponse(bool success, string accessToken, DateTime expireTime, object data)
        {
            Success = success;
            AccessToken = accessToken;
            ExpireTime = expireTime;
            Data = data;
        }
    }
}
