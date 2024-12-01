using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectedCards", menuName = "Card System/Selected Cards")]
public class SelectedCardData : ScriptableObject
{
    public List<string> selectedCharacterIDs = new List<string>();
    public List<string> selectedActionCardIDs = new List<string>();

    public void AddCharacter(string id)
    {
        if (!selectedCharacterIDs.Contains(id))
        {
            selectedCharacterIDs.Add(id);
        }
    }

    public void RemoveCharacter(string id)
    {
        if (selectedCharacterIDs.Contains(id))
        {
            selectedCharacterIDs.Remove(id);
        }
    }

    public void AddActionCard(string id)
    {
        selectedActionCardIDs.Add(id);
    }

    public void RemoveActionCard(string id)
    {
        selectedActionCardIDs.Remove(id);
    }

    public void RemoveAllInstancesOfActionCard(string id)
    {
        selectedActionCardIDs.RemoveAll(cardID => cardID == id);
    }

    public void ClearAll()
    {
        selectedCharacterIDs.Clear();
        selectedActionCardIDs.Clear();
    }

    public int CountActionCardInstances(string id)
    {
        return selectedActionCardIDs.FindAll(actionId => actionId == id).Count;
    }
}