namespace LeaderboardApp.Models
{
    public class TeamScoreEntryViewModel
    {
        public string Nickname { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string ActivityName { get; set; } = string.Empty;
        public bool IsTeamActivity { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
