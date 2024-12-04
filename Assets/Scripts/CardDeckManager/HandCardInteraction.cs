using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static HandCardInteraction CurrentDraggingCard { get; private set; }

    [Header("Animation Settings")]
    public float moveUpDistance = 50f;
    public float moveDuration = 0.3f;

    private enum CardState
    {
        Idle,
        Previewing,
        Dragging,
        Animating
    }

    private CardState currentState = CardState.Idle;
    private Vector3 originalLocalPosition;
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private CustomHandLayout handLayout;

    private HandCard handCard;
    public CardUseManager cardUseManager;

    private List<Rect> cachedAreaRects;
    private Tween longPressTween;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        handLayout = GetComponentInParent<CustomHandLayout>();
        originalLocalPosition = rectTransform.localPosition;

        // Cache HandCard reference
        handCard = GetComponent<HandCard>();
        cardUseManager = CardUseManager.Instance;

        CacheInteractableAreaRects();
    }

    #region Input Events

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentState == CardState.Animating) return;

        CurrentDraggingCard = this;
        currentState = CardState.Idle;

        originalLocalPosition = rectTransform.localPosition;

        longPressTween = DOVirtual.DelayedCall(0.2f, OnLongPress);

        if (handLayout != null)
        {
            handLayout.RemoveCard(rectTransform);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (currentState == CardState.Animating) return;

        longPressTween?.Kill();

        if (currentState == CardState.Dragging)
        {
            if (cardUseManager != null)
            {
                // Check if the card is released on a hovered area
                if (cardUseManager.GetCurrentHoveredArea() != null)
                {
                    cardUseManager.HandleCardUseOnHoveredArea(gameObject);
                    return; // Exit here to avoid resetting the position
                }
            }

            // If not released on an interactable area, reset position
            ResetCardPosition(true);
        }
        else if (currentState == CardState.Previewing)
        {
            ResetCardPosition(true);
        }

        CurrentDraggingCard = null;
        currentState = CardState.Idle;

        if (HandCardPreview.Instance != null)
        {
            HandCardPreview.Instance.HidePreview();
        }

        if (cardUseManager != null)
        {
            cardUseManager.HideSelfInteractableAreas();
            cardUseManager.ClearHoveredArea(null);
        }

        if (handLayout != null)
        {
            handLayout.AddCard(rectTransform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentState == CardState.Previewing)
        {
            currentState = CardState.Dragging;

            if (HandCardPreview.Instance != null)
            {
                HandCardPreview.Instance.HidePreview();
            }

            if (cardUseManager != null)
            {
                cardUseManager.ShowSelfInteractableAreas();
            }
        }

        if (currentState == CardState.Idle)
        {
            longPressTween?.Kill();
            currentState = CardState.Dragging;

            if (cardUseManager != null)
            {
                cardUseManager.ShowSelfInteractableAreas();
            }
        }

        if (currentState == CardState.Dragging)
        {
            // Move the card along with the pointer
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                eventData.position,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );

            Vector3 newPosition = parentCanvas.GetComponent<RectTransform>().TransformPoint(localPoint);
            rectTransform.position = newPosition;

            rectTransform.localRotation = Quaternion.Euler(rectTransform.localRotation.eulerAngles.x, rectTransform.localRotation.eulerAngles.y, 0);

            // Perform overlap detection with interactable areas
            CheckOverlapWithInteractableAreas();
        }
    }

    #endregion

    #region Card Movement and Preview

    private void OnLongPress()
    {
        currentState = CardState.Previewing;
        MoveCardUp();
    }

    private void MoveCardUp()
    {
        currentState = CardState.Animating;

        rectTransform.DOLocalMove(originalLocalPosition + new Vector3(0, moveUpDistance, 0), moveDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                currentState = CardState.Previewing;

                // Show the card preview
                if (handCard != null)
                {
                    handCard.TriggerCardPreview();
                }
            });
    }

    public void ResetCardPosition(bool useOriginalPosition)
    {
        rectTransform.DOKill(); // Stop any running tweens to avoid conflicts
        currentState = CardState.Animating;

        Vector3 targetPosition = useOriginalPosition ? originalLocalPosition : rectTransform.localPosition;

        rectTransform.DOLocalMove(targetPosition, moveDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                currentState = CardState.Idle;
            });
    }

    #endregion

    #region Interactable Areas

    private void CacheInteractableAreaRects()
    {
        cachedAreaRects = new List<Rect>();

        if (cardUseManager != null)
        {
            foreach (var area in cardUseManager.selfInteractableAreas)
            {
                cachedAreaRects.Add(GetWorldRect(area.rectTransform));
            }
        }
    }

    private void CheckOverlapWithInteractableAreas()
    {
        Rect cardRect = GetWorldRect(rectTransform);

        for (int i = 0; i < cachedAreaRects.Count; i++)
        {
            if (cardRect.Overlaps(cachedAreaRects[i], true))
            {
                cardUseManager.SetHoveredArea(cardUseManager.selfInteractableAreas[i], i);
                return; // Only handle one area at a time
            }
        }

        cardUseManager.ClearHoveredArea(null);
    }

    #endregion

    #region Utility

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // Bottom-left and top-right corners
        Vector3 bottomLeft = corners[0];
        Vector3 topRight = corners[2];

        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    #endregion
}