using System;
using Attribution.Domain.Managers;
using Attribution.Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace Attribution.UserActionService.Tests.Common
{
    public abstract class UserActionServiceTests
    {
        protected readonly Mock<IUserActionManager> UserActionManagerMock;
        protected readonly Mock<IChannelAttributesManager> ChannelAttributesManagerMock;
        protected UserAction SavedUserAction;
        private readonly ITestOutputHelper _testOutputHelper;
        
        protected UserActionServiceTests(ITestOutputHelper testOutputHelper) 
        {
            _testOutputHelper = testOutputHelper;
            
            UserActionManagerMock = new Mock<IUserActionManager>();
            UserActionManagerMock
                .Setup(m => m.SaveUserActionAsync(It.IsAny<UserAction>()))
                .Callback<UserAction>(ua => SavedUserAction = ua);

            ChannelAttributesManagerMock = new Mock<IChannelAttributesManager>();
            ChannelAttributesManagerMock
                .Setup(m => m.GetChannelAttributesIdAsync(It.IsAny<long?>()))
                .ReturnsAsync(145L); //direct
        }

        protected void SetupChannelAttributesManagerMock(long? utmInfoHash, long channelAttributesId)
        {
            ChannelAttributesManagerMock
                .Setup(m => m.GetChannelAttributesIdAsync(utmInfoHash))
                .ReturnsAsync(channelAttributesId);
        }

        protected ILogger<T> CreateTestLogger<T>()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(_testOutputHelper));
            return loggerFactory.CreateLogger<T>();
        }
        
        private class XunitLoggerProvider : ILoggerProvider
        {
            private readonly ITestOutputHelper _testOutputHelper;

            public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }

            public ILogger CreateLogger(string categoryName)
                => new XunitLogger(_testOutputHelper, categoryName);

            public void Dispose() { }
            
            private class XunitLogger : ILogger
            {
                private readonly ITestOutputHelper _testOutputHelper;
                private readonly string _categoryName;

                public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
                {
                    _testOutputHelper = testOutputHelper;
                    _categoryName = categoryName;
                }

                public IDisposable BeginScope<TState>(TState state) => NoopDisposable.Instance;
                public bool IsEnabled(LogLevel logLevel) => true;
                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    _testOutputHelper.WriteLine($"{_categoryName} [{eventId}] {formatter(state, exception)}");
                    if (exception != null)
                        _testOutputHelper.WriteLine(exception.ToString());
                }

                private class NoopDisposable : IDisposable
                {
                    public static readonly NoopDisposable Instance = new NoopDisposable();
                    public void Dispose() { }
                }
            }
        }
    }
}