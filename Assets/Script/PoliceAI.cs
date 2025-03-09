using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour
{

    [Header("巡逻设置")]
    public bool isGuard = false; // 是否为站岗模式（true = 原地不动, false = 巡逻）
    public Transform pointA; // 巡逻点 A
    public Transform pointB; // 巡逻点 B
    private Transform currentTarget; // 当前目标点

    [Header("巡逻参数")]
    public float speed = 3.0f; // 巡逻速度
    public float waitTime = 2.0f; // 在巡逻点的停留时间
    private bool isWaiting = false; // 是否正在等待
    private bool goingToB = true; // 是否正在前往 B 点

    [Header("警戒范围")]
    public Collider detectionCollider; // 检测玩家的 Collider
    public Transform respawnPoint; // 玩家复活点
    public float caughtWaitTime = 1.0f; // 被抓住后等待时间

    [Header("气泡设置")]
    private BubbleController bubbleController;
    public Sprite npcBubbleIcon; // NPC 触发的气泡图标

    private Rigidbody rb;
    private bool isChasing = false; // 是否在追捕玩家

    public static event System.Action<Transform> OnPlayerCaught; // 事件：玩家被发现

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 运动学 Rigidbody

        if (!isGuard) // 只有非站岗模式时，才设置巡逻目标
        {
            currentTarget = pointA;
            StartCoroutine(Patrol());
        }

        bubbleController = GetComponent<BubbleController>();

        if (bubbleController == null)
        {
            Debug.LogError("BubbleController 组件未挂载在 NPC 上！");
        }
        if (npcBubbleIcon == null)
        {
            Debug.LogError("NPC 没有指定气泡图标！");
        }
        if (detectionCollider == null)
        {
            Debug.LogError("NPC 没有检测范围 Collider！");
        }
    }

    private IEnumerator Patrol()
    {
        while (!isGuard) // 站岗模式下不执行巡逻
        {
            if (!isChasing && !isWaiting)
            {
                MoveToNextPatrolPoint();
            }
            yield return null;
        }
    }

    private void MoveToNextPatrolPoint()
    {
        if (isGuard) return; // 如果是站岗模式，则不巡逻

        Vector3 targetPos = currentTarget.position;
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0f; // 保持水平移动
        direction.Normalize();

        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance < 0.2f)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
        else
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(direction), 5f * Time.fixedDeltaTime);
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 只检查Tag是否是"Player"
        {
            bubbleController.ShowBubble(npcBubbleIcon);
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("玩家进入警戒区域！");

                OnPlayerCaught?.Invoke(transform); // 触发事件，通知 CameraManager
                StartCoroutine(HandlePlayerCaught(other.gameObject));
            }
        }
    }

    private IEnumerator HandlePlayerCaught(GameObject player)
    {
        yield return new WaitForSeconds(caughtWaitTime);
        player.transform.position = respawnPoint.position;
        isChasing = false;
    }
}
