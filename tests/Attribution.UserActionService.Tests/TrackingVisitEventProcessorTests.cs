using System.Threading;
using System.Threading.Tasks;
using Attribution.UserActionService.Models.YouDoEvents;
using Attribution.UserActionService.Processors;
using Attribution.UserActionService.Tests.Builders;
using Attribution.UserActionService.Tests.Common;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Attribution.UserActionService.Tests
{
    [Trait("Category", "Unit")]
    public class TrackingVisitEventProcessorTests : UserActionServiceTests
    {
        private readonly TrackingVisitEventProcessor _processor;

        public TrackingVisitEventProcessorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _processor = new TrackingVisitEventProcessor(
                Mock.Of<ILogger<TrackingVisitEventProcessor>>(),
                ChannelAttributesManagerMock.Object);
        }

        [Fact]
        public async Task ProcessMessageAsync_MessageBodyIsNull_Nack()
        {
            var receivedMessage = MessageBuilder.BuildWithoutBody<UtmVisitTraceEvent>();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);

            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }

        [Fact]
        public async Task ProcessMessageAsync_BodyContextIsNull_Nack()
        {
            var receivedMessage = new UtmVisitTraceEventBuilder().Build().ToMessage();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);

            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }

        [Fact]
        public async Task ProcessMessageAsync_FilledBody_Ack()
        {
            var receivedMessage = new UtmVisitTraceEventBuilder().WithContext().Build().ToMessage();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);

            Mock.Get(receivedMessage).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_HashIsZero_Nack()
        {
            var userRegisterEvent = new UtmVisitTraceEventBuilder()
                .WithContext()
                .WithHash(0L)
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(userRegisterEvent, CancellationToken.None);

            Mock.Get(userRegisterEvent).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_AttributionDataIsNull_Nack()
        {
            var userRegisterEvent = new UtmVisitTraceEventBuilder()
                .WithContext()
                .WithAttributionData(null)
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(userRegisterEvent, CancellationToken.None);

            Mock.Get(userRegisterEvent).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_AttributionDataIsWhitespace_Nack()
        {
            var userRegisterEvent = new UtmVisitTraceEventBuilder()
                .WithContext()
                .WithAttributionData("\t")
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(userRegisterEvent, CancellationToken.None);

            Mock.Get(userRegisterEvent).Verify(m => m.Nack(), Times.Once);
        }

        [Fact]
        public async Task ProcessMessageAsync_AddesChannelAttributes()
        {
            var userRegisterEvent = new UtmVisitTraceEventBuilder().WithContext().Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);

            ChannelAttributesManagerMock.Verify(m => 
                m.CreateChannelAttributesDataAsync(userRegisterEvent.Hash, userRegisterEvent.AttributionData), Times.Once);
        }
    }
}