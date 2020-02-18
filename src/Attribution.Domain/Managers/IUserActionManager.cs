using System.Threading.Tasks;
using Attribution.Domain.Models;

namespace Attribution.Domain.Managers
{
    public interface IUserActionManager
    {
        Task SaveUserActionAsync(UserAction userAction);
    }
}