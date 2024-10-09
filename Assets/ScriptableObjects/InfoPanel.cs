using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardSkill1;
    public TextMeshProUGUI cardSkill2;
    public TextMeshProUGUI cardSkill3;

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
            cardSkill1.text = cardData.skill1;
            cardSkill2.text = cardData.skill2;
            cardSkill3.text = cardData.skill3;
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
        cardSkill1.enabled = isVisible;
        cardSkill2.enabled = isVisible;
        cardSkill3.enabled = isVisible;
    }
}