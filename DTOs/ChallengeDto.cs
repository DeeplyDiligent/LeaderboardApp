namespace LeaderboardApp.DTOs
{
    public class ChallengeDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; }
        public int ActivityId { get; set; }
        public int ChallengeId { get; set; }
        public bool IsCompleted { get; set; }
    }
}
