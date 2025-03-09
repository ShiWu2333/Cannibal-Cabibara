using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NoticeBoardManager : MonoBehaviour
{
    [Header("公告栏 UI 控制")]
    public GameObject noticeBoardPanel; // 公告栏的 Panel
    public Button closeButton; // 关闭按钮

    public GameObject noticeBoardIcon; //公告栏icon
    public GameObject backpackIcon; //背包icon
    public GameObject inspectionIcon; //检视icon
    public GameObject interactionIcon; //抓取icon

    [Header("公告栏 UI")]
    public GameObject[] dayNotices;  // 6 个公告 `Image`，按顺序排列
    public Image[] evidenceImages; 
    public Sprite[] evidenceSprites; // 存储所有证据图片（按顺序）
    public Sprite questionMarkSprite; // "?" 的 Sprite（如果证据被销毁）

    private HashSet<int> destroyedEvidences = new HashSet<int>(); // 存储已销毁的证据
    private SkyAndTimeSystem skySystem; // 引用 `SkyAndTimeSystem`

    private void Start()
    {
        skySystem = FindObjectOfType<SkyAndTimeSystem>(); // 自动获取 `SkyAndTimeSystem`
        closeButton.onClick.AddListener(ToggleNoticeBoardUI); // ✅ 绑定关闭按钮事件
        noticeBoardPanel.SetActive(false); // 开始时隐藏公告栏
        closeButton.gameObject.SetActive(false);

        // ✅ 只显示 `Day1`，隐藏 `Day2 ~ Day6`
        for (int i = 0; i < dayNotices.Length; i++)
        {
            if (i == 0)
            {
                dayNotices[i].SetActive(true); // 显示第一天
            }
            else
            {
                dayNotices[i].SetActive(false); // 隐藏 Day2 ~ Day6
            }
        }

        UpdateNoticeBoard(); // 初始化公告栏
    }

    // **玩家销毁证据**
    public void DestroyEvidence(int evidenceIndex)
    {
        if (evidenceIndex >= 0 && evidenceIndex < evidenceSprites.Length)
        {
            destroyedEvidences.Add(evidenceIndex);
            evidenceImages[evidenceIndex].sprite = questionMarkSprite; // **变成 `?`**
            Debug.Log($"证据 {evidenceIndex + 1} 被销毁！");
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


    // **每天更新公告栏（由 `SkyAndTimeSystem` 触发）**
    public void UpdateNoticeBoard()
    {
        int currentDay = skySystem.currentDay; // 获取当前天数
        Debug.Log($"公告栏更新：今天是第 {currentDay + 1} 天");

        // **遍历所有公告栏 Image**
        for (int i = 0; i < dayNotices.Length; i++)
        {
            if (i <= currentDay) // ✅ 只显示当前天数及之前的证据
            {
                dayNotices[i].gameObject.SetActive(true); // 显示该天的证据

                if (destroyedEvidences.Contains(i))
                {
                    evidenceImages[i].sprite = questionMarkSprite; // **销毁 → 显示 `?`**
                }
                else
                {
                    evidenceImages[i].sprite = evidenceSprites[i]; // **未销毁 → 显示正常证据**
                }
            }
           
        }
    }
}