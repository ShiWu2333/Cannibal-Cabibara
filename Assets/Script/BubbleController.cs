using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleController : MonoBehaviour
{
    public GameObject bubbleCanvas; // 气泡 UI
    public Image bubbleIcon; // 气泡内部的图标 (Display Image)
    private BubbleTrigger currentTrigger = null; // 记录当前触发的Trigger
    public float delayTime;

    void Start()
    {
        if (bubbleCanvas == null)
        {
            Debug.LogError("BubbleController: bubbleCanvas 未赋值！");
            return;
        }
        if (bubbleIcon == null)
        {
            Debug.LogError("BubbleController: bubbleIcon 未赋值！");
            return;
        }

        bubbleCanvas.SetActive(false); // 初始隐藏
    }

    public void ShowBubble(Sprite newIcon, BubbleTrigger trigger)
    {
        if (currentTrigger == null && bubbleCanvas != null && bubbleIcon != null) // 确保对象不为空
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
            bubbleIcon.sprite = newIcon; // **更新气泡图案**
            Debug.Log("BubbleController: 显示气泡，图片 = " + newIcon.name);
        }
        else
        {
            Debug.LogError("BubbleController: bubbleCanvas 或 bubbleIcon 为空！");
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
