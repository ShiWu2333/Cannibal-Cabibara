using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkyChange : MonoBehaviour
{
    //public float totalDayTime = 13f;  
    private float currentTime = 0f;    
    public Material daySkybox;         // 白天 Skybox
    public Material duskSkybox;        // 黄昏 Skybox
    public Material nightSkybox;       // 夜晚 Skybox
    public Light sunLight;             // 太阳光

    public Image blackScreen;
    public Image capybaraSleepImage;   // 卡皮巴拉睡觉的图片
    private bool isNightSequenceStarted = false;

    void Start()
    {
        currentTime = 0f; 
        RenderSettings.skybox = daySkybox; // 初始为白天
        capybaraSleepImage.gameObject.SetActive(false);
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

        else if (currentTime >= 6f && currentTime < 10f)  // 6-10秒 = 黄昏
        {
            float blendFactor = (currentTime - 6f) / 4f;

            Material tempSkybox = new Material(duskSkybox);
            tempSkybox.Lerp(daySkybox, duskSkybox, blendFactor);
            RenderSettings.skybox = tempSkybox; 

            sunLight.intensity = Mathf.Lerp(1.0f, 0.2f, blendFactor);
        }
        else if (currentTime >= 10f && currentTime < 13f)  // 10-13秒 = 黑夜
        {
            float blendFactor = (currentTime - 10f) / 3f;

            Material tempSkybox = new Material(nightSkybox);
            tempSkybox.Lerp(duskSkybox, nightSkybox, blendFactor);
            tempSkybox.SetFloat("_Exposure", Mathf.Lerp(1.0f, 0.3f, blendFactor));
            RenderSettings.skybox = tempSkybox;

            sunLight.intensity = Mathf.Lerp(0.2f, 0f, blendFactor);
            RenderSettings.ambientIntensity = Mathf.Lerp(0.5f, 0.3f, blendFactor);
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
        Debug.Log(" 新的一天开始！");
        currentTime = 0f; // 时间归零
        RenderSettings.skybox = daySkybox; // 重新设置白天
        sunLight.intensity = 1.0f;
        isNightSequenceStarted = false;
    }
}