using UnityEngine;
using UnityEngine.UI;

public class ThinkingBubble : MonoBehaviour
{
    public GameObject bubbleCanvas; // 气泡 UI
    public Image bubbleIcon; // 气泡内的图标
    public Sprite defaultBubbleIcon; // NPC 默认气泡图标
    private bool isShowing = false;

    private void Start()
    {
        HideBubble(); // 初始隐藏气泡
    }

    void Update()
    {
        if (isShowing && Camera.main != null)
        {
            // 让气泡始终面向摄像机
            bubbleCanvas.transform.LookAt(Camera.main.transform);
            bubbleCanvas.transform.Rotate(0, 180, 0);
        }
    }

    public void ShowBubble(Sprite icon = null)
    {
        if (icon == null)
        {
            icon = defaultBubbleIcon; // 如果没有传入图标，则使用默认图标
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
