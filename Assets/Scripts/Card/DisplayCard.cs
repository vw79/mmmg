using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayCard : MonoBehaviour
{
    public List<Card> displayCard = new List<Card>();
    public int displayId;

    // Card info to display
    public int id;
    public string cardName;
    public CardType cardType;
    public ActionType actionType;
    public string description;

    // For Character Cards
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;
    public TextMeshPro skillText1;
    public TextMeshPro skillText2;
    public TextMeshPro skillText3;

    // For Buffed Action Cards
    public TextMeshPro buffText;

    private void Start()
    {
        // Fetch the card based on displayId from CardDatabase
        displayCard[0] = CardDatabase.cardList[displayId];

        // Display the card details
        DisplayCardInfo();
    }

    private void Update()
    {
        // Update card ID (if needed in your game logic)
        id = displayCard[0].id;
    }

    // Function to display the card information based on its type
    private void DisplayCardInfo()
    {
        // Get the card information
        Card card = displayCard[0];
        cardName = card.cardName;
        cardType = card.cardType;
        description = card.description;

        // Display card name and description
        nameText.text = "Name: " + cardName;
        descriptionText.text = "Description: " + description;

        // If it's a Character Card, display the skills
        if (cardType == CardType.Character)
        {
            // Ensure the character has skills
            if (card.characterSkills.Count >= 3)
            {
                skillText1.text = "Skill 1: " + card.characterSkills[0].skillName +
                                  " (Type: " + card.characterSkills[0].requiredActionType +
                                  ", Cost: " + card.characterSkills[0].skillCost + ")";

                skillText2.text = "Skill 2: " + card.characterSkills[1].skillName +
                                  " (Type: " + card.characterSkills[1].requiredActionType +
                                  ", Cost: " + card.characterSkills[1].skillCost + ")";

                skillText3.text = "Skill 3: " + card.characterSkills[2].skillName +
                                  " (Type: " + card.characterSkills[2].requiredActionType +
                                  ", Cost: " + card.characterSkills[2].skillCost + ")";
            }
        }
        // If it's a Buffed Action Card, display the buff description
        else if (cardType == CardType.BuffedAction)
        {
            if (card.hasBuff)
            {
                buffText.text = "Buff: " + card.buffDescription;
            }
        }
        // Otherwise, it's a normal action card; no buff or skill to display
        else
        {
            // You can hide the skill or buff text fields if needed
            skillText1.text = "";
            skillText2.text = "";
            skillText3.text = "";
            buffText.text = "";
        }
    }
}
