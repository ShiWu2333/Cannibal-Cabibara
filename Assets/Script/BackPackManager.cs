using System.Collections.Generic;
using UnityEngine;

// 用于定义每个记忆片段的数据结构
[System.Serializable]
public class MemoryFragment
{
    public int memoryId;                  // 记忆片段ID（例如：1 ~ 7）
    public List<int> requiredItemIDs;     // 解锁该记忆片段所需的道具ID列表
    public bool isUnlocked;               // 是否已解锁
}

public class BackPackManager : MonoBehaviour
{
    public static BackPackManager Instance { get; private set; }

    // 存储玩家已收集的线索（道具），用 Item 脚本实例存储
    public List<Item> collectedItems = new List<Item>();

    // 定义 7 个记忆片段，可在 Inspector 中编辑每个片段需要的道具ID
    public List<MemoryFragment> memoryFragments = new List<MemoryFragment>();

    // 用于通知 UI 更新的事件
    public delegate void OnBackPackUpdated();
    public event OnBackPackUpdated OnBackPackChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 添加一个道具到背包，并检查是否有记忆片段可以解锁
    /// </summary>
    public void AddItem(Item item)
    {
        // 避免重复收集：检查是否已经存在同样的 itemID
        if (collectedItems.Exists(i => i.itemID == item.itemID))
        {
            Debug.Log($"道具 {item.itemName} 已经被收集过。");
            return;
        }

        collectedItems.Add(item);
        item.isPickedUp = true;
        Debug.Log($"收集到线索：{item.itemName}");

        // 检查每个记忆片段是否满足解锁条件
        CheckMemoryFragmentsUnlock();

        OnBackPackChanged?.Invoke(); // 通知 UI 更新
    }

    /// <summary>
    /// 检查所有记忆片段的解锁条件
    /// </summary>
    private void CheckMemoryFragmentsUnlock()
    {
        foreach (MemoryFragment fragment in memoryFragments)
        {
            // 如果该记忆片段已经解锁，直接跳过
            if (fragment.isUnlocked)
                continue;

            bool allCollected = true;
            // 检查该记忆片段所需的每个道具是否都已收集
            foreach (int requiredId in fragment.requiredItemIDs)
            {
                if (!collectedItems.Exists(i => i.itemID == requiredId))
                {
                    allCollected = false;
                    break;
                }
            }

            // 当三个必需道具都收集后，标记记忆片段解锁
            if (allCollected)
            {
                fragment.isUnlocked = true;
                Debug.Log($"记忆片段 {fragment.memoryId} 已解锁！");
                // 此处可以调用触发动画的接口，例如 TriggerMemoryAnimation(fragment.memoryId);

                CutSceneManager.Instance.PlayMemoryVideoDelayed(fragment.memoryId);
            }
        }
    }

    /// <summary>
    /// 判断指定记忆片段是否解锁
    /// </summary>
    public bool IsMemoryFragmentUnlocked(int memoryId)
    {
        MemoryFragment fragment = memoryFragments.Find(f => f.memoryId == memoryId);
        return fragment != null && fragment.isUnlocked;
    }

    /// <summary>
    /// 获取当前收集的所有道具
    /// </summary>
    public List<Item> GetCollectedItems()
    {
        return new List<Item>(collectedItems);
    }
}
