using System.Threading.Tasks;

using Attribution.Domain.Dal;
using Attribution.Domain.Models;

namespace Attribution.Domain.Managers
{
    public class UserActionManager : IUserActionManager
    {
        private readonly IUserActionRepository _userActionRepository;
        
        public UserActionManager(IUserActionRepository userActionRepository)
        {
            _userActionRepository = userActionRepository;
        }

        public Task SaveUserActionAsync(UserAction userAction)
        {
            return _userActionRepository.SaveUserActionAsync(userAction);
        }
    }
}