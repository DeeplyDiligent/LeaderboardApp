namespace LeaderboardApp.Models
{
    public class ParticipantScoreInputModel
    {
        public Guid ParticipantId { get; set; }
        public int ActivityId { get; set; }
        public decimal? CustomScore { get; set; } // Optional for custom scoring with multipliers
    }
}
