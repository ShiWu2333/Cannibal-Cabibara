using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndingManager : MonoBehaviour
{
    [Header("结局 UI 控制")]
    public GameObject endingPanel; // 结局面板
    public Image endingImage; // 结局图片

    [Header("结局 Sprite")]
    public Sprite runawaySprite; // ✅ 卡皮巴拉逃脱罪名
    public Sprite hawaiiSprite; // ✅ 夏威夷度假图片

    public Sprite arrestedSprite; // ❌ 卡皮巴拉被捕入狱
    public Sprite jailedSprite;

    private NoticeBoardManager noticeBoardManager;
    private int destroyedPoliceEvidenceCount = 0; // 已销毁的证据数量
    private bool isEndingTriggered = false;

    private void Start()
    {
        noticeBoardManager = FindObjectOfType<NoticeBoardManager>();
        endingPanel.SetActive(false); // ✅ 初始隐藏结局界面
    }

    // **外部调用：摧毁警察证据**
    public void RecordDestroyedEvidence()
    {
        destroyedPoliceEvidenceCount++;
        Debug.Log($"证据销毁进度: {destroyedPoliceEvidenceCount} / 5");
    }

    // **触发游戏结局**
    public void TriggerGameEnding()
    {
        if (isEndingTriggered) return; // ✅ 避免重复触发
        isEndingTriggered = true;

        Debug.Log("🚨 进入最终结局检查...");

        endingPanel.SetActive(true); // ✅ 显示结局界面

        if (destroyedPoliceEvidenceCount >= 5)
        {
            Debug.Log("🎉 触发 Good Ending！");
            StartCoroutine(ShowGoodEnding());
        }
        else
        {
            Debug.Log("🚔 触发 Bad Ending...");
            StartCoroutine(ShowBadEnding());
        }
    }

    // **Good Ending 逻辑**
    private IEnumerator ShowGoodEnding()
    {
        endingImage.sprite = runawaySprite; // ✅ 显示逃脱罪名的图片
        yield return new WaitForSeconds(3f); // **展示 3 秒**

        endingImage.sprite = hawaiiSprite; // 
        yield return new WaitForSeconds(3f);

        Debug.Log("✅ Good Ending 结束");
        //Application.Quit(); // **退出游戏**
    }

    // **Bad Ending 逻辑**
    private IEnumerator ShowBadEnding()
    {
        endingImage.sprite = arrestedSprite; // ❌ 显示入狱图片
        yield return new WaitForSeconds(5f);

        endingImage.sprite = jailedSprite; // 
        yield return new WaitForSeconds(3f);

        Debug.Log("❌ Bad Ending 结束");
        //Application.Quit(); // **退出游戏**
    }
}