using System.Threading.Tasks;
using Attribution.Domain.Models;

namespace Attribution.Domain.Dal
{
    public interface IUserActionRepository
    {
        Task SaveUserActionAsync(UserAction userAction);
    }
}