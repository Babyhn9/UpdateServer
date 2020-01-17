using System;
using System.Collections.Generic;
using System.Linq;
using UpdateServer.AllEventArgs;
using UpdateServer.Commands.ServerMessageCommand;

namespace UpdateServer.Workers
{
    class ClientWorker : IWorker
    {
        private List<IServerReact> CommandList;

        public ClientWorker()
        {
            CommandList = new List<IServerReact>();
            InitCommand();
        }

        public void React(ServerEventArgs val)
        {
            var convertedVal = val as ClientMessageEventArgs;

            foreach (var command in CommandList)
            {
                if (command.Name == convertedVal.Message.Split(' ')[0] )
                {
                    command.React(convertedVal);
                    return;
                }
            }

        }

        private void InitCommand()
        {
            var thisAssembly = typeof(ClientWorker).Assembly;
            var clientServerListCommand = thisAssembly.GetTypes().Where(type => type.GetInterfaces().Contains(typeof(IServerReact))).ToArray();
            
            foreach(var command in clientServerListCommand)
            {
                var instance = thisAssembly.CreateInstance(command.FullName);
                CommandList.Add(instance as IServerReact);
            }
        }

    }
}
