namespace SimpleChat.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new ClientWorker();
            c.Run();
        }
    }
}
