using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    private static ResolutionManager instance;

    private readonly Vector2Int[] commonResolutions = {
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;

        Debug.Log("Current Device Resolution: " + currentWidth + "x" + currentHeight);
        SetClosest16by9Resolution(currentWidth, currentHeight);
    }

    void SetClosest16by9Resolution(int nativeWidth, int nativeHeight)
    {
        float deviceAspect = (float)nativeWidth / nativeHeight;

        Vector2Int closestResolution = commonResolutions[0];
        int closestHeightDifference = Mathf.Abs(nativeHeight - closestResolution.y);

        foreach (Vector2Int resolution in commonResolutions)
        {
            int heightDifference = Mathf.Abs(nativeHeight - resolution.y);

            if (heightDifference < closestHeightDifference ||
                (heightDifference == closestHeightDifference && resolution.x > closestResolution.x))
            {
                closestResolution = resolution;
                closestHeightDifference = heightDifference;
            }
        }
        Screen.SetResolution(closestResolution.x, closestResolution.y, true);

        Debug.Log($"Set Resolution to: {closestResolution.x}x{closestResolution.y} (16:9)");
    }

    void OnGUI()
    {
        float targetAspect = 16.0f / 9.0f;
        float windowAspect = (float)Screen.width / Screen.height;

        if (windowAspect > targetAspect)
        {
            float inset = (Screen.width - Screen.height * targetAspect) / 2;
            GUI.Box(new Rect(0, 0, inset, Screen.height), GUIContent.none);
            GUI.Box(new Rect(Screen.width - inset, 0, inset, Screen.height), GUIContent.none);
        }
        else if (windowAspect < targetAspect)
        {
            float inset = (Screen.height - Screen.width / targetAspect) / 2;
            GUI.Box(new Rect(0, 0, Screen.width, inset), GUIContent.none);
            GUI.Box(new Rect(0, Screen.height - inset, Screen.width, inset), GUIContent.none);
        }
    }
}