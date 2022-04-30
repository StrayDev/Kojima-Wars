using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Networking.Relay
{
    public static class RelayManager
    {
        private const string Environment = "production";
        private static int MaxConnections = 10;

        private static UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
    
        private static async Task InitialiseAndAuthenticateAsync()
        {
            // create the options for the environment
            var options = new InitializationOptions().SetEnvironmentName(Environment);

            // initialise external unity services 
            await UnityServices.InitializeAsync(options);

            // register with external service 
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                catch
                {
                    //Debug.Log("This is not actually an error");
                }
            }
        }
    
        public static async Task<RelayHostData> SetupRelayAsync()
        {
            await InitialiseAndAuthenticateAsync();
        
            // create the allocations for each player 
            var allocation = await Unity.Services.Relay.Relay.Instance.CreateAllocationAsync(MaxConnections);

            var data = new RelayHostData
            {
                IPv4Address = allocation.RelayServer.IpV4, 
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                Key = allocation.Key
            };
        
            // request the join code
            data.JoinCode = await Unity.Services.Relay.Relay.Instance.GetJoinCodeAsync(data.AllocationID);
        
            // set up the transport
            Transport.SetRelayServerData(data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, data.ConnectionData);
        
            return data;
        }

        public static async Task<RelayJoinData> JoinRelayAsync(string joinCode)
        {
            await InitialiseAndAuthenticateAsync();

            // allocate allocation
            JoinAllocation allocation = null;
            
            try
            {
                // join with the join code
                allocation = await Unity.Services.Relay.Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
                var errorData = new RelayJoinData { ConnectionFailed = true };
                return errorData;
            }

            var data = new RelayJoinData
            {
                JoinCode = joinCode,
                IPv4Address = allocation.RelayServer.IpV4,
                Port = (ushort)allocation.RelayServer.Port,
                AllocationID = allocation.AllocationId,
                AllocationIDBytes = allocation.AllocationIdBytes,
                ConnectionData = allocation.ConnectionData,
                HostConnectionData = allocation.HostConnectionData,
                Key = allocation.Key
            };
        
            // set up the transport
            Transport.SetRelayServerData(
                data.IPv4Address, data.Port, data.AllocationIDBytes, data.Key, 
                data.ConnectionData, data.HostConnectionData);

            return data;
        }
    
    
    
    
    
    }
}
