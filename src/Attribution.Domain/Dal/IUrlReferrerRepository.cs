using System.Collections.Generic;
using System.Threading.Tasks;
using Attribution.Domain.Models;

namespace Attribution.Domain.Dal
{
    public interface IUrlReferrerRepository
    {
        Task<IReadOnlyCollection<UrlReferrer>> GetAllAsync(bool isDeleted = false);
        Task<UrlReferrer> GetByHostAsync(string host);
        Task<UrlReferrer> GetByIdAsync(int id);
        Task AddOrUpdateAsync(UrlReferrer urlReferrer);
        Task RemoveByHostAsync(string host);
    }
}