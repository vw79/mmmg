using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConsoleGUIController : MonoBehaviour
{
    public enum ConsoleGUIAnchor
    {
        Top = 1,
        Bottom = 2
    }

    public enum ConsoleGUIHeight
    {
        Full = 1,
        Half = 2,
        Quarter = 4
    }

    public enum ConsoleGUIOrder
    {
        Normal = 1,
        Reverse = 2
    }

    private List<string> logValues = new List<string>();
    private string logText = string.Empty;
    private Vector2 scrollPosition;
    private Texture2D backgroundTexture;
    private string inputLogMessage = string.Empty;

    [Header("Show Properties")]
    public bool ShowConsole = false;
    public bool ShowStackTrace = true;
    public bool ShowTitle = true;
    public ConsoleGUIOrder ShowOrder = ConsoleGUIOrder.Normal;

    [Header("GUI Properties")]
    public ConsoleGUIAnchor GUIAnchor = ConsoleGUIAnchor.Bottom;
    public ConsoleGUIHeight GUIHeight = ConsoleGUIHeight.Half;
    public int GUIFontSize = 15;
    public Color GUIColor = Color.black;

    [Header("Behaviours Properties")]
    public bool DoNotDestroyOnLoad = false;

    [Header("UI Elements")]
    public TMP_InputField inputField;
    public Button enterButton;

    public GUIStyle DialogBoxStyle { get; private set; }

    void OnEnable() { UnityEngine.Application.logMessageReceived += Log; }
    void OnDisable() { UnityEngine.Application.logMessageReceived -= Log; }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void Start()
    {
        // Use in different scenes
        if (this.DoNotDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        // Set static background color
        backgroundTexture = MakeTex(2, 2, this.GUIColor);

        this.ShowConsole = false;

        // Hide input field and enter button on start
        if (inputField != null) inputField.gameObject.SetActive(false);
        if (enterButton != null) enterButton.gameObject.SetActive(false);

        // Attach a listener to the enter button
        if (enterButton != null)
        {
            enterButton.onClick.AddListener(OnEnterPressed);
        }
    }

    public void OnEnterPressed()
    {
        if (inputField != null && inputField.text == "clear")
        {
            // Clear the log when "clear" is entered
            logValues.Clear();
            logText = string.Empty;
            inputField.text = ""; // Clear the input field
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        string openTag = "";
        string closeTag = "";

        if (type == LogType.Error) { openTag = "<color=#FF534A>"; closeTag = "</color>"; }
        else if (type == LogType.Warning) { openTag = "<color=#FFC107>"; closeTag = "</color>"; }

        string stack = string.Empty;
        if (this.ShowStackTrace)
        {
            stack = "\n" + "<size=" + this.GUIFontSize / 1.25 + ">" + stackTrace + "</size>";
        }

        if (this.ShowOrder == ConsoleGUIOrder.Normal)
        {
            logValues.Add(openTag + "[" + DateTime.Now.ToLongTimeString() + "] " + logString + stack + closeTag);
        }
        else
        {
            logValues.Insert(0, openTag + "[" + DateTime.Now.ToLongTimeString() + "] " + logString + stack + closeTag);
        }

        logText = string.Empty;
        foreach (string s in logValues)
        {
            logText = logText + "\n" + s;
        }
    }

    void OnGUI()
    {
        if (this.ShowConsole)
        {
            string startTag = "<size=" + this.GUIFontSize + ">";
            string closeTag = "</size>";

            // Style
            GUIStyle newStyle = new GUIStyle(GUI.skin.box);
            newStyle.alignment = TextAnchor.UpperLeft;
            newStyle.richText = true;
            newStyle.normal.background = backgroundTexture;
            newStyle.wordWrap = true;

            // Size
            Rect newRect = new Rect();
            if (this.GUIAnchor == ConsoleGUIAnchor.Top)
            {
                newRect = new Rect(10, 10, Screen.width - 20, (Screen.height / (int)this.GUIHeight) - 20);
            }
            else if (this.GUIAnchor == ConsoleGUIAnchor.Bottom)
            {
                newRect = new Rect(10, Screen.height - (Screen.height / (int)this.GUIHeight), Screen.width - 20, (Screen.height / (int)this.GUIHeight) - 10);
            }

            // Contents
            string title = string.Empty;
            if (this.ShowTitle)
            {
                title = UnityEngine.Application.productName + " " + UnityEngine.Application.version + " LOG CONSOLE: \n";
            }
            GUIContent content = new GUIContent(startTag + title + logText + closeTag);

            // Final height
            float dynamicHeight = Mathf.Max((Screen.height / (int)this.GUIHeight), newStyle.CalcHeight(content, Screen.width));

            // Begin scroll
            scrollPosition = GUI.BeginScrollView(newRect, scrollPosition, new Rect(0, 0, 0, dynamicHeight), false, true);

            // Draw box
            GUI.Box(new Rect(0, 0, Screen.width, dynamicHeight), content, newStyle);

            // End scroll
            GUI.EndScrollView();
        }
    }

    public void ToggleConsole()
    {
        this.ShowConsole = !this.ShowConsole;

        // Toggle input field and enter button visibility
        if (inputField != null) inputField.gameObject.SetActive(this.ShowConsole);
        if (enterButton != null) enterButton.gameObject.SetActive(this.ShowConsole);
    }
}
