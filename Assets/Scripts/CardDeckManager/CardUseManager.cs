using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardUseManager : MonoBehaviour
{
    private Image currentHoveredArea;
    private int currentHoveredAreaIndex;
    public PromptManager promptManager;

    [Header("Interactable Areas")]
    public Image[] selfInteractableAreas;
    public GameObject selectEnemyPanel;
    public Image[] opponentInteractableAreas;

    private Color originalColor;
    private float originalAlpha;

    private GameManager gameManagerRef;
    public static CardUseManager Instance { get; private set; }

    private string attackerID;

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
        if (selfInteractableAreas.Length > 0 && selfInteractableAreas[0] != null)
        {
            originalColor = selfInteractableAreas[0].color;
            originalAlpha = originalColor.a;
        }

        foreach (var area in selfInteractableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(false);
            }
        }

        foreach (var area in opponentInteractableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(false);
            }
        }
    }

    #region Show/Hide Interactable Areas

    public void ShowSelfInteractableAreas()
    {
        foreach (var area in selfInteractableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(true);
                area.GetComponent<CanvasGroup>().alpha = 0;
                area.transform.localScale = Vector3.one; // Reset scale
                area.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
            }
        }
    }

    public void ShowOpponentInteractableAreas()
    {
        selectEnemyPanel.SetActive(true);
        for (int i = 0; i < 4 ; i++)
        {
            var area = opponentInteractableAreas[i];
            if (area != null)
            {
                if (gameManagerRef.ValidateOpponentCharacterAlive(i))
                {
                    area.gameObject.SetActive(true);
                    area.transform.localScale = Vector3.one; // Reset scale
                }
                else
                {
                    area.gameObject.SetActive(false);
                }
            }
        }
    }

    public void HideSelfInteractableAreas()
    {
        if (currentHoveredArea != null)
        {
            ResetAreaColor(currentHoveredArea);
            currentHoveredArea = null;
        }

        foreach (var area in selfInteractableAreas)
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

    private void HideInteractableAreasExcept(Image areaToExclude)
    {
        if (currentHoveredArea != null && currentHoveredArea != areaToExclude)
        {
            ResetAreaColor(currentHoveredArea);
            currentHoveredArea = null;
        }

        foreach (var area in selfInteractableAreas)
        {
            if (area != null && area != areaToExclude)
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

    public void SetHoveredArea(Image area, int index)
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
            currentHoveredAreaIndex = index;
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
            currentHoveredAreaIndex = -1;
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

        if (CardManager.Instance.canUseCard == false)
        {
            promptManager.ShowPopup("Not Your Turn", Color.red);
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

        // Validate if card is alive
        if (!gameManagerRef.ValidateSelfCharacterAlive(currentHoveredAreaIndex))
        {
            promptManager.ShowPopup("Character dead", Color.red);
            ReturnCardToOriginalPosition(card);
            return;
        }

        if (ValidateCardColor(usedCardData, hoveredCardData))
        {
            HandleCardUsage(card, usedCardData, hoveredCardData);
            return;
        }

        Debug.Log($"Card '{usedCardData.cardName}' color does not match with '{hoveredCardData.cardName}'!");
        promptManager.ShowPopup("Wrong Color", Color.red);
        ReturnCardToOriginalPosition(card);
    }

    private void HandleCardUsage(GameObject card, CardData usedCardData, CardData hoveredCardData)
    {
        Debug.Log($"Card '{usedCardData.cardName}' is used on '{hoveredCardData.cardName}'.");
        attackerID = hoveredCardData.cardID;

        // Synchronize card usage across clients (optional netcode integration)
        SynchronizeCardUsage(card, usedCardData, hoveredCardData);

        // Hide other interactable areas except the used one
        HideInteractableAreasExcept(currentHoveredArea);

        // Animate the used area
        AnimateUsedArea(currentHoveredArea);

        // Destroy the card
        Destroy(card);

        // Show opponent's interactable areas to choose one opponent's character
        ShowOpponentInteractableAreas();
    }

    private void AnimateUsedArea(Image area)
    {
        if (area != null)
        {
            area.color = HexToColor("#00FF00");
            // Scale up slightly
            area.transform.DOScale(1.3f, 0.2f).OnComplete(() =>
            {
                // Then fade out and hide after a delay
                area.GetComponent<CanvasGroup>().DOFade(0, 0.3f).SetDelay(0.2f).OnComplete(() =>
                {
                    area.gameObject.SetActive(false);
                    // Reset scale
                    area.transform.localScale = Vector3.one;
                });
            });
        }
    }

    public void OpponentCharacterOnSelected(int selectedCharacterIndex)
    {
        Debug.Log($"Selected opponent character: {selectedCharacterIndex}");

        // Hide opponent's interactable areas
        foreach (var area in opponentInteractableAreas)
        {
            if (area != null)
            {
                area.gameObject.SetActive(false);
            }
        }

        selectEnemyPanel.SetActive(false);

        // Netcode: Broadcast the selected character index to all clients
        // Example: Send selectedCharacterIndex
        SynchronizeOpponentCharacterHit(selectedCharacterIndex);
        CardManager.Instance.DoAction(1);
    }

    #endregion

    #region Utility Methods

    private Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        Debug.LogWarning($"Invalid Hex Color: {hex}. Using default color.");
        return Color.white; // Fallback color
    }

    private void SetAreaHoverColor(Image area)
    {
        if (area != null)
        {
            Color hoverColor = Color.white;
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

    private void SetAreaInvalidColor(Image area)
    {
        if (area != null)
        {
            area.color = HexToColor("#FF0000"); // Red for invalid use
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
                HideSelfInteractableAreas();
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
    // Netcode methods (if any)

    public void LinkNetworkGameManager(GameManager gameManager)
    {
        gameManagerRef = gameManager;
    }

    private void SynchronizeCardUsage(GameObject card, CardData usedCardData, CardData hoveredCardData)
    {
        // Netcode: Broadcast card usage event to all clients
        // Example: Send cardID and hoveredAreaID
        gameManagerRef.UseCardServerRpc(usedCardData.cardID, currentHoveredAreaIndex);
    }

    private void SynchronizeOpponentCharacterHit(int charIndex)
    {
        gameManagerRef.CharacterHitServerRpc(charIndex, 1, attackerID);
    }

    private void SynchronizeCardReturn(GameObject card)
    {
        // Netcode: Notify all clients to return the card to its original position
    }

    #endregion
}