using System;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Models;
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
    public class TaskOfferCreatedProcessorTests : UserActionServiceTests
    {
        private readonly TaskOfferCreatedProcessor _processor;

        public TaskOfferCreatedProcessorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _processor = new TaskOfferCreatedProcessor(
                Mock.Of<ILogger<TaskCreatedProcessor>>(),
                ChannelAttributesManagerMock.Object,
                UserActionManagerMock.Object);
        }

        [Fact]
        public async Task ProcessMessageAsync_MessageBodyIsNull_Nack()
        {
            var receivedMessage = MessageBuilder.BuildWithoutBody<TaskOfferCreatedEvent>();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);

            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_BodyContextIsNull_Nack()
        {
            var receivedMessage = new TaskOfferCreatedEventBuilder().Build().ToMessage();
            
            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_FilledBody_Ack()
        {
            var receivedMessage = new TaskOfferCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext())
                .WithTaskOfferEntityDto()
                .Build().ToMessage();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_AndroidClient_DoesNotCallUserActionManager()
        {
            var message = new TaskOfferCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext().WithClientType(ClientType.AndroidApplication))
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Never);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_IosClient_DoesNotCallUserActionManager()
        {
            var message = new TaskOfferCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext().WithClientType(ClientType.iPhoneApplication))
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Never);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_WebClient_SavesUserAction()
        {
            var message = new TaskOfferCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext().WithClientType(ClientType.DesktopBrowser))
                .WithTaskOfferEntityDto()
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Once);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionType()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder().WithContext().WithTaskOfferEntityDto().Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(UserActionType.TaskOfferCreated,SavedUserAction.Type);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_UserId()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto(new TaskOfferEntityDtoBuilder().WithCreatorId(7))
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskOfferCreatedEvent.TaskOffer.CreatorId, SavedUserAction.UserId);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectType()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder().WithContext().WithTaskOfferEntityDto().Build();
            
            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(ObjectType.TaskOffer, SavedUserAction.ObjectType);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectId()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto(new TaskOfferEntityDtoBuilder().WithId(7))
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskOfferCreatedEvent.TaskOffer.Id, SavedUserAction.ObjectId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectGuid()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto(new TaskOfferEntityDtoBuilder().WithGuid())
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskOfferCreatedEvent.TaskOffer.Guid, SavedUserAction.ObjectGuid);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserActionWithoutObjectGuid_ObjectGuidIsNull()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Null(SavedUserAction.ObjectGuid);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorId()
        {
            const int initiatorId = 55;
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorId(initiatorId)))
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorId, SavedUserAction.InitiatorId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorType()
        {
            const InitiatorType initiatorType = InitiatorType.Moderator;
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorType(initiatorType)))
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorType, SavedUserAction.InitiatorType);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionDate()
        {
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithTimeStamp(new DateTime(2011,11,11,11,11,11)))
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskOfferCreatedEvent.Context.Timestamp, SavedUserAction.ActionDateTimeUtc);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActualHash()
        {
            const long actualHash = 42;
            
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(actualHash)))
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(actualHash, SavedUserAction.ActualHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_LingeringHash()
        {
            const long lingeringHash = 42;
            
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithLingeringAttributionDataHash(lingeringHash)))
                .Build();

            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(lingeringHash, SavedUserAction.LingeringHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ChannelAttributesHash()
        {
            const long expectedChannelAttributesId = 42;
            
            var taskOfferCreatedEvent = new TaskOfferCreatedEventBuilder()
                .WithTaskOfferEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(423)))
                .Build();
            
            SetupChannelAttributesManagerMock(taskOfferCreatedEvent.Context.WebContext.AttributionDataHash, expectedChannelAttributesId);
            
            await _processor.ProcessMessageAsync(taskOfferCreatedEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(expectedChannelAttributesId, SavedUserAction.ChannelAttributesId);
        }
    }
}