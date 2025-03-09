using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleController : MonoBehaviour
{
    public GameObject bubbleCanvas; // 气泡 UI
    public Image bubbleIcon; // 气泡内部的图标 (Display Image)
    public Sprite defaultIcon; // 默认气泡图标（可选，比如感叹号）

    private Coroutine hideCoroutine; // 记录隐藏的 Coroutine，防止重复调用

    void Start()
    {
        if (bubbleCanvas == null || bubbleIcon == null)
        {
            Debug.LogError("BubbleController: bubbleCanvas 或 bubbleIcon 未赋值！");
            return;
        }

        bubbleCanvas.SetActive(false); // 初始隐藏
    }

    public void ShowBubble(Sprite newIcon)
    {
        if (bubbleCanvas != null && bubbleIcon != null)
        {
            bubbleCanvas.SetActive(true);
            bubbleIcon.sprite = newIcon; // **更新气泡图案**
            Debug.Log("BubbleController: 显示气泡，图片 = " + newIcon.name);

            // 取消旧的隐藏计时器，防止多次触发导致提前隐藏
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine);
            }
            hideCoroutine = StartCoroutine(HideBubbleWithDelay(3f)); // 3 秒后自动隐藏
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
