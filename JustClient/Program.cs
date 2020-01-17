using CrossData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
namespace JustClient
{
    class Program
    {


        static void Main(string[] args)
        {
            var client = new Client();
            client.Start();
            Console.ReadKey();

        }
    }
}
