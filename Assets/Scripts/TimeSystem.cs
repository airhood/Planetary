using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public string ToText()
    {
        return day + "day " + hour + ":" + minute;
    }

    public string ToGapFilledText()
    {
        string displayMinute = "";
        string displayHour = "";
        if (minute < 10)
        {
            displayMinute += "0";
        }
        displayMinute += minute.ToString();
        if (hour < 10)
        {
            displayHour += "0";
        }
        displayHour += hour.ToString();
        return day + "day " + displayHour + ":" + displayMinute;
    }

    public string ToDisplayFormatedText()
    {
        return day + "day\n" + hour + ":" + minute;
    }

    public string ToGapFilledDisplayFormatedText()
    {
        string displayMinute = "";
        string displayHour = "";
        if (minute < 10)
        {
            displayMinute += "0";
        }
        displayMinute += minute.ToString();
        if (hour < 10)
        {
            displayHour += "0";
        }
        displayHour += hour.ToString();
        return day + "day\n" + displayHour + ":" + displayMinute;
    }

    public float ToFloat()
    {
        float result = 0;
        result += hour * (1 / 24f);
        result += minute * ((1 / 24f) / 60f);
        return result;
    }
}

public class TimeSystem : MonoBehaviour
{
    public InGameTime gameTime;

    int timeTickAmount;

    public Text time;
    public Image clock;

    void Start()
    {
        SetGameTime();
        updateUI();
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
            updateUI();
        }
    }

    public void updateUI()
    {
        time.text = gameTime.ToGapFilledDisplayFormatedText();
        clock.fillAmount = gameTime.ToFloat();
    }
}
