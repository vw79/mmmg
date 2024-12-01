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

        // Card ID
        EditorGUILayout.LabelField("Card Details", EditorStyles.boldLabel);
        cardData.cardID = EditorGUILayout.TextField("Card ID", cardData.cardID);

        // Card Colour enum field
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Color", EditorStyles.boldLabel);
        cardData.colourSelection = (CardData.CardColour)EditorGUILayout.EnumPopup("Card Color", cardData.colourSelection);

        // Card Attribute enum field
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Attribute", EditorStyles.boldLabel);
        cardData.attributeSelection = (CardData.CardAttribute)EditorGUILayout.EnumPopup("Card Attribute", cardData.attributeSelection);

        // Card Name
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Name", EditorStyles.boldLabel);
        cardData.cardName = EditorGUILayout.TextField("Card Name", cardData.cardName);

        // Card Image
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Image", EditorStyles.boldLabel);
        cardData.cardImage = (Sprite)EditorGUILayout.ObjectField("Card Image", cardData.cardImage, typeof(Sprite), false);

        // Define Style
        GUIStyle wordWrappedTextArea = new GUIStyle(EditorStyles.textArea);
        wordWrappedTextArea.wordWrap = true;

        // Character or Action Card
        if (cardData.attributeSelection == CardData.CardAttribute.Attacker ||
            cardData.attributeSelection == CardData.CardAttribute.Supporter ||
            cardData.attributeSelection == CardData.CardAttribute.Defender)
        {
            // Basic Damage and Health
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Basic Damage and Health", EditorStyles.boldLabel);
            cardData.basicDmg = EditorGUILayout.IntField("Basic Damage", cardData.basicDmg);
            cardData.health = EditorGUILayout.IntField("Health", cardData.health);

            // Skill Settings
            EditorGUILayout.Space(); // Skill 1
            EditorGUILayout.LabelField("Skill 1 Settings", EditorStyles.boldLabel);
            cardData.skill1Text = EditorGUILayout.TextArea(cardData.skill1Text, wordWrappedTextArea, GUILayout.Height(60));
            cardData.displaySkill1AsPassive = EditorGUILayout.Toggle("Display Skill 1 As Passive", cardData.displaySkill1AsPassive);
            if (!cardData.displaySkill1AsPassive)
            {
                // Colour 1
                cardData.skill1HexColor = EditorGUILayout.TextField("Skill 1 Hex Color", cardData.skill1HexColor);
                Color skill1Color = HexToColor(cardData.skill1HexColor);
                Rect color1PreviewRect = GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false));
                EditorGUI.DrawRect(color1PreviewRect, skill1Color);

                // Skill 1 energy count
                cardData.skillEnergy1Count = EditorGUILayout.IntField("Skill Energy 1 Count", cardData.skillEnergy1Count);
            }

            EditorGUILayout.Space(); // Skill 2
            EditorGUILayout.LabelField("Skill 2 Settings", EditorStyles.boldLabel);
            cardData.skill2Text = EditorGUILayout.TextArea(cardData.skill2Text, wordWrappedTextArea, GUILayout.Height(60));

            // Colour 2
            cardData.skill2HexColor = EditorGUILayout.TextField("Skill 2 Hex Color", cardData.skill2HexColor);
            Color skill2Color = HexToColor(cardData.skill2HexColor);
            Rect color2PreviewRect = GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false));
            EditorGUI.DrawRect(color2PreviewRect, skill2Color);

            // Skill 2 energy count
            cardData.skillEnergy2Count = EditorGUILayout.IntField("Skill Energy 2 Count", cardData.skillEnergy2Count);

            EditorGUILayout.Space(); // Skill 3
            EditorGUILayout.LabelField("Skill 3 Settings", EditorStyles.boldLabel);
            cardData.skill3Text = EditorGUILayout.TextArea(cardData.skill3Text, wordWrappedTextArea, GUILayout.Height(60));

            // Colour 3
            cardData.skill3HexColor = EditorGUILayout.TextField("Skill 3 Hex Color", cardData.skill3HexColor);
            Color skill3Color = HexToColor(cardData.skill3HexColor);
            Rect color3PreviewRect = GUILayoutUtility.GetRect(20, 20, GUILayout.ExpandWidth(false));
            EditorGUI.DrawRect(color3PreviewRect, skill3Color);

            // Skill 3 energy count
            cardData.skillEnergy3Count = EditorGUILayout.IntField("Skill Energy 3 Count", cardData.skillEnergy3Count);
        }
        else
        {
            // Action Card Description
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Action Card Description", EditorStyles.boldLabel);
            cardData.cardSkillDescription = EditorGUILayout.TextArea(cardData.cardSkillDescription, wordWrappedTextArea, GUILayout.Height(60));
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(cardData);
        }
    }

    private Color HexToColor(string hex)
    {
        if (string.IsNullOrEmpty(hex)) return Color.black;

        if (hex.Length == 6 && ColorUtility.TryParseHtmlString("#" + hex, out Color color))
        {
            return color;
        }

        return Color.black;
    }
}