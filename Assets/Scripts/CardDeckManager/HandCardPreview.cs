using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandCardPreview : MonoBehaviour
{
    [Header("UI Elements")]
    public Image previewCardImage;
    public Image previewFrameImage;
    public TextMeshProUGUI previewCardNameText;
    public TextMeshProUGUI previewCardDescriptionText;
    public CanvasGroup canvasGroup; // Use CanvasGroup to control visibility

    public static HandCardPreview Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of HandCardPreview detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        HidePreview(); // Initially hide the preview
    }

    public void SetPreviewData(CardData data)
    {
        if (data != null)
        {
            // Update preview UI elements
            previewCardImage.sprite = data.cardImage;
            previewCardNameText.text = data.cardName;
            previewCardDescriptionText.text = data.cardSkillDescription;
            SetPreviewFrameColor(data.colour);

            ShowPreview(); // Ensure the preview is visible
        }
        else
        {
            Debug.LogWarning("CardData is null. Cannot assign data to HandCardPreview.");
        }
    }

    private void SetPreviewFrameColor(string color)
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

        if (previewFrameImage != null)
        {
            previewFrameImage.color = frameColor;
        }
        else
        {
            Debug.LogWarning("PreviewFrameImage is not assigned in the HandCardPreview.");
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
    public void ShowPreview()
    {
        if (canvasGroup != null)
        {
            // Kill any existing animations on canvasGroup
            canvasGroup.DOKill();

            // Smooth fade-in
            canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutQuad);

            // Optional: Scale animation for added effect
            transform.DOKill(); // Kill ongoing scale animations
            transform.localScale = Vector3.zero; // Start at scale 0
            transform.DOScale(Vector3.one * 2, 0.3f).SetEase(Ease.OutBack); // Target scale is 2

            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void HidePreview()
    {
        if (canvasGroup != null)
        {
            // Kill any existing animations on canvasGroup
            canvasGroup.DOKill();

            // Smooth fade-out
            canvasGroup.DOFade(0, 0.15f).SetEase(Ease.InQuad);

            // Optional: Scale animation for added effect
            transform.DOKill(); // Kill ongoing scale animations
            transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.Linear); // Scale back to 0

            // Disable interactions after fade-out is complete
            DOTween.Sequence()
                .AppendInterval(0.3f)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                });
        }
    }
}