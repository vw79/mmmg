using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class GameHost : MonoBehaviour
{
    public static GameHost Instance { get; private set; }

    public GameObject gameManagerPrefab;
    public List<GameManager> gameManagers;
    public List<NewPlayerData> playerData = new List<NewPlayerData>();


    public int currentTurn = 0;
    public PlayerOrder gameState = PlayerOrder.Undefined;
    public int playerReadyToStart = 0;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (NetworkManager.Singleton == null)
        {
            return;
        }

        if (!NetworkManager.Singleton.IsHost)
        {
            enabled = false;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            Debug.Log("Player Fully Connected.");
            SpawnGameManager();
            return;
        }
    }

    private void SpawnGameManager()
    {
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            GameObject gameManager = Instantiate(gameManagerPrefab);
            gameManager.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
            gameManagers.Add(gameManager.GetComponent<GameManager>());
        }

        gameManagers[0].UploadCardRpc();
        gameManagers[1].UploadCardRpc();

        DOVirtual.DelayedCall(1f, () => SyncCharacterCard());
    }

    private void SyncCharacterCard()
    {
        gameManagers[0].GetEnemyCharactersRpc(gameManagers[1].characterCardData.Value);
        gameManagers[1].GetEnemyCharactersRpc(gameManagers[0].characterCardData.Value);

        Debug.Log("Character Card Synced.");

        foreach (GameManager gameManager in gameManagers)
        {
            playerData.Add(new NewPlayerData
            {
                clientId = gameManager.OwnerClientId,
                gameManager = gameManager,
                playOrder = PlayerOrder.Undefined
            });
        }

        Debug.Log("Doing other actions");
        InitPlayOrder();
        ClientsInitGame();
    }

    private void InitPlayOrder()
    {
        Debug.Log("Initializing Player Order");
        int random = Random.Range(0, 2);
        playerData[random].playOrder = PlayerOrder.First;
        playerData[(random + 1) % 2].playOrder = PlayerOrder.Second;

        currentTurn = 0;
        gameState = PlayerOrder.First;
    }

    private void ClientsInitGame()
    {
        Debug.Log("All Clients Initialized Game.");
        foreach(GameManager gm in gameManagers)
        {
            gm.StartGameRpc();
        }
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestStartGameServerRpc()
    {
        playerReadyToStart++;
        if (playerReadyToStart == 2)
        {
            SwitchTurn();
        }
    }

    private void SwitchTurn()
    {
        currentTurn++;

        // Check if max turn reached
        if (currentTurn == 30)
        {
            foreach(GameManager gm in gameManagers)
            {
                gm.GameDrawClientRpc();
            }
            return;
        }

        if (currentTurn % 2 == 1)
        {
            gameState = PlayerOrder.First;
        }
        else
        {
            gameState = PlayerOrder.Second;
        }

        GameManager playerOnTurn = playerData.Find(x => x.playOrder == gameState).gameManager;
        GameManager playerNotOnTurn = playerData.Find(x => x.playOrder != gameState).gameManager;

        playerOnTurn.StartTurnClientRpc();
        playerNotOnTurn.OpponentStartTurnClientRpc();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestEndTurnServerRpc()
    {
        SwitchTurn();
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestDrawCardServerRpc(ulong clientID)
    {
        Debug.Log("Card drawn, sender: " + clientID);
        if (!NetworkManager.Singleton.IsHost) return;

        Debug.Log(gameManagers.Count + " " + gameManagers[0].OwnerClientId + " " + gameManagers[1].OwnerClientId);

        // Find opponent game manager
        gameManagers.Find(x => x.OwnerClientId != clientID).AnimateDrawClientRpc(clientID);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestEmptyDeckServerRpc(ulong clientID)
    {
        gameManagers.Find(x => x.OwnerClientId != clientID).EmptyDeckClientRpc();
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestUseCardServerRpc(ulong clientID, string cardID, int charIndex)
    {
        gameManagers.Find(x => x.OwnerClientId != clientID).OpponentUseCardClientRpc(cardID, charIndex);
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestCharacterHitServerRpc(ulong clientID, int charIndex, int damage, string attackerID)
    {
        GameManager attacker = gameManagers.Find(x => x.OwnerClientId == clientID);
        GameManager target = gameManagers.Find(x => x.OwnerClientId != clientID);

        attacker.CharacterAttackClientRpc(charIndex, damage, attackerID);
        target.CharacterHitClientRpc(charIndex, damage, attackerID);

    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestAddScoreServerRpc(ulong clientID)
    {
        gameManagers.Find(x => x.OwnerClientId == clientID).Score.Value += 1;

        gameManagers[0].UpdateScoreClientRpc(gameManagers[0].Score.Value, gameManagers[1].Score.Value);
        gameManagers[1].UpdateScoreClientRpc(gameManagers[1].Score.Value, gameManagers[0].Score.Value);
    }
}

public class NewPlayerData
{
    public ulong clientId;
    public GameManager gameManager;
    public PlayerOrder playOrder;
}
