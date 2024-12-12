using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public TextMeshProUGUI selfScoreText;
    public TextMeshProUGUI opponentScoreText;

    public GameObject winLosePanel;
    public GameObject playerWinText;
    public GameObject playerLoseText;

    public SceneTransition transitionTool;

    private void Start()
    {
        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        ShowWinPanel();
    }

    public void UpdateScore(int selfScore, int opponentScore)
    {
        selfScoreText.text = selfScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }

    public void ShowWinPanel()
    {
        winLosePanel.SetActive(true);
        DOVirtual.DelayedCall(1f, () =>
        {
            winLosePanel.GetComponent<CanvasGroup>().DOFade(0.5f, 1f).OnComplete(() =>
            {
                playerWinText.SetActive(true);
                DOVirtual.DelayedCall(3f, () =>
                {
                    DelayAndQuitToMainMenu();
                });
            });
        });
    }

    public void ShowLosePanel()
    {
        winLosePanel.SetActive(true); 
        DOVirtual.DelayedCall(1f, () =>
        {
            winLosePanel.GetComponent<CanvasGroup>().DOFade(0.5f, 1f).OnComplete(() =>
            {
                playerLoseText.SetActive(true);
                DOVirtual.DelayedCall(3f, () =>
                {
                    DelayAndQuitToMainMenu();
                });
            });
        });
    }

    public void DelayAndQuitToMainMenu()
    {
        NetworkManager.Singleton.Shutdown();

        // Cleanup
        Destroy(NetworkManager.Singleton.gameObject);
        


        transitionTool.TransitionToScene(0);
    }
}
