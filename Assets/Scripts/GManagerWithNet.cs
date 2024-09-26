using Ron.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GManagerWithNet : MonoBehaviour
{
    [SerializeField] private Transform[] spawnpoints;
    [SerializeField, ReadOnly] private bool[] spawnpointsOccupied = new bool[2];
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject networkUI;

    // Start is called before the first frame update
    void Start()
    {
        //if (!NetworkManager.Singleton.IsHost)
        //{
        //    Destroy(this);
        //    return;
        //}

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Change after Lobby System done
        networkUI.SetActive(false);
        if (!NetworkManager.Singleton.IsHost) return;
        Debug.Log("Client connected with id: " + clientId);

        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            Debug.Log("Room Full.");
        }

        SpawnPlayerObject(clientId);
    }

    private void SpawnPlayerObject(ulong clientId)
    {
        Transform selectedPlace = FindEmptySpawnpoint();
        GameObject player = Instantiate(playerPrefab, selectedPlace.position , selectedPlace.rotation);

        //Set up for multiplayer stuff
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);


    }

    private Transform FindEmptySpawnpoint()
    {
        int randomIndex = Random.Range(0, 2);
        Debug.Log("Location: " + randomIndex);
        if (!spawnpointsOccupied[randomIndex])
        {
            spawnpointsOccupied[randomIndex] = true;
            return spawnpoints[randomIndex];
        }
        else
        {
            Debug.Log("Location Occupied. Trying next location.");
            randomIndex = randomIndex == 1 ? 0 : 1;
            spawnpointsOccupied[(randomIndex + 1) % 2] = true;
            return spawnpoints[(randomIndex + 1) % 2];
        }
    }
}
