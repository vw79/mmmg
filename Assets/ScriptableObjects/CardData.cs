using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]

public class CardData : ScriptableObject
{
    public string attribute;
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public int basicDmg;
    public int health;
    public string skill1;
    public string skill2;
    public string skill3;
}