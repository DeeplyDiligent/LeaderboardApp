namespace LeaderboardApp.DTOs
{
    public class ParticipantScoreDto
    {
        public int ScoreId { get; set; }
        public Guid ParticipantId { get; set; }
        public int ActivityId { get; set; }
        public decimal Score { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ParticipantScoreDetailDto
    {
        public Guid ParticipantId { get; set; }
        public string ParticipantName { get; set; }
        public string TeamName { get; set; }
        public decimal TotalScore { get; set; }
    }
}
