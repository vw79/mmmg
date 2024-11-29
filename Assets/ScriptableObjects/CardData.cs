using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "CardData")]

public class CardData : ScriptableObject
{
    public string cardID;

    [Header("Card Colour")]
    [SerializeField] private CardColour colourSelection;
    public string colour => colourSelection.ToString(); // Return the string value of the selected colour

    [Header("Card Attribute")]
    public CardAttribute attributeSelection;
    public string attribute => attributeSelection.ToString(); // Return the string value of the selected attribute

    public string cardName;
    [TextArea] public string cardSkillDescription;
    public Sprite cardImage;
    public int basicDmg;
    public int health;

    [Header("Skill 1 Settings")]
    [TextArea] public string skill1Text;
    public string skill1HexColor = "FFFFFF"; // Hex color without "#"
    public bool displaySkill1AsPassive; // Checkbox for [P]
    public int skillEnergy1Count; // Energy count for Skill 1

    [Header("Skill 2 Settings")]
    [TextArea] public string skill2Text;
    public string skill2HexColor = "FFFFFF"; // Hex color without "#"
    public int skillEnergy2Count; // Energy count for Skill 2

    [Header("Skill 3 Settings")]
    [TextArea] public string skill3Text;
    public string skill3HexColor = "FFFFFF"; // Hex color without "#"
    public int skillEnergy3Count; // Energy count for Skill 3

    // Enums for predefined options
    public enum CardColour
    {
        None,
        Red,
        Green,
        Blue
    }

    public enum CardAttribute
    {
        Attacker,
        Supporter,
        Defender,
        Normal,
        Weapon,
        Consumable,
        Structure,
        Event,
        Magic,
        Buff
    }

    public string GetFormattedSkill1()
    {
        string color = $"#{skill1HexColor}";
        if (displaySkill1AsPassive)
        {
            return $"Skill 1: <space=20> <color=#C0C0C0><b>[P]</b></color>\n{skill1Text}";
        }
        else
        {
            return $"Skill 1: <space=20> <sprite=0><color={color}><b>x{skillEnergy1Count}</b></color>\n{skill1Text}";
        }
    }

    // Generate formatted Skill 2 text
    public string GetFormattedSkill2()
    {
        string color = $"#{skill2HexColor}";
        return $"Skill 2: <space=20> <sprite=0><color={color}><b>x{skillEnergy2Count}</b></color>\n{skill2Text}";
    }

    // Generate formatted Skill 3 text
    public string GetFormattedSkill3()
    {
        string color = $"#{skill3HexColor}";

        return $"Skill 3: <space=20> <sprite=0><color={color}><b>x{skillEnergy3Count}</b></color>\n{skill3Text}";
    }
}