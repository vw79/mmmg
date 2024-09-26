using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public Image cardImage;

    private Button button;
    private InfoPanel infoPanel;

    void Start()
    {
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
    }

    void OnCardClicked()
    {
        if (infoPanel != null)
        {
            infoPanel.ShowCardInfo(cardData);
        }
    }
}