using NetGameRunning;
using NetSystem;
using System;
using System.Collections.Generic;

namespace AE_NetWork
{
    public class MessagePool
    {
        private Dictionary<int, Type> messages = new Dictionary<int, Type>();
        private Dictionary<int, Type> handlers = new Dictionary<int, Type>();

        public MessagePool()
        {
            //<route>
            Register(1, typeof(QuitMessage), typeof(QuitMessageHandler));
            Register(2, typeof(HeartMessage), typeof(HeartMessageHandler));
            Register(10000, typeof(PositionMessage), typeof(PositionMessageHandler));
            Register(10001, typeof(PlayerMessage), typeof(PlayerMessageHandler));
            //</route>
        }

        private void Register(int id, Type message, Type handler)
        {
            messages.Add(id, message);
            handlers.Add(id, handler);
        }

        public BaseMessage GetMessage(int id)
        {
            if (messages.ContainsKey(id))
            {
                object message = Activator.CreateInstance(messages[id]);
                return message as BaseMessage;
            }
            return null;
        }

        public BaseHandler GetHandler(int id)
        {
            if (handlers.ContainsKey(id))
            {
                object handler = Activator.CreateInstance(handlers[id]);
                return handler as BaseHandler;
            }
            return null;
        }
    }
}
