using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "World/Achievement", fileName = "new Achievement")]
public class Achievement : ScriptableObject
{
    [Header("Info")]
    public string achievementName;
    public string achievementMessage;
    public List<string> conditionListeners;
    public AudioClip achieveSound;
}
