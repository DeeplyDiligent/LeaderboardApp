namespace LeaderboardApp.Security
{
    public class JwtTokenCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the token exists in the cookies
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                var token = context.Request.Cookies["AuthToken"];

                // Add the token to the Authorization header
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }
}
