using Common.Models;
namespace Common.InterfacesBL;
public interface IAdService
{
    Task<IReadOnlyList<Ad>> GetAllAsync(string? q = null, double? lat = null, double? lng = null, double? radiusKm = null);
    Task<Ad?> GetByIdAsync(Guid id);
    Task<Ad> CreateAsync(Ad ad, string currentUser);
    Task<Ad?> UpdateAsync(Guid id, Ad updated, string currentUser);
    Task<bool> DeleteAsync(Guid id, string currentUser);
    Task<GoogleUser> ValidateGoogleToken(string idToken);
}