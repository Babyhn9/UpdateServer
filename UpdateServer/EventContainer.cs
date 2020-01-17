using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdateServer.AllEventArgs;

namespace UpdateServer
{
    public static class EventContainer
    {
        public static event Action<ServerEventArgs> OnServerInput;
        public static event Action<ServerEventArgs> OnClientMessage;
        public static event Action OnFilesUpdate;
        public static event Action OnServerShutdown;


        public static void Raise(EventType eType, ServerEventArgs args)
        {
            switch (eType)
            {
                case EventType.S_TEXT_INPUT: OnServerInput?   .Invoke(args)   ; break;
                case EventType.S_SHUTDOWN:   OnServerShutdown?.Invoke()       ; break;
                case EventType.C_MESSAGE:    OnClientMessage? .Invoke(args)   ; break;
                case EventType.ML_UPDATE:    OnFilesUpdate?   .Invoke()       ; break;
            }

        }

    }

    public enum EventType
    {
        S_TEXT_INPUT,
        S_SHUTDOWN,
        C_MESSAGE,
        ML_UPDATE
    }
}
