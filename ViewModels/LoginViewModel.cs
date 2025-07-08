namespace LeaderboardApp.ViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Passcode { get; set; }
        public bool PasscodeSent { get; set; }
        public int BearerTokenExpirationInSeconds { get; set; }
    }
}
