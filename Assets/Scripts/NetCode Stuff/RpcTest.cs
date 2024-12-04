using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{
    [Rpc(SendTo.Owner)]
    public void TestFunctionRpc()
    {
        if (!IsSpawned)
        {
            GetComponent<NetworkObject>().Spawn();
        }
        Debug.Log("RpcTestFunction called");
    }
}
