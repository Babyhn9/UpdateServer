using CrossData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Utils;

namespace UpdateServer.Commands.ServerMessageCommand
{

    public class GetFileCommand : IServerReact
    {
        public string Name => "file";
        private Socket Client;
        private BinaryFormatter Formatter;
        private static List<byte[]> PathToMl = new List<byte[]>();
        private Connector Connector;

        public GetFileCommand()
        {
            Formatter = new BinaryFormatter();

            PathToMl = new List<byte[]>(new MLContainer().GetFiles());

            EventContainer.OnFilesUpdate += () =>
            {
                PathToMl = new List<byte[]>(new MLContainer().GetFiles());
            };

        }

        public void React(ServerEventArgs args)
        {
            var convertedArgs = args as ClientMessageEventArgs;
            var connector = new Connector(convertedArgs.Client);

            var ML = new MLContainer();
            var bytes = ML.GetFiles();

            for(int i = 0; i < bytes.Length; i++)
            {
                connector.SendFile(bytes[i]);
            }


            connector.Client.Shutdown(SocketShutdown.Both);


        }

        /// <summary>
        /// Пытает клиента до тех пор, пока он не передаст не битую позицию файла
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int ConvertPosToInt(string pos)
        {
            var result = 0;
            bool beatByte = false;

            try
            {
                result = int.Parse(pos);
                Connector.Send("ok");
            }
            catch (Exception)
            {
                Connector.Send("re");
                beatByte = true;
            }

            while (beatByte)
            {
                var answer = Connector.WaitAnswer();
                try
                {
                    result = int.Parse(answer);
                    beatByte = false;
                    Connector.Send("ok");
                }
                catch
                {
                    Connector.Send("re");
                }
            }

            return result;

        }


    }

}
