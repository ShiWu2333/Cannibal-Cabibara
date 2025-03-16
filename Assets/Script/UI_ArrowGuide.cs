using UnityEngine;
using UnityEngine.UI;

public class TriggerArrow : MonoBehaviour
{
    public GameObject arrowUI;  // ✅ 拖入 `ArrowImage`（箭头 UI）
    public Transform player;    // ✅ 玩家
    public Transform river;     // ✅ 目标（河流）
    public float minDistanceToShow = 3f; // ✅ 靠近后隐藏箭头

    private RectTransform arrowRect;
    private Image arrowImage;
    private bool isTriggered = false; // 🚀 **控制是否激活箭头**

    private void Start()
    {
        arrowRect = arrowUI.GetComponent<RectTransform>();
        arrowImage = arrowUI.GetComponent<Image>();

        // **开始时隐藏箭头**
        arrowUI.SetActive(false);
    }

    private void Update()
    {
        if (!isTriggered || player == null || river == null) return;

        Vector3 directionToRiver = (river.position - player.position).normalized;
        float distance = Vector3.Distance(player.position, river.position);

        // 计算箭头旋转
        float angle = Mathf.Atan2(directionToRiver.x, directionToRiver.z) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, -angle);

        // 让箭头随着距离缩小 & 隐藏
        float scaleFactor = Mathf.Clamp(distance / 20f, 0.5f, 1f);
        arrowRect.localScale = new Vector3(scaleFactor, scaleFactor, 1);

        // **靠近目标，隐藏箭头**
        arrowImage.enabled = distance > minDistanceToShow;
    }

    // **🚀 当玩家进入触发区域**
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🔹 玩家进入触发区域，箭头启动！");
            isTriggered = true;
            arrowUI.SetActive(true);  // **开启箭头**
        }
    }
}