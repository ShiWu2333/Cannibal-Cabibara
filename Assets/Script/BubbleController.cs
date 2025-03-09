using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleController : MonoBehaviour
{
    public GameObject bubbleCanvas; // ���� UI
    public Image bubbleIcon; // �����ڲ���ͼ�� (Display Image)
    public Sprite defaultIcon; // Ĭ������ͼ�꣨��ѡ�������̾�ţ�

    private Coroutine hideCoroutine; // ��¼���ص� Coroutine����ֹ�ظ�����

    void Start()
    {
        if (bubbleCanvas == null || bubbleIcon == null)
        {
            Debug.LogError("BubbleController: bubbleCanvas �� bubbleIcon δ��ֵ��");
            return;
        }

        bubbleCanvas.SetActive(false); // ��ʼ����
    }

    public void ShowBubble(Sprite newIcon)
    {
        if (bubbleCanvas != null && bubbleIcon != null)
        {
            bubbleCanvas.SetActive(true);
            bubbleIcon.sprite = newIcon; // **��������ͼ��**
            Debug.Log("BubbleController: ��ʾ���ݣ�ͼƬ = " + newIcon.name);

            // ȡ���ɵ����ؼ�ʱ������ֹ��δ���������ǰ����
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideBubbleWithDelay(3f)); // 3 ����Զ�����
        }
    }


    IEnumerator HideBubbleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (bubbleCanvas != null)
        {
            bubbleCanvas.SetActive(false);
        }
    }
}
