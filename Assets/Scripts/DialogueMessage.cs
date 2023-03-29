using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/DialogueMessage", fileName = "new DialogueMessage")]
public class DialogueMessage : ScriptableObject
{
    public string title;
    public string message;
    public int titleFontSize;
    public int messageFontSize;
    public Vector2 displayPosition;
    public AudioClip messageSound;
}
