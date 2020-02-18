using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models;
using Attribution.UserActionService.Models.YouDoEvents;
using Microsoft.Extensions.Logging;

namespace Attribution.UserActionService.Processors
{
    public class TaskCreatedProcessor : MessageProcessorBase<TaskCreatedEvent>
    {
        private readonly IUserActionManager _userActionManager;
        
        public TaskCreatedProcessor
        (
            ILogger<TaskCreatedProcessor> logger,
            IChannelAttributesManager channelAttributesManager,
            IUserActionManager userActionManager
        ) : base(logger, channelAttributesManager)
        {
            _userActionManager = userActionManager;
        }

        protected override async Task SaveUserActionAsync(
            TaskCreatedEvent data, 
            MobileAppType? mobileAppType, 
            long channelAttributesId,
            AttributionDataHashes attributionDataHashes,
            CancellationToken cancellation)
        {
            if (mobileAppType != null)
            {
                // обрабатываем создание заданий только из веба 
                return;
            }
            
            var userAction = new UserAction
            {
                UserId = data.NewTaskEntity.CreatorId,
                Type = UserActionType.TaskCreated,
                ActionDateTimeUtc = data.Context.Timestamp,
                ObjectType = ObjectType.Task,
                ObjectId = data.NewTaskEntity.Id,
                ObjectGuid = data.NewTaskEntity.Attributes?.TaskGuid,
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