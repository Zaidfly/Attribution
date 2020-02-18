using System;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Managers;
using Attribution.UserActionService.Models.YouDoEvents;
using Coworking;
using Coworking.Rabbit;
using Microsoft.Extensions.Logging;

namespace Attribution.UserActionService.Processors
{
    public class TrackingVisitEventProcessor : MessageProcessor<UtmVisitTraceEvent>
    {
        private readonly IChannelAttributesManager _channelAttributesManager;
        private readonly ILogger<TrackingVisitEventProcessor> _logger;
        
        public TrackingVisitEventProcessor
        (
            ILogger<TrackingVisitEventProcessor> logger,
            IChannelAttributesManager channelAttributesManager
        )
        {
            _channelAttributesManager = channelAttributesManager;
            _logger = logger;
        }

        public override async Task ProcessMessageAsync(
            IReceivedMessage<UtmVisitTraceEvent> message, 
            CancellationToken cancellation)
        {
            try
            {
                var data = message.Body;
                if (data?.Context == null)
                {
                    _logger.LogError($"Message body or {nameof(data.Context)} value cannot be empty: " +
                                     $"\n{nameof(data)}: {{@message:j}}", data);
                    message.Nack();
                    return;
                }

                if (data.Hash == 0L)
                {
                    _logger.LogError($"{data.Hash} value cannot be zero: {{@message:j}}", data);
                    message.Nack();
                }

                if (string.IsNullOrWhiteSpace(data.AttributionData))
                {
                    _logger.LogError($"{data.AttributionData} value cannot be empty: {{@message:j}}", data);
                    message.Nack();
                }

                await _channelAttributesManager.CreateChannelAttributesDataAsync(data.Hash, data.AttributionData);

                message.Ack();
            }
            catch (Exception ex)
            {
                _logger.LogError(0, ex, "Unexpected error:\nMessage: {@message:j}", message.Body);

                if (ex is ArgumentException)
                {
                    message.Nack(); // не возвращаем в очередь заведомо невалидные сообщения
                }
                
                message.Requeue();
            }
        }
    }
}