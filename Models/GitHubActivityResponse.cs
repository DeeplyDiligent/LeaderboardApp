using System.Text.Json.Serialization;

namespace LeaderboardApp.Models
{
    public class GitHubActivityResponse
    {
        [JsonPropertyName("created_at")]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string? UpdatedAt { get; set; }

        [JsonPropertyName("pending_cancellation_date")]
        public string? PendingCancellationDate { get; set; }

        [JsonPropertyName("last_activity_at")]
        public string? LastActivityAt { get; set; }

        [JsonPropertyName("last_activity_editor")]
        public string? LastActivityEditor { get; set; }

        [JsonPropertyName("plan_type")]
        public string? PlanType { get; set; }

        [JsonPropertyName("assignee")]
        public Assignee? Assignee { get; set; }

        [JsonPropertyName("assigning_team")]
        public AssigningTeam? AssigningTeam { get; set; }
    }

    public class Assignee
    {
        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }
    }

    public class AssigningTeam
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }
    }
}
