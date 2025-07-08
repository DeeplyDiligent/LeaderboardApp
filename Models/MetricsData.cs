namespace LeaderboardApp.Models
{
    public class MetricsData
    {
        public int MetricsId { get; set; } // Primary Key
        public DateTime Date { get; set; } // Date of the metrics
        public string OrgName { get; set; } = string.Empty;  // Organization name
        public string JsonResponse { get; set; } = string.Empty; // JSON response as a string
    }
}
