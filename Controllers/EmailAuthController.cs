using LeaderboardApp.Models;
using LeaderboardApp.Security;
using Microsoft.AspNetCore.Mvc;
using LeaderboardApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using LeaderboardApp.Services;
using System.Runtime.InteropServices;

namespace LeaderboardApp.Controllers
{
    public class EmailAuthController : Controller
    {
        private readonly TokenService _tokenService;
        private readonly GhcacDbContext _context;
        private readonly PasscodeEmailService _passcodeEmailService;        

        private const int PasscodeExpiration = 300; // 5 minutes
        private const int BearerTokenExpiry = 86400; // 1 day
        private const int RefreshTokenExpiry = 2628000; // 1 month

        public EmailAuthController(TokenService tokenService, GhcacDbContext context, PasscodeEmailService passcodeEmailService)
        {
            _tokenService = tokenService;
            _context = context;
            _passcodeEmailService = passcodeEmailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<JsonResult> SendPasscode(LoginViewModel model)
        {
            // Validate email format using regex
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(model.Email))
            {
                return Json(new { success = false, message = "Invalid email format" });
            }

            Participant? participant = await CreateParticipant(model.Email);

            // Generate the passcode
            var passcode = _passcodeEmailService.GeneratePasscode();

            // Save the passcode and expiration in the DB
            await _passcodeEmailService.SavePasscode(participant, passcode);


#if DEBUG
            return Json(new { success = true, passcode = passcode, message = "Passcode sent for debugging." });
#else
            // Send the passcode via email
            await _passcodeEmailService.SendPasscodeEmail(participant.Email, passcode);
            return Json(new { success = true, message = "Passcode sent to your email." });
#endif
        }

        private async Task<Participant?> CreateParticipant(string Email,Guid? Sid = null)
        {
            if(Sid == null)
            {
                Sid = Guid.NewGuid();
            }

            var participant = await _context.Participants.SingleOrDefaultAsync(p => p.Email == Email);

            if (participant == null)
            {
                // If participant not found, create a new one
                participant = new Participant
                {
                    Participantid = Sid.Value,
                    Email = Email,
                    Firstname = Email.Split('@')[0],  // Customize this
                    Lastname = "Participant",
                    Nickname = NicknameService.GetNickName(),
                    Externalid = Guid.NewGuid(),
                    Passcode = null, // Passcode will be set after generation
                    Passcodeexpiration = null, // Set after passcode is generated
                    Lastlogin = DateTime.UtcNow // Set to current time
                };

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();
            }

            return participant;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var participant = await _context.Participants.SingleOrDefaultAsync(p => p.Email == model.Email);
            if (participant == null || participant.Passcode != model.Passcode || participant.Passcodeexpiration < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            await SetTokensAsync(participant);

            model.BearerTokenExpirationInSeconds = BearerTokenExpiry;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token is missing");
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshToken);
            if (principal == null)
            {
                return Unauthorized("Invalid refresh token");
            }

            var participantId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var participant = await _context.Participants.FindAsync(Guid.Parse(participantId));
            if (participant == null || participant.Refreshtoken != refreshToken)
            {
                return Unauthorized("Invalid refresh token");
            }

            await SetTokensAsync(participant);

            return Ok(new { Token = Request.Cookies["AuthToken"] });
        }

        [HttpGet]
        public IActionResult Logout()
        {
            ClearCookies();
            return RedirectToAction("Login", "Auth");
        }

        private async Task SetTokensAsync(Participant participant)
        {
            var token = _tokenService.GenerateToken(participant);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(participant);

            participant.Lastlogin = DateTime.UtcNow;
            participant.Refreshtoken = refreshToken;
            await _context.SaveChangesAsync();

            SetCookie("AuthToken", token, BearerTokenExpiry);
            SetCookie("RefreshToken", refreshToken, RefreshTokenExpiry);
        }

        private void SetCookie(string cookieName, string value, int expirationInSeconds)
        {
            Response.Cookies.Append(cookieName, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddSeconds(expirationInSeconds)
            });
        }

        private void ClearCookies()
        {
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");
        }
    }
}