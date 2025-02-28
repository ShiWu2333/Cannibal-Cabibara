using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkyChange : MonoBehaviour
{
    private float currentTime = 0f;    
    public Material daySkybox;         // 白天 Skybox
    public Light sunLight;             // 太阳光

    public Image blackScreen;
    public Image capybaraSleepImage;   // 卡皮巴拉睡觉的图片
    private bool isNightSequenceStarted = false;

    public Image calendarImage;  //  只用 1 个 Image，改 Sprite
    public Sprite[] calendarSpritesNormal; //  7 张无划线图片
    public Sprite[] calendarSpritesMarked; // 7 张划掉的图片

    private int currentDay = 0;


    void Start()
    {
        currentTime = 0f; 
        RenderSettings.skybox = daySkybox; // 初始为白天
        capybaraSleepImage.gameObject.SetActive(false);
        calendarImage.gameObject.SetActive(false);
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        Debug.Log("currentTime: " + currentTime);

        if (currentTime < 6f)  // 0-6秒 = 白天
        {
            RenderSettings.skybox = daySkybox;
            sunLight.intensity = 1.0f;
        }

        else if (currentTime >= 6f && currentTime < 13f)
        {
            float blendFactor = (currentTime - 6f) / 7f;
            sunLight.intensity = Mathf.Lerp(1.0f, 0f, blendFactor);
            RenderSettings.ambientIntensity = Mathf.Lerp(RenderSettings.ambientIntensity, 0.3f, blendFactor);
        }

        else if (currentTime >= 13f && !isNightSequenceStarted)
        {
            StartCoroutine(NightSequence());
            isNightSequenceStarted = true;
        }

        if (Mathf.FloorToInt(currentTime) % 1 == 0)
        {
            Debug.Log("当前天空: " + RenderSettings.skybox.name);
        }
    }

    IEnumerator NightSequence()
    {
        yield return new WaitForSeconds(2f);

        capybaraSleepImage.gameObject.SetActive(true);
        capybaraSleepImage.canvasRenderer.SetAlpha(0f);
        capybaraSleepImage.CrossFadeAlpha(1f, 2f, false);

        yield return new WaitForSeconds(7f);

  
        capybaraSleepImage.CrossFadeAlpha(0f, 2f, false);
        
        NewDay();

        yield return new WaitForSeconds(2f);
        capybaraSleepImage.gameObject.SetActive(false);

        
    }

    void NewDay()
    {
        currentDay++;

        // 如果是第 7 天（索引 6），根据数值触发结局（还未设定）
        if (currentDay == 7)
        {
            Debug.Log("根据数值触发结局");
            //TriggerGameEnding();
        }
        else
        {
            Debug.Log(" 新的一天开始！");
            currentTime = 0f; // 时间归零
            RenderSettings.skybox = daySkybox; // 重新设置白天
            sunLight.intensity = 1.0f;
            isNightSequenceStarted = false;

            StartCoroutine(ShowCalendar());
        }
    }
    IEnumerator ShowCalendar()
    {
        yield return new WaitForSeconds(3f); // 等待 3 秒后显示日历

        // ✅ 显示 `calendarImageNormal`（当天无划线的日历）
        calendarImage.sprite = calendarSpritesNormal[currentDay];
        calendarImage.gameObject.SetActive(true);
        calendarImage.canvasRenderer.SetAlpha(0f);
        calendarImage.CrossFadeAlpha(1f, 1f, false);


        yield return new WaitForSeconds(2f); // 2 秒后换成划掉的日历

        // ✅ 切换到 `calendarImageMarked`
        calendarImage.sprite = calendarSpritesMarked[currentDay];

        yield return new WaitForSeconds(2f);
        calendarImage.CrossFadeAlpha(0f, 1f, false);
        yield return new WaitForSeconds(1f); // 显示 3 秒后隐藏
        

        // ✅ 隐藏日历
        calendarImage.gameObject.SetActive(false);
    }
}