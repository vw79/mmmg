using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomHandLayout : MonoBehaviour
{
    public List<RectTransform> cards = new List<RectTransform>();

    [Header("Arc Layout Settings")]
    public float maxArcHeight = 200f;
    public float minArcHeight = 50f;
    public float maxRotation = 15f;
    public float minRotation = 5f;
    public float maxSpacing = 100f;
    public float minSpacing = 40f;
    public int maxCards = 10;

    [Header("Animation Settings")]
    public float updateDuration = 0.3f;

    public void AddCard(RectTransform card)
    {
        cards.Add(card);
        UpdateLayout();
    }

    public void RemoveCard(RectTransform card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            UpdateLayout();
        }
    }

    public void UpdateLayout()
    {
        if (cards.Count == 0) return;

        // Calculate spacing, arc height, and rotation based on the number of cards
        float totalWidth = Mathf.Lerp(maxSpacing, minSpacing, (float)(cards.Count - 1) / (maxCards - 1)) * (cards.Count - 1);
        float startX = -totalWidth / 2;

        float arcHeight = Mathf.Lerp(minArcHeight, maxArcHeight, Mathf.Clamp01((float)(cards.Count - 1) / (maxCards - 1)));
        float maxRotationAdjusted = Mathf.Lerp(minRotation, maxRotation, Mathf.Clamp01((float)(cards.Count - 1) / (maxCards - 1)));
        float spacing = Mathf.Lerp(maxSpacing, minSpacing, Mathf.Clamp01((float)(cards.Count - 1) / (maxCards - 1)));

        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform card = cards[i];

            // Calculate horizontal position
            float xPos = startX + (i * spacing);

            // Calculate arc height (parabolic)
            float t = (float)i / (cards.Count - 1);
            float yPos = -Mathf.Pow(t - 0.5f, 2) * 4 * arcHeight + arcHeight;

            // Adjust yPos to ensure the first and last cards don't dip too low
            if (i == 0 || i == cards.Count - 1)
            {
                yPos += arcHeight * 0.3f; // Slightly elevate the first and last cards
            }

            // Ensure arc height is not NaN
            if (float.IsNaN(yPos)) yPos = 0;

            // Adjust rotation: first card decreases z-rotation, last card increases z-rotation
            float zRotation = Mathf.Lerp(maxRotationAdjusted, -maxRotationAdjusted, t);

            // Ensure z-rotation is not NaN
            if (float.IsNaN(zRotation)) zRotation = 0;

            // Dynamic scaling for larger hands
            float scale = Mathf.Lerp(1f, 0.8f, Mathf.Clamp01((float)(cards.Count - 1) / (maxCards - 1)));

            // Animate position, rotation, and scale
            card.DOLocalMove(new Vector3(xPos, yPos, 0), updateDuration).SetEase(Ease.OutCubic);
            card.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, zRotation), updateDuration).SetEase(Ease.OutCubic);
            card.DOScale(new Vector3(scale, scale, 1), updateDuration).SetEase(Ease.OutCubic);

            // Ensure proper depth order
            card.SetSiblingIndex(i);
        }
    }
}