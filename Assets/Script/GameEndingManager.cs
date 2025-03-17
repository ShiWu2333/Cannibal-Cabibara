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
    public Sprite loadingEndingSprite; // 🌀 Loading 结局画面
    public Sprite runawaySprite; // ✅ 卡皮巴拉逃脱罪名
    public Sprite hawaiiSprite; // ✅ 夏威夷度假图片

    public Sprite arrestedSprite; // ❌ 卡皮巴拉被捕入狱
    public Sprite jailedSprite;

    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip happyMusic; // 🎵 欢乐音乐 (Good Ending)
    public AudioClip sadMusic;   // 🎵 悲伤音乐 (Bad Ending)

    private NoticeBoardManager noticeBoardManager;
    private bool isEndingTriggered = false;

    // ✅ 需要销毁的 3 个警察证据
    private static readonly HashSet<int> requiredDestroyedEvidences = new HashSet<int> { 2, 9, 12 };

    // ✅ 记录已销毁的证据
    private HashSet<int> destroyedEvidences = new HashSet<int>();


    private void Start()
    {
        
        endingPanel.SetActive(false); // ✅ 初始隐藏结局界面
    }

    // **✅ 外部调用：记录销毁的证据**
    public void RecordDestroyedEvidence(int itemID)
    {
        if (requiredDestroyedEvidences.Contains(itemID))
        {
            destroyedEvidences.Add(itemID);
            Debug.Log($"🛑 证据 {itemID} 被销毁！（当前进度: {destroyedEvidences.Count} / 3）");
        }
    }

    // **✅ 触发游戏结局**
    public void TriggerGameEnding()
    {
        if (isEndingTriggered) return; // ✅ 避免重复触发
        isEndingTriggered = true;

        Debug.Log("🚨 进入最终结局检查...");
        Debug.Log($"📝 已销毁的证据: {string.Join(", ", destroyedEvidences)}");

        endingPanel.SetActive(true); // ✅ 显示结局界面

        // ✅ **确保至少销毁了 ID 2, 9, 12**
        bool isGoodEnding = destroyedEvidences.Contains(2) &&
                            destroyedEvidences.Contains(9) &&
                            destroyedEvidences.Contains(12);

        if (isGoodEnding)
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

    // **✅ Good Ending 逻辑**
    private IEnumerator ShowGoodEnding()
    {
        if (audioSource != null && happyMusic != null)
        {
            audioSource.clip = happyMusic;
            audioSource.Play();
        }

        // ✅ Step 1: 显示 Loading Ending
        endingImage.sprite = loadingEndingSprite;
        yield return new WaitForSeconds(2f);

        endingImage.sprite = runawaySprite; // ✅ 显示逃脱罪名的图片
        yield return new WaitForSeconds(3f);

        endingImage.sprite = hawaiiSprite; // ✅ 显示夏威夷图片
        yield return new WaitForSeconds(3f);

        Debug.Log("✅ Good Ending 结束");
        // Application.Quit(); // **退出游戏**
    }

    // **✅ Bad Ending 逻辑**
    private IEnumerator ShowBadEnding()
    {
        if (audioSource != null && sadMusic != null)
        {
            audioSource.clip = sadMusic;
            audioSource.Play();
        }

        // ✅ Step 1: 显示 Loading Ending
        endingImage.sprite = loadingEndingSprite;
        yield return new WaitForSeconds(2f);

        endingImage.sprite = arrestedSprite; // ❌ 显示被捕入狱
        yield return new WaitForSeconds(5f);

        endingImage.sprite = jailedSprite; // ❌ 显示牢房画面
        yield return new WaitForSeconds(3f);

        Debug.Log("❌ Bad Ending 结束");
        // Application.Quit(); // **退出游戏**
    }
}
