using System;
using System.Collections.Generic;
using UpdateServer.AllEventArgs;

namespace UpdateServer.Commands.ServerInputCommand
{
    public class ShowAllCommands : ICommand
    {
        private List<ICommand> Commands;


        public string Name => "help";
        public string Desctiption => "Выводит все доступные комманды";

        public ShowAllCommands(List<ICommand> commands)
        {
            Commands = commands;
        }


        public void Invoke(ServerEventArgs args)
        {
            foreach (var command in Commands)
            {
                Console.WriteLine($"{command.Name} - {command.Desctiption}");
            }
        }

    }
}
