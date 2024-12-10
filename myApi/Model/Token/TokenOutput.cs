using myApi.Model.User;

namespace myApi.Model.Token
{
    public class TokenOutput
    {
        public string Token { get; set; }
        public UserOutput User { get; set; }
    }
}
