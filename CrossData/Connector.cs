using CrossData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CrossData
{
    public class Connector
    {
        public Socket Client;
        private BinaryFormatter formatter;
        public Connector(Socket Client)
        {
            formatter = new BinaryFormatter();
            this.Client = Client;
        }



        public void SendFile(byte[] itemForSend)
        {
            Send(itemForSend);

            while (WaitAnswer() != "ok")
                Send(itemForSend);
        }


        public byte[] ParseString(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        public string ParseByte(byte[] arr)
        {
            return Encoding.UTF8.GetString(arr).Replace("\0", "");

        }

        public string WaitAnswer()
        {
            try
            {
                var clientAsk = new byte[4096];
                Client.Receive(clientAsk);
                return ParseByte(clientAsk);
            }
            catch
            {
                return null;
            }

        }

        public string WaitAnswer(out int recivedBytes)
        {
            var clientAsk = new byte[128];
            recivedBytes = Client.Receive(clientAsk);
            return ParseByte(clientAsk);

        }

        public byte[] WaitAllAnswer(int bytesInAnswerMustBe)
        {
            int gettedBytes = 0;
            byte[] buffer = new byte[bytesInAnswerMustBe];
            do
            {
                gettedBytes += Client.Receive(buffer, gettedBytes, bytesInAnswerMustBe - gettedBytes, SocketFlags.None);

            } while (gettedBytes < bytesInAnswerMustBe);

            return buffer;
        }
        public void Send(string message) => Client.Send(ParseString(message));
        public void Send(byte[] message) => Client.Send(message);

        public byte[] Serialize(MLFileInfo info)
        {
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, info);
            return stream.ToArray();
        }

        public void SendFileSize(int fileSize)
        {
            Send(fileSize.ToString());

            while (WaitAnswer() != fileSize.ToString())
            {
                Send(fileSize.ToString());
            }
        }
    }
}
