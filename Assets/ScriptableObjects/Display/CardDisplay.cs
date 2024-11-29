using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public Image cardImage;

    private Button button;
    private InfoPanel infoPanel;
    private GameObject count;
    private TextMeshProUGUI countText;
    public SwitchToggle switchToggle;

    private CardSelectionManager cardSelectionManager;
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
            if (cardSelectionManager.isSelectCooldown) return;

            if (cardData.cardID.Contains("c")) // Character Card
            {
                bool isSelected = cardSelectionManager.AddOrRemoveCard(cardData.cardID, cardData.attribute, cardData.colour);

                // Update visual indication of selection
                count.SetActive(isSelected);
                if (isSelected)
                {
                    countText.text = "1";
                }
            }
            else if (cardData.cardID.Contains("a")) // Action Card
            {
                // Attempt to add or remove action card
                bool actionAdded = cardSelectionManager.AddOrRemoveCard(cardData.cardID, cardData.attribute, cardData.colour);

                // Update count display only if the action card was successfully added or removed
                if (actionAdded)
                {
                    currentCount = cardSelectionManager.GetActionCardCount(cardData.cardID);
                    UpdateCountDisplay();
                }
                else
                {
                    Debug.LogWarning($"Action card {cardData.cardID} could not be added. Maximum limit reached.");
                    currentCount = cardSelectionManager.GetActionCardCount(cardData.cardID);
                    UpdateCountDisplay(); // Ensures display is cleared if needed
                }
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