using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData; // Holds data for this card
    public Image cardImage;  // Displays the card's image

    private Button button;
    private InfoPanel infoPanel;
    private GameObject count;
    private TextMeshProUGUI countText;
    public SwitchToggle switchToggle;

    private CardSelectionManager cardSelectionManager;
    private bool isSelected = false;
    private int currentCount = 0;

    void Start()
    {
        count = transform.Find("Count")?.gameObject;
        if (count != null)
        {
            countText = count.GetComponentInChildren<TextMeshProUGUI>();
            count.SetActive(false);
        }

        if (cardData != null && cardImage != null)
        {
            cardImage.sprite = cardData.cardImage;
        }

        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnCardClicked);
        }

        infoPanel = FindObjectOfType<InfoPanel>();
        cardSelectionManager = FindObjectOfType<CardSelectionManager>();

        // Synchronize the initial count with CardSelectionManager
        if (cardData != null && cardData.cardID.Contains("a"))
        {
            currentCount = cardSelectionManager.GetActionCardCount(cardData.cardID);
            UpdateCountDisplay();
        }
    }

    private void Update()
    {
        if (!switchToggle.isEditorMode)
        {
            count.SetActive(false);
            currentCount = 0;
        }
    }

    void OnCardClicked()
    {
        if (switchToggle.isEditorMode && cardData != null)
        {
            if (cardData.cardID.Contains("c")) // Character Card
            {
                bool isSelected = cardSelectionManager.AddOrRemoveCard(cardData.cardID, cardData.attribute);

                // Update visual indication of selection
                count.SetActive(isSelected);
                if (isSelected)
                {
                    countText.text = "1";
                }
            }
            else if (cardData.cardID.Contains("a")) // Action Card
            {
                // Synchronize currentCount with CardSelectionManager
                bool actionAdded = cardSelectionManager.AddOrRemoveCard(cardData.cardID, cardData.attribute);
                currentCount = cardSelectionManager.GetActionCardCount(cardData.cardID);
                UpdateCountDisplay();
            }
        }
        else
        {
            if (infoPanel != null)
            {
                infoPanel.ShowCardInfo(cardData);
            }
        }
    }

    private void UpdateCountDisplay()
    {
        if (currentCount > 0)
        {
            count.SetActive(true);
            countText.text = currentCount.ToString();
        }
        else
        {
            count.SetActive(false);
        }
    }
}