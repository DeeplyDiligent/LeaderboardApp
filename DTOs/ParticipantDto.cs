namespace LeaderboardApp.DTOs
{
    public class ParticipantDto
    {
        public Guid ParticipantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string GitHubHandle { get; set; }
        public string MsLearnHandle { get; set; }
        public Guid TeamId { get; set; }
    }
}
