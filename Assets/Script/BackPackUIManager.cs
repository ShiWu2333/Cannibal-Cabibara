using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemIDToImage
{
    public int itemID;      // 该 UI 对应的道具 ID
    public Image imageSlot; // UI Image（显示道具）
    
    public Sprite defaultSprite;  //  未收集时的黑灰剪影
    public Sprite collectedSprite; //  收集后显示的彩色道具
}

[System.Serializable]
public class MemoryIDToImage
{
    public int memoryID;      // 回忆片段 ID
    public Button memoryButton;
    public Image imageSlot;   // UI Image（用于显示回忆片段）
    public Sprite unlockedSprite; // ✅ 解锁后显示的 Sprite（回忆片段图片）
    public GameObject lockImage;  // ❌ 未解锁时的“锁”图标
}

public class BackPackUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backPackPanel; // 背包整体面板，方便切换显示
    public GameObject backpackIcon; //背包icon
    public Button backButton;
    public Button prevPageButton; // 上一页按钮
    public Button nextPageButton; // 下一页按钮

    [Tooltip("手动指定每个道具ID对应的UI Image。")]
    public List<ItemIDToImage> itemIDToImages; // 存储“道具ID -> UI格子”的对应关系
    [Tooltip("手动指定每个回忆片段ID对应的UI Image。")]
    public List<MemoryIDToImage> memoryIDToImages; // 存储“回忆片段ID -> UI格子”的对应关系

    [Header("翻页系统")]
    public int memoriesPerPage = 3; // 每页显示 3 个记忆碎片
    private int currentPage = 1; // 当前页码
    private int totalPages = 2; // 总页数



    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (BackPackManager.Instance != null)
        {
            BackPackManager.Instance.OnBackPackChanged -= UpdateBackPackUI;
        }
    }

    private void Start()
    {
        backButton.onClick.AddListener(ToggleBackPackUI);
        prevPageButton.onClick.AddListener(() => ChangePage(-1));
        nextPageButton.onClick.AddListener(() => ChangePage(1));

        if (BackPackManager.Instance != null)
        {
            BackPackManager.Instance.OnBackPackChanged += UpdateBackPackUI;
        }

        foreach (var memoryMapping in memoryIDToImages)
        {
            if (memoryMapping.memoryButton != null)
            {
                memoryMapping.memoryButton.gameObject.SetActive(true);
                int memoryId = memoryMapping.memoryID;
                memoryMapping.memoryButton.onClick.AddListener(() => PlayMemory(memoryId));
            }
        }

        backPackPanel.SetActive(false);
        backpackIcon.SetActive(true);
        UpdateBackPackUI();
    }

    void PlayMemory(int memoryId)
    {
        Debug.Log("播放回忆片段: " + memoryId);
        CutSceneManager.Instance.PlayMemoryVideo(memoryId);
    }

    /// <summary>
    /// 切换到上一页 / 下一页
    /// </summary>
    private void ChangePage(int direction)
    {
        currentPage += direction;
        currentPage = Mathf.Clamp(currentPage, 1, 2);
        UpdateBackPackUI();
    }

    /// <summary>
    /// 根据 BackPackManager 中收集的道具以及回忆片段解锁状态，更新 UI 显示
    /// </summary>
    public void UpdateBackPackUI()
    {
        if (BackPackManager.Instance == null) return;

        // 计算当前页的起始索引
        int startIdx = (currentPage - 1) * memoriesPerPage;
        int endIdx = Mathf.Min(startIdx + memoriesPerPage, memoryIDToImages.Count);

        // 1️⃣ 重置所有道具为默认（未收集状态）
        foreach (var mapping in itemIDToImages)
        {
            if (mapping.imageSlot != null && mapping.defaultSprite != null)
            {
                mapping.imageSlot.sprite = mapping.defaultSprite;  //  显示黑灰剪影
            }
        }

        // 2️⃣ 遍历已收集的道具，将对应的 UI 切换为彩色实物
        var collectedItems = BackPackManager.Instance.GetCollectedItems();
        foreach (var item in collectedItems)
        {
            var mapping = itemIDToImages.FirstOrDefault(m => m.itemID == item.itemID);
            if (mapping != null && mapping.imageSlot != null && mapping.collectedSprite != null)
            {
                mapping.imageSlot.sprite = mapping.collectedSprite;  //  显示彩色道具
            }
        }

        // 3️⃣ 更新回忆片段 UI（和道具逻辑一样）
        for (int i = 0; i < memoryIDToImages.Count; i++)
        {
            var memoryMapping = memoryIDToImages[i];

            if (i >= startIdx && i < endIdx)
            {
                // ✅ 这个回忆碎片属于当前页，显示它
                memoryMapping.imageSlot.gameObject.SetActive(true);
                memoryMapping.memoryButton.gameObject.SetActive(true);

                bool unlocked = BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryMapping.memoryID);
                Color imageColor = memoryMapping.imageSlot.color;


                if (unlocked)
                {
                    // ✅ 如果已解锁，显示回忆碎片图片，锁消失，并启用按钮
                    if (memoryMapping.unlockedSprite != null)
                    {
                        memoryMapping.imageSlot.sprite = memoryMapping.unlockedSprite;
                        Debug.Log("成功更新回忆片段图片: " + memoryMapping.memoryID);
                    }
                    imageColor.a = 1;  // **让图片完全显示 * *
                    memoryMapping.memoryButton.interactable = true;
                    memoryMapping.lockImage.SetActive(false);

                    // 确保只在按钮解锁时才添加事件，避免重复绑定
                    memoryMapping.memoryButton.onClick.RemoveAllListeners();
                    int memoryId = memoryMapping.memoryID;
                    memoryMapping.memoryButton.onClick.AddListener(() => PlayMemory(memoryId));
                }
                else
                {
                    // ❌ 未解锁：不显示图片，锁出现，禁用按钮
                    memoryMapping.imageSlot.sprite = null;
                    memoryMapping.memoryButton.interactable = false;
                    memoryMapping.lockImage.SetActive(true);
                    imageColor.a = 0; // **让图片完全透明**
                }
                // **彻底移除 onClick 事件，防止误触**
                memoryMapping.memoryButton.onClick.RemoveAllListeners();   
                memoryMapping.imageSlot.color = imageColor;

                if (unlocked)
                {
                    int memoryId = memoryMapping.memoryID;
                    memoryMapping.memoryButton.onClick.AddListener(() => PlayMemory(memoryId));
                }
            }
            else
            {
                // ❌ 这个回忆碎片不在当前页，隐藏它
                memoryMapping.imageSlot.gameObject.SetActive(false);
                memoryMapping.memoryButton.gameObject.SetActive(false);
            }
        }

        //  更新翻页按钮状态**
        prevPageButton.interactable = (currentPage > 1);
        nextPageButton.interactable = (currentPage < totalPages);
    }
        
    


    /// <summary>
    /// 切换背包 UI 显示或隐藏
    /// </summary>
    public void ToggleBackPackUI()
    {
        bool isActive = backPackPanel.activeSelf;
        backPackPanel.SetActive(!isActive);
        backpackIcon.SetActive(isActive);

    }
}

