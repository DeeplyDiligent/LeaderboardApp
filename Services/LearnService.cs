using LeaderboardApp.DTOs;
using LeaderboardApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LeaderboardApp.Services
{
    public class LearnService
    {
        private readonly GhcacDbContext _context;

        public LearnService(GhcacDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetCurrentUserLearnHandleAsync(ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("preferred_username")
            ?? User.FindFirstValue("emails");

            if (string.IsNullOrEmpty(email))
            {
                throw new InvalidOperationException("Unable to get email details for the signed-in user");
            }

            var user = await _context.Participants.FirstOrDefaultAsync(p => p.Email == email);

            if (user == null)
            {
                throw new InvalidOperationException("User not found in the database");
            }

            return user.Mslearnhandle ?? string.Empty;
        }

        public async Task<ChallengeDto?> GetChallengeByIdAsync(int challengeId)
        {
            var challenge = await _context.Challenges
                .Where(c => c.ChallengeId == challengeId)
                .Select(c => new ChallengeDto
                {
                    Title = c.Title,
                    Content = c.Content,
                    PostedDate = c.PostedDate ?? DateTime.MinValue,
                    ActivityId = c.ActivityId ?? 0,
                    ChallengeId = c.ChallengeId,
                    IsCompleted = false // TODO: Replace with actual logic
                })
                .FirstOrDefaultAsync();

            return challenge;
        }
    }
}