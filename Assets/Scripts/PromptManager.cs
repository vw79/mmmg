using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PromptManager : MonoBehaviour
{
    public RectTransform popupPanel;
    public TextMeshProUGUI popupText;

    public CanvasGroup popupCanvasGroup;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = popupPanel.localPosition;
    }

    public void ShowPopup(string message, Color textColor)
    {
        popupText.text = message;
        popupText.color = textColor;

        popupPanel.localPosition = originalPosition + new Vector3(200, 0, 0);
        popupCanvasGroup.alpha = 0;

        popupPanel.DOLocalMove(originalPosition, 0.5f).SetEase(Ease.OutQuad);
        popupCanvasGroup.DOFade(1, 0.5f);

        DOVirtual.DelayedCall(2f, () =>
        {
            popupPanel.DOLocalMove(originalPosition + new Vector3(-200, 0, 0), 0.5f).SetEase(Ease.InQuad);
            popupCanvasGroup.DOFade(0, 0.5f);
        });
    }
}