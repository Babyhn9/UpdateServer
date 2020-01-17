using UpdateServer.AllEventArgs;

namespace UpdateServer.Commands.ServerMessageCommand
{
    public interface IServerReact
    {
        string Name { get; }
        void React(ServerEventArgs args);
    }
}
