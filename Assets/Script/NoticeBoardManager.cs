using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoticeBoardManager : MonoBehaviour
{
    [Header("公告栏 UI 控制")]
    public GameObject noticeBoardPanel; // 公告栏的 Panel
    public Button closeButton; // 关闭按钮
    public Button noticeBoardIconButton; // ✅ 公告栏 Icon 作为按钮

    public GameObject noticeBoardIcon; //公告栏icon
    public GameObject backpackIcon; //背包icon
    public GameObject inspectionIcon; //检视icon
    public GameObject interactionIcon; //抓取icon

    [Header("公告栏 UI")]
    public GameObject[] dayNotices;  // 6 个公告 `Image`，按顺序排列
    public Image[] evidenceImages; 
    public Sprite[] evidenceSprites; // 存储所有证据图片（按顺序）
    public Sprite questionMarkSprite; // "?" 的 Sprite（如果证据被销毁）

    [Header("地图 UI")]
    public Image mapImage; // ✅ 地图的 `Image` 组件
    public Sprite[] mapSprites; // ✅ 每天对应的 `Map Sprite`（长度应该是 6）

    private HashSet<int> destroyedEvidences = new HashSet<int>(); // 存储已销毁的证据
    private SkyAndTimeSystem skySystem; // 引用 `SkyAndTimeSystem`

    public Dictionary<int, int> dayToNoticeIndex = new Dictionary<int, int>
{
    { 1, 0 }, // Day 2 对应 `dayNotices[0]`
    { 3, 1 }, // Day 4 对应 `dayNotices[1]`
    { 4, 2 }  // Day 5 对应 `dayNotices[2]`
};

    private void Start()
    {
        skySystem = FindObjectOfType<SkyAndTimeSystem>(); // 自动获取 `SkyAndTimeSystem`
        closeButton.onClick.AddListener(ToggleNoticeBoardUI); // ✅ 绑定关闭按钮事件
                                                              // ✅ 让 `公告栏 Icon` 可以点击打开/关闭公告栏
        if (noticeBoardIconButton != null)
        {
            noticeBoardIconButton.onClick.AddListener(ToggleNoticeBoardUI);
        }
        
        noticeBoardPanel.SetActive(false); // 开始时隐藏公告栏
        closeButton.gameObject.SetActive(false);

        // ✅ **隐藏所有公告**
        for (int i = 0; i < dayNotices.Length; i++)
        {
            dayNotices[i].SetActive(false); // **隐藏所有天数的公告**
        }

        Debug.Log("✅ 所有公告已隐藏！");

        UpdateNoticeBoard(); // 初始化公告栏
    }

    // **玩家销毁证据**
    // **改成使用 itemID**
    public void DestroyEvidence(int evidenceIndex)
    {
        Debug.Log($"⚠️ 证据 {evidenceIndex} 被销毁，影响公告栏");

        if (dayToNoticeIndex.ContainsKey(evidenceIndex))
        {
            int noticeIndex = dayToNoticeIndex[evidenceIndex]; // 找到公告栏上的索引

            if (noticeIndex >= 0 && noticeIndex < evidenceImages.Length)
            {
                evidenceImages[noticeIndex].sprite = questionMarkSprite; // ✅ 显示 `?`
                Debug.Log($"✅ 公告栏已更新，证据 {evidenceIndex} 变成问号！");
            }
            else
            {
                Debug.LogWarning($"⚠️ 证据 {evidenceIndex} 没有找到对应的 `evidenceImages` 索引！");
            }
        }
    }

    public void ToggleNoticeBoardUI()
    {
        bool isActive = noticeBoardPanel.activeSelf;
        noticeBoardPanel.SetActive(!isActive);
        noticeBoardIcon.SetActive(isActive);
        backpackIcon.SetActive(isActive);
        inspectionIcon.SetActive(isActive);
        interactionIcon.SetActive(isActive);
        closeButton.gameObject.SetActive(!isActive);
        Debug.Log("公告栏 " + (isActive ? "关闭" : "打开"));
    }

    public void UpdateNoticeBoard()
    {
        int currentDay = skySystem.currentDay;
        Debug.Log($"📢 公告栏更新：今天是第 {currentDay + 1} 天");

        // ✅ 遍历所有过去的天数，确保所有已解锁的公告都显示
        for (int day = 0; day <= currentDay; day++)
        {
            if (dayToNoticeIndex.ContainsKey(day))
            {
                int noticeIndex = dayToNoticeIndex[day]; // 获取 `dayNotices` 的索引

                // ✅ 确保索引有效，防止数组越界
                if (noticeIndex >= 0 && noticeIndex < dayNotices.Length)
                {
                    dayNotices[noticeIndex].SetActive(true); // ✅ 显示该天的公告
                    Debug.Log($"📢 显示公告：Day {day + 1} -> Notice {noticeIndex}");

                    // ✅ **检查该天的证据是否被销毁**
                    if (destroyedEvidences.Contains(day))
                    {
                        if (noticeIndex < evidenceImages.Length)
                        {
                            evidenceImages[noticeIndex].sprite = questionMarkSprite; // ✅ 证据销毁 → `?`
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ 证据 {day + 1} 没有找到对应的 `evidenceImages` 索引！");
                        }
                    }
                    else
                    {
                        if (noticeIndex < evidenceSprites.Length)
                        {
                            evidenceImages[noticeIndex].sprite = evidenceSprites[noticeIndex]; // **未销毁 → 正常证据**
                        }
                        else
                        {
                            Debug.LogWarning($"⚠️ `evidenceSprites` 索引超出范围！");
                        }
                    }
                }
            }
        }

        // ✅ **更新 Map Image**
        if (mapImage != null && mapSprites.Length > currentDay)
        {
            mapImage.sprite = mapSprites[currentDay];
            Debug.Log($"🗺️ 地图已更新：Day {currentDay + 1}");
        }
        else
        {
            Debug.LogWarning("❌ `mapImage` 为空 或 `mapSprites` 数组长度不足！");
        }
    }
}