using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using TMPro;

public class Btn_Lobby : MonoBehaviour
{
    public TextMeshProUGUI lobbyName;
    public TextMeshProUGUI currentPlayer;
    public string lobbyId;

    public void Initialize(Lobby lobby)
    {
        lobbyName.text = lobby.Name;
        currentPlayer.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        lobbyId = lobby.Id;
    }

    public void JoinLobby()
    {
        TestLobby.Instance.JoinLobbyById(lobbyId);
    }
}
