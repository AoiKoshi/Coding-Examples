using System;

namespace Threadwork.Networking
{
    [Serializable]
    public class ConnectionPayload
    {
        public string clientGUID;
        public int clientScene = -1;
        public string playerName;
        public string password;
    }
}
