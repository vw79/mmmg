using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private CardSelectionManager cardSelectionManager;
    [SerializeField] PromptManager promptManager;

    public void OnSaveButtonClicked()
    {
        if (cardSelectionManager != null)
        {
            // Validate character and action card selection before saving
            if (cardSelectionManager.ValidateSelections())
            {
                cardSelectionManager.SaveSelectedCards();
                promptManager.ShowPopup("Deck Saved", Color.green);
            }
            else
            {
                promptManager.ShowPopup("Cannot Save", Color.red);
            }
        }
        else
        {
            Debug.LogError("CardSelectionManager not found.");
        }
    }
}