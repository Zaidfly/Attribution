using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models;
using Attribution.UserActionService.Models.YouDoEvents;
using Microsoft.Extensions.Logging;

namespace Attribution.UserActionService.Processors
{
    public class TaskOfferCreatedProcessor : MessageProcessorBase<TaskOfferCreatedEvent>
    {
        private readonly IUserActionManager _userActionManager;

        public TaskOfferCreatedProcessor
        (
            ILogger<TaskCreatedProcessor> logger,
            IChannelAttributesManager channelAttributesManager,
            IUserActionManager userActionManager
        ) : base(logger, channelAttributesManager)
        {
            _userActionManager = userActionManager;
        }

        protected override async Task SaveUserActionAsync(
            TaskOfferCreatedEvent data, 
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
                UserId = data.TaskOffer.CreatorId,
                Type = UserActionType.TaskOfferCreated,
                ActionDateTimeUtc = data.Context.Timestamp,
                ObjectType = ObjectType.TaskOffer,
                ObjectId = data.TaskOffer.Id,
                ObjectGuid = data.TaskOffer.Guid,
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