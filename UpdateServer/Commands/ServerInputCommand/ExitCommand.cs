using System;
using UpdateServer.AllEventArgs;

namespace UpdateServer.Commands.ServerInputCommand
{
    public class ExitCommand : ICommand
    {

        private Server Owner;

        public string Name => "exit";
        public string Desctiption => "Заканчивает работу сервера";

        public ExitCommand(Server owner)
        {
            Owner = owner;
        }


        public void Invoke()
        {

          
        }

        public void Invoke(ServerEventArgs args)
        {
            Console.WriteLine("Закрываемся");
            Owner.Stop();
            //Server.ImSocet.Disconnect(false);
            Server.ImSocet.Close();
        }
    }
}
