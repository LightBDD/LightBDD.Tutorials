using System;
using System.Threading.Tasks;
using LightBDD.Framework.Messaging;
using Rebus.Handlers;

namespace OrdersService.ServiceTests
{
    class MessageDispatcher : IMessageSource, IHandleMessages<object>
    {
        public static readonly MessageDispatcher Instance = new MessageDispatcher();
        public event Action<object> OnMessage;
        
        private MessageDispatcher() { }
        public Task Handle(object message)
        {
            OnMessage?.Invoke(message);
            return Task.CompletedTask;
        }
    }
}