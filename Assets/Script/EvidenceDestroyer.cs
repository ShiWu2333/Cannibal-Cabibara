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

            if (item.isPoliceEvidence)
            {
                noticeBoardManager.DestroyEvidence(item.itemID); // ✅ 通知公告栏
                GameEndingManager endingManager = FindObjectOfType<GameEndingManager>();
                if (endingManager != null)
                {
                    endingManager.RecordDestroyedEvidence(item.itemID); // ✅ 现在 GameEndingManager 知道证据被销毁了
                }
            }

            Destroy(item.gameObject); // ✅ 物品消失
        }
    }
}