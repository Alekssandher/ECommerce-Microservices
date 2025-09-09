namespace SalesService.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string Role { get; }
    }
}