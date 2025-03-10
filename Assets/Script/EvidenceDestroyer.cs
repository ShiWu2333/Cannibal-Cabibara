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
                noticeBoardManager.DestroyEvidence(item.itemID);
            }
            else
            {
                Debug.Log($"✅ 物品 {item.itemID} 不是证据，仅仅被销毁");
            }

            Destroy(item.gameObject); // ✅ 物品消失
        }
    }
}