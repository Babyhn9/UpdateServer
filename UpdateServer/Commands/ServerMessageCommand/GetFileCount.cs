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
    public class GetFileCount : IServerReact
    {
        public string Name => "files";
        public Connector Client;
        public void React(ServerEventArgs args)
        {
            
           
            var ClientInfo = args as ClientMessageEventArgs;

            Client = new Connector(ClientInfo.Client);

            var beatBytes = true;
            var ML = new MLContainer();
            while(beatBytes)
            {
                Client.Send(ML.FilesCount().ToString());

                var answer = Client.WaitAnswer();
                
                beatBytes = answer == "ok";
            }

            Client.Client.Shutdown(SocketShutdown.Both);

        }
    }
}
