namespace SimpleChat.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerWorker.Instance.Listen();
        }
    }
}
