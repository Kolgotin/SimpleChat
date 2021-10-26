namespace SimpleChat.Common.Interfaces
{
    public interface IMessage
    {
        string Body { get; set; }

        IUser Sender { get; }
    }
}
