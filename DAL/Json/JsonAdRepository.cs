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
    }

    public async Task<IReadOnlyList<Ad>> GetAllAsync() => await ReadAllAsync();

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