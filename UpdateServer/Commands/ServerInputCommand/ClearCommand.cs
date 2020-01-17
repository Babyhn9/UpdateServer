using System;
using UpdateServer.AllEventArgs;

namespace UpdateServer.Commands.ServerInputCommand
{
    public class ClearCommand : ICommand
    {
        public string Name => "clear";

        public string Desctiption => "Очищает экран";

        public void Invoke()
        {
        }

        public void Invoke(ServerEventArgs args)
        {
            Console.Clear();
        }
    }
}
