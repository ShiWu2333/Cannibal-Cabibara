using UnityEngine;

public class ChiefAI : MonoBehaviour
{
    private BubbleController bubbleController;
    public Sprite npcBubbleIcon; // NPC 触发的气泡图标
    public Collider detectionCollider; // 触发范围的 Collider

    void Start()
    {
        bubbleController = GetComponent<BubbleController>();

        if (bubbleController == null)
        {
            Debug.LogError("BubbleController 组件未挂载在 NPC 上！");
        }
        if (npcBubbleIcon == null)
        {
            Debug.LogError("NPC 没有指定气泡图标！");
        }
        if (detectionCollider == null)
        {
            Debug.LogError("NPC 没有检测范围 Collider！");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && detectionCollider != null)
        {
            bubbleController.ShowBubble(npcBubbleIcon);
        }
    }
}
