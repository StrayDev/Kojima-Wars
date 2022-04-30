using System;

namespace Networking.Relay
{
    /// <summary>
    /// RelayHostData represents the necessary information
    /// for a Host to host a game on a Relay
    /// </summary>
    public struct RelayJoinData
    {
        public bool ConnectionFailed;
        
        public string JoinCode;
        public string IPv4Address;
        public ushort Port;
        public Guid   AllocationID;
        public byte[] AllocationIDBytes;
        public byte[] ConnectionData;
        public byte[] HostConnectionData;
        public byte[] Key;
    }
}
