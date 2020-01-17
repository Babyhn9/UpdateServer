using CrossData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Utils;

namespace UpdateServer.Commands.ServerMessageCommand
{
    public class GetDescription : IServerReact
    {
        public string Name => "desc";

        public void React(ServerEventArgs args)
        {
            var connectror = new Connector((args as ClientMessageEventArgs).Client);

            connectror.Send(MLContainer.GetDescription);

            connectror.Client.Shutdown(SocketShutdown.Both);
        }
    }
}
