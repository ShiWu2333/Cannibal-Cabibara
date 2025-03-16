using UnityEngine;

public class EvidenceDestroyer : MonoBehaviour
{
    private NoticeBoardManager noticeBoardManager;

    private void Start()
    {
        noticeBoardManager = FindObjectOfType<NoticeBoardManager>(); // 获取公告栏管理器
    }

    private void OnTriggerEnter(Collider other)
    {
        Item item = other.GetComponent<Item>(); // 检测进入 `Trigger` 的 `Item`

        if (item != null)
        {
            Debug.Log($"物品 {item.itemName} 被丢入 {gameObject.name}，正在销毁...");

            // **✅ 只有 `公告栏的证据` 被销毁，才影响公告**
            if (item.isPoliceEvidence)
            {
                noticeBoardManager.DestroyEvidence(item.itemID);
            }

            Destroy(item.gameObject); // ✅ 物品消失
        }
    }
}