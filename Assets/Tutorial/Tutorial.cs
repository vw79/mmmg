using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  

public class Tutorial : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject previousButton;
    public GameObject nextButton;
    public GameObject closeButton;
    private float moveDistance = 1920f;
    private int currentTutorialIndex = 0;
    private int totalTutorials = 4;
    private bool isAnimating;

    private void Start()
    {
        Reset();
    }

    public void ShowTutorial()
    {
        previousButton.SetActive(false);
        nextButton.SetActive(true);
        tutorial.SetActive(true);
        closeButton.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorial.SetActive(false);
    }

    public void NextTutorial()
    {
        if (currentTutorialIndex < totalTutorials - 1 && !isAnimating)
        {
            isAnimating = true;
            currentTutorialIndex++;
            tutorial.transform.DOLocalMoveX(tutorial.transform.localPosition.x - moveDistance, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => isAnimating = false);
            UpdateButtonVisibility();
        }
    }

    public void PreviousTutorial()
    {       
        if (currentTutorialIndex > 0 && !isAnimating)
        {
            isAnimating = true;
            currentTutorialIndex--;
            tutorial.transform.DOLocalMoveX(tutorial.transform.localPosition.x + moveDistance, 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => isAnimating = false);
            UpdateButtonVisibility();
        }
    }

    private void UpdateButtonVisibility()
    {
        previousButton.SetActive(currentTutorialIndex > 0);
        nextButton.SetActive(currentTutorialIndex < totalTutorials - 1);
    }

    public void CloseTutorial()
    {
        Reset();
        tutorial.transform.localPosition = new Vector3(0, 0, 0);
        currentTutorialIndex = 0;

    }

    private void Reset()
    {
        tutorial.SetActive(false);
        previousButton.SetActive(false);
        nextButton.SetActive(false);
        closeButton.SetActive(false);
    }
}
