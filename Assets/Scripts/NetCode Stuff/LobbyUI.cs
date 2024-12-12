using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public GameObject loginPanel;
    public GameObject lobbyPanel;
    public GameObject lobbyListPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI roomName;
    public GameObject room_PlayerListPanel;
    public Button startGameButton;

    public GameObject roomPrefab;
    public GameObject playerPrefab;

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(false);
    }

    public void ShowLobbyPanel()
    {
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
        //Debug.Log(AuthenticationService.Instance.PlayerName);
        playerName.text = AuthenticationService.Instance.PlayerName;
    }

    public void ShowRoomPanel(string lobbyRoomName)
    {
        roomName.text = lobbyRoomName;
        loginPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
    }

    public void UpdateLobbyList(List<Lobby> lobbies)
    {
        foreach (Transform child in lobbyListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbies)
        {
            GameObject room = Instantiate(roomPrefab, lobbyListPanel.transform);
            room.GetComponent<Btn_Lobby>().Initialize(lobby);
        }
    }

    public void UpdateLobbyPlayers(Lobby lobby, bool isHost, bool isGameStarted)
    {
        foreach (Transform child in room_PlayerListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in lobby.Players)
        {
            GameObject room = Instantiate(playerPrefab, room_PlayerListPanel.transform);
            room.GetComponent<Lobby_PlayerInfo>().Initialize(player);
        }

        if (isHost && lobby.Players.Count >= 2 && !isGameStarted)
        {
            startGameButton.interactable = true;
        }
        else
        {
            startGameButton.interactable = false;
        }
    }

    public void StartGame()
    {
        TestLobby.Instance.StartGame();
    }
}
