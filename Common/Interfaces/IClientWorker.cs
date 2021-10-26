namespace SimpleChat.Common.Interfaces
{
    public interface IClientWorker
    {
        bool CheckIn();

        bool Auth(string login, string password);

        void SendMessage();

        void TakeMessage();
    }
}
