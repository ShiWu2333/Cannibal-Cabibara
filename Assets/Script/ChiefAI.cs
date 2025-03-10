using UnityEngine;
using System.Collections;

public class ChiefAI : MonoBehaviour
{
    [Header("巡逻参数")]
    public float patrolRange = 5f;   // 初始巡逻范围
    public float patrolSpeed = 2f;   // 巡逻移动速度
    public float patrolWaitTime = 2f; // 每次到达点后等待时间

    [Header("逃跑参数")]
    public Transform safePoint;  // 逃跑的固定位置
    public float escapeSpeed = 5f; // 逃跑速度
    public float shockedDuration = 1f; // 受惊时长
    private bool isEscaping = false; // 是否正在逃跑

    [Header("检测范围")]
    public Collider detectionCollider; // 触发器范围
    public BubbleController bubbleController; // 负责显示气泡的组件
    public Sprite shockedBubbleIcon; // 受惊图标

    private Rigidbody rb;
    private Vector3 initialPosition; // 记录初始位置
    private bool isPatrolling = true; // 是否在随机巡逻

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        initialPosition = transform.position; // 记录 NPC 的初始位置
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (isPatrolling) // 只有在未被吓到时才巡逻
        {
            Vector3 targetPoint = GetRandomPatrolPoint();
            while (Vector3.Distance(transform.position, targetPoint) > 0.5f)
            {
                if (!isPatrolling) yield break; // 如果 NPC 进入逃跑状态，则停止巡逻
                MoveTo(targetPoint, patrolSpeed);
                yield return null;
            }
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        // 在初始位置附近随机选点
        return initialPosition + new Vector3(
            Random.Range(-patrolRange, patrolRange),
            0f,
            Random.Range(-patrolRange, patrolRange)
        );
    }

    private void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = (destination - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEscaping)
        {
            StartCoroutine(HandlePlayerDetected());
        }
    }

    private IEnumerator HandlePlayerDetected()
    {
        isPatrolling = false; // 停止巡逻
        isEscaping = true; // 进入逃跑模式

        // 显示受惊气泡
        if (bubbleController != null && shockedBubbleIcon != null)
        {
            bubbleController.ShowBubble(shockedBubbleIcon);
        }

        // 停留一小段时间表示受惊
        yield return new WaitForSeconds(shockedDuration);

        // 逃跑到安全点
        while (Vector3.Distance(transform.position, safePoint.position) > 0.5f)
        {
            MoveTo(safePoint.position, escapeSpeed);
            yield return null;
        }

        yield return new WaitForSeconds(1f); // 逃跑后稍作停留

        // 在安全点附近随机巡逻
        initialPosition = safePoint.position;
        isEscaping = false;
        isPatrolling = true;
        StartCoroutine(Patrol()); // 重新进入巡逻模式
    }
}

