using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour
{
    public int id;
    public string cardName;
    public CardType cardType; // Enum for card type (Character, NormalAction, BuffedAction)
    public ActionType actionType; // Enum for action type (Attack, Defend, Support, AllAround)
    public string description;

    // Character card specific properties
    public List<Skill> characterSkills; // Character card will have 3 skills, each tied to specific action types

    // Action card specific properties
    public bool hasBuff; // True for special action cards with buff
    public string buffDescription; // Description for the buff if it's a buffed action card

    // Constructor for Character Card
    public Card(int id, string cardName, string description, List<Skill> characterSkills)
    {
        this.id = id;
        this.cardName = cardName;
        this.cardType = CardType.Character;
        this.description = description;
        this.characterSkills = characterSkills;
    }

    // Constructor for Action Card (Normal or Buffed)
    public Card(int id, string cardName, ActionType actionType, bool hasBuff, string description, string buffDescription = "")
    {
        this.id = id;
        this.cardName = cardName;
        this.cardType = hasBuff ? CardType.BuffedAction : CardType.NormalAction;
        this.actionType = actionType;
        this.description = description;
        this.hasBuff = hasBuff;
        this.buffDescription = buffDescription;
    }
}
