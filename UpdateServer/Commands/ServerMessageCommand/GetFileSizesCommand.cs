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
    public class GetFileSizesCommand : IServerReact
    {
        public string Name => "file-size";
        private Connector Connector;
        public void React(ServerEventArgs args)
        {
         
            Connector = new Connector((args as ClientMessageEventArgs).Client);

            var sizes = new MLContainer().GetFilesSizes();
            //отправить размер строки в байтах
            Connector.Send(Encoding.UTF8.GetBytes(sizes).Length.ToString());

            while (Connector.WaitAnswer() != "ok")
                Connector.Send(Encoding.UTF8.GetBytes(sizes).Length.ToString());


            //отправить саму строку
            Connector.Send(sizes);

            while (Connector.WaitAnswer() != "ok")
                Connector.Send(sizes);

       
            Connector.Client.Shutdown(SocketShutdown.Both);

        }
    }
}
