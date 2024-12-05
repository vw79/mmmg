using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonSaveDeck : MonoBehaviour
{
    public SelectedCardData defaultCardData;

    public static JsonSaveDeck instance { get; private set; }
    private string path;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR
            path = Application.dataPath + "/deck.json";
#else
            path = Application.persistentDataPath + "/deck.json";
#endif
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveDeck(SelectedCardData deckData)
    {
        DeckData data = new DeckData()
        {
            selectedCharacterIDs = deckData.selectedCharacterIDs,
            selectedActionCardIDs = deckData.selectedActionCardIDs
        };

        string json = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(path, json);
    }

    public SelectedCardData LoadDeck()
    {
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            DeckData data = JsonUtility.FromJson<DeckData>(json);

            SelectedCardData deckData = ScriptableObject.CreateInstance<SelectedCardData>();
            deckData.selectedCharacterIDs = data.selectedCharacterIDs;
            deckData.selectedActionCardIDs = data.selectedActionCardIDs;

            return deckData;
        }
        else
        {
            return defaultCardData;
        }
    }
}

public class DeckData
{
    public List<string> selectedCharacterIDs = new List<string>();
    public List<string> selectedActionCardIDs = new List<string>();
}
