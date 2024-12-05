using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using DG.Tweening;

public class CardSelectionManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private SelectedCardData selectedCardsData;
    [SerializeField] private int maxSelectableCharacters = 6;
    [SerializeField] private int maxTotalActionCards = 40;
    [SerializeField] private int maxSingleActionCard = 2;

    [SerializeField] private TextMeshProUGUI attackerTxt;
    [SerializeField] private TextMeshProUGUI defenderTxt;
    [SerializeField] private TextMeshProUGUI supporterTxt;
    [SerializeField] private TextMeshProUGUI totalCharacterTxt;

    [SerializeField] private TextMeshProUGUI weaponTxt;
    [SerializeField] private TextMeshProUGUI consumablesTxt;
    [SerializeField] private TextMeshProUGUI structureTxt;
    [SerializeField] private TextMeshProUGUI eventTxt;
    [SerializeField] private TextMeshProUGUI magicTxt;
    [SerializeField] private TextMeshProUGUI totalActionTxt;

    [SerializeField] private TextMeshProUGUI redTxt;
    [SerializeField] private TextMeshProUGUI greenTxt;
    [SerializeField] private TextMeshProUGUI blueTxt;

    private int attackerCount = 0;
    private int defenderCount = 0;
    private int supporterCount = 0;
    private int totalCharacterCount = 0;

    private int weaponCount = 0;
    private int consumableCount = 0;
    private int structureCount = 0;
    private int eventCount = 0;
    private int magicCount = 0;
    private int totalActionCardCount = 0;

    private int redCount = 0; 
    private int greenCount = 0; 
    private int blueCount = 0; 

    public bool isSelectCooldown { get; private set; } = false; 

    private Dictionary<string, int> actionCardSelectionCount = new Dictionary<string, int>();
    #endregion

    #region Interactions with Card Display
    // Use in Card Display Script to get the current count of an action card
    public int GetActionCardCount(string cardID)
    {
        if (actionCardSelectionCount.ContainsKey(cardID))
        {
            return actionCardSelectionCount[cardID];
        }
        return 0;
    }

    // Use in Card Display Script to add or remove a card
    public bool AddOrRemoveCard(string cardID, string attribute, string cardColour)
    {
        if (isSelectCooldown) return false;

        isSelectCooldown = true;

        bool result = false;
        if (cardID.Contains("c")) // Character Card
        {
            result = AddOrRemoveCharacter(cardID, attribute);
        }
        else if (cardID.Contains("a")) // Action Card
        {
            result = AddOrRemoveActionCard(cardID, attribute, cardColour);
        }

        isSelectCooldown = false;
        return result;
    }
    #endregion

    #region Character
    private bool AddOrRemoveCharacter(string cardID, string attribute)
    {
        if (selectedCardsData.selectedCharacterIDs.Contains(cardID))
        {
            selectedCardsData.RemoveCharacter(cardID);
            UpdateCharacterCounts(attribute, false); // Decrease counts
            Debug.Log($"Character with ID {cardID} deselected.");
            return false;
        }
        else
        {
            if (selectedCardsData.selectedCharacterIDs.Count < maxSelectableCharacters)
            {
                selectedCardsData.AddCharacter(cardID);
                UpdateCharacterCounts(attribute, true); // Increase counts
                Debug.Log($"Character with ID {cardID} selected.");
                return true;
            }
            else
            {
                Debug.Log("Maximum number of selectable characters reached.");
                return false;
            }
        }
    }
    #endregion

    #region Action Card
    private bool AddOrRemoveActionCard(string cardID, string attribute, string cardColour)
    {
        if (actionCardSelectionCount.ContainsKey(cardID))
        {
            int currentCount = actionCardSelectionCount[cardID];

            if (totalActionCardCount == maxTotalActionCards)
            {
                // If at max limit, clicking the same card should remove it
                actionCardSelectionCount.Remove(cardID); 
                RemoveAllActionCardInstances(cardID);
                UpdateActionCounts(attribute, false, currentCount, cardColour);
                Debug.Log($"Action card {cardID} removed due to max limit. Current count: {currentCount - 1}");
                return false;
            }
            else if (currentCount == maxSingleActionCard)
            {
                // Fully deselect the card when clicked multiple times
                actionCardSelectionCount.Remove(cardID);
                RemoveAllActionCardInstances(cardID);
                UpdateActionCounts(attribute, false, currentCount, cardColour);
                Debug.Log($"Action card {cardID} fully deselected.");
                return false; 
            }
            else
            {
                // Increment count if below the max
                actionCardSelectionCount[cardID]++;
                AddActionCardInstance(cardID); 
                UpdateActionCounts(attribute, true, 1, cardColour); 
                Debug.Log($"Action card {cardID} selected. Current count: {actionCardSelectionCount[cardID]}");
                return true; 
            }
        }
        else
        {
            // Add the first instance if totalActionCardCount allows it
            if (totalActionCardCount < maxTotalActionCards)
            {
                actionCardSelectionCount[cardID] = 1;
                AddActionCardInstance(cardID); 
                UpdateActionCounts(attribute, true, 1, cardColour);
                Debug.Log($"Action card {cardID} selected. Current count: 1");
                return true; 
            }
            else
            {
                Debug.LogWarning($"Cannot add more action cards. Maximum limit of {maxTotalActionCards} reached.");
                return false; 
            }
        }
    }

    private void AddActionCardInstance(string cardID)
    {
        selectedCardsData.selectedActionCardIDs.Add(cardID);
        Debug.Log($"Added action card {cardID} to the list. Total instances: {CountCardInstances(cardID)}");
    }

    private void RemoveAllActionCardInstances(string cardID)
    {
        selectedCardsData.selectedActionCardIDs.RemoveAll(id => id == cardID);
        Debug.Log($"Removed all instances of action card {cardID} from the list.");
    }

    private int CountCardInstances(string cardID)
    {
        return selectedCardsData.selectedActionCardIDs.FindAll(id => id == cardID).Count;
    }
    #endregion

    #region Updating UI
    // Update character counts in UI
    private void UpdateCharacterCounts(string attribute, bool isAdding)
    {
        int change = isAdding ? 1 : -1;

        switch (attribute)
        {
            case "Attacker":
                attackerCount += change;
                UpdateTextWithAnimation(attackerTxt, attackerCount, isAdding);
                break;
            case "Defender":
                defenderCount += change;
                UpdateTextWithAnimation(defenderTxt, defenderCount, isAdding);
                break;
            case "Supporter":
                supporterCount += change;
                UpdateTextWithAnimation(supporterTxt, supporterCount, isAdding);
                break;
        }

        totalCharacterCount += change;
        UpdateTextWithAnimation(totalCharacterTxt, totalCharacterCount, isAdding, maxSelectableCharacters);
    }

    // Update action card counts in UI
    private void UpdateActionCounts(string attribute, bool isAdding, int change, string colour)
    {
        int adjustment = isAdding ? change : -change;

        // Update specific attribute count
        switch (attribute)
        {
            case "Weapon":
                weaponCount += adjustment;
                UpdateTextWithAnimation(weaponTxt, weaponCount, isAdding);
                break;
            case "Consumable":
                consumableCount += adjustment;
                UpdateTextWithAnimation(consumablesTxt, consumableCount, isAdding);
                break;
            case "Structure":
                structureCount += adjustment;
                UpdateTextWithAnimation(structureTxt, structureCount, isAdding);
                break;
            case "Event":
                eventCount += adjustment;
                UpdateTextWithAnimation(eventTxt, eventCount, isAdding);
                break;
            case "Magic":
                magicCount += adjustment;
                UpdateTextWithAnimation(magicTxt, magicCount, isAdding);
                break;
            default:
                Debug.LogWarning($"Unknown attribute: {attribute}");
                return; // Avoid updating total count for unknown attributes
        }

        // Update total action card count
        totalActionCardCount += adjustment;
        UpdateTextWithAnimation(totalActionTxt, totalActionCardCount, isAdding, maxTotalActionCards);

        // Update colour counts
        switch (colour)
        {
            case "Red":
                redCount += adjustment;
                UpdateTextWithAnimation(redTxt, redCount, isAdding);
                break;
            case "Green":
                greenCount += adjustment;
                UpdateTextWithAnimation(greenTxt, greenCount, isAdding);
                break;
            case "Blue":
                blueCount += adjustment;
                UpdateTextWithAnimation(blueTxt, blueCount, isAdding);
                break;
            default:
                Debug.LogWarning($"Unknown colour: {colour}");
                break;
        }

        Debug.Log($"Updated {attribute} count. WeaponCount: {weaponCount}, TotalActionCardCount: {totalActionCardCount}, Red: {redCount}, Green: {greenCount}, Blue: {blueCount}");
    }

    // Update with DOTween animation
    private void UpdateTextWithAnimation(TextMeshProUGUI textElement, int value, bool isAdding, int? maxValue = null)
    {
        textElement.text = maxValue.HasValue ? $"{value} / {maxValue}" : $"{value}";

        // Change text color based on increase or decrease
        Color targetColor = isAdding ? Color.green : Color.red;
        textElement.color = targetColor;

        textElement.DOColor(Color.white, 0.75f);

        // Apply the punch scale effect
        textElement.transform.localScale = Vector3.one;
        textElement.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1);
    }
    #endregion

    #region Reset on Switch to Editor Mode
    public void ResetSelectedCards()
    {
        selectedCardsData.ClearAll();

        // Reset character counts and UI
        attackerCount = 0;
        defenderCount = 0;
        supporterCount = 0;
        totalCharacterCount = 0;
        attackerTxt.text = "0";
        defenderTxt.text = "0";
        supporterTxt.text = "0";
        totalCharacterTxt.text = "0 / 4";

        // Reset action card counts and UI
        weaponCount = 0;
        consumableCount = 0;
        structureCount = 0;
        eventCount = 0;
        magicCount = 0;
        totalActionCardCount = 0;
        weaponTxt.text = "0";
        consumablesTxt.text = "0";
        structureTxt.text = "0";
        eventTxt.text = "0";
        magicTxt.text = "0";
        totalActionTxt.text = "0 / 20";

        // Reset colour counts and UI
        redCount = 0;
        greenCount = 0;
        blueCount = 0;
        redTxt.text = "0";
        greenTxt.text = "0";
        blueTxt.text = "0";

        actionCardSelectionCount.Clear();

        Debug.Log("Selected cards have been reset.");
    }
    #endregion

    #region Validation and Saving
    //Validate selections before saving
    public bool ValidateSelections()
    {
        // Check if exactly 6 characters are selected
        if (selectedCardsData.selectedCharacterIDs.Count != maxSelectableCharacters)
        {
            Debug.LogWarning($"You must select exactly {maxSelectableCharacters} characters.");
            return false;
        }

        // Check if total action cards are exactly 40
        if (totalActionCardCount != maxTotalActionCards)
        {
            Debug.LogWarning($"Total action cards must be exactly {maxTotalActionCards}.");
            return false;
        }

        // Check if no individual action card exceeds the maximum allowed
        foreach (var pair in actionCardSelectionCount)
        {
            Debug.Log($"Action Card {pair.Key}: Count = {pair.Value}");
            if (pair.Value > maxSingleActionCard)
            {
                Debug.LogWarning($"Action card {pair.Key} exceeds the allowed limit of {maxSingleActionCard} per card.");
                return false;
            }
        }

        return true; // All validations passed
    }

    // Save selected cards to a ScriptableObject asset
    public void SaveSelectedCards()
    {
        SelectedCardData newSelectedCardsData = ScriptableObject.CreateInstance<SelectedCardData>();

        newSelectedCardsData.selectedCharacterIDs = new List<string>(selectedCardsData.selectedCharacterIDs);

        newSelectedCardsData.selectedActionCardIDs = new List<string>(selectedCardsData.selectedActionCardIDs);

#if UNITY_EDITOR
        //string path = "Assets/SavedSelectedCards.asset";
        //AssetDatabase.CreateAsset(newSelectedCardsData, path);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();

        //Debug.Log($"Selected cards successfully saved at {path}");
        JsonSaveDeck.instance.SaveDeck(newSelectedCardsData);
#else
        JsonSaveDeck.instance.SaveDeck(newSelectedCardsData);
#endif
    }
    #endregion
}