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

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
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

        foreach (GameManager gameManager in gameManagers)
        {
            gameManager.StartGameRpc();
        }
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

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestUseCardServerRpc(ulong clientID, string cardID, int charIndex)
    {
        gameManagers.Find(x => x.OwnerClientId != clientID).OpponentUseCardClientRpc(cardID, charIndex);
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestCharacterHitServerRpc(ulong clientID, int charIndex, int damage)
    {
        GameManager attacker = gameManagers.Find(x => x.OwnerClientId == clientID);
        GameManager target = gameManagers.Find(x => x.OwnerClientId != clientID);

        attacker.CharacterAttackClientRpc(charIndex, damage);
        target.CharacterHitClientRpc(charIndex, damage);

    }
}
