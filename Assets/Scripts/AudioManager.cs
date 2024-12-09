using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioClip lobbyBGM;
    [SerializeField] private AudioClip gameplayBGM;

    [Header("Scene Groups")]
    [Tooltip("List of scenes where the lobby BGM should play.")]
    [SerializeField] private List<string> lobbyScenes = new List<string> { "MainMenu", "Lobby", "DeckEditor" };

    [Tooltip("The scene name where gameplay BGM should play.")]
    [SerializeField] private string gameplayScene = "NewGame";

    private AudioSource audioSource;
    private string currentSceneGroup = "";

    private void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Set up the AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        // Subscribe to scene change event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Play the correct BGM for the starting scene
        DetectAndPlayBGMForCurrentScene();
    }

    private void OnDestroy()
    {
        // Unsubscribe from scene change event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Detect the scene group and play the appropriate BGM
        DetectAndPlayBGMForCurrentScene();
    }

    private void DetectAndPlayBGMForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + sceneName);

        if (lobbyScenes.Contains(sceneName))
        {
            PlayBGMForSceneGroup("Lobby");
        }
        else if (sceneName == gameplayScene)
        {
            PlayBGMForSceneGroup("Gameplay");
        }
        else
        {
            StopBGM();
        }
    }

    public void PlayBGMForSceneGroup(string sceneGroup)
    {
        if (sceneGroup == currentSceneGroup) return;

        currentSceneGroup = sceneGroup;

        switch (sceneGroup)
        {
            case "Lobby":
                PlayBGM(lobbyBGM);
                break;

            case "Gameplay":
                PlayBGM(gameplayBGM);
                break;

            default:
                StopBGM();
                break;
        }
    }

    private void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopBGM()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }
}