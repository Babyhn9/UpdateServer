using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using UpdateServer.Utils;

namespace UpdateServer
{
    class Program
    {

        static void Main(string[] args)
        {
            var server = new Server();
            server.Start();


        }


    }
}
