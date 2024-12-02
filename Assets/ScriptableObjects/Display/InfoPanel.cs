using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Image heartImage;
    [SerializeField] private Image colourImage;
    [SerializeField] private TextMeshProUGUI heartTxt;
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
    [SerializeField] private float minHeight = 50f;
    [SerializeField] private float maxHeight = 300f;
    private Tween heartBumpTween;

    private void Start()
    {
        ToggleCardInfo(false, false);
    }

    public void ShowCardInfo(CardData cardData)
    {
        if (cardData != null)
        {
            // Set card image and other details
            cardImage.sprite = cardData.cardImage;
            cardAttribute.text = cardData.attribute;
            cardName.text = cardData.cardName;

            // Always show and set the colour image
            colourImage.color = ParseColor(cardData.colour);
            colourImage.enabled = true;

            if (cardData.cardID.Contains("c"))
            {
                // For character cards
                cardSkill1.text = cardData.GetFormattedSkill1();
                cardSkill2.text = cardData.GetFormattedSkill2();
                cardSkill3.text = cardData.GetFormattedSkill3();

                AdjustTextBoxSize(cardAttribute, cardAttributeRect);
                AdjustTextBoxSize(cardName, cardNameRect);
                AdjustTextBoxSize(cardSkill1, cardSkill1Rect);
                AdjustTextBoxSize(cardSkill2, cardSkill2Rect);
                AdjustTextBoxSize(cardSkill3, cardSkill3Rect);

                heartImage.enabled = true;
                heartTxt.enabled = true;
                heartTxt.text = cardData.health.ToString();

                StartBumpingHeart();
                ToggleCardInfo(true, true);
            }
            else
            {
                // For non-character cards
                cardSkill1.text = cardData.cardSkillDescription;
                AdjustTextBoxSize(cardAttribute, cardAttributeRect);
                AdjustTextBoxSize(cardName, cardNameRect);
                AdjustTextBoxSize(cardSkill1, cardSkill1Rect);

                // Hide heart image and text
                heartImage.enabled = false;
                heartTxt.enabled = false;
                StopBumpingHeart();
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

        preferredHeight = Mathf.Clamp(preferredHeight, minHeight, maxHeight);

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
        colourImage.enabled = isVisible;

        cardSkill2.enabled = isVisible && isCharacterCard;
        cardSkill3.enabled = isVisible && isCharacterCard;

        if (!isCharacterCard)
        {
            heartImage.enabled = false;
            heartTxt.enabled = false;
        }
    }

    private Color ParseColor(string colorString)
    {
        switch (colorString)
        {
            case "Red":
                if (ColorUtility.TryParseHtmlString("#D04D4D", out Color red))
                    return red;
                break;
            case "Green":
                if (ColorUtility.TryParseHtmlString("#228B22", out Color green))
                    return green;
                break;
            case "Blue":
                if (ColorUtility.TryParseHtmlString("#4169E1", out Color blue))
                    return blue;
                break;
            default:
                Debug.LogWarning($"Unknown color string: {colorString}. Defaulting to white.");
                return Color.white;
        }

        // Fallback in case of hex parsing errors
        Debug.LogWarning($"Failed to parse color string: {colorString}. Defaulting to white.");
        return Color.white;
    }

    private void StartBumpingHeart()
    {
        if (heartImage != null)
        {
            StopBumpingHeart();

            heartBumpTween = heartImage.transform.DOScale(new Vector3(1.2f, 1.2f, 1f), 0.5f)
                .SetEase(Ease.InOutQuad) 
                .SetLoops(-1, LoopType.Yoyo); 
        }
    }

    private void StopBumpingHeart()
    {
        if (heartBumpTween != null && heartBumpTween.IsActive())
        {
            heartBumpTween.Kill();
        }

        if (heartImage != null)
        {
            heartImage.transform.localScale = Vector3.one;
        }
    }
}