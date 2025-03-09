using UnityEngine;
using UnityEngine.UI;

public class ThinkingBubble : MonoBehaviour
{
    public GameObject bubbleCanvas; // ���� UI
    public Image bubbleIcon; // �����ڵ�ͼ��
    public Sprite defaultBubbleIcon; // NPC Ĭ������ͼ��
    private bool isShowing = false;

    private void Start()
    {
        HideBubble(); // ��ʼ��������
    }

    void Update()
    {
        if (isShowing && Camera.main != null)
        {
            // ������ʼ�����������
            bubbleCanvas.transform.LookAt(Camera.main.transform);
            bubbleCanvas.transform.Rotate(0, 180, 0);
        }
    }

    public void ShowBubble(Sprite icon = null)
    {
        if (icon == null)
        {
            icon = defaultBubbleIcon; // ���û�д���ͼ�꣬��ʹ��Ĭ��ͼ��
        }

        bubbleIcon.sprite = icon;
        bubbleCanvas.SetActive(true);
        isShowing = true;
    }

    public void HideBubble()
    {
        bubbleCanvas.SetActive(false);
        isShowing = false;
    }
}
