using Ron.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject battleUI;
    [SerializeField] private Button battleButton;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private TextMeshProUGUI enemyHealthText;
    public NetworkVariable<int> health = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> interactable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CheckMainMenuClosed();

        //Debug.Log("Player " + OwnerClientId + " spawned");
        playerCamera.SetActive(IsOwner);

        health.OnValueChanged += (int previousValue, int newValue) => 
        {
            Debug.Log(OwnerClientId + "'s Health: " + health.Value);
        };

        interactable.OnValueChanged += (bool previousValue, bool newValue) =>
        {
            battleButton.interactable = interactable.Value;
        };
    }

    // Change after lobby system done
    private void CheckMainMenuClosed()
    {
        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
        }
    }

    // Function will only be called by the owner
    // In other players world, functions will not be called if they are not the owner
    [Rpc(SendTo.Owner)]
    public void EnableBattleUIRpc()
    {
        Debug.Log("Battle UI Enabled" + OwnerClientId);
        battleUI.SetActive(true);
    }

    // Function will send to server only
    // If a client press this button, it will send to server and not this client
    // Buggy warning: I feel something wrong
    [Rpc(SendTo.Server,RequireOwnership = false)]
    public void RequestAttackRpc()
    {
        Debug.Log("Battle Button Clicked");
        GManagerWithNet.Instance.AttackRpc(OwnerClientId);
    }

    // Function will only be called at the owner world
    // In other players world, functions will not be called if they are not the owner
    [Rpc(SendTo.Owner)]
    public void UpdateHealthTextRpc(int selfHealth, int enemyHealth)
    {
        playerHealthText.text = selfHealth + " / 10";
        enemyHealthText.text = enemyHealth + " / 10";
    }

    // Function will only be called at the owner world
    // In other players world, functions will not be called if they are not the owner
    [Rpc(SendTo.Owner)]
    public void StartTurnRpc()
    {
        if (!IsOwner) return;
        interactable.Value = true;
    }

    [Rpc(SendTo.Owner)]
    public void CloseInteractRpc()
    {
        if (!IsOwner) return;
        interactable.Value = false;
    }
}
