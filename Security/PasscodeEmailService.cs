using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using LeaderboardApp.Models;

namespace LeaderboardApp.Security
{
    public class PasscodeEmailService
    {
        private readonly GhcacDbContext _context;
        private readonly IConfiguration _configuration;


        public PasscodeEmailService(GhcacDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SendPasscodeEmail(string email, string passcode)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            var message = new MailMessage
            {
                From = new MailAddress(smtpSettings!.SenderEmail, smtpSettings.SenderName),
                Subject = "Your Login Passcode",
                Body = $"Your login passcode is: {passcode}"
            };
            message.To.Add(email);

            using (var smtpClient = new SmtpClient(smtpSettings.Server, smtpSettings.Port))
            {
                smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password);
                smtpClient.EnableSsl = smtpSettings.EnableSsl;

                await smtpClient.SendMailAsync(message);
            }
        }

        public string GeneratePasscode()
        {
            return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        }

        public async Task SavePasscode(Participant participant, string passcode)
        {
            participant.Passcode = passcode;
            participant.Passcodeexpiration = DateTime.UtcNow.AddMinutes(10); // Set expiration time, e.g., 10 minutes
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidatePasscode(Guid participantId, string passcode)
        {
            var participant = await _context.Participants.FindAsync(participantId);
            if (participant == null || participant.Passcode != passcode || participant.Passcodeexpiration < DateTime.UtcNow)
            {
                return false; // Invalid passcode or expired
            }

            // Invalidate the passcode after successful use
            participant.Passcode = null;
            participant.Passcodeexpiration = null;
            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
