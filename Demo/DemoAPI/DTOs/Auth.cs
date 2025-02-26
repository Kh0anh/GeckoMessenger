using ServiceStack;

namespace DemoAPI.DTOs
{
    [Route("/auth/register")]
    public class Register : IReturn<RegisterResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Route("/auth/login")]
    public class Login : IReturn<LoginResponse>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterResponse
    {
        public int UserId { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
