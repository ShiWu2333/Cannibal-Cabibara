using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidenceDestroyer : MonoBehaviour
{
    private NoticeBoardManager noticeBoardManager; // 引用公告栏管理器

    private void Start()
    {
        noticeBoardManager = FindObjectOfType<NoticeBoardManager>(); // 自动获取 `NoticeBoardManager`
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>(); // 检测进入 `Trigger` 的 `Item`

        if (item != null)
        {
            Debug.Log($"物品 {item.itemName} 被丢入 {gameObject.name}，正在销毁...");

            // **如果 `Item` 是警察证据，通知公告栏**
            if (item.isPoliceEvidence) // ✅ 只有 `isPoliceEvidence = true` 才触发公告栏更新
            {
                Debug.Log($"⚠️ 证据 {item.itemID} 被销毁，影响公告栏");

                if (noticeBoardManager != null)
                {
                    // ✅ **确保 `itemID` 对应 `公告栏上的天数`**
                    if (noticeBoardManager.dayToNoticeIndex.ContainsKey(item.itemID))
                    {
                        noticeBoardManager.DestroyEvidence(item.itemID);
                        Debug.Log($"✅ 公告栏已更新，证据 {item.itemID} 变成问号");
                    }
                    else
                    {
                        Debug.LogWarning($"⚠️ 证据 {item.itemID} 没有在公告栏上显示，不更新 UI");
                    }
                }
            }
            else
            {
                Debug.Log($"✅ 物品 {item.itemID} 不是证据，仅仅被销毁");
            }

            Destroy(item.gameObject); // ✅ 物品消失
        }
    }
}