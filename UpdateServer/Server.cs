using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Workers;

namespace UpdateServer
{
    public class Server
    {
        public static IWorker ServerInputWorker;
        public static IWorker ClientInputWorker;
        public static Socket ImSocet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static bool NeedExit = true;

        private Task InputThread;

        private List<Socket> ConnectedSocets;
        private List<Task> Threads;

        public Server()
        {

            ImSocet.Bind(new IPEndPoint(IPAddress.Any, 2027));
            ImSocet.Listen(30);

            InputThread = new Task(ServerUserInput);

            ConnectedSocets = new List<Socket>();
            Threads = new List<Task>();

            ServerInputWorker = new InputWorker(this);
            ClientInputWorker = new ClientWorker();


            //Подписка на оповещения
            EventContainer.OnServerInput += ServerInputWorker.React;
            EventContainer.OnClientMessage += ClientInputWorker.React;

        }




        /// <summary>
        /// Ввод комманд на сервере
        /// </summary>
        private static void ServerUserInput()
        {
            while (!NeedExit)
            {
                var input = Console.ReadLine();
                EventContainer.Raise(EventType.S_TEXT_INPUT, new InWorkArgs(input));
            }

        }


        public void Start()
        {
            NeedExit = false;
            InputThread.Start();


            while (!NeedExit)
                WaitAndWork();

        }


        public void Stop()
        {
            EventContainer.Raise(EventType.S_SHUTDOWN, null);

            NeedExit = true;
        }



        /// <summary>
        /// Основное тело сервера
        /// </summary>
        private void WaitAndWork()
        {
            try
            {
                var clientSocet = ImSocet.Accept();
                Threads.Add(Task.Factory.StartNew(param => { CreateNewConnection((Socket)param); }, clientSocet));
            }
            catch (SocketException e)
            {

            }
        }

        private void CreateNewConnection(Socket newSocet)
        {
            try
            {

                byte[] recivedAnswer = new byte[4096];
                int gettedSize = newSocet.Receive(recivedAnswer);
                ConnectedSocets.Add(newSocet);
                EventContainer.Raise(EventType.C_MESSAGE, new ClientMessageEventArgs(newSocet, recivedAnswer));

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
