using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackpackIconBlink : MonoBehaviour
{
    public Image backpackIcon; // ✅ 背包图标的 UI Image 组件
    public float blinkDuration = 4f; // ✅ 总共闪烁的时间（秒）
    public float blinkInterval = 0.5f; // ✅ 每次闪烁间隔

    private void Start()
    {
        if (backpackIcon == null)
        {
            backpackIcon = GetComponent<Image>();
        }
    }

    /// <summary>
    /// 在 Inspect 关闭后调用，开始闪烁
    /// </summary>
    public void StartBlinking()
    {
        StartCoroutine(BlinkEffect());
    }

    private IEnumerator BlinkEffect()
    {
        float elapsedTime = 0f;
        bool isVisible = true;

        while (elapsedTime < blinkDuration)
        {
            elapsedTime += blinkInterval;
            isVisible = !isVisible; // 切换可见状态

            // ✅ 切换 UI 透明度
            Color iconColor = backpackIcon.color;
            iconColor.a = isVisible ? 1f : 0.3f; // 1 = 显示，0.3 = 透明
            backpackIcon.color = iconColor;

            yield return new WaitForSeconds(blinkInterval);
        }

        // ✅ 结束后恢复正常
        backpackIcon.color = new Color(backpackIcon.color.r, backpackIcon.color.g, backpackIcon.color.b, 1f);
    }
}