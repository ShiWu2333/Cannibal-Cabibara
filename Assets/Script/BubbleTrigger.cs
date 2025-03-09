using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public Sprite bubbleSprite; // 触发气泡时的图标
    private bool hasTriggered = false; // 记录是否已触发

    private void OnTriggerEnter(Collider other)
    {
        BubbleController bubbleController = other.GetComponent<BubbleController>();

        if (!hasTriggered && bubbleController != null)
        {
            bubbleController.ShowBubble(bubbleSprite);
            hasTriggered = true; // 确保只触发一次
        }
    }
}

