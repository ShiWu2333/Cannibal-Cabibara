using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public Sprite bubbleSprite; // ��������ʱ��ͼ��
    private bool hasTriggered = false; // ��¼�Ƿ��Ѵ���

    private void OnTriggerEnter(Collider other)
    {
        BubbleController bubbleController = other.GetComponent<BubbleController>();

        if (!hasTriggered && bubbleController != null)
        {
            bubbleController.ShowBubble(bubbleSprite);
            hasTriggered = true; // ȷ��ֻ����һ��
        }
    }
}

