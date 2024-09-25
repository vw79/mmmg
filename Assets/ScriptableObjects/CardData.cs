using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]

public class CardData : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
}