using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandCard : MonoBehaviour
{
    [Header("UI Elements")]
    public string cardIDText;
    public Image cardImage;
    public Image frameImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardDescriptionText;
    public string cardColour;
    public string cardAttribute;

    private HandCardPreview handCardPreview;

    public CardData cardData;

    private void Start()
    {
        handCardPreview = HandCardPreview.Instance;
    }
    public void SetCardData(CardData data)
    {
        cardData = data;

        if (cardData != null)
        {
            cardImage.sprite = cardData.cardImage;
            cardNameText.text = cardData.cardName;
            cardDescriptionText.text = cardData.cardSkillDescription;
            cardColour = cardData.colour;
            cardAttribute = cardData.attribute;
            SetFrameColor(cardData.colour);
        }
        else
        {
            Debug.LogWarning("CardData is null. Cannot assign data to HandCard.");
        }
    }

    private void SetFrameColor(string color)
    {
        Color frameColor;

        switch (color)
        {
            case "Red":
                frameColor = HexToColor("#D04D4D");
                break;
            case "Blue":
                frameColor = HexToColor("#4169E1");
                break;
            case "Green":
                frameColor = HexToColor("#228B22");
                break;
            default:
                frameColor = Color.white;
                Debug.LogWarning($"Unrecognized color: {color}. Setting frame to default (white).");
                break;
        }

        if (frameImage != null)
        {
            frameImage.color = frameColor;
        }
        else
        {
            Debug.LogWarning("FrameImage is not assigned in the HandCard.");
        }
    }

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogWarning($"Invalid hex color string: {hex}. Returning default white color.");
        return Color.white;
    }

    public void TriggerCardPreview()
    {
        if (handCardPreview != null && cardData != null)
        {
            handCardPreview.SetPreviewData(cardData); // Update and show preview
        }
        else if (handCardPreview != null)
        {
            handCardPreview.HidePreview(); // Hide preview if no data is available
        }
        else
        {
            Debug.LogWarning("HandCardPreview reference is null. Cannot update or hide preview.");
        }
    }
}