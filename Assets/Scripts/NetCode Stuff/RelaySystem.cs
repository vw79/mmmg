using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelaySystem : MonoBehaviour
{
    public static RelaySystem Instance;

    private void Awake()
    {
        Instance = this;
    }

    public async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData serverData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    public async void JoinRelay(string code)
    {
        try
        {
            Debug.Log("Joining Relay with code: " + code);

            JoinAllocation joinallocation = await RelayService.Instance.JoinAllocationAsync(code);

            RelayServerData serverData = new RelayServerData(joinallocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
}
