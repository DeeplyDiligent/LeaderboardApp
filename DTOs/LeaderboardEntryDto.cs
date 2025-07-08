namespace LeaderboardApp.DTOs
{
    public class LeaderboardEntryDto
    {
        public Guid TeamId{ get; set; }
        public string TeamName { get; set; }
        public string TeamTagline { get; set; }
        public string TeamIcon { get; set; }
        public int Score { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<string> ParticipantNames { get; set; }
    }
}
