using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoticeBoardManager : MonoBehaviour
{
    [Header("公告栏 UI 控制")]
    public GameObject noticeBoardPanel; // 公告栏的 Panel
    public Button closeButton; // 关闭按钮
    public Button noticeBoardIconButton; // ✅ 公告栏 Icon 作为按钮

    public GameObject noticeBoardIcon; // 公告栏 icon
    public GameObject backpackIcon; // 背包 icon
    public GameObject inspectionIcon; // 检视 icon
    public GameObject interactionIcon; // 抓取 icon

    [Header("公告栏 UI")]
    public GameObject[] dayNotices;  // 3 个公告（Day 2, 4, 5）
    public Image[] evidenceImages;   // 对应 `dayNotices` 里的 `Image`
    public Sprite[] evidenceSprites; // 存储 3 个证据的 `Sprite`
    public Sprite questionMarkSprite; // "?" 的 Sprite（如果证据被销毁）

    [Header("公告栏日历 UI")]
    public Image calendarImage; // 📅 公告栏的日历 `Image`
    public Sprite[] calendarSprites; // 📅 每天对应的 `Calendar Sprite`

    [Header("地图 UI")]
    public Image mapImage; // 地图的 `Image` 组件
    public Sprite[] mapSprites; // 每天对应的 `Map Sprite`（6 天）

    private HashSet<int> destroyedEvidences = new HashSet<int>(); // 存储已销毁的证据
    private SkyAndTimeSystem skySystem; // 引用 `SkyAndTimeSystem`

    // ✅ **公告栏展示的 3 个线索（天数 -> Item ID）**
    private Dictionary<int, int> dayToItemID = new Dictionary<int, int>
    {
        { 1, 2 },  // Day 2 → Nailed Planks (item ID 2)
        { 3, 9 },  // Day 4 → Paws C&M (item ID 9)
        { 4, 12 }  // Day 5 → Letter (item ID 12)
    };

    private void Start()
    {
        skySystem = FindObjectOfType<SkyAndTimeSystem>(); // 获取 `SkyAndTimeSystem`
        closeButton.onClick.AddListener(ToggleNoticeBoardUI); // 绑定关闭按钮事件

        if (noticeBoardIconButton != null)
        {
            noticeBoardIconButton.onClick.AddListener(ToggleNoticeBoardUI);
        }

        noticeBoardPanel.SetActive(false); // 开始时隐藏公告栏
        closeButton.gameObject.SetActive(false);

        // ✅ **初始隐藏所有公告**
        for (int i = 0; i < dayNotices.Length; i++)
        {
            dayNotices[i].SetActive(false);
        }

        Debug.Log("✅ 所有公告已隐藏！");
        UpdateNoticeBoard(); // 初始化公告栏
    }

    // **✅ 只有 `公告板的线索` 被销毁，才会影响公告**
    public void DestroyEvidence(int itemID)
    {
        Debug.Log($"⚠️ 证据 {itemID} 被销毁，影响公告栏");

        if (!dayToItemID.ContainsValue(itemID)) return; // **如果不是公告栏展示的线索，直接返回**

        destroyedEvidences.Add(itemID);
        UpdateNoticeBoard(); // **更新公告栏**
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

    // **✅ 更新公告栏（保留之前展示的线索）**
    public void UpdateNoticeBoard()
    {
        int currentDay = skySystem.currentDay;
        Debug.Log($"📢 公告栏更新：今天是第 {currentDay + 1} 天");

        // ✅ **遍历 `dayNotices`（公告栏总共 3 个线索）**
        for (int i = 0; i < dayNotices.Length; i++)
        {
            int displayDay = i == 0 ? 1 : (i == 1 ? 3 : 4); // Day 2 (索引 0), Day 4 (索引 1), Day 5 (索引 2)

            if (displayDay <= currentDay) // **只展示 `当前天数` 及 `之前天数`**
            {
                dayNotices[i].SetActive(true); // ✅ 显示公告

                int evidenceItemID = dayToItemID[displayDay];

                if (destroyedEvidences.Contains(evidenceItemID))
                {
                    evidenceImages[i].sprite = questionMarkSprite; // ✅ **线索被销毁 → 问号**
                }
                else
                {
                    evidenceImages[i].sprite = evidenceSprites[i]; // ✅ **线索未销毁 → 正常 Sprite**
                }
            }
        }
        // ✅ **更新公告栏的日历**
        if (calendarImage != null && calendarSprites.Length > currentDay)
        {
            calendarImage.sprite = calendarSprites[currentDay]; // **每天更换日历**
            Debug.Log($"📅 公告栏日历更新：Day {currentDay + 1}");
        }
        else
        {
            Debug.LogWarning("❌ `calendarImage` 为空 或 `calendarSprites` 数组长度不足！");
        }

        // ✅ **更新 Map Image**
        if (mapImage != null && mapSprites.Length > currentDay)
        {
            mapImage.sprite = mapSprites[currentDay]; // **每天更换地图**
            Debug.Log($"🗺️ 地图已更新：Day {currentDay + 1}");
        }
    }
}