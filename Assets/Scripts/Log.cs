using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void LogInfo(string message)
    {
        string s = "[Info] " + message;
        Debug.Log(s);
    }

    public static void LogWarning(string message)
    {
        string s = "[Warning] " + message;
        Debug.LogWarning(s);
    }

    public static void LogError(string message)
    {
        string s = "[Error] " + message;
        Debug.LogError(s);
    }
}
