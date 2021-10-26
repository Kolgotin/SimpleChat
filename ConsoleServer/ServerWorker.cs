using SimpleChat.Common.Implementations;
using SimpleChat.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace SimpleChat.ConsoleServer
{
    internal class ServerWorker : IServerWorker
    {
        private const int MessagesCountForNewUser = 10;

        private TcpListener tcpListener;
        private List<SimpleUser> users = new List<SimpleUser>();
        private List<SimpleMessage> ChatHistory = new List<SimpleMessage>();

        private static ServerWorker _instance;

        internal static ServerWorker Instance
            => _instance ?? (_instance = new ServerWorker());

        private ServerWorker()
        {
            tcpListener = new TcpListener(IPAddress.Any, 8888);
        }

        public void Listen()
        {
            try
            {
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    SimpleUser user = new SimpleUser(tcpClient, this);
                    users.Add(user);
                    Thread clientThread = new Thread(new ThreadStart(user.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        public void SendMessageToAllClients(IMessage message)
        {
            var simpleMessage = message as SimpleMessage;
            var data = Encoding.Unicode.GetBytes(message.ToString());

            users.ForEach(x =>
            {
                if (!x.Equals(message.Sender))
                {
                    x.Stream.Write(data, 0, data.Length);
                }
            });
        }

        public void KeepMessage(IMessage message)
        {
            Console.WriteLine(message.ToString());
            SendMessageToAllClients(message);
            ChatHistory.Add(message as SimpleMessage);
        }

        public void KeepUser(IUser user)
        {
            SendLastMessagesForNewUser(user as SimpleUser);
            Console.WriteLine($"{user.Name} вошел в чат");
            var messageBody = "Вошел в чат";
            var message = new SimpleMessage(user, messageBody);
            SendMessageToAllClients(message);
        }

        public void RegisterNewUser()
        {
            throw new NotImplementedException();
        }

        public void DisconnectUser(IUser user)
        {
            if (users.Contains(user))
            {
                users.Remove(user as SimpleUser);
            }

            Console.WriteLine($"{user.Name} покинул чат");
            var messageBody = "Покинул чат";
            var message = new SimpleMessage(user, messageBody);
            SendMessageToAllClients(message);
        }


        private void Disconnect()
        {
            tcpListener.Stop();
            users.ForEach(x => x.Close());
            Environment.Exit(0);
        }

        private void SendLastMessagesForNewUser(SimpleUser user)
        {
            var slipCount =
                ChatHistory.Count > MessagesCountForNewUser
                ?
                ChatHistory.Count - MessagesCountForNewUser
                : 0;

            var lastMessages = ChatHistory
                .OrderBy(x => x.SendTime)
                .Skip(slipCount)
                .Take(MessagesCountForNewUser)
                .ToList();

            var lastMessagesStr = string.Join("\n", lastMessages.Select(x=>x.ToString()));
            var data = Encoding.Unicode.GetBytes(lastMessagesStr);

            user.Stream.Write(data, 0, data.Length);
        }
    }
}
