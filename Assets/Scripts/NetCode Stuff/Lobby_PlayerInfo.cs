using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class Lobby_PlayerInfo : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;

    public void Initialize(Player player)
    {
        playerNameText.text = player.Data["Name"].Value;
    }
}
