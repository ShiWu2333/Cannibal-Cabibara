using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isPickedUp = false; // 是否被捡起
    public string itemName; // 物品名称
    public int itemID; // 物品唯一ID
    public Sprite itemIcon; // 物品图标（用于UI）
    public bool canBePickedUp = true; // 是否可以被拾取

    // 高亮选择部分的参数
    public float fadeDuration = 0.5f; // 渐变持续时间
    public Color highlightColor = Color.white; // 高亮颜色
    public Color selectedColor = Color.yellow; // 选中颜色
    public float highlightDistance = 5f; // 高亮距离
    public float selectDistance = 2f; // 选中距离
    public float highLightWith = 2f; // 高亮宽度
    public float selectedWith = 5f; // 选中宽度

    //高亮呼吸
    public float breathSpeed = 1f; // 呼吸速度
    public float minAlpha = 0.2f; // 最小宽度
    public float maxAlpha = 0.8f; // 最大宽度

    [SerializeField] Outline outline;
    private bool isHighlighted = false; // 是否高亮
    private bool isSelected = false; // 是否选中
    private GameObject player; // 缓存玩家对象

    private void Start()
    {
        if (outline != null) 
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = highlightColor; // 设置初始颜色
            outline.OutlineWidth = highLightWith; // 设置初始宽度
            outline.enabled = false; // 初始禁用外发光
        }
        else
        {
            Debug.LogError("Failed to find outline");
        }
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = highlightColor; // 设置初始颜色
        outline.OutlineWidth = highLightWith; // 设置初始宽度
        outline.enabled = false; // 初始禁用外发光


        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Failed to find player");
        }
    }

    private void Update()
    {
        if (isPickedUp) return; // 如果物品已被捡起，则跳过

        // 获取玩家位置
        if (player == null) return;

        // 计算玩家与物体的距离
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 判断是否高亮
        if (distance <= highlightDistance)
        {
            if (!isHighlighted)
            {
                Highlight(); // 高亮物体

            }

            // 判断是否选中
            if (distance <= selectDistance && IsRaycastHit())
            {
                if (!isSelected)
                {
                    Select(); // 选中物体
                }
            }
            else
            {
                if (isSelected)
                {
                    Deselect(); // 取消选中
                }
            }

            // 高亮时启用呼吸效果，选中时停止呼吸效果
            if (outline != null && outline.enabled)
            {
                if (isHighlighted && !isSelected) // 仅在高亮且未选中时启用呼吸效果
                {
                    float breath = (Mathf.Sin(Time.time * breathSpeed) + 1) * 0.5f; // 将范围从 [-1, 1] 映射到 [0, 1]
                    float alpha = Mathf.Lerp(minAlpha, maxAlpha, breath); // 在最小值和最大值之间插值

                    // 更新 OutlineColor 的 Alpha 值
                    Color currentColor = outline.OutlineColor;
                    currentColor.a = alpha; // 修改 Alpha 值
                    outline.OutlineColor = currentColor;
                }
                else if (isSelected) // 选中时固定 Alpha 值为最大值
                {
                    Color currentColor = outline.OutlineColor;
                    currentColor.a = maxAlpha; // 固定 Alpha 值
                    outline.OutlineColor = currentColor;
                }
            }

        }
        else
        {
            if (isHighlighted)
            {
                RemoveHighlight(); // 取消高亮
            }
        }
    }

    private void Select()
    {
        if (outline != null)
        {
            outline.OutlineColor = selectedColor; // 设置选中颜色
            outline.OutlineWidth = selectedWith; // 设置选中宽度

            // 选中时固定 Alpha 值为最大值
            Color currentColor = outline.OutlineColor;
            currentColor.a = maxAlpha; // 固定 Alpha 值
            outline.OutlineColor = currentColor;

            isSelected = true;
        }
    }

    private void Deselect()
    {
        if (outline != null)
        {
            outline.OutlineColor = highlightColor; // 恢复高亮颜色
            outline.OutlineWidth = highLightWith; // 恢复高亮宽度

            // 取消选中时恢复呼吸效果
            Color currentColor = outline.OutlineColor;
            currentColor.a = maxAlpha; // 初始设置为最大值
            outline.OutlineColor = currentColor;

            isSelected = false;
        }
    }

    private void Highlight()
    {
        if (outline != null)
        {
            outline.enabled = true; // 启用外发光
            outline.OutlineColor = highlightColor; // 设置高亮颜色
            outline.OutlineWidth = highLightWith; // 设置高亮宽度
            isHighlighted = true;

            // 启动渐变变亮效果
            StartCoroutine(FadeOutline(true));
            Debug.Log("Highlight enabled.");
        }
    }

    private void RemoveHighlight()
    {
        if (outline != null)
        {
            isHighlighted = false;
            isSelected = false;

            // 启动渐变变暗效果
            StartCoroutine(FadeOutline(false));
            Debug.Log("Highlight disabled.");
        }
    }

    private bool IsRaycastHit()
    {
        if (player == null)
        {
            Debug.Log("Player not found!");
            return false;
        }

        RaycastHit hit;
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, selectDistance))
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything.");
        }
        return false;
    }

    public void OnPickedUp()
    {
        RemoveHighlight();
    }


    private IEnumerator FadeOutline(bool fadeIn)
    {
        float startAlpha = outline.OutlineColor.a; // 当前 Alpha 值
        float targetAlpha = fadeIn ? maxAlpha : 0f; // 目标 Alpha 值
        float elapsedTime = 0f; // 已用时间

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration); // 计算插值比例

            // 更新 Alpha 值
            Color currentColor = outline.OutlineColor;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            outline.OutlineColor = currentColor;

            yield return null; // 等待下一帧
        }

        // 确保最终 Alpha 值正确
        Color finalColor = outline.OutlineColor;
        finalColor.a = targetAlpha;
        outline.OutlineColor = finalColor;

        // 如果渐变结束且是关闭操作，则禁用 Outline
        if (!fadeIn)
        {
            outline.enabled = false;
        }
    }

}
