using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;
using UpdateServer.Utils;

namespace UpdateServer.Commands.ServerInputCommand
{
    public class SetNewFilesCommand : ICommand
    {
        public string Name => "set";

        public string Desctiption => "Записывает новые файлы для ML и устанавливает флаг," +
            " обязательного/необязательного скачивания(без параметра, считается, что" +
            " обнавление не обязательное) пример: \n\tset C:/folderWithProgram n устанавливает новую ML на сервер, и ОБЯЗЫВАЕТ пользователей её скачать";




        public void Invoke(ServerEventArgs args)
        {
            var convertedArgs = args as InWorkArgs;
            var splittedParams = convertedArgs.Params.Split(' ');

            if (splittedParams.Length == 1)
                MLContainer.SetRecommend(false);
            else if (splittedParams.Length == 2 && splittedParams[1] == "n")
                MLContainer.SetRecommend(true);

            MLContainer.SetNewML(convertedArgs.Params.Split(' ')[0]);

            EventContainer.Raise(EventType.ML_UPDATE, null);
        }
    }
}
