using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject achievementDisplayUI;

    public List<Achievement> achievements = new List<Achievement>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayAchievement(achievements[0]));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DisplayAchievement(Achievement achievement)
    {
        achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().text = achievement.achievementName;
        achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().text = achievement.achievementMessage;

        yield return new WaitForSecondsRealtime(10f);

        StartCoroutine(FadeOutAchievement());
    }

    IEnumerator FadeOutAchievement()
    {
        for(int i = 1; 3 * i <= 255; i++)
        {
            print("alpha: " + achievementDisplayUI.GetComponent<Image>().color.a);
            achievementDisplayUI.GetComponent<Image>().color = new Color(achievementDisplayUI.GetComponent<Image>().color.r, achievementDisplayUI.GetComponent<Image>().color.g, achievementDisplayUI.GetComponent<Image>().color.b, 1 - (3 * i/255f));
            achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color = new Color(achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.r, achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.g, achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.b, 1 - (3 * i / 255f));
            achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color = new Color(achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.r, achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.g, achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.b, 1 - (3 * i / 255f));
            yield return new WaitForSecondsRealtime(0.005f);
        }

        achievementDisplayUI.GetComponent<Image>().color = new Color(achievementDisplayUI.GetComponent<Image>().color.r, achievementDisplayUI.GetComponent<Image>().color.g, achievementDisplayUI.GetComponent<Image>().color.b, 0);
        achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color = new Color(achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.r, achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.g, achievementDisplayUI.transform.GetChild(0).GetComponent<Text>().color.b, 0);
        achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color = new Color(achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.r, achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.g, achievementDisplayUI.transform.GetChild(1).GetComponent<Text>().color.b, 0);
    }
}
