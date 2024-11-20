using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectedCards", menuName = "Card System/Selected Cards")]
public class SelectedCardData : ScriptableObject
{
    public List<string> selectedCharacterIDs = new List<string>();
    public List<string> selectedActionCardIDs = new List<string>();

    /// <summary>
    /// Adds a character ID to the selection list if it doesn't already exist.
    /// </summary>
    /// <param name="id">The ID of the character card.</param>
    public void AddCharacter(string id)
    {
        if (!selectedCharacterIDs.Contains(id))
        {
            selectedCharacterIDs.Add(id);
        }
    }

    /// <summary>
    /// Removes a character ID from the selection list if it exists.
    /// </summary>
    /// <param name="id">The ID of the character card.</param>
    public void RemoveCharacter(string id)
    {
        if (selectedCharacterIDs.Contains(id))
        {
            selectedCharacterIDs.Remove(id);
        }
    }

    /// <summary>
    /// Adds an instance of an action card ID to the selection list.
    /// </summary>
    /// <param name="id">The ID of the action card.</param>
    public void AddActionCard(string id)
    {
        selectedActionCardIDs.Add(id);
    }

    /// <summary>
    /// Removes a single instance of an action card ID from the selection list.
    /// </summary>
    /// <param name="id">The ID of the action card.</param>
    public void RemoveActionCard(string id)
    {
        selectedActionCardIDs.Remove(id);
    }

    /// <summary>
    /// Removes all instances of a specific action card ID from the selection list.
    /// </summary>
    /// <param name="id">The ID of the action card.</param>
    public void RemoveAllInstancesOfActionCard(string id)
    {
        selectedActionCardIDs.RemoveAll(actionId => actionId == id);
    }

    /// <summary>
    /// Clears all selected cards, both character and action cards.
    /// </summary>
    public void ClearAll()
    {
        selectedCharacterIDs.Clear();
        selectedActionCardIDs.Clear();
    }

    /// <summary>
    /// Counts the number of instances of a specific action card ID in the selection list.
    /// </summary>
    /// <param name="id">The ID of the action card.</param>
    /// <returns>The number of instances of the specified action card.</returns>
    public int CountActionCardInstances(string id)
    {
        return selectedActionCardIDs.FindAll(actionId => actionId == id).Count;
    }
}