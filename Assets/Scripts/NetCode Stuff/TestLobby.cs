using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestLobby : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float pingTimer = 15f;
    private float lobbyUpdateTimer = 1.5f;
    private string playerName;

    public LobbyUI lobbyUI;
    public string gameSceneName;

    public static TestLobby Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Similar to Login Function
    public async void Authenticate(TMP_InputField textBox)
    {
        string playerName = textBox.text;
        this.playerName = playerName;
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(playerName);

        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        lobbyUI.ShowLobbyPanel();
        RefreshLobbyList();
    }

    private void Update()
    {
        HandlePingTimer();
        HandleLobbyUpdates();
    }

    private async void HandlePingTimer()
    {
        if(hostLobby != null)
        {
            pingTimer -= Time.deltaTime;
            if (pingTimer <= 0)
            {
                pingTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    private async void HandleLobbyUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer <= 0)
            {
                lobbyUpdateTimer = 1.5f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                lobbyUI.UpdateLobbyPlayers(lobby, IsHost());

                if (joinedLobby.Data["GameKey"].Value != "0")
                {
                    if(!IsHost())
                    {
                        RelaySystem.Instance.JoinRelay(joinedLobby.Data["GameKey"].Value);
                    }
                }
            }
        }
    }

    public async void CreateLobby(TMP_InputField input)
    {
        try
        {
            string lobbyName = input.text;
            int maxPlayer = 2;
            CreateLobbyOptions options = new CreateLobbyOptions() 
            {
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "GameKey", new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);

            Debug.Log("Lobby created : " + lobby.Name + " " + lobby.LobbyCode);
            hostLobby = lobby;
            joinedLobby = lobby;

            lobbyUI.ShowRoomPanel();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void RefreshLobbyList()
    {
        try
        {
            QueryResponse QR = await Lobbies.Instance.QueryLobbiesAsync();

            lobbyUI.UpdateLobbyList(QR.Results);

            foreach (Lobby lobby in QR.Results)
            {
                Debug_PrintLobby(lobby);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void Debug_PrintLobby(Lobby lobby)
    {
        Debug.Log(lobby.Name + " " + lobby.Players.Count + "/" + lobby.MaxPlayers);
    }

    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions()
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByCodeAsync(lobbyCode,options);
            joinedLobby = lobby;

            PrintPlayers(lobby);
            lobbyUI.ShowRoomPanel();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
            {
                Player = GetPlayer()
            };
            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, options);
            joinedLobby = lobby;

            PrintPlayers(lobby);
            lobbyUI.ShowRoomPanel();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void PrintPlayers(Lobby lb)
    {
        foreach (Player player in lb.Players)
        {
            Debug.Log("Player Name: " + player.Data["Name"].Value);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "Name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
            }
        };
    }

    public bool IsHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void StartGame()
    {
        if(IsHost())
        {
            try
            {
                string relayCode = await RelaySystem.Instance.CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        { "GameKey", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;

                SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}
