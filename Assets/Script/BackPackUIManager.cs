using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemIDToImage
{
    public int itemID;      // 该UI对应的道具ID
    public Image imageSlot; // 用来显示或变色的UI Image
}

[System.Serializable]
public class MemoryIDToImage
{
    public int memoryID;    // 回忆片段ID（对应 BackPackManager.memoryFragments 中的 memoryId）
    public Image imageSlot; // 用来显示或变色的UI Image
}

public class BackPackUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backPackPanel; // 背包整体面板，方便切换显示
    [Tooltip("手动指定每个道具ID对应的UI Image。")]
    public List<ItemIDToImage> itemIDToImages; // 存储“道具ID -> UI格子”的对应关系
    [Tooltip("手动指定每个回忆片段ID对应的UI Image。")]
    public List<MemoryIDToImage> memoryIDToImages; // 存储“回忆片段ID -> UI格子”的对应关系



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
        if (BackPackManager.Instance != null)
        {
            BackPackManager.Instance.OnBackPackChanged += UpdateBackPackUI;
            Debug.Log("BackPackUIManager 订阅了 OnBackPackChanged 事件");
        }
        else
        {
            Debug.Log("BackPackUManager is not found");
        }
        // 启动时隐藏菜单
        backPackPanel.SetActive(false);
        // 启动时更新一次 UI
        UpdateBackPackUI();
    }

    /// <summary>
    /// 根据 BackPackManager 中收集的道具以及回忆片段解锁状态，更新 UI 显示
    /// </summary>
    public void UpdateBackPackUI()
    {
        // 如果没有背包管理器，直接返回
        if (BackPackManager.Instance == null) return;

        // 1. 更新道具部分：先将所有道具 UI 重置为灰色（未收集）
        foreach (var mapping in itemIDToImages)
        {
            if (mapping.imageSlot != null)
                mapping.imageSlot.color = Color.gray;
        }

        // 再遍历已收集的道具，将对应的 UI 变成绿色
        var collectedItems = BackPackManager.Instance.GetCollectedItems();
        foreach (var item in collectedItems)
        {
            var mapping = itemIDToImages.FirstOrDefault(m => m.itemID == item.itemID);
            if (mapping != null && mapping.imageSlot != null)
            {
                mapping.imageSlot.color = Color.green;
            }
        }

        // 2. 更新回忆片段部分：将所有映射的 UI 根据是否解锁设置颜色
        foreach (var memoryMapping in memoryIDToImages)
        {
            if (memoryMapping.imageSlot != null)
            {
                // 调用 BackPackManager 的 IsMemoryFragmentUnlocked 判断指定回忆片段是否解锁
                bool unlocked = BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryMapping.memoryID);
                memoryMapping.imageSlot.color = unlocked ? Color.green : Color.gray;
            }
        }
    }

    /// <summary>
    /// 切换背包 UI 显示或隐藏
    /// </summary>
    public void ToggleBackPackUI()
    {
        bool isActive = backPackPanel.activeSelf;
        backPackPanel.SetActive(!isActive);
    }
}
