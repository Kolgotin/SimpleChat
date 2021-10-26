using SimpleChat.Common.Interfaces;
using System;

namespace SimpleChat.Common.Implementations
{
    public class SimpleMessage : IMessage
    {
        public string Body { get; set; }

        public DateTime SendTime { get; set; }

        public IUser Sender { get; set; }

        public SimpleMessage(IUser sender, string body)
        {
            Sender = sender;
            Body = body;
            SendTime = DateTime.Now;
        }

        public override string ToString() => $"{SendTime:g} {Sender.Name}: {Body}";
    }
}
