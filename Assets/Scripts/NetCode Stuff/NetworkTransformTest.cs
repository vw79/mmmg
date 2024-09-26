using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsServer)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                transform.position = transform.position + new Vector3(-1, 0, 0);
            }
        }
    }
}
