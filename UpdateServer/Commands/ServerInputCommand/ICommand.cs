
using UpdateServer.AllEventArgs;

namespace UpdateServer.Commands.ServerInputCommand
{
    public interface ICommand
    {
        string Name { get; }
        string Desctiption { get; }

        void Invoke(ServerEventArgs args);

    }
}