using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]

public class CardData : ScriptableObject
{
    public string cardID;
    public string colour;
    public string attribute;
    public string cardName;
    [TextArea] public string cardSkillDescription;
    public Sprite cardImage;
    public int basicDmg;
    public int health;
    [TextArea] public string skill1;
    [TextArea] public string skill2;
    [TextArea] public string skill3;
}