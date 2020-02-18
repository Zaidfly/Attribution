using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitLink;
using RabbitLink.Messaging;
using RabbitLink.Producer;
using RabbitLink.Topology;
using YouDo.Caching.Redis;

namespace Attribution.Dal.Redis
{
    /// <summary>
    /// Producer для удаления ключей из Redis
    /// </summary>
    internal class RedisInvalidationProducer : IInvalidationFallback, IDisposable
    {
        private readonly ILinkProducer _producer;

        public RedisInvalidationProducer(ILink linkManager)
        {
            _producer = linkManager.Producer
                .Exchange(cfg => cfg.ExchangeDeclare("youdo.cache.invalidation", LinkExchangeType.Direct))
                .Build();
        }

        public async Task<bool> FallbackAsync(string key, CancellationToken token)
        {
            var msg = new LinkPublishMessage<RedisInvalidationMessage>(
                new RedisInvalidationMessage(key),
                new LinkMessageProperties
                {
                    DeliveryMode = LinkDeliveryMode.Persistent
                }
            );
            await _producer.PublishAsync(msg).ConfigureAwait(false);
            return true;
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}