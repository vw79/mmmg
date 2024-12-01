using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardAttribute;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardSkill1;
    [SerializeField] private TextMeshProUGUI cardSkill2;
    [SerializeField] private TextMeshProUGUI cardSkill3;

    [SerializeField] private RectTransform cardAttributeRect;
    [SerializeField] private RectTransform cardNameRect;
    [SerializeField] private RectTransform cardSkill1Rect;
    [SerializeField] private RectTransform cardSkill2Rect;
    [SerializeField] private RectTransform cardSkill3Rect;

    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        ToggleCardInfo(false, false);
    }

    public void ShowCardInfo(CardData cardData)
    {
        if (cardData != null)
        {
            cardImage.sprite = cardData.cardImage;
            cardAttribute.text = cardData.attribute;
            cardName.text = cardData.cardName;

            if (cardData.cardID.Contains("c"))
            {
                // Add action point icons using <sprite> tags
                cardSkill1.text = cardData.GetFormattedSkill1();
                cardSkill2.text = cardData.GetFormattedSkill2();
                cardSkill3.text = cardData.GetFormattedSkill3();

                AdjustTextBoxSize(cardAttribute, cardAttributeRect);
                AdjustTextBoxSize(cardName, cardNameRect);
                AdjustTextBoxSize(cardSkill1, cardSkill1Rect);
                AdjustTextBoxSize(cardSkill2, cardSkill2Rect);
                AdjustTextBoxSize(cardSkill3, cardSkill3Rect);

                ToggleCardInfo(true, true);
            }
            else
            {
                cardSkill1.text = cardData.cardSkillDescription;
                AdjustTextBoxSize(cardAttribute, cardAttributeRect);
                AdjustTextBoxSize(cardName, cardNameRect);
                AdjustTextBoxSize(cardSkill1, cardSkill1Rect);

                ToggleCardInfo(true, false);
            }

            ResetScrollPosition();
        }
        else
        {
            ToggleCardInfo(false, false);
        }

        
    }

    private void ResetScrollPosition()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }

    private void AdjustTextBoxSize(TextMeshProUGUI textComponent, RectTransform rectTransform)
    {
        textComponent.ForceMeshUpdate();

        float preferredHeight = textComponent.preferredHeight;

        Vector2 sizeDelta = rectTransform.sizeDelta;
        sizeDelta.y = preferredHeight;
        rectTransform.sizeDelta = sizeDelta;
    }

    private void ToggleCardInfo(bool isVisible, bool isCharacterCard)
    {
        cardImage.enabled = isVisible;
        cardAttribute.enabled = isVisible;
        cardName.enabled = isVisible;
        cardSkill1.enabled = isVisible;

        cardSkill2.enabled = isVisible && isCharacterCard;
        cardSkill3.enabled = isVisible && isCharacterCard;
    }
}