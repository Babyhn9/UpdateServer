using System;
using System.Collections.Generic;
using UpdateServer.AllEventArgs;
using UpdateServer.Commands.ServerInputCommand;

namespace UpdateServer.Workers
{
    public class InputWorker : IWorker
    {
        private Server Owner;
        private List<ICommand> CommandList;

        public InputWorker(Server owner)
        {
            Owner = owner;
            CommandList = new List<ICommand>();
            InitCommands();
        }




        public void React(ServerEventArgs args)
        {
            var convertedArgs = args as InWorkArgs;

            foreach (var command in CommandList)
            {
                if (command.Name == convertedArgs.Name)
                {
                    command.Invoke(args);
                    return;
                }
            }

            Console.WriteLine($"Комманды \"{convertedArgs.Name}\" не обнаруженно \nВведите help для просмотра всех доступных комманд ");
        }

        private void InitCommands()
        {
            CommandList.Add(new ExitCommand(Owner));
            CommandList.Add(new ShowAllCommands(CommandList));
            CommandList.Add(new ClearCommand());
            CommandList.Add(new SetNewFilesCommand());
            CommandList.Add(new SetDescriptionCommmand());
        }

    }
}
