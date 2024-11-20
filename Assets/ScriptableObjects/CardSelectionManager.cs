using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
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
    private int consumablesCount = 0;
    private int structureCount = 0;
    private int eventCount = 0;
    private int magicCount = 0;
    private int totalActionCardCount = 0;

    private Dictionary<string, int> actionCardSelectionCount = new Dictionary<string, int>();

    public int GetActionCardCount(string cardID)
    {
        if (actionCardSelectionCount.ContainsKey(cardID))
        {
            return actionCardSelectionCount[cardID];
        }
        return 0;
    }

    public bool AddOrRemoveCard(string cardID, string attribute)
    {
        if (cardID.Contains("c")) // Character Card
        {
            return AddOrRemoveCharacter(cardID, attribute);
        }
        else if (cardID.Contains("a")) // Action Card
        {
            return AddOrRemoveActionCard(cardID, attribute);
        }

        return false;
    }

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

    private bool AddOrRemoveActionCard(string cardID, string attribute)
    {
        if (actionCardSelectionCount.ContainsKey(cardID))
        {
            if (actionCardSelectionCount[cardID] == maxSingleActionCard)
            {
                // Reset the count if it's already at the max
                actionCardSelectionCount.Remove(cardID); // Remove from the dictionary
                RemoveAllActionCardInstances(cardID); // Remove all instances from the list
                UpdateActionCounts(attribute, false); // Update counts and UI
                Debug.Log($"Action card {cardID} fully deselected. Current count: 0");
                return false; // Card removed
            }
            else
            {
                // Increment count if below the max
                actionCardSelectionCount[cardID]++;
                AddActionCardInstance(cardID); // Add to the list
                UpdateActionCounts(attribute, true); // Update counts and UI
                Debug.Log($"Action card {cardID} selected. Current count: {actionCardSelectionCount[cardID]}");
                return true; // Card added
            }
        }
        else
        {
            // Initialize and select the card if not in the dictionary
            actionCardSelectionCount[cardID] = 1;
            AddActionCardInstance(cardID); // Add to the list
            UpdateActionCounts(attribute, true); // Update counts and UI
            Debug.Log($"Action card {cardID} selected. Current count: {actionCardSelectionCount[cardID]}");
            return true; // Card added
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

    private void UpdateCharacterCounts(string attribute, bool isAdding)
    {
        int change = isAdding ? 1 : -1;

        switch (attribute)
        {
            case "Attacker":
                attackerCount += change;
                attackerTxt.text = $"{attackerCount}";
                break;
            case "Defender":
                defenderCount += change;
                defenderTxt.text = $"{defenderCount}";
                break;
            case "Supporter":
                supporterCount += change;
                supporterTxt.text = $"{supporterCount}";
                break;
        }

        totalCharacterCount += change;
        totalCharacterTxt.text = $"{totalCharacterCount} / 6"; ;
    }

    private void UpdateActionCounts(string attribute, bool isAdding)
    {
        int change = isAdding ? 1 : -1;

        switch (attribute)
        {
            case "Weapon":
                weaponCount += change;
                weaponTxt.text = $"{weaponCount}";
                break;
            case "Consumables":
                consumablesCount += change;
                consumablesTxt.text = $"{consumablesCount}";
                break;
            case "Stucture":
                structureCount += change;
                structureTxt.text = $"{structureCount}";
                break;
            case "Event":
                eventCount += change;
                eventTxt.text = $"{eventCount}";
                break;
            case "Magic":
                magicCount += change;
                magicTxt.text = $"{magicCount}";
                break;
        }

        totalActionCardCount += change;
        totalActionTxt.text = $"{totalActionCardCount} / 40";
    }

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
        totalCharacterTxt.text = "0 / 6";

        // Reset action card counts and UI
        weaponCount = 0;
        consumablesCount = 0;
        structureCount = 0;
        eventCount = 0;
        magicCount = 0;
        totalActionCardCount = 0;
        weaponTxt.text = "0";
        consumablesTxt.text = "0";
        structureTxt.text = "0";
        eventTxt.text = "0";
        magicTxt.text = "0";
        totalActionTxt.text = "0 / 40";

        actionCardSelectionCount.Clear();

        Debug.Log("Selected cards have been reset.");
    }

    public bool ValidateSelections()
    {
        // Check if exactly 6 characters are selected
        if (selectedCardsData.selectedCharacterIDs.Count != maxSelectableCharacters)
        {
            Debug.LogWarning("You must select exactly 6 characters.");
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

    public void SaveSelectedCards()
    {
        // Create a new ScriptableObject instance to save selected cards
        SelectedCardData newSelectedCardsData = ScriptableObject.CreateInstance<SelectedCardData>();

        // Copy selected character IDs
        newSelectedCardsData.selectedCharacterIDs = new List<string>(selectedCardsData.selectedCharacterIDs);

        // Copy selected action card IDs
        newSelectedCardsData.selectedActionCardIDs = new List<string>(selectedCardsData.selectedActionCardIDs);

        // Save the ScriptableObject as an asset in the Unity Editor
#if UNITY_EDITOR
        string path = "Assets/SavedSelectedCards.asset";
        AssetDatabase.CreateAsset(newSelectedCardsData, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Selected cards successfully saved at {path}");
#else
        Debug.LogError("Saving ScriptableObject assets is only supported in the Unity Editor.");
#endif
    }
}