using System.Text.Json;
using Common.InterfacesDAL;
using Common.Models;

namespace DAL.Json;

public class JsonAdRepository : IAdRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonAdRepository(string filePath)
    {
        _filePath = filePath;
        if (!File.Exists(_filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            File.WriteAllText(_filePath, "[]");
        }
    }

    private async Task<List<Ad>> ReadAllAsync()
    {
        using var stream = File.OpenRead(_filePath);
        var ads = await JsonSerializer.DeserializeAsync<List<Ad>>(stream) ?? new List<Ad>();
        return ads;
    }

    private async Task WriteAllAsync(List<Ad> ads)
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, ads, _jsonOptions);
        await stream.FlushAsync();
    }

    public async Task<IReadOnlyList<Ad>> GetAllAsync(string? q = null, double? lat = null, double? lng = null, double? radiusKm = null)
    {
        var ads = await ReadAllAsync();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim().ToLowerInvariant();
            ads = ads
                .Where(a => a.Title.ToLowerInvariant().Contains(q) || a.Description.ToLowerInvariant().Contains(q))
                .ToList();
        }

        if (lat.HasValue && lng.HasValue && radiusKm.HasValue && radiusKm > 0)
        {
            ads = ads
                .Where(a => a.Location is not null && DistanceKm(lat.Value, lng.Value, a.Location!.Lat, a.Location!.Lng) <= radiusKm.Value)
                .ToList();
        }

        return ads.OrderByDescending(a => a.CreatedAt).ToList();
    }

    // חישוב מרחק
    private static double DistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRad(double v) => v * Math.PI / 180.0;
    public async Task<Ad?> GetByIdAsync(Guid id)
    {
        var ads = await ReadAllAsync();
        return ads.FirstOrDefault(a => a.Id == id);
    }

    public async Task<Ad> AddAsync(Ad ad)
    {
        var ads = await ReadAllAsync();
        ads.Add(ad);
        await WriteAllAsync(ads);
        return ad;
    }

    public async Task<Ad?> UpdateAsync(Ad ad)
    {
        var ads = await ReadAllAsync();
        var idx = ads.FindIndex(a => a.Id == ad.Id);
        if (idx == -1) return null;
        ads[idx] = ad;
        await WriteAllAsync(ads);
        return ad;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var ads = await ReadAllAsync();
        var removed = ads.RemoveAll(a => a.Id == id) > 0;
        if (removed) await WriteAllAsync(ads);
        return removed;
    }
}