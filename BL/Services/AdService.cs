using Common.InterfacesBL;
using Common.InterfacesDAL;
using Common.Models;
using Google.Apis.Auth;
using System.Net.Http;

namespace BL;

public class AdService : IAdService
{
    private readonly IAdRepository _repo;
    public AdService(IAdRepository repo) { _repo = repo; }

    public async Task<IReadOnlyList<Ad>> GetAllAsync(string? q = null, double? lat = null, double? lng = null, double? radiusKm = null)
    {
        var ads = await _repo.GetAllAsync();
        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLowerInvariant();
            ads = ads.Where(a => a.Title.ToLowerInvariant().Contains(q) || a.Description.ToLowerInvariant().Contains(q)).ToList();
        }
        if (lat.HasValue && lng.HasValue && radiusKm.HasValue && radiusKm > 0)
        {
            ads = ads.Where(a => a.Location is not null && DistanceKm(lat.Value, lng.Value, a.Location!.Lat, a.Location!.Lng) <= radiusKm.Value).ToList();
        }
        return ads.OrderByDescending(a => a.CreatedAt).ToList();
    }

    public Task<Ad?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

    public async Task<Ad> CreateAsync(Ad ad, string currentUser)
    {
        if (string.IsNullOrWhiteSpace(ad.Title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(currentUser)) throw new ArgumentException("User email is required");
        ad.CreatedBy = currentUser;
        ad.CreatedAt = DateTime.UtcNow;
        return await _repo.AddAsync(ad);
    }

    public async Task<Ad?> UpdateAsync(Guid id, Ad updated, string currentUser)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return null;
        if (!string.Equals(existing.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("You can only edit your own ads");
        existing.Title = updated.Title;
        existing.Description = updated.Description;
        existing.Location = updated.Location;
        existing.UpdatedAt = DateTime.UtcNow;
        return await _repo.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(Guid id, string currentUser)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return false;
        if (!string.Equals(existing.CreatedBy, currentUser, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("You can only delete your own ads");
        return await _repo.DeleteAsync(id);
    }

    // Haversine distance
    private static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    private static double ToRad(double v) => v * Math.PI / 180.0;


    public async Task<GoogleUser> ValidateGoogleToken(string idToken)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { "YOUR_GOOGLE_CLIENT_ID" } // ה-clientId שלך מהקונסולה של גוגל
        });
        return new GoogleUser
        {
            GoogleId = payload.Subject,   // מזהה ייחודי של המשתמש בגוגל
            Email = payload.Email,
            Name = payload.Name,
            Picture = payload.Picture
        };
    }

}