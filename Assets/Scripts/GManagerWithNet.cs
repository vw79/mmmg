using Ron.Utility;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GManagerWithNet : MonoBehaviour
{
    public static GManagerWithNet Instance { get; private set; }

    [SerializeField] private Transform[] spawnpoints;
    [SerializeField, ReadOnly] private bool[] spawnpointsOccupied = new bool[2];
    [SerializeField] private GameObject playerPrefab;
    [ReadOnly] public List<PlayerNetwork> players;
    [SerializeField, ReadOnly] private List<PlayerData> playerData = new List<PlayerData>();

    #region Turn-based Variables
    [ReadOnly] public int currentRound = 1;
    [ReadOnly] public int currentTurn = 1;
    [ReadOnly] public PlayerOrder gameState = PlayerOrder.Undefined;
    #endregion

    //public override void OnNetworkSpawn()
    //{
    //    if (!NetworkManager.Singleton.IsHost)
    //    {
    //        enabled = false;
    //    }

    //    if(Instance != null)
    //    {
    //        gameObject.SetActive(false);
    //        return;
    //    }
        
    //    base.OnNetworkSpawn();
    //    Instance = this;
    //    NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    //}

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            enabled = false;
        }


        // If the client connect before the scene loaded
        // Tested: client connected after the scene loaded
        //if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        //{
        //    Debug.Log("Client connected before the scene loaded.");
        //}

        // If the client connect after the scene loaded
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    #region Network Stuff
    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            Debug.Log("Client connected after the scene loaded.");
            InitializeGame();
            return;
        }
        Debug.Log("Client connected with id: " + clientId);

        SpawnPlayerObject(clientId);

        if (players.Count == 2)
        {
            EnableBattleUI();
            StartGame();
        }

    }

    private void InitializeGame()
    {
        // Spawn player objects
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayerObject(client.ClientId);
        }

        // Enable battle UI
        EnableBattleUI();

        // Start the game
        StartGame();
    }

    private void SpawnPlayerObject(ulong clientId)
    {
        Transform selectedPlace = FindEmptySpawnpoint();
        GameObject player = Instantiate(playerPrefab, selectedPlace.position , selectedPlace.rotation);

        //Set up for multiplayer stuff
        player.GetComponent<NetworkObject>().SpawnWithOwnership(clientId, true);
        players.Add(player.GetComponent<PlayerNetwork>());
        Debug.Log("Added Player " + clientId + " to the game.");

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
            spawnpointsOccupied[(randomIndex + 1) % 2] = true;
            Debug.Log("Location: " + (randomIndex + 1) % 2);
            return spawnpoints[(randomIndex + 1) % 2];
        }
    }
    #endregion

    // Only called by host
    private void EnableBattleUI()
    {
        foreach (PlayerNetwork player in players)
        {
            // Server will call this function for each player
            player.EnableBattleUIRpc();
            player.CloseInteractRpc();
            playerData.Add(new PlayerData
            {
                clientId = player.OwnerClientId,
                playerRef = player,
                playOrder = PlayerOrder.Undefined
            });
        }
    }

    // Request by client, only worked on server
    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void AttackRpc(ulong requestedPlayer)
    {
        if(!NetworkManager.Singleton.IsHost) return;
        playerData.Find(x => x.clientId == requestedPlayer).playerRef.CloseInteractRpc();
        PlayerData target = playerData.Find(x => x.clientId != requestedPlayer);
        target.playerRef.health.Value -= 1;
        SendHealthUpdateToClient();

        // After done attacking, switch turn
        SwitchTurn();
    }

    private void SendHealthUpdateToClient()
    {
        foreach (PlayerData player in playerData)
        {
            player.playerRef.UpdateHealthTextRpc(player.playerRef.health.Value, playerData.Find(x => x.clientId != player.clientId).playerRef.health.Value);
        }
    }

    // After prepare the battle UI and players, start the game
    private void StartGame()
    {
        // Reset everything
        currentRound = 0;
        currentTurn = 0;
        gameState = PlayerOrder.Undefined;

        // Randomly choose who goes first
        // Now make it simple
        int random = Random.Range(0, 2);
        playerData[random].playOrder = PlayerOrder.First;
        playerData[(random + 1) % 2].playOrder = PlayerOrder.Second;

        // Start the game actually
        SwitchTurn();
    }

    private void SwitchTurn()
    {
        currentTurn = currentTurn + 1;
        if (currentTurn % 2 == 1)
        {
            gameState = PlayerOrder.First;
        }
        else
        {
            gameState = PlayerOrder.Second;
        }

        // Determine which player can interact with button
        PlayerData currentPlayer = playerData.Find(x => x.playOrder == gameState);
        currentPlayer.playerRef.StartTurnRpc();
    }
}

public class PlayerData
{
    public ulong clientId;
    public PlayerNetwork playerRef;
    public PlayerOrder playOrder;
}
