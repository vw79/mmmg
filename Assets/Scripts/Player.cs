using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ron.Utility;

public class PlayerTest : MonoBehaviour
{
    [ReadOnly] public int currentHealth;
    public int initialHealth = 5;
    public bool canMove = false;
    public PlayerOrder playerOrder = PlayerOrder.Undefined;
}

public enum PlayerOrder
{
    Undefined,
    First,
    Second
}
