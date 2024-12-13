using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron.Utility;
using Unity.Netcode;
using Unity.Collections;

public class GameManager : NetworkBehaviour
{
    public CardManager cardManager;
    public CardUseManager cardUseManager;
    public Scoreboard scoreboard;

    public SelectedCardData selectedCards;
    public NetworkVariable<int> Score = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<CharacterCardData> characterCardData = new NetworkVariable<CharacterCardData>(new CharacterCardData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct CharacterCardData : INetworkSerializable
    {
        public FixedString32Bytes character1ID;
        public FixedString32Bytes character2ID;
        public FixedString32Bytes character3ID;
        public FixedString32Bytes character4ID;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref character1ID);
            serializer.SerializeValue(ref character2ID);
            serializer.SerializeValue(ref character3ID);
            serializer.SerializeValue(ref character4ID);
        }
    }

    #region Start Game Functions

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        if (cardManager == null)
        {
            cardManager = FindObjectOfType<CardManager>();
        }

        if (cardUseManager == null)
        {
            cardUseManager = FindObjectOfType<CardUseManager>();
        }

        if (scoreboard == null)
        {
            scoreboard = FindObjectOfType<Scoreboard>();
        }

        cardManager.LinkNetworkGameManager(this);
        cardUseManager.LinkNetworkGameManager(this);

        GetComponent<NetworkObject>().DestroyWithScene = true;
    }

    [Rpc(SendTo.Owner)]
    public void UploadCardRpc()
    {
        if (cardManager == null)
        {
            cardManager = FindObjectOfType<CardManager>();
        }

        selectedCards = cardManager.savedCards;
        List<string> characters = selectedCards.selectedCharacterIDs;

        Debug.Log(NetworkManager.Singleton.LocalClientId + " " + characters[0] + " " + characters[1] + " " + characters[2] + " " + characters[3]);

        characterCardData.Value = new CharacterCardData
        {
            character1ID = characters[0],
            character2ID = characters[1],
            character3ID = characters[2],
            character4ID = characters[3]
        };

        Debug.Log(NetworkManager.Singleton.LocalClientId + " " + characterCardData.Value.character1ID + " " + characterCardData.Value.character2ID + " " + characterCardData.Value.character3ID + " " + characterCardData.Value.character4ID);
    }

    [Rpc(SendTo.Owner)]
    public void GetEnemyCharactersRpc(CharacterCardData characters)
    {
        Debug.Log(characters.character1ID + " " + characters.character2ID + " " + characters.character3ID + " " + characters.character4ID);
        cardManager.opponentCharacterIDs = new List<string>
        {
            characters.character1ID.ToString(),
            characters.character2ID.ToString(),
            characters.character3ID.ToString(),
            characters.character4ID.ToString()
        };

        Debug.Log(cardManager.opponentCharacterIDs[0]);
    }

    [Rpc(SendTo.Owner)]
    public void StartGameRpc()
    {
        if(cardManager == null)
        {
            cardManager = FindObjectOfType<CardManager>();
        }
        cardManager.StartGame();
    }

    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void ReadyToGameServerRpc()
    {
        GameHost.Instance.RequestStartGameServerRpc();
    }

    [ClientRpc]
    public void StartTurnClientRpc(int currentTurn)
    {
        if (!IsOwner) return;
        cardManager.StartTurn(currentTurn);
    }

    [ClientRpc]
    public void OpponentStartTurnClientRpc(int currentTurn)
    {
        if (!IsOwner) return;
        cardManager.OpponentStartTurn(currentTurn);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SendEndTurnRpc()
    {
        GameHost.Instance.RequestEndTurnServerRpc();
    }
    #endregion

    #region Server Draw Card Animation
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DrawCardServerRpc()
    {
        Debug.Log("Card drawn, sender: " + OwnerClientId);

        GameHost.Instance.RequestDrawCardServerRpc(OwnerClientId);
    }

    [ClientRpc]
    public void AnimateDrawClientRpc(ulong drawerClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != drawerClientId)
        {
            // Play draw animation for the other player
            PlayOpponentDrawAnimation();
        }
    }

    private void PlayOpponentDrawAnimation()
    {
        Debug.Log("Animating Opponent Draw Card");
        if (!IsOwner) return;
        cardManager.OpponentAnimateDrawCard();
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SendEmptyDeckServerRpc()
    {
        GameHost.Instance.RequestEmptyDeckServerRpc(OwnerClientId);
    }

    [ClientRpc]
    public void EmptyDeckClientRpc()
    {
        if (!IsOwner) return;
        cardManager.OnOpponentEmptyDeck();
    }
    #endregion

    #region Server Card Use Animation
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void UseCardServerRpc(string cardID, int charIndex)
    {
        GameHost.Instance.RequestUseCardServerRpc(OwnerClientId, cardID, charIndex);
    }

    [ClientRpc]
    public void OpponentUseCardClientRpc(string cardID, int charIndex)
    {
        if (!IsOwner) return;
        cardManager.AnimateOpponentUseCard(cardID, charIndex);
    }
    #endregion

    #region Server Character Hit
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void CharacterHitServerRpc(int charIndex, int damage, string attackerID)
    {
        GameHost.Instance.RequestCharacterHitServerRpc(OwnerClientId, charIndex, damage, attackerID);
    }

    // ClientRpc for target
    [ClientRpc]
    public void CharacterHitClientRpc(int charIndex, int damage, string attackerID)
    {
        if (!IsOwner) return;
        cardManager.OnSelfCharacterGetHit(charIndex, damage, attackerID);
    }

    // ClientRpc for attacker
    [ClientRpc]
    public void CharacterAttackClientRpc(int charIndex, int damage, string attackerID)
    {
        if (!IsOwner) return;
        cardManager.OnOpponentCharacterGetHit(charIndex, damage, attackerID);
    }
    #endregion

    #region Score Condition
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void AddScoreServerRpc()
    {
        GameHost.Instance.RequestAddScoreServerRpc(OwnerClientId);
    }

    [ClientRpc]
    public void AddScoreClientRpc()
    {
        if (!IsOwner) return;
        Debug.Log("Score added");
    }

    [ClientRpc]
    public void UpdateScoreClientRpc(int selfScore, int opponentScore)
    {
        if (!IsOwner) return;
        scoreboard.UpdateScore(selfScore, opponentScore);
        if(selfScore >= 4)
        {
            scoreboard.ShowWinPanel();
        }
        else if(opponentScore >= 4)
        {
            scoreboard.ShowLosePanel();
        }
    }

    [ClientRpc]
    public void GameDrawClientRpc(int selfScore, int opponentScore)
    {
        if (!IsOwner) return;
        scoreboard.UpdateScore(selfScore, opponentScore);
        if (selfScore > opponentScore)
        {
            scoreboard.ShowWinPanel();
        }
        else if (opponentScore > selfScore)
        {
            scoreboard.ShowLosePanel();
        }
        else
        {
            scoreboard.ShowDrawPanel();
        }
    }
    #endregion

    public bool ValidateSelfCharacterAlive(int charIndex)
    {
        int health = cardManager.selfCharacterCardData[charIndex].GetHealth();
        if(health <= 0)
        {
            return false;
        }
        return true;
    }

    public bool ValidateOpponentCharacterAlive(int charIndex)
    {
        int health = cardManager.opponentCharacterCardData[charIndex].GetHealth();
        if (health <= 0)
        {
            return false;
        }
        return true;
    }

    //[ReadOnly] public int currentRound = 1;
    //[ReadOnly] public TurnState gameState = TurnState.PlayerOneTurn;
    //public PlayerTest[] players = new PlayerTest[2];

    //private void Start()
    //{
    //    StartGame();
    //}

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SwitchTurn();
    //    }
    //}

    //public void StartGame()
    //{
    //    // Basically restart everything here. Anything you can think of.
    //    currentRound = 1;
    //    gameState = TurnState.PlayerOneTurn;
    //    Debug.Log("Game Start.");

    //    // Initialize every players.
    //    foreach(PlayerTest player in players)
    //    {
    //        player.currentHealth = player.initialHealth;
    //        player.canMove = false;
    //    }

    //    // Randomly choose who goes first.
    //    int random = Random.Range(0, 2);
    //    players[random].playerOrder = PlayerOrder.First;
    //    players[(random + 1) % 2].playerOrder = PlayerOrder.Second;
    //}

    //public void SwitchTurn()
    //{
    //    if(gameState == TurnState.PlayerOneTurn)
    //    {
    //        gameState = TurnState.PlayerTwoTurn;
    //        Debug.Log("Player Two Turn");
    //    }
    //    else
    //    {
    //        gameState = TurnState.PlayerOneTurn;
    //        currentRound++;
    //        Debug.Log("Round " + currentRound + ", Player One Turn");
    //    }
    //}

}

public enum TurnState
{
    PlayerOneTurn,
    PlayerTwoTurn
}
