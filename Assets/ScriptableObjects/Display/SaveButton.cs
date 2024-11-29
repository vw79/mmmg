using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    private CardSelectionManager cardSelectionManager;

    void Start()
    {
        cardSelectionManager = FindObjectOfType<CardSelectionManager>();
    }

    public void OnSaveButtonClicked()
    {
        if (cardSelectionManager != null)
        {
            // Validate character and action card selection before saving
            if (cardSelectionManager.ValidateSelections())
            {
                cardSelectionManager.SaveSelectedCards();
                Debug.Log("Selected cards successfully saved.");
            }
            else
            {
                Debug.LogWarning("Invalid selection. Ensure you have selected 6 characters and within the allowed action cards limit.");
            }
        }
        else
        {
            Debug.LogError("CardSelectionManager not found.");
        }
    }
}