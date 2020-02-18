using Coworking.Rabbit;
using Moq;

namespace Attribution.UserActionService.Tests.Builders
{
    public static class MessageBuilder
    {
        public static IReceivedMessage<TBody> BuildWithoutBody<TBody>() where TBody : class
        {
            return ToMessage<TBody>(null);
        }
        
        public static IReceivedMessage<TBody> ToMessage<TBody>(this TBody body) where TBody : class
        {
            var message = new Mock<IReceivedMessage<TBody>>();
            message.SetupGet(m => m.Body).Returns(body);
            return message.Object;
        }
    }
}