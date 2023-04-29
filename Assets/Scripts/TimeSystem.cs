using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InGameTime
{
    public int day;
    public int hour;
    public int minute;

    public InGameTime()
    {
        day = 0;
        hour = 0;
        minute = 0;
    }

    public InGameTime(int day, int hour, int minute)
    {
        this.day = day;
        this.hour = hour;
        this.minute = minute;
    }

    public void AddTime(int day, int hour, int minute)
    {
        if (this.minute + minute >= 60)
        {
            this.hour++;
            this.minute = (this.minute + minute) - 60;
        }
        else
        {
            this.minute += minute;
        }
        
        if (this.hour + hour >= 24)
        {
            this.day++;
            this.hour = (this.hour + hour) - 24;
        }
        else
        {
            this.hour += hour;
        }

        this.day += day;
    }
}

public class TimeSystem : MonoBehaviour
{
    public InGameTime gameTime;

    int timeTickAmount;

    void Start()
    {
        SetGameTime();
    }

    public void SetGameTime()
    {
        gameTime = new InGameTime(0, 0, 0);
    }

    public void timeTick()
    {
        timeTickAmount++;

        if (timeTickAmount >= 15)
        {
            timeTickAmount -= 15;
            gameTime.AddTime(0, 0, 15);
        }
    }
}
