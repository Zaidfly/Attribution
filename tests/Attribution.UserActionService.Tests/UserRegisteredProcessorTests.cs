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
    public class UserRegisteredProcessorTests : UserActionServiceTests
    {
        private readonly UserRegisteredProcessor _processor;
        
        public UserRegisteredProcessorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _processor = new UserRegisteredProcessor(
                Mock.Of<ILogger<UserRegisteredProcessor>>(),
                UserActionManagerMock.Object,
                ChannelAttributesManagerMock.Object);
        }

        [Fact]
        public async Task ProcessMessageAsync_MessageBodyIsNull_Nack()
        {
            var receivedMessage = MessageBuilder.BuildWithoutBody<UserRegisteredEvent>();
            
            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }

        [Fact]
        public async Task ProcessMessageAsync_BodyContextIsNull_Nack()
        {
            var receivedMessage = new UserRegisterEventBuilder().Build().ToMessage();
            
            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Nack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_FilledBody_Ack()
        {
            var receivedMessage = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext())
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(receivedMessage, CancellationToken.None);
            
            Mock.Get(receivedMessage).Verify(m => m.Ack(), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction()
        {
            var userRegisterEvent = new UserRegisterEventBuilder().WithContext().Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Once);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionType()
        {
            var userRegisterEvent = new UserRegisterEventBuilder().WithContext().Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(UserActionType.UserRegistered,SavedUserAction.Type);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectType()
        {
            var userRegisterEvent = new UserRegisterEventBuilder().WithContext().Build();
            
            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(ObjectType.User, SavedUserAction.ObjectType);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectId()
        {
            var userRegisterEvent = new UserRegisterEventBuilder().WithUserId(3).WithContext().Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(userRegisterEvent.UserId, SavedUserAction.ObjectId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ObjectGuid()
        {
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithGuid()
                .WithContext()
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(userRegisterEvent.UserGuid, SavedUserAction.ObjectGuid);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorId()
        {
            const int initiatorId = 55;
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorId(initiatorId)))
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorId, SavedUserAction.InitiatorId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_InitiatorType()
        {
            const InitiatorType initiatorType = InitiatorType.Moderator;
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithInitiator(new OperationInitiatorBuilder()
                        .WithInitiatorType(initiatorType)))
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(initiatorType, SavedUserAction.InitiatorType);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_UserId()
        {
            var userRegisterEvent = new UserRegisterEventBuilder().WithUserId(3).WithContext().Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(userRegisterEvent.UserId, SavedUserAction.UserId);
        }

        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActionDate()
        {
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithTimeStamp(new DateTime(2011, 11, 11, 11, 11, 11)))
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(userRegisterEvent.Context.Timestamp, SavedUserAction.ActionDateTimeUtc.ToUniversalTime());
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ActualHash()
        {
            const long actualHash = 42;
            
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(actualHash)))
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(actualHash, SavedUserAction.ActualHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_LingeringHash()
        {
            const long lingeringHash = 42;
            
            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithLingeringAttributionDataHash(lingeringHash)))
                .Build();

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(lingeringHash, SavedUserAction.LingeringHash);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_SavesUserAction_ChannelAttributesHash()
        {
            const long expectedChannelAttributesId = 42;

            var userRegisterEvent = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder()
                    .WithWebContext(new WebContextBuilder()
                        .WithAttributionDataHash(5)))
                .Build();
            
            SetupChannelAttributesManagerMock(
                userRegisterEvent.Context.WebContext.AttributionDataHash, 
                expectedChannelAttributesId);

            await _processor.ProcessMessageAsync(userRegisterEvent.ToMessage(), CancellationToken.None);
            
            Assert.Equal(expectedChannelAttributesId, SavedUserAction.ChannelAttributesId);
        }
        
        [Fact]
        public async Task ProcessMessageAsync_AndroidClient_DoesNotCallUserActionManager()
        {
            var message = new UserRegisterEventBuilder()
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
            var message = new UserRegisterEventBuilder()
                .WithContext(new EventContextBuilder().WithWebContext().WithClientType(ClientType.iPhoneApplication))
                .Build()
                .ToMessage();

            await _processor.ProcessMessageAsync(message, CancellationToken.None);
            
            UserActionManagerMock.Verify(m => m.SaveUserActionAsync(It.IsAny<UserAction>()), Times.Never);
            Mock.Get(message).Verify(m => m.Ack(), Times.Once);
        }
    }
}