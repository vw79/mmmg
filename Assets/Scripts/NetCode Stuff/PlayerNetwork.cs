using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;
    private NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CheckMainMenuClosed();

        Debug.Log("Player " + OwnerClientId + " spawned");
        playerCamera.SetActive(IsOwner);
    }

    // Change after lobby system done
    private void CheckMainMenuClosed()
    {
        if (Camera.main != null)
        {
            Camera.main.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            health.Value -= 10;
        }

        Vector3 moveDirection = new Vector3(0, 0, 0);
        
        if (Input.GetKey(KeyCode.W)) moveDirection += new Vector3(0, 0, 1);
        if (Input.GetKey(KeyCode.S)) moveDirection += new Vector3(0, 0, -1);
        if (Input.GetKey(KeyCode.A)) moveDirection += new Vector3(-1, 0, 0);
        if (Input.GetKey(KeyCode.D)) moveDirection += new Vector3(1, 0, 0);

        transform.position += moveDirection * Time.deltaTime;
    }
}
