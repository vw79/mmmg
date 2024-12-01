using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardUseManager : MonoBehaviour
{
    private Image currentHoveredArea;

    [Header("Interactable Areas")]
    public Image[] interactableAreas;

    private Color originalColor;
    private float originalAlpha;

    public static CardUseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (interactableAreas.Length > 0 && interactableAreas[0] != null)
        {
            originalColor = interactableAreas[0].color;
            originalAlpha = originalColor.a;
        }

        foreach (var area in interactableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(false);
            }
        }
    }

    #region Show/Hide Interactable Areas

    public void ShowInteractableAreas()
    {
        foreach (var area in interactableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(true);
                area.GetComponent<CanvasGroup>().alpha = 0;
                area.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
            }
        }
    }

    public void HideInteractableAreas()
    {
        if (currentHoveredArea != null)
        {
            ResetAreaColor(currentHoveredArea);
            currentHoveredArea = null;
        }

        foreach (var area in interactableAreas)
        {
            if (area != null)
            {
                area.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() =>
                {
                    area.gameObject.SetActive(false);
                });
            }
        }
    }

    #endregion

    #region Interaction Handlers

    public void SetHoveredArea(Image area)
    {
        if (currentHoveredArea != area)
        {
            // Reset the previous area's color
            if (currentHoveredArea != null)
            {
                ResetAreaColor(currentHoveredArea);
            }

            // Update to the new hovered area
            currentHoveredArea = area;
            if (currentHoveredArea != null)
            {
                SetAreaHoverColor(currentHoveredArea);
            }
        }
    }

    public void ClearHoveredArea(Image area)
    {
        if (currentHoveredArea == area || area == null)
        {
            // Reset the current area's color
            if (currentHoveredArea != null)
            {
                ResetAreaColor(currentHoveredArea);
            }

            currentHoveredArea = null;
        }
    }

    public void HandleCardUseOnHoveredArea(GameObject card)
    {
        if (currentHoveredArea == null)
        {
            Debug.Log("No valid area hovered. Card returned.");
            ReturnCardToOriginalPosition(card);
            return;
        }

        // Get CardData for the hovered area
        CardData hoveredCardData = GetCardDataForArea(currentHoveredArea);

        // Get CardData for the used card
        HandCard handCard = card.GetComponent<HandCard>();
        if (handCard == null || handCard.cardData == null)
        {
            Debug.LogWarning("The card being used does not have valid CardData!");
            ReturnCardToOriginalPosition(card);
            return;
        }

        CardData usedCardData = handCard.cardData;

        // Validate card colors
        if (ValidateCardColor(usedCardData, hoveredCardData))
        {
            HandleCardUsage(card, usedCardData, hoveredCardData);
        }
        else
        {
            Debug.Log($"Card '{usedCardData.cardName}' color does not match with '{hoveredCardData.cardName}'!");
            ReturnCardToOriginalPosition(card);
        }
    }

    #endregion

    #region Utility Methods

    private void SetAreaHoverColor(Image area)
    {
        if (area != null)
        {
            Color hoverColor = Color.red;
            hoverColor.a = originalAlpha;
            area.color = hoverColor;
        }
    }

    public void ResetAreaColor(Image area)
    {
        if (area != null)
        {
            Color resetColor = originalColor;
            resetColor.a = originalAlpha;
            area.color = resetColor;
        }
    }

    public Image GetCurrentHoveredArea()
    {
        return currentHoveredArea;
    }

    private CardData GetCardDataForArea(Image area)
    {
        if (CardManager.Instance.interactableAreaToCardData.TryGetValue(area, out CardData cardData))
        {
            return cardData;
        }

        Debug.LogWarning("No CardData is associated with the hovered area!");
        return null;
    }

    private bool ValidateCardColor(CardData usedCardData, CardData hoveredCardData)
    {
        if (hoveredCardData == null)
        {
            Debug.LogWarning("Hovered card data is null.");
            return false;
        }

        return usedCardData.colour == hoveredCardData.colour;
    }
    #endregion

    #region Card Position Management
    private void ReturnCardToOriginalPosition(GameObject card)
    {
        HandCardInteraction handCardInteraction = card.GetComponent<HandCardInteraction>();

        if (handCardInteraction != null)
        {
            handCardInteraction.ResetCardPosition(true);

            CustomHandLayout handLayout = handCardInteraction.GetComponentInParent<CustomHandLayout>();
            if (handLayout != null)
            {
                HideInteractableAreas();
                RectTransform cardRectTransform = card.GetComponent<RectTransform>();
                handLayout.AddCard(cardRectTransform);
            }
            else
            {
                Debug.LogWarning("HandLayout is missing in the parent hierarchy. Cannot add the card back to the layout.");
            }
        }
        else
        {
            Debug.LogWarning("HandCardInteraction script is missing on the card. Cannot reset position.");
        }
    }

    #endregion

    #region Netcode
    //Eon Pls Work Here
    private void HandleCardUsage(GameObject card, CardData usedCardData, CardData hoveredCardData)
    {
        Debug.Log($"Card '{usedCardData.cardName}' is used on '{hoveredCardData.cardName}'.");

        // Synchronize card usage across clients (optional netcode integration)
        SynchronizeCardUsage(card, usedCardData, hoveredCardData);

        // Hide interactable areas
        HideInteractableAreas();

        // Destroy the card
        Destroy(card);
    }

    private void SynchronizeCardUsage(GameObject card, CardData usedCardData, CardData hoveredCardData)
    {
        // Netcode: Broadcast card usage event to all clients
        // Example: Send cardID and hoveredAreaID
    }

    private void SynchronizeCardReturn(GameObject card)
    {
        // Netcode: Notify all clients to return the card to its original position
    }

    #endregion
}