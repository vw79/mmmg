using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    // Cached card data
    private Dictionary<string, CardData> charactersDictionary = new Dictionary<string, CardData>();
    private Dictionary<string, CardData> actionCardsDictionary = new Dictionary<string, CardData>();
    //private Dictionary<string, CardData> buffsDictionary = new Dictionary<string, CardData>();

    [Header("VFX Mapping")]
    public Dictionary<string, GameObject> characterVfxDictionary = new Dictionary<string, GameObject>();

    private List<string> selectedCharacterIDs = new List<string>();
    private List<string> selectedActionCardIDs = new List<string>();

    [Header("Opponent's Card Data")]
    public List<string> opponentCharacterIDs = new List<string>();

    [Header("Scriptable Objects")]
    public AllCardDatabase allCardDatabase;
    public SelectedCardData selectedCards;
    public SelectedCardData savedCards;

    [Header("Card Pool")]
    public int initialPoolSize;
    public int currentPoolSize;
    private List<GameObject> cardPool = new List<GameObject>();
    private GameObject cardBackInstance;
    private GameObject opponentCardBackInstance;
    public bool canDraw;

    [Header("Draw Card")]
    public Transform deckPosition;
    public Transform opponentDeckPosition;
    public Transform offScreenPosition;
    public Transform opponentOffScreenPosition;
    public GameObject cardBackPrefab;
    public float drawAnimDuration = 2f;
    public GameObject uiCardPrefab;
    public GameObject characterCardPrefab;
    public Transform handArea;
    public Transform enemyHandArea;

    [Header("Deck Visuals")]
    public GameObject cardDeckMesh;
    public Button deckButton;

    [Header("Character Placement")]
    public Transform characterPosition1;
    public Transform characterPosition2;
    public Transform characterPosition3;
    public Transform characterPosition4;
    [Header("Enemy Character Placement")]
    public Transform enemyCharacterPosition1;
    public Transform enemyCharacterPosition2;
    public Transform enemyCharacterPosition3;
    public Transform enemyCharacterPosition4;

    [Header("Interactable Areas")]
    public List<Image> interactableAreas;
    public Dictionary<Image, CardData> interactableAreaToCardData = new Dictionary<Image, CardData>();

    [Header("Character Card Data Handle")]
    public List<CharacterCard> selfCharacterCardData;
    public List<CharacterCard> opponentCharacterCardData;

    private GameManager gameManagerRef;
    public GameHost gameHost;
    public TurnIndicator turnIndicator;
    public Button endTurnButton;

    // Turn-based variables
    public bool canUseCard = false;
    public int actionPoint = 0;

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
        GetDeckFromSaveFile();
    }

    private void GetDeckFromSaveFile()
    {
          savedCards = JsonSaveDeck.instance.LoadDeck();
    }

    public void StartGame()
    {
        CacheAllCards();
        CacheSelectedCardIDs();
        MapCharacterVfx();

        initialPoolSize = selectedActionCardIDs.Count;
        currentPoolSize = initialPoolSize;

        InitializeCardBack();
        PlaceCharactersOnStage();
        PlaceOpponentCharactersOnStage();
        InitGameHandCard();
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
        if (savedCards != null)
        {
            selectedCharacterIDs = new List<string>(savedCards.selectedCharacterIDs);
            selectedActionCardIDs = new List<string>(savedCards.selectedActionCardIDs);
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

    #region Map VFX to Characters
    private void MapCharacterVfx()
    {
        if (allCardDatabase != null && allCardDatabase.charactersSO != null && allCardDatabase.vfx != null)
        {
            if (allCardDatabase.charactersSO.Count != allCardDatabase.vfx.Count)
            {
                Debug.LogWarning("Character list and VFX list sizes do not match!");
                return;
            }

            for (int i = 0; i < allCardDatabase.charactersSO.Count; i++)
            {
                string characterID = allCardDatabase.charactersSO[i].cardID;
                GameObject vfx = allCardDatabase.vfx[i];

                if (!characterVfxDictionary.ContainsKey(characterID))
                {
                    characterVfxDictionary.Add(characterID, vfx);
                    Debug.Log($"Character ID: {characterID} - VFX: {vfx.name}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate VFX mapping for character ID: {characterID}. Ignoring duplicate.");
                }
            }

            Debug.Log("VFX mapping completed successfully.");
        }
        else
        {
            Debug.LogWarning("Cannot map VFX - check allCardDatabase configuration.");
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
            case 4: return enemyCharacterPosition1;
            case 5: return enemyCharacterPosition2;
            case 6: return enemyCharacterPosition3;
            case 7: return enemyCharacterPosition4;
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
                    selfCharacterCardData.Add(characterCardScript);
                }

                Transform targetPosition = GetCharacterPosition(i);

                characterCard.transform.DOMove(targetPosition.position, drawAnimDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canDraw = true;
                    });

                SpriteRenderer spriteRenderer = targetPosition.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color color;
                    if (TryGetColorFromString(characterData.colour, out color))
                    {
                        spriteRenderer.color = color; // Set the color on the SpriteRenderer
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown color string '{characterData.colour}' for character ID {characterID}.");
                    }
                }

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

    private void PlaceOpponentCharactersOnStage()
    {
        if (opponentCharacterIDs.Count < 4)
        {
            Debug.LogWarning("Not enough opponent characters selected to place on stage.");
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            string characterID = opponentCharacterIDs[i];

            if (charactersDictionary.TryGetValue(characterID, out CardData characterData))
            {
                GameObject characterCard = Instantiate(characterCardPrefab, opponentDeckPosition.position, Quaternion.identity);

                CharacterCard characterCardScript = characterCard.GetComponent<CharacterCard>();
                if (characterCardScript != null)
                {
                    characterCardScript.SetCardData(characterData);
                    opponentCharacterCardData.Add(characterCardScript);
                }

                Transform targetPosition = GetCharacterPosition(i + 4);

                characterCard.transform.DOMove(targetPosition.position, drawAnimDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        canDraw = true;
                    });

                SpriteRenderer spriteRenderer = targetPosition.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color color;
                    if (TryGetColorFromString(characterData.colour, out color))
                    {
                        spriteRenderer.color = color; // Set the color on the SpriteRenderer
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown color string '{characterData.colour}' for character ID {characterID}.");
                    }
                }

                //if (i < interactableAreas.Count)
                //{
                //    interactableAreaToCardData[interactableAreas[i]] = characterData;
                //}
            }
            else
            {
                Debug.LogWarning($"Character ID {characterID} not found in charactersDictionary!");
            }
        }
    }

    private bool TryGetColorFromString(string colorString, out Color color)
    {
        switch (colorString.ToLower())
        {
            case "red":
                color = Color.red;
                return true;
            case "green":
                color = Color.green;
                return true;
            case "blue":
                color = Color.blue;
                return true;
            default:
                color = Color.white; // Use a default color if the string doesn't match
                return false;
        }
    }
    #endregion

    #region Turn Management
    public void InitGameHandCard()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.56f)
                .AppendCallback(() => DrawCard(false))
                .AppendInterval(0.56f)
                .AppendCallback(() => DrawCard(false))
                .AppendInterval(0.56f)
                .AppendCallback(() => DrawCard(false))
                .AppendInterval(0.56f)
                .AppendCallback(() => DrawCard(false))
                .AppendInterval(0.56f)
                .AppendCallback(() => DrawCard(false))
                .AppendCallback(() => gameManagerRef.ReadyToGameServerRpc());

        sequence.Play();
    }

    public void StartTurn()
    {
        actionPoint = 1;

        // Start Turn Anim -> Draw Card -> Start Action
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => turnIndicator.ShowYourTurn())
                .AppendInterval(turnIndicator.animDuration)
                .AppendCallback(() => DrawCard(false)) // Draw a card
                .AppendInterval(drawAnimDuration)
                .AppendCallback(() => StartAction());  // Start Action

        sequence.Play();
    }

    public void OpponentStartTurn()
    {
        // Opponent Start Turn Anim
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => turnIndicator.ShowOpponentTurn())
                .AppendInterval(turnIndicator.animDuration);

        sequence.Play();
    }

    private void StartAction()
    {
        canUseCard = true;
        endTurnButton.interactable = true;
    }

    public void DoAction(int AP)
    {
        actionPoint -= AP;
        CheckActionPoint();
    }

    private void CheckActionPoint()
    {
        if (actionPoint == 0)
        {
            EndTurn();
        }
    }

    public void EndTurn()
    {
        canUseCard = false;
        endTurnButton.interactable = false;
        // End Turn Anim -> End Turn
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => turnIndicator.EndYourTurn())
                .AppendInterval(turnIndicator.animDuration)
                .AppendCallback(() => gameManagerRef.SendEndTurnRpc());

        sequence.Play();
    }
    #endregion

    #region Card Drawing
    // Mock up for card drawing
    private void InitializeCardBack()
    {
        cardBackInstance = Instantiate(cardBackPrefab, deckPosition.position, Quaternion.identity);
        cardBackInstance.SetActive(false);
        opponentCardBackInstance = Instantiate(cardBackPrefab, opponentDeckPosition.position, Quaternion.identity);
        opponentCardBackInstance.SetActive(false);
    }

    public void DrawCard(bool isCharacterCard)
    {
        if (currentPoolSize > 0)
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

                // Synchronize the card animation with the opponent
                if (gameManagerRef != null & gameManagerRef.IsOwner)
                {
                    gameManagerRef.DrawCardServerRpc();
                }
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

        // Control point now considers the height (Y-axis) of offScreenPosition
        Vector3 controlPoint = new Vector3(
            startPosition.x - 2f, // Adjust X for a nice curve
            (startPosition.y + endPosition.y) / 2, // Adjust Y for height transition
            (startPosition.z + endPosition.z) / 2  // Z midpoint
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

    public void OpponentAnimateDrawCard()
    {
        // If this is the last card, disable the deck visuals
        // but we don't do this first
        //if (isLastCard)
        //{
        //    cardDeckMesh.SetActive(false);
        //    deckButton.interactable = false;
        //}

        Debug.Log("Animation Created Successfully");
        opponentCardBackInstance.transform.position = opponentDeckPosition.position;
        opponentCardBackInstance.SetActive(true);

        Vector3 startPosition = opponentDeckPosition.position;
        Vector3 endPosition = opponentOffScreenPosition.position;
        Vector3 controlPoint = new Vector3(
            startPosition.x + 2f, // Adjust X for a nice curve
            (startPosition.y + endPosition.y) / 2, // Adjust Y for height transition
            (startPosition.z + endPosition.z) / 2  // Z midpoint);
        );

        // Animate the cardBackPrefab
        opponentCardBackInstance.transform.DOPath(new Vector3[] { startPosition, controlPoint, endPosition }, drawAnimDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                opponentCardBackInstance.SetActive(false);
                // Display the card back on the canvas
                if (actionCardsDictionary.TryGetValue("e00", out CardData cardData))
                {
                    AddCardToEnemyHand(cardData);
                }
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

    private void AddCardToEnemyHand(CardData cardData)
    {
        GameObject uiCard = Instantiate(uiCardPrefab, enemyHandArea);

        HandCard handCard = uiCard.GetComponent<HandCard>();
        if (handCard != null)
        {
            handCard.SetCardData(cardData);
        }

        CustomHandLayout handLayout = enemyHandArea.GetComponent<CustomHandLayout>();
        if (handLayout != null)
        {
            handLayout.AddCard(uiCard.GetComponent<RectTransform>());
        }
    }
    #endregion

    #region Animate Opponent Use Card
    public void AnimateOpponentUseCard(string cardID, int charIndex)
    {
        GameObject uiCard = Instantiate(uiCardPrefab, enemyHandArea);
        uiCard.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        HandCard handCard = uiCard.GetComponent<HandCard>();
        if (handCard != null)
        {
            if (actionCardsDictionary.TryGetValue(cardID, out CardData cardData))
            {
                handCard.SetCardData(cardData);
            }
            else
            {
                Debug.LogWarning($"Card ID {cardID} not found in actionCardsDictionary!");
            }
        }

        CustomHandLayout handLayout = enemyHandArea.GetComponent<CustomHandLayout>();
        RectTransform currentCard = handLayout.cards[0];
        if (handLayout != null)
        {
            handLayout.RemoveCard(currentCard);
            Destroy(currentCard.gameObject);
        }

        // Animate the card
        uiCard.transform.DOMove(CardUseManager.Instance.opponentInteractableAreas[charIndex].rectTransform.position, 1f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                Destroy(uiCard);
            });
    }
    #endregion

    #region Character Attack

    // Function for attacker attacks target
    public void OnOpponentCharacterGetHit(int charIndex, int damage, string attackerID)
    {
        opponentCharacterCardData[charIndex].TakeDamage(damage);
        if (opponentCharacterCardData[charIndex].GetHealth() <= 0)
        {
            // Callback to server
            gameManagerRef.AddScoreServerRpc();
        }
        CallVFX(charIndex + 4, attackerID);
    }

    // Function for target get hit
    public void OnSelfCharacterGetHit(int charIndex, int damage, string attackerID)
    {
        selfCharacterCardData[charIndex].TakeDamage(damage);
        CallVFX(charIndex, attackerID);
    }

    public void CallVFX(int charIndex, string attackerID)
    {
        GameObject vfxObject = Instantiate(characterVfxDictionary[attackerID], GetCharacterPosition(charIndex).position + new Vector3(0,0.1f,0), Quaternion.identity);
        Destroy(vfxObject, 3f);
    }

    #endregion

    #region Network
    public void LinkNetworkGameManager(GameManager gameManager)
    {
        gameManagerRef = gameManager;
    }
    
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void RequestDrawCardToServerRpc(ulong clientID)
    {
        GameHost.Instance.RequestDrawCardServerRpc(clientID);
    }
    #endregion
}