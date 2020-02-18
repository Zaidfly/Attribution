using System.Threading.Tasks;
using Attribution.Domain.Dal;
using Attribution.Domain.Models;
using LinqToDB;

namespace Attribution.Dal.Repositories
{
    public class UserActionRepository : IUserActionRepository
    {
        private readonly AttributionDb _dbContext;
        
        public UserActionRepository(AttributionDb dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task SaveUserActionAsync(UserAction userAction)
        {
            await _dbContext.UserActions.InsertOrUpdateAsync(
                () => new UserActionDb
                {
                    ActionDatetimeUtc = userAction.ActionDateTimeUtc.ToUniversalTime(),
                    UserId = userAction.UserId,
                    ActionTypeId = (int)userAction.Type,
                    ObjectType = (int)userAction.ObjectType,
                    ObjectId = userAction.ObjectId,
                    ObjectGuid = userAction.ObjectGuid,
                    InitiatorId = userAction.InitiatorId,
                    InitiatorType = (int?)userAction.InitiatorType,
                    ChannelAttributesId = userAction.ChannelAttributesId,
                    ActualHash = userAction.ActualHash,
                    LingeringHash = userAction.LingeringHash
                },
                _ => new UserActionDb
                {
                    ChannelAttributesId = userAction.ChannelAttributesId,
                    InitiatorId = userAction.InitiatorId,
                    InitiatorType = (int?)userAction.InitiatorType,
                    ObjectGuid = userAction.ObjectGuid,
                    ActualHash = userAction.ActualHash,
                    LingeringHash = userAction.LingeringHash
                },
                () => new UserActionDb
                {
                    UserId = userAction.UserId,
                    ActionTypeId = (int)userAction.Type,
                    ObjectType = (int)userAction.ObjectType,
                    ObjectId = userAction.ObjectId
                });
        }
    }
}