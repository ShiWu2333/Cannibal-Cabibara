using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleController : MonoBehaviour
{
    public GameObject bubbleCanvas; // ���� UI
    public Image bubbleIcon; // �����ڲ���ͼ�� (Display Image)
    private BubbleTrigger currentTrigger = null; // ��¼��ǰ������Trigger
    public float delayTime;

    void Start()
    {
        if (bubbleCanvas == null)
        {
            Debug.LogError("BubbleController: bubbleCanvas δ��ֵ��");
            return;
        }
        if (bubbleIcon == null)
        {
            Debug.LogError("BubbleController: bubbleIcon δ��ֵ��");
            return;
        }

        bubbleCanvas.SetActive(false); // ��ʼ����
    }

    public void ShowBubble(Sprite newIcon, BubbleTrigger trigger)
    {
        if (currentTrigger == null && bubbleCanvas != null && bubbleIcon != null) // ȷ������Ϊ��
        {
            StartCoroutine(ShowBubbleWithDelay(newIcon, delayTime));
            currentTrigger = trigger;
        }
    }

    public void HideBubble(BubbleTrigger trigger)
    {
        if (currentTrigger == trigger)
        {
            StartCoroutine(HideBubbleWithDelay(delayTime));
            currentTrigger = null;
        }
    }

    IEnumerator ShowBubbleWithDelay(Sprite newIcon, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (bubbleCanvas != null && bubbleIcon != null)
        {
            bubbleCanvas.SetActive(true);
            bubbleIcon.sprite = newIcon; // **��������ͼ��**
            Debug.Log("BubbleController: ��ʾ���ݣ�ͼƬ = " + newIcon.name);
        }
        else
        {
            Debug.LogError("BubbleController: bubbleCanvas �� bubbleIcon Ϊ�գ�");
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
