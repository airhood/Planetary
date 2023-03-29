using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EventOccured(string internalEvent)
    {

    }

    bool OnPlayerEatFoodCheck = true;

    public void OnPlayerEatFood()
    {
        if (!OnPlayerEatFoodCheck) return;
        EventOccured("OnPlayerEatFood");
    }
}
