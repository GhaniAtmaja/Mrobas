using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogHandler : MonoBehaviour
{
    public Text _text;
    private string output = "";
    private string stack = "";
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;
        _text.text = output + " : " + stack;
        
    }
}
