using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    private void Awake()
    {
        // Add a sample character card with skills
        List<Skill> warriorSkills = new List<Skill>
        {
            new Skill("Slash Attack", ActionType.Attack, "A strong sword slash", 2),
            new Skill("Defensive Stance", ActionType.Defend, "Reduces incoming damage", 3),
            new Skill("Healing Aura", ActionType.Support, "Heals nearby allies", 4)
        };

        cardList.Add(new Card(0, "Warrior", "A powerful warrior character", warriorSkills));

        // Add a sample normal action card (Attack)
        cardList.Add(new Card(1, "Basic Attack", ActionType.Attack, false, "A normal attack action"));

        // Add a sample buffed action card (Support with Buff)
        cardList.Add(new Card(2, "Empowered Heal", ActionType.Support, true, "A support action with healing boost", "Heals an additional 5 HP"));

        // Add a sample all-around action card (Can be used for any skill activation)
        cardList.Add(new Card(3, "Wildcard", ActionType.AllAround, false, "A flexible action card that can activate any skill"));
    }
}
