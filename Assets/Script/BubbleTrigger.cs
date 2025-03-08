using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public Sprite bubbleSprite; // 当前Trigger的气泡图案
    private BubbleController bubbleController;
    private bool hasTriggered = false; // 确保同一个Trigger不会重复触发

    void Start()
    {
        bubbleController = FindObjectOfType<BubbleController>(); // 获取 BubbleController
        if (bubbleController == null)
        {
            Debug.LogError("BubbleTrigger: 找不到 BubbleController，请检查 Player 是否挂载！");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (bubbleSprite == null)
            {
                Debug.LogError("BubbleTrigger: bubbleSprite 为空，请在 Inspector 设置！");
                return;
            }

            bubbleController.ShowBubble(bubbleSprite, this); // 传递 Trigger 的图片
            hasTriggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bubbleController.HideBubble(this); // 退出后隐藏气泡
        }
    }
}
