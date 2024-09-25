using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    // List of common 16:9 resolutions
    private readonly Vector2Int[] commonResolutions = {
        new Vector2Int(1280, 720),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)
    };

    void Start()
    {
        // Get the current resolution of the device
        int currentWidth = Screen.currentResolution.width;
        int currentHeight = Screen.currentResolution.height;

        Debug.Log("Current Device Resolution: " + currentWidth + "x" + currentHeight);

        // Set the closest resolution based on the current resolution
        SetClosest16by9Resolution(currentWidth, currentHeight);
    }

    void SetClosest16by9Resolution(int nativeWidth, int nativeHeight)
    {
        // Calculate the device aspect ratio
        float deviceAspect = (float)nativeWidth / nativeHeight;

        // Find the closest common 16:9 resolution based on height first
        Vector2Int closestResolution = commonResolutions[0];
        int closestHeightDifference = Mathf.Abs(nativeHeight - closestResolution.y);

        foreach (Vector2Int resolution in commonResolutions)
        {
            // Prioritize matching height first, as 16:9 is fixed for aspect ratio
            int heightDifference = Mathf.Abs(nativeHeight - resolution.y);

            // If the height difference is smaller, or same but with a larger width, choose that resolution
            if (heightDifference < closestHeightDifference ||
                (heightDifference == closestHeightDifference && resolution.x > closestResolution.x))
            {
                closestResolution = resolution;
                closestHeightDifference = heightDifference;
            }
        }

        // Set the closest resolution that matches the device's vertical resolution the best
        Screen.SetResolution(closestResolution.x, closestResolution.y, true);

        Debug.Log($"Set Resolution to: {closestResolution.x}x{closestResolution.y} (16:9)");
    }

    void OnGUI()
    {
        // Draw black bars if necessary
        float targetAspect = 16.0f / 9.0f;
        float windowAspect = (float)Screen.width / Screen.height;

        if (windowAspect > targetAspect)
        {
            // Pillarbox (black bars on left and right)
            float inset = (Screen.width - Screen.height * targetAspect) / 2;
            GUI.Box(new Rect(0, 0, inset, Screen.height), GUIContent.none);  // Left black bar
            GUI.Box(new Rect(Screen.width - inset, 0, inset, Screen.height), GUIContent.none);  // Right black bar
        }
        else if (windowAspect < targetAspect)
        {
            // Letterbox (black bars on top and bottom)
            float inset = (Screen.height - Screen.width / targetAspect) / 2;
            GUI.Box(new Rect(0, 0, Screen.width, inset), GUIContent.none);  // Top black bar
            GUI.Box(new Rect(0, Screen.height - inset, Screen.width, inset), GUIContent.none);  // Bottom black bar
        }
    }
}