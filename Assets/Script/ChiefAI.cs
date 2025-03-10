using UnityEngine;

public class ChiefAI : MonoBehaviour
{
    private BubbleController bubbleController;
    public Sprite npcBubbleIcon; // NPC ����������ͼ��
    public Collider detectionCollider; // ������Χ�� Collider

    void Start()
    {
        bubbleController = GetComponent<BubbleController>();

        if (bubbleController == null)
        {
            Debug.LogError("BubbleController ���δ������ NPC �ϣ�");
        }
        if (npcBubbleIcon == null)
        {
            Debug.LogError("NPC û��ָ������ͼ�꣡");
        }
        if (detectionCollider == null)
        {
            Debug.LogError("NPC û�м�ⷶΧ Collider��");
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
