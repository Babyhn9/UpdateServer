using System;
using UpdateServer.AllEventArgs;

namespace UpdateServer.Workers
{
    public interface IWorker
    {
        void React(ServerEventArgs args);
    }
}
