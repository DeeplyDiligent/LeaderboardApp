using System.Text.RegularExpressions;

namespace LeaderboardApp.Services
{
    public static class StringExtensions
    {
        public static string RemoveHtmlTags(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}
