using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
    private bool isGameStarted = false;
    private float pingTimer = 15f;
    private float lobbyUpdateTimer = 1.5f;
    private float lobbyRefreshTimer = 3f;
    private string playerName;

    public LobbyUI lobbyUI;
    public string gameSceneName;
    public SceneTransition transitionTool;
    public PromptManager promptManager;

    public static TestLobby Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

        if(UnityServices.State == ServicesInitializationState.Initialized)
        {
            lobbyUI.ShowLobbyPanel();
        }
        else
        {
            lobbyUI.ShowLoginPanel();
        }
    }

    // Similar to Login Function
    public async void Authenticate(TMP_InputField textBox)
    {
        // Validate if the player name is empty
        if (string.IsNullOrEmpty(textBox.text))
        {
            promptManager.ShowPopup("Invalid name", Color.red);
            return;
        }

        if (textBox.text.Length > 8)
        {
            promptManager.ShowPopup("Too long", Color.red);
            return;
        }

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
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);

        lobbyUI.ShowLobbyPanel();
        //RefreshLobbyList();
    }

    private void Update()
    {
        HandlePingTimer();
        HandleLobbyUpdates();
        HandleLobbyRefresh();
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
            if (lobbyUpdateTimer <= 0 & !isGameStarted)
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
                        isGameStarted = true;
                    }
                }
            }
        }
    }

    private void HandleLobbyRefresh()
    {
        if (joinedLobby != null | UnityServices.State == ServicesInitializationState.Uninitialized) return;
        lobbyRefreshTimer -= Time.deltaTime;
        if (lobbyRefreshTimer <= 0)
        {
            lobbyRefreshTimer = 3f;
            RefreshLobbyList();
        }
    }

    public async void CreateLobby(TMP_InputField input)
    {
        try
        {
            if (string.IsNullOrEmpty(input.text))
            {
                promptManager.ShowPopup("Invalid name", Color.red);
                return;
            }
            if (input.text.Length > 12)
            {
                promptManager.ShowPopup("Too long", Color.red);
                return;
            }
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

            lobbyUI.ShowRoomPanel(lobby.Name);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void RefreshLobbyList()
    {
        try
        {
            QueryResponse QR = await Lobbies.Instance.QueryLobbiesAsync();

            lobbyUI.UpdateLobbyList(QR.Results);
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
            lobbyUI.ShowRoomPanel(lobby.Name);
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
            lobbyUI.ShowRoomPanel(lobby.Name);
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

                Debug.Log("Move to Battle Scene");
                transitionTool.TransitionToScene(3);
                //SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Single);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if(joinedLobby != null)
        {
            try
            {
                if (IsHost() & joinedLobby.Players.Count == 1)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(joinedLobby.Id);
                }
                else
                {
                    await Lobbies.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                }
                joinedLobby = null;
                hostLobby = null;
                isGameStarted = false;
                lobbyUI.ShowLobbyPanel();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    //[Rpc(SendTo.ClientsAndHost)]
    //public void MoveToBattleSceneRpc()
    //{
    //    Debug.Log("Moved to Battle Scene");
    //    transitionTool.TransitionToScene(4);
    //}
}
