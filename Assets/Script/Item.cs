using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isPickedUp = false; // 是否被捡起
    public string itemName; // 物品名称
    public int itemID; // 公告栏线索编号顺序
    public Sprite itemIcon; // 物品图标（用于UI）
    public bool canBePickedUp = true; // 是否可以被拾取

    public bool isPoliceEvidence; // ✅ 是否是警察证据

    // 高亮选择部分的参数
    [SerializeField] private float fadeDuration = 0.5f; // 渐变持续时间
    [SerializeField] private Color highlightColor = Color.blue; // 高亮颜色
    [SerializeField] private Color selectedColor = Color.yellow; // 选中颜色
    [SerializeField] private float highlightDistance = 5f; // 高亮距离
    [SerializeField] private float selectDistance = 0.5f; // 选中距离
    [SerializeField] private float highLightWith = 2f; // 高亮宽度
    [SerializeField] private float selectedWith = 5f; // 选中宽度

    // 高亮呼吸
    [SerializeField] private float breathSpeed = 1f; // 呼吸速度
    [SerializeField] private float minAlpha = 0.2f; // 最小宽度
    [SerializeField] private float maxAlpha = 0.8f; // 最大宽度

    [SerializeField] private Outline outline;
    public bool isHighlighted = false; // 是否高亮
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

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Failed to find player");
        }
    }

    private void Update()
    {

        // 获取玩家位置
        if (player == null) return;

        // 计算玩家与物体的距离
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        // 确保高亮和选中状态根据距离动态更新
        bool shouldHighlight = distance <= highlightDistance;
        bool shouldSelect = IsInSelectionBox();

        if (shouldHighlight)
        {
            if (!isHighlighted)
            {
                Highlight(); // 进入高亮状态
            }
        }
        else
        {
            if (isHighlighted)
            {
                RemoveHighlight(); // 退出高亮状态
            }
        }

        // 处理选中逻辑
        if (shouldSelect)
        {
            if (!isSelected)
            {
                Select(); // 进入选中状态
            }
        }
        else
        {
            if (isSelected)
            {
                Deselect(); // 取消选中
            }
        }

        // ✅ 重新加入呼吸效果
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

    private bool IsInSelectionBox()
    {
        if (player == null) return false;

        // 获取 PlayerMovement 脚本
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null) return false;

        // 计算立方体中心点（与 PlayerMovement 一致）
        Vector3 boxCenter = player.transform.position + player.transform.forward * 0.5f;

        // 检测物体是否在立方体区域内
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, new Vector3(playerMovement.selectDistance, playerMovement.selectDistance, 0.5f), player.transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
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

    public void Deselect()
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

    public void RemoveHighlight()
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

    public void UpdateHighlightState()
    {
        // 让 Update() 来决定是否高亮，而不是强行开启高亮
        isHighlighted = false;
    }

    public void OnPickedUp()
    {
        isPickedUp = true;
        Debug.Log($"{itemName} 被拾取");
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

    private void OnDrawGizmosSelected()
    {
        // 绘制高亮范围
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, highlightDistance);

        // 绘制选中范围（与 PlayerMovement 一致）
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector3 boxCenter = player.transform.position + player.transform.forward * 0.5f;
                Gizmos.color = Color.yellow;
                Gizmos.matrix = Matrix4x4.TRS(boxCenter, player.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(playerMovement.selectDistance * 2, playerMovement.selectDistance * 2, 1f)); // 长 1f 的立方体
            }
        }
    }
}