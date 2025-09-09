using SalesService.Services.Interfaces;

namespace SalesService.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value;
                return int.TryParse(userIdClaim, out var id) ? id : 0;
            }
        }

        public string Role =>
            _httpContextAccessor.HttpContext?.User.FindFirst("Role")?.Value ?? string.Empty;
    }

}