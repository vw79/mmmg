using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Debug_Device : MonoBehaviour
{
    public List<CinemachineVirtualCamera> cameras;
    private int currentCameraIndex = 0;

    public void SwitchCamera()
    {
        if (currentCameraIndex == 0)
        {
            cameras[0].Priority = 1;
            cameras[1].Priority = 0;
            currentCameraIndex = 1;
        }
        else
        {
            cameras[0].Priority = 0;
            cameras[1].Priority = 1;
            currentCameraIndex = 0;
        }
        Debug.Log("Switch to Camera" + currentCameraIndex);
    }
}

[CustomEditor(typeof(Debug_Device))]
public class Debug_Device_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Debug_Device myScript = (Debug_Device)target;

        if (GUILayout.Button("Switch Camera"))
        {
            myScript.SwitchCamera();
        }
    }
}
