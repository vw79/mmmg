using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllCardDatabase", menuName = "Card System/ All Card Database")]
public class AllCardDatabase : ScriptableObject
{
    [Tooltip("Character IDs: c00 - c06")]
    public List<CardData> charactersSO;

    [Tooltip("Action IDs: a00 - a26")]
    public List<CardData> actionCardSO;

    [Tooltip("Buff IDs: b00 - b01")]
    public List<CardData> buffSO;

    [Tooltip("VFX IDs: v00 - v06")]
    public List<GameObject> vfx;
}
