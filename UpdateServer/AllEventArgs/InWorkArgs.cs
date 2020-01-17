using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateServer.AllEventArgs
{
    public class InWorkArgs : ServerEventArgs
    {
        public string Name { get; private set; }
        public string Params { get; private set; } = "";


        public InWorkArgs(string str)
        {
            var splittedStr = str.Split(' ');
            Name = splittedStr[0];

            for(int i = 0; i< splittedStr.Length;i++)
            {
                if (i == 0)
                    continue;

                Params += splittedStr[i]  + ((i == splittedStr.Length - 1) ? "" : " ");
            }
        }
    }
}
