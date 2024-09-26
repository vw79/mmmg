using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;

    private void Start()
    {
        ToggleCardInfo(false);
    }

    public void ShowCardInfo(CardData cardData)
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardImage;
            cardName.text = cardData.cardName;
            cardDescription.text = cardData.cardDescription;
            ToggleCardInfo(true);
        }
        else
        {
            ToggleCardInfo(false);
        }
    }

    private void ToggleCardInfo(bool isVisible)
    {
        cardImage.enabled = isVisible;
        cardName.enabled = isVisible;
        cardDescription.enabled = isVisible;
    }
}