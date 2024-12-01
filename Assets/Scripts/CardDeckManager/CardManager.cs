using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [Header("Scriptable Objects")]
    public AllCardDatabase allCardDatabase;
    public SelectedCardData selectedCards;

    //private void Awake()
    //{
    //    allCardDatabase = GameManager.Instance.allCardDatabase;
    //    selectedCards = GameManager.Instance.selectedCardData;
    //}

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

    // Cached card data
    private Dictionary<string, CardData> charactersDictionary = new Dictionary<string, CardData>();
    private Dictionary<string, CardData> actionCardsDictionary = new Dictionary<string, CardData>();
    //private Dictionary<string, CardData> buffsDictionary = new Dictionary<string, CardData>();

    private List<string> selectedCharacterIDs = new List<string>();
    private List<string> selectedActionCardIDs = new List<string>();

    [Header("Card Pool")]
    public int initialPoolSize;
    public int currentPoolSize;
    private List<GameObject> cardPool = new List<GameObject>();
    private GameObject cardBackInstance;
    public bool canDraw;

    void Start()
    {
        CacheAllCards();
        CacheSelectedCardIDs();

        initialPoolSize = selectedActionCardIDs.Count;
        currentPoolSize = initialPoolSize;

        InitializeCardBack();
        PlaceCharactersOnStage();
    }

    private void CacheAllCards()
    {
        // Populate dictionaries for quick card lookups from each category
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

    private void CacheSelectedCardIDs()
    {
        // Cache selected card IDs from SelectedCards SO
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

    private void InitializeCardBack()
    {
        cardBackInstance = Instantiate(cardBackPrefab, deckPosition.position, Quaternion.identity);
        cardBackInstance.SetActive(false);
    }

    private void PlaceCharactersOnStage()
    {
        if (selectedCharacterIDs.Count < 4)
        {
            Debug.LogWarning("Not enough characters selected to place on stage.");
            return;
        }

        // Get the first 4 characters from the selected list
        for (int i = 0; i < 4; i++)
        {
            string characterID = selectedCharacterIDs[i];

            if (charactersDictionary.TryGetValue(characterID, out CardData characterData))
            {
                // Instantiate the character card prefab
                GameObject characterCard = Instantiate(characterCardPrefab, deckPosition.position, Quaternion.identity);

                // Assign data to the card
                CharacterCard characterCardScript = characterCard.GetComponent<CharacterCard>();
                if (characterCardScript != null)
                {
                    characterCardScript.SetCardData(characterData);
                }

                // Determine the target position
                Transform targetPosition = GetCharacterPosition(i);

                // Animate the character card to the target position
                characterCard.transform.DOMove(targetPosition.position, drawAnimDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canDraw = true;
                    });
            }
            else
            {
                Debug.LogWarning($"Character ID {characterID} not found in charactersDictionary!");
            }
        }
    }

    private Transform GetCharacterPosition(int index)
    {
        // Return the corresponding position for the character
        switch (index)
        {
            case 0: return characterPosition1;
            case 1: return characterPosition2;
            case 2: return characterPosition3;
            case 3: return characterPosition4;
            default: return null;
        }
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
            Debug.Log($"Random Index: {randomIndex}, Card ID: {randomCardID}");

            // Find card data in the dictionary using cardID
            if (actionCardsDictionary.TryGetValue(randomCardID, out CardData cardData))
            {
                Debug.Log($"Found card in dictionary: {cardData.cardID} - {cardData.cardName}");
                Debug.Log($"Card Colour: {cardData.colour}");

                bool isLastCard = currentPoolSize == 1;

                // Animate the mockup cardBackPrefab
                AnimateCardBack(cardData, isLastCard);

                // Remove the card data from the pool after animation ends
                RemoveCardFromPool(randomIndex, randomCardID);

                // Update current pool size
                currentPoolSize--;

                // Debug: Print remaining cards in the pool
                Debug.Log($"Cards remaining in pool: {currentPoolSize}");
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
        // Remove card data from the pool and dictionary
        actionCardsDictionary.Remove(cardID);
        selectedActionCardIDs.RemoveAt(index);
    }

    private void DisplayCardOnCanvas(CardData cardData)
    {
        GameObject uiCard = Instantiate(uiCardPrefab, handArea);

        HandCard handCard = uiCard.GetComponent<HandCard>();

        if (handCard != null)
        {
            // Pass the card data to the HandCard script for display
            handCard.SetCardData(cardData);
        }
        else
        {
            Debug.LogWarning("HandCard component not found on the instantiated prefab.");
        }

        canDraw = true;
    }
}