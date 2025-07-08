namespace LeaderboardApp.DTOs
{
    public class TeamDailySummaryDto
    {
        public DateTime Day { get; set; }
        public int TotalSuggestionsCount { get; set; }
        public int TotalAcceptancesCount { get; set; }
        public int TotalLinesSuggested { get; set; }
        public int TotalLinesAccepted { get; set; }
        public int TotalActiveUsers { get; set; }
        public int TotalChatAcceptances { get; set; }
        public int TotalChatTurns { get; set; }
        public int TotalActiveChatUsers { get; set; }
        public List<LanguageBreakdownDto> Breakdown { get; set; } = new();
    }

    public class LanguageBreakdownDto
    {
        public string Language { get; set; }
        public string Editor { get; set; }
        public int SuggestionsCount { get; set; }
        public int AcceptancesCount { get; set; }
        public int LinesSuggested { get; set; }
        public int LinesAccepted { get; set; }
        public int ActiveUsers { get; set; }
    }
}
