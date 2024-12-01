using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    // Cached card data
    private Dictionary<string, CardData> charactersDictionary = new Dictionary<string, CardData>();
    private Dictionary<string, CardData> actionCardsDictionary = new Dictionary<string, CardData>();
    //private Dictionary<string, CardData> buffsDictionary = new Dictionary<string, CardData>();

    private List<string> selectedCharacterIDs = new List<string>();
    private List<string> selectedActionCardIDs = new List<string>();

    [Header("Scriptable Objects")]
    public AllCardDatabase allCardDatabase;
    public SelectedCardData selectedCards;

    [Header("Card Pool")]
    public int initialPoolSize;
    public int currentPoolSize;
    private List<GameObject> cardPool = new List<GameObject>();
    private GameObject cardBackInstance;
    public bool canDraw;

    [Header("Draw Card")]
    public Transform deckPosition;
    public Transform offScreenPosition;
    public GameObject cardBackPrefab;
    public float drawAnimDuration = 2f;
    public GameObject uiCardPrefab;
    public GameObject characterCardPrefab;
    public Transform handArea;

    [Header("Deck Visuals")]
    public GameObject cardDeckMesh;
    public Button deckButton;

    [Header("Character Placement")]
    public Transform characterPosition1;
    public Transform characterPosition2;
    public Transform characterPosition3;
    public Transform characterPosition4;

    [Header("Interactable Areas")]
    public List<Image> interactableAreas;
    public Dictionary<Image, CardData> interactableAreaToCardData = new Dictionary<Image, CardData>();

    //private void Awake()
    //{
    //    allCardDatabase = GameManager.Instance.allCardDatabase;
    //    selectedCards = GameManager.Instance.selectedCardData;
    //}
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

    void Start()
    {
        CacheAllCards();
        CacheSelectedCardIDs();

        initialPoolSize = selectedActionCardIDs.Count;
        currentPoolSize = initialPoolSize;

        InitializeCardBack();
        PlaceCharactersOnStage();
    }

    #region Cache Data from Scriptable Objects
    private void CacheAllCards()
    {
        if (allCardDatabase != null)
        {
            AddCardsToDictionary(allCardDatabase.charactersSO, charactersDictionary, "Character");
            AddCardsToDictionary(allCardDatabase.actionCardSO, actionCardsDictionary, "Action");
            //AddCardsToDictionary(allCardDatabase.buffSO, buffsDictionary, "Buff");
        }
        else
        {
            Debug.LogWarning("AllCardDatabase is null!");
        }
    }

    private void CacheSelectedCardIDs()
    {
        if (selectedCards != null)
        {
            selectedCharacterIDs = new List<string>(selectedCards.selectedCharacterIDs);
            selectedActionCardIDs = new List<string>(selectedCards.selectedActionCardIDs);
        }
        else
        {
            Debug.LogWarning("SelectedCards is null!");
        }
    }

    private void AddCardsToDictionary(List<CardData> cardList, Dictionary<string, CardData> dictionary, string category)
    {
        if (cardList != null)
        {
            foreach (var card in cardList)
            {
                if (!dictionary.ContainsKey(card.cardID))
                {
                    dictionary.Add(card.cardID, card);
                }
                else
                {
                    Debug.LogWarning($"Duplicate card ID found in {category} category: {card.cardID}. Ignoring duplicate.");
                }
            }
        }
    }
    #endregion

    #region Place selected characters on stage
    private Transform GetCharacterPosition(int index)
    {
        switch (index)
        {
            case 0: return characterPosition1;
            case 1: return characterPosition2;
            case 2: return characterPosition3;
            case 3: return characterPosition4;
            default: return null;
        }
    }

    private void PlaceCharactersOnStage()
    {
        if (selectedCharacterIDs.Count < 4)
        {
            Debug.LogWarning("Not enough characters selected to place on stage.");
            return;
        }

        if (interactableAreas.Count < selectedCharacterIDs.Count)
        {
            Debug.LogWarning("Not enough interactable areas for selected characters.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            string characterID = selectedCharacterIDs[i];

            if (charactersDictionary.TryGetValue(characterID, out CardData characterData))
            {
                GameObject characterCard = Instantiate(characterCardPrefab, deckPosition.position, Quaternion.identity);

                CharacterCard characterCardScript = characterCard.GetComponent<CharacterCard>();
                if (characterCardScript != null)
                {
                    characterCardScript.SetCardData(characterData);
                }

                Transform targetPosition = GetCharacterPosition(i);

                characterCard.transform.DOMove(targetPosition.position, drawAnimDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canDraw = true;
                    });

                if (i < interactableAreas.Count)
                {
                    interactableAreaToCardData[interactableAreas[i]] = characterData;
                }
            }
            else
            {
                Debug.LogWarning($"Character ID {characterID} not found in charactersDictionary!");
            }
        }
    }
    #endregion

    #region Card Drawing
    // Mock up for card drawing
    private void InitializeCardBack()
    {
        cardBackInstance = Instantiate(cardBackPrefab, deckPosition.position, Quaternion.identity);
        cardBackInstance.SetActive(false);
    }

    public void DrawCard(bool isCharacterCard)
    {
        if (currentPoolSize > 0 && canDraw)
        {
            canDraw = false;

            // Random
            int randomIndex = Random.Range(0, currentPoolSize);

            // Get cardID of the random number in the cachedList
            string randomCardID = selectedActionCardIDs[randomIndex];

            // Debug: Print the random index and card ID
            //Debug.Log($"Random Index: {randomIndex}, Card ID: {randomCardID}");

            // Find card data in the dictionary using cardID
            if (actionCardsDictionary.TryGetValue(randomCardID, out CardData cardData))
            {
                Debug.Log($"Found card in dictionary: {cardData.cardID} - {cardData.cardName} and Card Colour: {cardData.colour}");

                bool isLastCard = currentPoolSize == 1;

                // Animate the mockup cardBackPrefab
                AnimateCardBack(cardData, isLastCard);

                // Remove the card data from the pool after animation ends
                RemoveCardFromPool(randomIndex, randomCardID);

                // Update current pool size
                currentPoolSize--;

                // Debug: Print remaining cards in the pool
                //Debug.Log($"Cards remaining in pool: {currentPoolSize}");
            }
            else
            {
                Debug.LogWarning($"Card ID {randomCardID} not found in actionCardsDictionary!");
            }
        }
        else
        {
            Debug.LogWarning("canDraw = false or No cards left!");
        }      
    }

    private void AnimateCardBack(CardData cardData, bool isLastCard)
    {
        // If this is the last card, disable the deck visuals
        if (isLastCard)
        {
            cardDeckMesh.SetActive(false);
            deckButton.interactable = false;
        }

        cardBackInstance.transform.position = deckPosition.position;
        cardBackInstance.SetActive(true);

        Vector3 startPosition = deckPosition.position;
        Vector3 endPosition = offScreenPosition.position;
        Vector3 controlPoint = new Vector3(
            startPosition.x - 0.5f,
            startPosition.y,
            (startPosition.z + endPosition.z) / 2
        );

        // Animate the cardBackPrefab
        cardBackInstance.transform.DOPath(new Vector3[] { startPosition, controlPoint, endPosition }, drawAnimDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // Display the card on the canvas
                DisplayCardOnCanvas(cardData);
                cardBackInstance.SetActive(false);
            });
    }

    private void RemoveCardFromPool(int index, string cardID)
    {
        selectedActionCardIDs.RemoveAt(index);
    }

    private void DisplayCardOnCanvas(CardData cardData)
    {
        GameObject uiCard = Instantiate(uiCardPrefab, handArea);

        HandCard handCard = uiCard.GetComponent<HandCard>();
        if (handCard != null)
        {
            handCard.SetCardData(cardData);
        }

        CustomHandLayout handLayout = handArea.GetComponent<CustomHandLayout>();
        if (handLayout != null)
        {
            handLayout.AddCard(uiCard.GetComponent<RectTransform>());
        }

        canDraw = true;
    }
    #endregion
}