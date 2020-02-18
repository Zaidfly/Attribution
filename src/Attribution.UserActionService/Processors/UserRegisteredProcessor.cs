using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models;
using Attribution.UserActionService.Models.YouDoEvents;
using Microsoft.Extensions.Logging;

namespace Attribution.UserActionService.Processors
{
    public class UserRegisteredProcessor : MessageProcessorBase<UserRegisteredEvent>
    {
        private readonly IUserActionManager _userActionManager;
        
        public UserRegisteredProcessor(
            ILogger<UserRegisteredProcessor> logger,
            IUserActionManager userActionManager,
            IChannelAttributesManager channelAttributesManager) : base(logger, channelAttributesManager)
        {
            _userActionManager = userActionManager;
        }

        protected override async Task SaveUserActionAsync(
            UserRegisteredEvent data, 
            MobileAppType? mobileAppType,
            long channelAttributesId,
            AttributionDataHashes attributionDataHashes,
            CancellationToken cancellation)
        {
            if (mobileAppType == null)
            {
                var userAction = new UserAction
                {
                    UserId = data.UserId,
                    Type = UserActionType.UserRegistered,
                    ActionDateTimeUtc = data.Context.Timestamp,
                    ObjectType = ObjectType.User,
                    ObjectId = data.UserId,
                    ObjectGuid = data.UserGuid,
                    InitiatorId = data.Context.Initiator?.Id,
                    InitiatorType = data.Context.Initiator?.Type,
                    ChannelAttributesId = channelAttributesId,
                    ActualHash = attributionDataHashes.ActualHash,
                    LingeringHash = attributionDataHashes.LingeringHash
                };
                
                await _userActionManager.SaveUserActionAsync(userAction);
            }
        }
    }
}