using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer.AllEventArgs
{
    public class ClientMessageEventArgs : ServerEventArgs
    {
        public byte[] MessageAsBytes { get; private set; }
        public string Message { get; private set; }
        public Socket Client { get; private set; }
        public object AdvancedData { get; private set; }

        public ClientMessageEventArgs(Socket client, byte[] message, object advandecData = null)
        {
            MessageAsBytes = message;
            Client = client;
            AdvancedData = advandecData;
            Message = Encoding.UTF8.GetString(message).Trim().Replace("\0", "");
        }


        public ClientMessageEventArgs(Socket client, string message, object advandecData = null)
        {
            Client = client;
            Message = message;
            MessageAsBytes = Encoding.UTF8.GetBytes(message);
            AdvancedData = advandecData;

        }
    }
}
