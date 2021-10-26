namespace SimpleChat.Common.Interfaces
{
    public interface IUser
    {
        string Name { get; }

        string GetNewMessage();

        IMessage TakeMessageByStr(string messageBody);
    }
}
