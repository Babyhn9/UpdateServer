using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Utils;

namespace UpdateServer.Commands.ServerInputCommand
{
    public class SetDescriptionCommmand : ICommand
    {
        public string Name => "desc";
        public string Desctiption => "Устанвливает путь до файла с описанием\n\t description c:\\file.txt";

        public void Invoke(ServerEventArgs args)
        {
            var convertedArgs = args as InWorkArgs;

            try
            {
                MLContainer.SetDescription(convertedArgs.Params);
                Console.WriteLine("Описание успешно обновленно");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }





        }
    }
}
