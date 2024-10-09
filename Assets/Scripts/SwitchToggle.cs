using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform uiHandle;
    [SerializeField] List<RectTransform> gameObjectsToMove; // List of GameObjects to move (easily scalable)
    public Toggle toggle;

    Vector2 handlePosition;
    List<Vector2> gameObjectsOriginalPos; // Store original positions of all objects in the list
    private const float moveDistance = 620f; // Define a constant for the move distance to avoid magic numbers

    private void Awake()
    {
        handlePosition = uiHandle.anchoredPosition;
        gameObjectsOriginalPos = new List<Vector2>();

        // Store the original positions of the gameObjects
        foreach (RectTransform gameObject in gameObjectsToMove)
        {
            gameObjectsOriginalPos.Add(gameObject.anchoredPosition);
        }

        // Add toggle listener
        toggle.onValueChanged.AddListener(OnSwitch);

        // Trigger switch at start if toggle is already on
        if (toggle.isOn)
        {
            OnSwitch(true);
        }
    }

    private void OnSwitch(bool isOn)
    {
        // Handle the position and color change for the toggle handle
        uiHandle.DOAnchorPos(isOn ? handlePosition * -1 : handlePosition, 0.4f).SetEase(Ease.InOutBack);
        uiHandle.GetComponent<Image>().DOColor(isOn ? Color.green : Color.red, 0.6f);

        // Move all gameObjects based on toggle value
        for (int i = 0; i < gameObjectsToMove.Count; i++)
        {
            Vector2 targetPos = isOn ? new Vector2(gameObjectsOriginalPos[i].x + moveDistance, gameObjectsOriginalPos[i].y) : gameObjectsOriginalPos[i];
            gameObjectsToMove[i].DOAnchorPos(targetPos, 0.6f).SetEase(Ease.InOutBack);
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}