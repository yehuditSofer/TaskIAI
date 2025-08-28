using Common.Models;
namespace Common.InterfacesDAL;
public interface IAdRepository
{
    Task<IReadOnlyList<Ad>> GetAllAsync();
    Task<Ad?> GetByIdAsync(Guid id);
    Task<Ad> AddAsync(Ad ad);
    Task<Ad?> UpdateAsync(Ad ad);
    Task<bool> DeleteAsync(Guid id);
}