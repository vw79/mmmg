using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    public float animDuration = 0.5f;
    public RectTransform yourTurn;
    public RectTransform opponentTurn;
    public bool yourTurnShown = false;
    public bool opponentTurnShown = false;

    public void ShowYourTurn()
    {
        if (opponentTurnShown)
        {
            opponentTurn.DOAnchorPosY(100, animDuration);
            opponentTurnShown = false;
        }
        yourTurn.DOAnchorPosY(0, animDuration);
        yourTurnShown = true;
    }

    public void EndYourTurn()
    {
        yourTurn.DOAnchorPosY(-100, animDuration);
        yourTurnShown = false;
    }

    public void ShowOpponentTurn()
    {
        if (yourTurnShown)
        {
            yourTurn.DOAnchorPosY(-100, animDuration);
            yourTurnShown = false;
        }
        opponentTurn.DOAnchorPosY(0, animDuration);
        opponentTurnShown = true;
    }
}
