using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target ScriptableObject
        CardData cardData = (CardData)target;

        // Show the Card ID
        EditorGUILayout.LabelField("Card Details", EditorStyles.boldLabel);
        cardData.cardID = EditorGUILayout.TextField("Card ID", cardData.cardID);

        // Show the Card Attribute enum field
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Attribute", EditorStyles.boldLabel);
        cardData.attributeSelection = (CardData.CardAttribute)EditorGUILayout.EnumPopup("Card Attribute", cardData.attributeSelection);

        // Conditional Display Logic for Skill or Card Description
        if (cardData.attributeSelection == CardData.CardAttribute.Attacker ||
            cardData.attributeSelection == CardData.CardAttribute.Supporter ||
            cardData.attributeSelection == CardData.CardAttribute.Defender)
        {
            // Show Skill Settings
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Skill 1 Settings", EditorStyles.boldLabel);
            cardData.skill1Text = EditorGUILayout.TextArea(cardData.skill1Text, GUILayout.Height(60));
            cardData.displaySkill1AsPassive = EditorGUILayout.Toggle("Display Skill 1 As Passive", cardData.displaySkill1AsPassive);
            if (!cardData.displaySkill1AsPassive)
            {
                cardData.skill1HexColor = EditorGUILayout.TextField("Skill 1 Hex Color", cardData.skill1HexColor);
                cardData.skillEnergy1Count = EditorGUILayout.IntField("Skill Energy 1 Count", cardData.skillEnergy1Count);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Skill 2 Settings", EditorStyles.boldLabel);
            cardData.skill2Text = EditorGUILayout.TextArea(cardData.skill2Text, GUILayout.Height(60));
            cardData.skill2HexColor = EditorGUILayout.TextField("Skill 2 Hex Color", cardData.skill2HexColor);
            cardData.skillEnergy2Count = EditorGUILayout.IntField("Skill Energy 2 Count", cardData.skillEnergy2Count);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Skill 3 Settings", EditorStyles.boldLabel);
            cardData.skill3Text = EditorGUILayout.TextArea(cardData.skill3Text, GUILayout.Height(60));
            cardData.skill3HexColor = EditorGUILayout.TextField("Skill 3 Hex Color", cardData.skill3HexColor);
            cardData.skillEnergy3Count = EditorGUILayout.IntField("Skill Energy 3 Count", cardData.skillEnergy3Count);
        }
        else
        {
            // Show Card Skill Description
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Card Skill Description", EditorStyles.boldLabel);
            cardData.cardSkillDescription = EditorGUILayout.TextArea(cardData.cardSkillDescription, GUILayout.Height(60));
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(cardData);
        }
    }
}