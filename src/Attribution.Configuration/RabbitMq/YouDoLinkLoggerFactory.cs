using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace Attribution.Configuration.RabbitMq
{
    internal class LinkLoggerFactory : ILinkLoggerFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public LinkLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILinkLogger CreateLogger(string name)
            => new LinkLogger(_loggerFactory.CreateLogger(name));
        
        private class LinkLogger : ILinkLogger
        {
            private readonly ILogger _logger;

            public LinkLogger(ILogger logger)
            {
                _logger = logger;
            }

            public void Dispose()
            {

            }

            public void Write(LinkLoggerLevel level, string message)
            {
                switch (level)
                {
                    case LinkLoggerLevel.Error:
                        _logger.LogError($"RabbitLink:: {message}");
                        break;
                    case LinkLoggerLevel.Warning:
                        _logger.LogWarning($"RabbitLink:: {message}");
                        break;
                    case LinkLoggerLevel.Info:
                        _logger.LogInformation($"RabbitLink:: {message}");
                        break;                    
                }
            }
        }
    }
}