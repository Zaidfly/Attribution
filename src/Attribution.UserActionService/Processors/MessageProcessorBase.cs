using System;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models;
using Attribution.UserActionService.Models.YouDoEvents;
using Coworking;
using Coworking.Rabbit;
using Microsoft.Extensions.Logging;

namespace Attribution.UserActionService.Processors
{
    public abstract class MessageProcessorBase<TBody> : MessageProcessor<TBody> where TBody: YouDoEventBase
    {
        private readonly IChannelAttributesManager _channelAttributesManager;
        protected readonly ILogger Logger;

        protected MessageProcessorBase(ILogger logger, IChannelAttributesManager channelAttributesManager)
        {
            Logger = logger;
            _channelAttributesManager = channelAttributesManager;
        }
        
        public override async Task ProcessMessageAsync(IReceivedMessage<TBody> message, CancellationToken cancellation)
        {
            try
            {
                var data = message.Body;
                if (data?.Context == null)
                {
                    Logger.LogError($"Message body is incorrect\n{nameof(data)}: {{@message:j}}", data);
                    message.Nack();
                    return;
                }
                
                if (data.Context?.WebContext == null)
                {
                    message.Ack();
                }
            
                var mobileAppType = GetMobileAppType(data.Context.ClientType);

                var attributionHashes = new AttributionDataHashes(
                    data.Context.WebContext?.AttributionDataHash,
                    data.Context.WebContext?.AttributionDataLingeringHash);
                
                var channelAttributesId = await _channelAttributesManager.GetChannelAttributesIdAsync(attributionHashes.ActualHash);
                
                await SaveUserActionAsync(message.Body, mobileAppType, channelAttributesId, attributionHashes, cancellation);
                
                message.Ack();
            }
            catch (Exception ex)
            {
                Logger.LogError(0, ex, "Unexpected error:\nMessage: {@message:j}", message.Body);
                message.Requeue();
            }
        }

        protected abstract Task SaveUserActionAsync(
            TBody data, 
            MobileAppType? mobileAppType, 
            long channelAttributesId,
            AttributionDataHashes attributionDataHashes,
            CancellationToken cancellation);
        
        protected static MobileAppType? GetMobileAppType(ClientType clientType)
        {
            return clientType == ClientType.AndroidApplication
                ? MobileAppType.Android
                : clientType == ClientType.iPhoneApplication
                    ? MobileAppType.Ios
                    : (MobileAppType?) null;
        }
    }
}