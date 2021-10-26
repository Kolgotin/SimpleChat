using SimpleChat.Common.Implementations;
using SimpleChat.Common.Interfaces;
using System;
using System.Net.Sockets;
using System.Text;

namespace SimpleChat.ConsoleServer
{
    internal class SimpleUser : IUser
    {
        TcpClient _client;
        IServerWorker _server; // объект сервера

        internal SimpleUser(
            TcpClient tcpClient,
            ServerWorker serverObject)
        {
            _client = tcpClient;
            _server = serverObject;
        }

        protected internal NetworkStream Stream { get; private set; }

        public string Name { get; private set; }

        internal void Process()
        {
            try
            {
                Stream = _client.GetStream();
                Name = GetNewMessage();
                _server.KeepUser(this);

                while (true)
                {
                    var messageBody = GetNewMessage();
                    var message = TakeMessageByStr(messageBody);
                    _server.KeepMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _server.DisconnectUser(this);
                Close();
            }
        }

        public string GetNewMessage()
        {
            byte[] data = new byte[64];
            StringBuilder builder = new StringBuilder();
            int bytes;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        public IMessage TakeMessageByStr(string messageBody)
            => new SimpleMessage(this, messageBody);

        protected internal void Close()
        {
            if (Stream != null)
            {
                Stream.Close();
            }

            if (_client != null)
            {
                _client.Close();
            }
        }
    }
}
