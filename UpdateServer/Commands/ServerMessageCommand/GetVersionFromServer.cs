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
    public class GetVersionFromServer : IServerReact
    {
        public string Name => "ver";

        public void React(ServerEventArgs args)
        {
            var connector = new Connector((args as ClientMessageEventArgs).Client);
            var desc = MLContainer.GetDescription;
            var version = desc.Substring(desc.IndexOf('{') + 1, desc.LastIndexOf('}') - desc.IndexOf('{') - 1);
            connector.Send(version);
            connector.Client.Shutdown(SocketShutdown.Both);
        }
    }
}
