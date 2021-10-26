using SimpleChat.Common.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SimpleChat.ConsoleClient
{
    class ClientWorker : IClientWorker
    {
        string userName;
        private const string host = "127.0.0.1";
        private const int port = 8888;
        TcpClient client;
        NetworkStream stream;

        public void Run()
        {
            client = new TcpClient();
            try
            {
                if (!CheckIn())
                {
                    return;
                }

                Thread receiveThread = new Thread(new ThreadStart(TakeMessage));
                receiveThread.Start();
                Console.WriteLine($"Добро пожаловать, {userName}");
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        
        public bool CheckIn()
        {
            try
            {
                Console.WriteLine("Введите свой логин: ");
                userName = Console.ReadLine();

                client.Connect(host, port);
                stream = client.GetStream();

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                return true;
            }
            catch
            {
                Console.WriteLine("Вход не удался");
                return false;
            }
        }

        public bool Auth(string login, string password)
        {
            throw new NotImplementedException();
        }

        public void SendMessage()
        {
            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }
        }

        public void TakeMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!");
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        void Disconnect()
        {
            if (stream != null)
            {
                stream.Close();
            }

            if (client != null)
            {
                client.Close();
            }

            Environment.Exit(0);
        }
    }
}
