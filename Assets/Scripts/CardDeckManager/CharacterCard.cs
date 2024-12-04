using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterCard : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI cardNameText;
    public Image cardImage;

    [Header("Skill 1")]
    public Image skill1BG;
    public Image skill1Image;
    public TextMeshProUGUI skill1EnergyText;
    public TextMeshProUGUI skill1DescriptionText;

    [Header("Skill 2")]
    public Image skill2BG;
    public Image skill2Image;
    public TextMeshProUGUI skill2EnergyText;
    public TextMeshProUGUI skill2DescriptionText;

    [Header("Skill 3")]
    public Image skill3BG;
    public Image skill3Image;
    public TextMeshProUGUI skill3EnergyText;
    public TextMeshProUGUI skill3DescriptionText;

    [Header("Health Bar")]
    public GameObject healthBar;
    public GameObject healthPrefab;
    public int maxHealth;
    private int currentHealth;

    public List<Sprite> skillIcons;
    private CardData cardData;

    public void SetCardData(CardData data)
    {
        cardData = data;

        if (cardData != null)
        {
            // Assign maxHealth to cardData.health
            maxHealth = cardData.health;
            currentHealth = cardData.health;

            // Set card name and image
            cardNameText.text = cardData.cardName;
            cardImage.sprite = cardData.cardImage;

            // Skill 1
            skill1EnergyText.text = cardData.skillEnergy1Count == 0 ? "P" : cardData.skillEnergy1Count.ToString();
            skill1DescriptionText.text = cardData.GetFormattedSkill1();
            SetBackgroundColor(skill1BG, cardData.skill1HexColor);
            SetSkillIcon(skill1Image, cardData.skill1HexColor);

            // Skill 2
            skill2EnergyText.text = cardData.skillEnergy2Count.ToString();
            skill2DescriptionText.text = cardData.GetFormattedSkill2();
            SetBackgroundColor(skill2BG, cardData.skill2HexColor);
            SetSkillIcon(skill2Image, cardData.skill2HexColor);

            // Skill 3
            skill3EnergyText.text = cardData.skillEnergy3Count.ToString();
            skill3DescriptionText.text = cardData.GetFormattedSkill3();
            SetBackgroundColor(skill3BG, cardData.skill3HexColor);
            SetSkillIcon(skill3Image, cardData.skill3HexColor);

            // Populate health bar with maxHealth
            PopulateHealthBar(maxHealth);
        }
        else
        {
            Debug.LogWarning("CardData is null. Cannot assign data to CharacterCard.");
        }
    }

    private void PopulateHealthBar(int health)
    {
        // Instantiate health units based on the card's health value
        for (int i = 0; i < health; i++)
        {
            Instantiate(healthPrefab, healthBar.transform);
        }
    }

    private void SetBackgroundColor(Image backgroundImage, string hexColor)
    {
        if (ColorUtility.TryParseHtmlString($"#{hexColor}", out Color color))
        {
            backgroundImage.color = color;
        }
        else
        {
            Debug.LogWarning($"Invalid hex color string: {hexColor}. Setting default white color.");
            backgroundImage.color = Color.white;
        }
    }

    private void SetSkillIcon(Image skillImage, string hexColor)
    {
        int spriteIndex = GetSpriteIndexFromHex(hexColor);
        if (spriteIndex >= 0 && spriteIndex < skillIcons.Count)
        {
            skillImage.sprite = skillIcons[spriteIndex];
        }
        else
        {
            Debug.LogWarning($"No sprite found for hex color: {hexColor}. Setting default sprite.");
            skillImage.sprite = null;
        }
    }

    private int GetSpriteIndexFromHex(string hexColor)
    {
        switch (hexColor.ToUpper())
        {
            case "D04D4D": return 0; // Red
            case "228B22": return 1; // Green
            case "4169E1": return 2; // Blue
            case "C0C0C0": return 3; // Silver
            default: return -1; // Invalid color
        }
    }

    #region On Battle Actions

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        // Destroy all health units
        foreach (Transform child in healthBar.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate health bar with currentHealth
        PopulateHealthBar(currentHealth);
    }

    #endregion
}