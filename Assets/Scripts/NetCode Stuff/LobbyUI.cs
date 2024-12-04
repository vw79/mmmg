using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
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

    public void UpdateLobbyPlayers(Lobby lobby, bool isHost)
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

        if (isHost && lobby.Players.Count >= 2)
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
