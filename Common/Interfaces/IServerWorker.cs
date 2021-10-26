namespace SimpleChat.Common.Interfaces
{
    public interface IServerWorker
    {
        void Listen();

        void SendMessageToAllClients(IMessage message);

        void KeepUser(IUser user);

        void RegisterNewUser();

        void KeepMessage(IMessage message);

        void DisconnectUser(IUser user);
    }
}
