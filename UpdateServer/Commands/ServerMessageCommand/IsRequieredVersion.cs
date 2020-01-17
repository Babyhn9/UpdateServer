using CrossData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Utils;

namespace UpdateServer.Commands.ServerMessageCommand
{
    public class IsRequieredVersion : IServerReact
    {
        public string Name => "req";

        public void React(ServerEventArgs args)
        {
            var convertedArgs = args as ClientMessageEventArgs;
            var connector = new Connector(convertedArgs.Client);

            var sendMsg = MLContainer.GetReccomended();
            connector.Send(sendMsg);

            while (connector.WaitAnswer() != "ok")
                connector.Send(sendMsg);

            connector.Client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
        }
    }
}
