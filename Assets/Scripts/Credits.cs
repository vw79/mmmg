using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class Credits : MonoBehaviour
{
    public CanvasGroup logo;
    public CanvasGroup[] buttons;
    public float fadeDuration = 0.5f;
    public GameObject creditPanel;

    private void Start()
    {
        creditPanel.SetActive(false);
    }

    public void PlayCredits()
    {
        Sequence sequence = DOTween.Sequence();

        // Fade out buttons
        foreach (var buttonCanvasGroup in buttons)
        {
            sequence.Join(buttonCanvasGroup.DOFade(0f, fadeDuration));
        }

        // Fade out logo
        sequence.Join(logo.DOFade(0f, fadeDuration));

        // Enable the credit panel
        sequence.AppendCallback(() =>
        {
            creditPanel.SetActive(true);
        });

        // Wait for 8 seconds while the credit panel is active
        sequence.AppendInterval(8f);

        // Disable the credit panel
        sequence.AppendCallback(() =>
        {
            creditPanel.SetActive(false);
        });

        // Fade in buttons
        foreach (var buttonCanvasGroup in buttons)
        {
            sequence.Join(buttonCanvasGroup.DOFade(1f, fadeDuration));
        }

        // Fade in logo
        sequence.Join(logo.DOFade(1f, fadeDuration));
    }
}