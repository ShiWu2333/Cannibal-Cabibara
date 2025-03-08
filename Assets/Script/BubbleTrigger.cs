using UnityEngine;

public class BubbleTrigger : MonoBehaviour
{
    public Sprite bubbleSprite; // ��ǰTrigger������ͼ��
    private BubbleController bubbleController;
    private bool hasTriggered = false; // ȷ��ͬһ��Trigger�����ظ�����

    void Start()
    {
        bubbleController = FindObjectOfType<BubbleController>(); // ��ȡ BubbleController
        if (bubbleController == null)
        {
            Debug.LogError("BubbleTrigger: �Ҳ��� BubbleController������ Player �Ƿ���أ�");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            if (bubbleSprite == null)
            {
                Debug.LogError("BubbleTrigger: bubbleSprite Ϊ�գ����� Inspector ���ã�");
                return;
            }

            bubbleController.ShowBubble(bubbleSprite, this); // ���� Trigger ��ͼƬ
            hasTriggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bubbleController.HideBubble(this); // �˳�����������
        }
    }
}
