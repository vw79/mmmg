using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardEffect : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonPressed);
        }
    }

    private void OnButtonPressed()
    {
        // Animate the scale of the button when pressed
        button.transform.DOScale(0.9f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            button.transform.DOScale(1f, 0.2f).SetEase(Ease.OutQuad);
        });
    }
}
