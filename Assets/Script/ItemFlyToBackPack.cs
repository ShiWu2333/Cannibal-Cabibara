using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemFlyToBackpack : MonoBehaviour
{
    public static ItemFlyToBackpack Instance;

    [Header("UI References")]
    public Image floatingItemImage; // ✅ 物品图标 UI
    public RectTransform backpackIcon; // ✅ 目标位置（背包）

    [Header("Animation Settings")]
    public float stayDuration = 1f; // ✅ 在屏幕中央停留的时间
    public float flyDuration = 2f; // ✅ 飞行时间
    public AnimationCurve flyCurve; // ✅ 让动画更流畅

    private Vector3 startPosition = new Vector3(0f, 0f, 0f); // ✅ 初始 UI 位置
    private Vector3 targetPosition; // ✅ 物品最终飞行到的位置
    private Vector3 startScale = new Vector3(5f, 5f, 5f); // ✅ 初始大小
    private Vector3 endScale = new Vector3(1f, 1f, 1f); // ✅ 目标大小

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        floatingItemImage.gameObject.SetActive(false); // 初始隐藏
    }

    void Update()
    {
        Debug.Log("Floating Image Position: " + floatingItemImage.rectTransform.anchoredPosition);
    }

    /// <summary>
    /// 在 Inspect 关闭后调用，播放物品飞行动画
    /// </summary>
    public void PlayItemFlyToBackpack(int itemID)
    {
        if (floatingItemImage == null || backpackIcon == null) return;

        // ✅ 获取物品的 Sprite
        Sprite itemSprite = BackPackUIManager.Instance.GetCollectedSpriteByItemID(itemID);
        if (itemSprite == null) return; // 避免 Sprite 为空

        floatingItemImage.sprite = itemSprite;
        floatingItemImage.gameObject.SetActive(true);

        // **转换目标位置到 UI 坐标**
        targetPosition = ConvertToUIPosition(backpackIcon.position);

        // **🔹 设定初始位置**
        floatingItemImage.rectTransform.anchoredPosition = startPosition; // ✅ UI 坐标 (0,0,0)
        floatingItemImage.rectTransform.localScale = startScale; // ✅ 初始大小 (5,5,5)

        // ✅ 开始动画流程
        StartCoroutine(ItemAnimationSequence());
    }

    private IEnumerator ItemAnimationSequence()
    {
        Vector3 startPos = floatingItemImage.rectTransform.anchoredPosition;
        Vector3 endPos = targetPosition; // ✅ 确保 UI 坐标正确
        float elapsedTime = 0f;

        Debug.Log($"开始动画！起点: {startPos} 目标: {endPos}");

        while (elapsedTime < flyDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flyDuration;
            t = flyCurve.Evaluate(t); // ✅ 让动画更平滑

            floatingItemImage.rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            floatingItemImage.rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            Debug.Log($"当前时间: {elapsedTime}, 插值: {t}, 当前 Pos: {floatingItemImage.rectTransform.anchoredPosition}");

            yield return null;
        }

        Debug.Log($"动画完成！最终位置: {floatingItemImage.rectTransform.anchoredPosition}");
    }

    /// <summary>
    /// ✅ 把世界坐标转换成 UI 坐标
    /// </summary>
    private Vector3 ConvertToUIPosition(Vector3 worldPosition)
    {
        RectTransform canvasRect = floatingItemImage.canvas.GetComponent<RectTransform>();
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPos);
        return localPos;
    }
}