using System;
using System.Threading;
using System.Threading.Tasks;
using Attribution.Domain.Models;
using Attribution.UserActionService.Models.YouDoEvents;
using Attribution.UserActionService.Processors;
using Attribution.UserActionService.Tests.Builders;
using Attribution.UserActionService.Tests.Common;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Attribution.UserActionService.Tests
{
    [Trait("Category", "Unit")]
    public class TaskCreatedProcessorTests : UserActionServiceTests
    {
        private readonly TaskCreatedProcessor _processor;
        public TaskCreatedProcessorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _processor = new TaskCreatedProcessor(
                CreateTestLogger<TaskCreatedProcessor>(),
                ChannelAttributesManagerMock.Object,
                UserActionManagerMock.Object);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_MessageBodyIsNull_Nack()
        {
            var receivedMessage = MessageBuilder.BuildWithoutBody<TaskCreatedEvent>();
            
            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }

        [Fact]
        public async Task ProcessMessageAsync_BodyContextIsNull_Nack()
        {
            var receivedMessage = new TaskCreatedEventBuilder().Build().ToMessage();
            
            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_FilledBody_Ack()
        {
            var receivedMessage = new TaskCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext())
                .WithTaskEntityDto()
                .Build().ToMessage();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_AndroidClient_DoesNotSaveUserAction()
        {
            var message = new TaskCreatedEventBuilder()
                .WithContext(
                    new EventContextBuilder()
                        .WithWebContext()
                        .WithClientType(ClientType.AndroidApplication))
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Never);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_IosClient_DoesNotSaveUserAction()
        {
            var message = new TaskCreatedEventBuilder()
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
            var message = new TaskCreatedEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext().WithClientType(ClientType.DesktopBrowser))
                .WithTaskEntityDto()
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Once);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionType()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder().WithContext().WithTaskEntityDto().Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(UserActionType.TaskCreated,SavedUserAction.Type);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectType()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder().WithContext().WithTaskEntityDto().Build();
            
            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(ObjectType.Task, SavedUserAction.ObjectType);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_UserId()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto(new TaskEntityDtoBuilder().WithCreatorId(7))
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskCreateEvent.NewTaskEntity.CreatorId, SavedUserAction.UserId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectGuid()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithContext()
                .WithTaskEntityDto(new TaskEntityDtoBuilder().WithGuid())
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskCreateEvent.NewTaskEntity.Attributes.TaskGuid, SavedUserAction.ObjectGuid);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectId()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto(new TaskEntityDtoBuilder().WithId(7))
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskCreateEvent.NewTaskEntity.Id, SavedUserAction.ObjectId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorId()
        {
            const int initiatorId = 55;
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorId(initiatorId)))
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorId, SavedUserAction.InitiatorId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorType()
        {
            const InitiatorType initiatorType = InitiatorType.Moderator;
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorType(initiatorType)))
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorType, SavedUserAction.InitiatorType);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionDate()
        {
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithTimeStamp(new DateTime(2011,11,11,11,11,11)))
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(taskCreateEvent.Context.Timestamp, SavedUserAction.ActionDateTimeUtc);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActualHash()
        {
            const long actualHash = 42;
            
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(actualHash)))
                .WithTaskEntityDto(new TaskEntityDtoBuilder().WithGuid())
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(actualHash, SavedUserAction.ActualHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_LingeringHash()
        {
            const long lingeringHash = 42;
            
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithLingeringAttributionDataHash(lingeringHash)))
                .Build();

            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(lingeringHash, SavedUserAction.LingeringHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ChannelAttributesId()
        {
            const long expectedChannelAttributesId = 42;
            
            var taskCreateEvent = new TaskCreatedEventBuilder()
                .WithTaskEntityDto()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(423)))
                .Build();
            
            SetupChannelAttributesManagerMock(taskCreateEvent.Context.WebContext.AttributionDataHash, expectedChannelAttributesId);
            
            await _processor.ProcessMessageAsync(taskCreateEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(expectedChannelAttributesId, SavedUserAction.ChannelAttributesId);
        }
    }
}